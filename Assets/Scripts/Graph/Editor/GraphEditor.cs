using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using UnityEngine;
using UnityEditor;

namespace Graph.Editor
{
    public struct Connected
    {
        public int index;
        public float value;
    }

    public struct Nodes
    {
        public int index;
        public bool status;
        public Vector3 position;
        public Connected[] connected;
    }

    [CanEditMultipleObjects]
    public class GraphEditor : EditorWindow
    {
        private string nameGraph;
        private string error;

        private int size = 15;

        private float maxSlope = 30f;
        private float maxBound = 5f;
        private float granularity = 10f;

        private GameObject initialVertex;
        private GameObject nodesLocation;
        private GameObject edgesLocation;
        private GameObject node;
        private GameObject edge;
        private GameObject graph;
        private GameObject[,] vertexMatrix;

        private Transform positions;

        public static Node[] nodes = new Node[0];

        private List<Edge> edges = new List<Edge>();

        #region Editor
        [MenuItem("Graph/Create Graph...")]
        private static void Init()
        {
            EditorWindow.GetWindow<GraphEditor>().Show();
        }

        private void OnGUI()
        {
            var centralizedWords = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };

            GUILayout.Label("GRAPH SETTINGS", centralizedWords);
            
            EditorGUILayout.Separator();
            maxSlope = EditorGUILayout.FloatField("Max Slope:", maxSlope);
            maxBound = EditorGUILayout.FloatField("Max Bound:", maxBound);
            granularity = EditorGUILayout.FloatField("Granularity:", granularity);
            size = EditorGUILayout.IntField("Size:", size);
            initialVertex = (GameObject)EditorGUILayout.ObjectField("Initial Vertex (Seed):", initialVertex, typeof(GameObject), true);
            node = (GameObject)EditorGUILayout.ObjectField("Vertex (Prefab):", node, typeof(GameObject), true);
            edge = (GameObject)EditorGUILayout.ObjectField("Edge (Prefab):", edge, typeof(GameObject), true);
            nameGraph = EditorGUILayout.TextField("Name of the Graph: ", nameGraph);

            EditorGUILayout.Separator();
            GUILayout.Label("GENERATE GRAPH", centralizedWords);
            EditorGUI.BeginDisabledGroup(!initialVertex || !node || !edge);
            if (GUILayout.Button("Generate Graph"))
            {
                DeleteGraph();
                graph = new GameObject("Graph");
                CreateGraph(size);
                DrawEdges(nodes);
                error = null;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!GameObject.Find("Graph"));
            if (GUILayout.Button("Save Graph"))
            {
                if (nameGraph == "")
                    error = "GIVE YOUR GRAPH A NAME TO SAVE";
                else
                {
                    SaveGraphXML(nameGraph);
                    error = null;
                }
            }
            if (GUILayout.Button("Delete Graph"))
            {
                DeleteGraph();
                error = null;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(GameObject.Find("Graph"));
            if (GUILayout.Button("Load Graph") && !GameObject.Find("Graph"))
            {
                if (nameGraph == "")
                    error = "ENTER THE NAME OF THE GRAPH TO LOAD";
                else if (File.Exists("Assets/GraphsXML/" + nameGraph + ".xml"))
                {
                    LoadGraphXML(nameGraph);
                    error = null;
                }
                else
                    error = "THERE IS NO GRAPH AVAILABLE FOR LOADING";
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(15);
            GUILayout.Label(error, centralizedWords);

            // Play Mode
            if (Application.isPlaying && !GameObject.Find("Graph"))
            {
                LoadGraphXML(nameGraph);
                error = null;
            }
        }

        private void OnInspectorUpdate()
        {
            if (edges.Count > 0)
                if (edges[0] != null)
                    UpdateEdges();
        }
        #endregion

        public void CreateGraph(int size)
        {
            nodesLocation = new GameObject("Nodes");
            nodesLocation.transform.SetParent(graph.transform);
            nodes = new Node[size * size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    RaycastHit hit;
                    Vector3 posNode = initialVertex.transform.position + new Vector3(i * granularity, 0, j * granularity);

                    if (Physics.Raycast(posNode, Vector3.down, out hit, Mathf.Infinity))
                    {
                        GameObject newNode = Instantiate(node, hit.point, Quaternion.identity);
                        newNode.name = "v_" + (i * size + j);
                        newNode.GetComponent<Node>().index = i * size + j;
                        newNode.GetComponent<Node>().position = hit.point;
                        newNode.transform.SetParent(nodesLocation.transform);

                        if (hit.collider.gameObject.layer == 23)
                            newNode.GetComponent<Node>().Active = false;

                        if (!IsSlopeValid(hit))
                            newNode.GetComponent<Node>().Active = false;

                        if (IsNearWall(newNode, hit))
                            newNode.GetComponent<Node>().Active = false;

                        nodes[i * size + j] = newNode.GetComponent<Node>();
                    }
                }
            }
        }

        public void DeleteGraph()
        {
            if (graph != null)
                DestroyImmediate(GameObject.Find("Graph"));

            nodes = new Node[0];
            edges = new List<Edge>();
        }

        /// <summary>
        /// Checks if the slope is valid
        /// </summary>
        /// <returns></returns>
        private bool IsSlopeValid(RaycastHit raycast)
        {
            float slope = Vector3.Angle(raycast.collider.gameObject.transform.TransformDirection(Vector3.up), raycast.normal);

            if (slope > 90f)
                slope = 180f - slope;

            if (slope > maxSlope)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if vertex is near a blocking wall
        /// </summary>
        /// <param name="node"></param>
        /// <param name="raycast"></param>
        /// <returns></returns>
        private bool IsNearWall(GameObject node, RaycastHit raycast)
        {
            var forward = Physics.Raycast(node.transform.position + new Vector3(0, -maxBound, 0), Vector3.forward, out raycast, maxBound);
            var back = Physics.Raycast(node.transform.position + new Vector3(0, -maxBound, 0), Vector3.back, out raycast, maxBound);
            var left = Physics.Raycast(node.transform.position + new Vector3(0, -maxBound, 0), Vector3.right, out raycast, maxBound);
            var right = Physics.Raycast(node.transform.position + new Vector3(0, -maxBound, 0), Vector3.left, out raycast, maxBound);

            if (forward || back || left || right)
                if (raycast.transform.gameObject.layer == 23)
                    return true;

            return false;
        }

        private int ArrayIndex(int j, int i)
        {
            return (j * size) + i;
        }

        private void DrawEdges(Node[] matrix)
        {
            edgesLocation = new GameObject("Edges");
            edgesLocation.transform.SetParent(graph.transform);
            if (matrix == null)
                return;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i + 1 < size && matrix[(j * size) + i] != null && matrix[(j * size) + (i + 1)] != null)
                        CreateEdge(matrix[(j * size) + i], matrix[(j * size) + (i + 1)]);

                    if (j + 1 < size && matrix[(j * size) + i] != null && matrix[((j + 1) * size) + (i)] != null)
                        CreateEdge(matrix[(j * size) + i], matrix[((j + 1) * size) + (i)]);

                    if (i + 1 < size && j + 1 < size) {
                        if (matrix[(j * size) + i] != null && matrix[((j + 1) * size) + (i + 1)] != null)
                            CreateEdge(matrix[(j * size) + i], matrix[((j + 1) * size) + (i + 1)]);

                        if (matrix[((j + 1) * size) + i] != null && matrix[((j) * size) + (i + 1)] != null)
                            CreateEdge(matrix[((j + 1) * size) + i], matrix[((j) * size) + (i + 1)]);
                    }
                }
            }
        }

        private void CreateEdge(Node vertexA, Node vertexB)
        {
            Vector3 rayDirection = vertexB.position - vertexA.position;
            RaycastHit hit;

            if (Physics.Raycast(vertexA.position, rayDirection.normalized, out hit, rayDirection.magnitude))
                return;

            GameObject edgeGO = Instantiate(this.edge, Vector3.zero, Quaternion.identity, edgesLocation.transform);
            Edge edge = edgeGO.GetComponent<Edge>();
            edge.SetEdge(vertexA, vertexB);
            edges.Add(edge);
        }

        private void UpdateEdges()
        {
            foreach (Edge item in edges)
            {
                item.UpdateEdge();
            }
        }

        #region Save/Load
        public void SaveGraphXML(string name)
        {
            Nodes[] savedData = new Nodes[nodes.Length];

            for (int i = 0; i < savedData.Length; i++)
            {
                Nodes aux = new Nodes
                {
                    index = nodes[i].index,
                    status = nodes[i].Active,
                    position = nodes[i].transform.position,
                    connected = new Connected[nodes[i].GetComponent<Node>().connectedList.Count]
                };

                for (int k = 0; k < aux.connected.Length; k++)
                {
                    Connected auxConnected = new Connected
                    {
                        index = nodes[i].connectedList[k].node.index,
                        value = nodes[i].connectedList[k].weight
                    };

                    aux.connected[k] = auxConnected;
                }

                savedData[i] = aux;
            }

            var path = "Assets/GraphsXML/" + name + ".xml";
            XmlSerializer writer = new XmlSerializer(typeof(Nodes[]));
            FileStream file = File.Create(path);
            writer.Serialize(file, savedData);

            file.Flush();
            file.Close();
            Debug.Log("Saved");
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Method for loading an object from an XML file
        /// </summary>
        public void LoadGraphXML(string name)
        {
            Nodes[] loadedData;
            string path = "Assets/GraphsXML/" + name + ".xml";
            XmlSerializer reader = new XmlSerializer(typeof(Nodes[]));
            StreamReader file = new StreamReader(path);

            try
            {
                loadedData = (Nodes[])reader.Deserialize(file);
                file.Close();
                nodes = new Node[loadedData.Length];
                graph = new GameObject("Graph");
                nodesLocation = new GameObject("Nodes");
                nodesLocation.transform.SetParent(graph.transform);

                for (int i = 0; i < nodes.Length; i++)
                {
                    GameObject newNode = Instantiate(node, loadedData[i].position , Quaternion.identity);
                    newNode.GetComponent<Node>().index = loadedData[i].index;
                    newNode.GetComponent<Node>().position = loadedData[i].position;
                    newNode.GetComponent<Node>().Active = loadedData[i].status;
                    newNode.name = "v_" + i.ToString();
                    newNode.transform.SetParent(nodesLocation.transform);
                    nodes[i] = newNode.GetComponent<Node>();
                }

                for (int i = 0; i < nodes.Length; i++)
                {
                    for (int k = 0; k < loadedData[i].connected.Length; k++)
                    {
                        Neighbor neighbor = new Neighbor
                        {
                            node = nodes[loadedData[i].connected[k].index].GetComponent<Node>(),
                            weight = loadedData[i].connected[k].value
                        };

                        nodes[i].GetComponent<Node>().connectedList.Add(neighbor);
                    }
                }

                DrawEdges(nodes);

                if (Application.isPlaying)
                    GraphManager.singleton.SetNodes(nodes);
            }
            catch (UnityException)
            {
                Debug.LogError("Save file corrupted!");
            }

            Debug.Log("Loaded");
        }
        #endregion
    }
}