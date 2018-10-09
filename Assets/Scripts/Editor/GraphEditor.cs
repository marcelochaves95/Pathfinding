using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class Connected
{
    public int index;
    public float value;
}

public class Nodes
{
    public int index;
    public bool status;
    public Vector3 position;
    public Connected[] connected;
}

[CanEditMultipleObjects]
public class GraphEditor : EditorWindow
{

    private int size = 15;
    private GameObject initialVertex;
    private float granularity = 10;

    [Range(0, 90)] public float maxSlope = 30; // Slope máximo do vértice para ele ser andável
    [Range(0, 10)] public float maxBound = 5;

    private GameObject nodesLocation;
    private GameObject edgesLocation;

    private GameObject node;
    private GameObject edge;
    public static Node[] nodes = new Node[0]; // Vetor de nodes, ele será usado no caminhamento
    private List<Edge> edges = new List<Edge>(); // Vetor de arestas

    private GameObject[,] vertexMatrix;
    private Transform positions;

    private GameObject graph;

    #region Editor
    [MenuItem("Graph/Create Graph...")]
    private static void Init()
    {
        EditorWindow.GetWindow<GraphEditor>().Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Granularity: ");
        granularity = EditorGUILayout.FloatField(granularity);
        GUILayout.Label("Size: ");
        size = EditorGUILayout.IntField(size);
        GUILayout.Label("Initial Vertex: ");
        initialVertex = (GameObject)EditorGUILayout.ObjectField(initialVertex, typeof(GameObject), true);
        GUILayout.Label("Vertex Prefab: ");
        node = (GameObject)EditorGUILayout.ObjectField(node, typeof(GameObject), true);
        GUILayout.Label("Edge Prefab: ");
        edge = (GameObject)EditorGUILayout.ObjectField(edge, typeof(GameObject), true);
        GUILayout.Label("");
        if (GUILayout.Button("Generate Graph"))
        {
            DeleteGraph();
            graph = new GameObject("Graph");
            CreateGraph(size);
            DrawEdges(nodes);
        }

        if (GUILayout.Button("Delete Graph"))
        {
            DeleteGraph();
        }

        if (GUILayout.Button("SaveGraph"))
        {
            SaveXML();
            // SaveGraph(nodes);
            /*
                Save deve pegar: nodes.length
                Para cada node: Posição x,y,z   ativo/inativo      lista de nodos conectados { Index nodo, Peso da aresta }
         
            */
        }

        if (GUILayout.Button("LoadGraph"))
        {
            LoadXML();
            // SaveGraph(nodes);
            /*
                Save deve pegar: nodes.length
                Para cada node: Posição x,y,z   ativo/inativo      lista de nodos conectados { Index nodo, Peso da aresta }
         
            */
        }
    }

    private void OnInspectorUpdate()
    {
        if (edges.Count > 0)
        {
            UpdateEdges();
        }
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
                    
                    // Se colidir com a layer No Walk
                    if (hit.collider.gameObject.layer == 23)
                    {
                        newNode.GetComponent<Node>().active = false;
                    }

                    // Desliga vértices muito inclinados
                    if (!IsSlopeValid(hit))
                    {
                        newNode.GetComponent<Node>().active = false;
                    }
                    
                    // Desliga vértices próximos de paredes
                    if (IsNearWall(newNode, hit))
                    {
                        newNode.GetComponent<Node>().active = false;
                    }

                    nodes[i * size + j] = newNode.GetComponent<Node>();
                    
                }
            }
        }
        GraphSingleton.Instance.SetNodes(nodes);
    }

    public void DeleteGraph()
    {
        if (graph != null)
        {
            DestroyImmediate(graph.gameObject);
        }
        nodes = new Node[0];
        edges = new List<Edge>();
    }

    /// <summary>
    /// Checks if the slope is valid
    /// </summary>
    /// <returns></returns>
    private bool IsSlopeValid(RaycastHit h)
    {
        // Calculates angle between hitPoint and hitNormal
        float slope = Vector3.Angle(h.collider.gameObject.transform.TransformDirection(Vector3.up), h.normal);
        if (slope > 90)
        {
            slope = 180 - slope;
        }
        if (slope > maxSlope)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if vertex is near a blocking wall
    /// </summary>
    /// <param name="n"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    private bool IsNearWall(GameObject n, RaycastHit h)
    {
        if (Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.forward, out h, maxBound) ||
            Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.back, out h, maxBound) ||
            Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.right, out h, maxBound) ||
            Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.left, out h, maxBound))
            if (h.transform.gameObject.layer == 23)
            {
                return true;
            }
        return false;
    }


    private void DrawEdges(Node[] m)
    {
        edgesLocation = new GameObject("Edges");
        edgesLocation.transform.SetParent(graph.transform);
        if (m == null)
        {
            return;
        }
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i + 1 < size)
                {
                    if (m[(j * size) + i] != null && m[(j * size) + (i + 1)] != null)
                        CreateEdge(m[(j * size) + i], m[(j * size) + (i + 1)]);
                }
                if (j + 1 < size)
                        if(m[(j * size) + i] != null && m[((j + 1) * size) + (i)] != null)
                        CreateEdge(m[(j * size) + i], m[((j + 1) * size) + (i)]);
                if (i + 1 < size && j + 1 < size) {
                    if (m[(j * size) + i] != null && m[((j + 1) * size) + (i + 1)] != null)
                        CreateEdge(m[(j * size) + i], m[((j + 1) * size) + (i + 1)]);
                    if (m[((j + 1) * size) + i] != null && m[((j) * size) + (i + 1)] != null)
                        CreateEdge(m[((j + 1) * size) + i], m[((j) * size) + (i + 1)]);
                }
            }
        }
    }

    private void CreateEdge(Node vertexA, Node vertexB)
    {
        GameObject e = Instantiate(edge, Vector3.zero, Quaternion.identity, edgesLocation.transform);
        Edge ed = e.GetComponent<Edge>();
        ed.SetEdge(vertexA, vertexB);
        edges.Add(ed);
    }

    private void UpdateEdges()
    {
        foreach (Edge e in edges)
        {
            e.UpdateEdge();
        }
    }

    #region Save/Load
    public void SaveXML()
    {
        Nodes[] savedData = new Nodes[nodes.Length];
        for (int i = 0; i < savedData.Length; i++)
        {
            Nodes aux = new Nodes();
            aux.index = nodes[i].index;
            aux.status = nodes[i].active;
            aux.position = nodes[i].transform.position;

            aux.connected = new Connected[nodes[i].GetComponent<Node>().connectedList.Count];
            for (int k = 0; k < aux.connected.Length; k++)
            {
                Connected auxCon = new Connected();
                auxCon.index = nodes[i].connectedList[k].node.index;
                auxCon.value = nodes[i].connectedList[k].weight;
                aux.connected[k] = auxCon;
            }

            savedData[i] = aux;
        }

        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Nodes[]));
        System.IO.FileStream file = System.IO.File.Create("Graph");

        writer.Serialize(file, savedData);

        file.Flush();
        file.Close();
        Debug.Log("Saved");
    }

    /** <summary> Metodo para carregar um objeto de um arquivo XML </summary>*/
    public void LoadXML()
    {
        Nodes[] loadedData;
        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Nodes[]));
        System.IO.StreamReader file = new System.IO.StreamReader("Graph");
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
            newNode.GetComponent<Node>().active = loadedData[i].status;
            newNode.name = "v_" + i.ToString();
            newNode.transform.SetParent(nodesLocation.transform);
            nodes[i] = newNode.GetComponent<Node>();
        }
        for (int i = 0; i < nodes.Length; i++)
        {
            for (int k = 0; k < loadedData[i].connected.Length; k++)
            {
                Neighbor n = new Neighbor();
                n.node = nodes[loadedData[i].connected[k].index].GetComponent<Node>();
                n.weight = loadedData[i].connected[k].value;
                nodes[i].GetComponent<Node>().connectedList.Add(n);
            }
        }

        DrawEdges(nodes);

        GraphSingleton.Instance.SetNodes(nodes);

        Debug.Log("Loaded");
    }
    #endregion
}