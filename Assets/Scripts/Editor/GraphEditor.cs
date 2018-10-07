using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

[CanEditMultipleObjects]
public class GraphEditor : EditorWindow {

    private int size;
    private GameObject initialVertex;
    private float granularity;

    [Range(0, 90)] public float maxSlope = 30;//slope máximo do vértice para ele ser andável
    [Range(0, 10)] public float maxBound = 5;

    private GameObject nodesLocation;
    private GameObject edgesLocation;

    private GameObject node;
    private GameObject edge;
    private GameObject[] nodes = new GameObject[0];//vetor de nodes
    private List<Edge> edges = new List<Edge>();// vetor de arestas

    private GameObject[,] vertexMatrix;
    private Transform positions;

    private Graph graph;

    [MenuItem("Graph/Create Graph...")]
    private static void Init () {
        EditorWindow.GetWindow<GraphEditor>().Show();
    }

    private void OnGUI () {
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
        if (GUILayout.Button("Generate Graph")) {
            DeleteGraph(size);
            CreateGraph(size);
            DrawEdges(nodes);
        }

        if (GUILayout.Button("Delete Graph")) {
            DeleteGraph(size);
        }

        if (GUILayout.Button("SaveGraph")) {
            graph.SaveGraph();

            // SaveGraph(nodes);
            /*
                Save deve pegar: nodes.length
                Para cada node: Posição x,y,z   ativo/inativo      lista de nodos conectados { Index nodo, Peso da aresta }
         
            */
        }
    }

    private void OnInspectorUpdate() {
        if(edges.Count > 0) UpdateEdges();
    }

    public void CreateGraph (int size) {
        nodesLocation = new GameObject("Nodes");
        nodes = new GameObject[size * size];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                RaycastHit hit;
                Vector3 posNode = initialVertex.transform.position + new Vector3(i * granularity, 0, j * granularity);
                if (Physics.Raycast(posNode, Vector3.down, out hit, Mathf.Infinity)) {
                    GameObject newNode = Instantiate(node, hit.point, Quaternion.identity);
                    newNode.name = "v_" + (i * size + j);
                    newNode.GetComponent<Node>().index = i * size + j;
                    newNode.transform.SetParent(nodesLocation.transform);
                    
                    // Se colidir com a layer No Walk
                    if (hit.collider.gameObject.layer == 23) {
                        newNode.GetComponent<Node>().active = false;
                    }

                    // Desliga vértices muito inclinados
                    if (!isSlopeValid(hit)) {
                        newNode.GetComponent<Node>().active = false;
                    }
                    
                    // Desliga vértices próximos de paredes    
                    if (isNearWall(newNode, hit)) {
                        newNode.GetComponent<Node>().active = false;
                    }
                    nodes[i * size + j] = newNode;
                }
            }
        }

    }

    public void DeleteGraph (int size) {
        if (nodesLocation != null) {
            DestroyImmediate(nodesLocation.gameObject);
        }
        if (edgesLocation != null) {
            DestroyImmediate(edgesLocation.gameObject);
        }
        nodes = new GameObject[0];
        edges = new List<Edge>();
    }

    /// <summary>
    /// Checks if the slope is valid
    /// </summary>
    /// <returns></returns>
    private bool isSlopeValid (RaycastHit h) {

        // Calculates angle between hitPoint and hitNormal
        float slope = Vector3.Angle(h.collider.gameObject.transform.TransformDirection(Vector3.up), h.normal);
        if (slope > 90) {
            slope = 180 - slope;
        }
        if (slope > maxSlope) {
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
    private bool isNearWall (GameObject n, RaycastHit h) {
        if (Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.forward, out h, maxBound) ||
            Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.back, out h, maxBound) ||
            Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.right, out h, maxBound) ||
            Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.left, out h, maxBound))
            if (h.transform.gameObject.layer == 23) {
                return true;
            }
        return false;
    }


    private void DrawEdges (GameObject[] m) {
        edgesLocation = new GameObject("Edges");
        if (m == null) {
            return;
        }
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (i + 1 < size) {
                    if(m[(j * size) + i] != null && m[(j * size) + (i + 1)] != null)
                        CreateEdge(m[(j * size) + i], m[(j * size) + (i + 1)]);
                }
                if (j + 1 < size)
                        if(m[(j * size) + i] != null && m[((j + 1) * size) + (i)] != null)
                        CreateEdge(m[(j * size) + i], m[((j + 1) * size) + (i)]);
                if (i + 1 < size && j + 1 < size) {
                    if(m[(j * size) + i] != null && m[((j + 1) * size) + (i + 1)] != null)
                        CreateEdge(m[(j * size) + i], m[((j + 1) * size) + (i + 1)]);
                    if(m[((j + 1) * size) + i] != null && m[((j) * size) + (i + 1)] != null)
                        CreateEdge(m[((j + 1) * size) + i], m[((j) * size) + (i + 1)]);
                }
            }
        }
    }

    private void CreateEdge (GameObject vertexA, GameObject vertexB) {
        GameObject e = Instantiate(edge, Vector3.zero, Quaternion.identity, edgesLocation.transform);
        Edge ed = e.GetComponent<Edge>();
        ed.SetEdge(vertexA.GetComponent<Node>(), vertexB.GetComponent<Node>());
        edges.Add(ed);
    }

    private void UpdateEdges () {
        foreach (Edge e in edges) e.UpdateEdge();
    }
}