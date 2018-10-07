using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CanEditMultipleObjects]
public class GraphEditor : EditorWindow {

    private int size;
    private GameObject initialVertex;
    private float granularity;

    [Range(0, 90)] public float maxSlope = 30; // Slope máximo do vértice para ele ser andável
    [Range(0, 10)] public float maxBound = 5;

    private GameObject nodesLocation;
    private GameObject edgesLocation;

    private GameObject node;
    private GameObject edge;
    private GameObject[] nodes = new GameObject[0]; // Vetor de nodes
    private List<Edge> edges = new List<Edge>(); // Vetor de arestas

    private GameObject[,] vertexMatrix;
    private Transform positions;

    private GameObject graph;

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
            graph = new GameObject("Graph");
            CreateGraph(size);
            DrawEdges(nodes);
        }

        if (GUILayout.Button("Delete Graph")) {
            DeleteGraph(size);
        }

        if (GUILayout.Button("SaveGraph")) {
            

            // SaveGraph(nodes);
            /*
                Save deve pegar: nodes.length
                Para cada node: Posição x,y,z   ativo/inativo      lista de nodos conectados { Index nodo, Peso da aresta }
         
            */
        }
    }

    private void OnInspectorUpdate() {
        if (edges.Count > 0) {
            UpdateEdges();
        }
    }

    public void CreateGraph (int size) {
        nodesLocation = new GameObject("Nodes");
        nodesLocation.transform.SetParent(graph.transform);
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
                    if (!IsSlopeValid(hit)) {
                        newNode.GetComponent<Node>().active = false;
                    }
                    
                    // Desliga vértices próximos de paredes    
                    if (IsNearWall(newNode, hit)) {
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
    private bool IsSlopeValid (RaycastHit h) {

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
    private bool IsNearWall (GameObject n, RaycastHit h) {
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
        edgesLocation.transform.SetParent(graph.transform);
        if (m == null) {
            return;
        }
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (i + 1 < size) {
                    if (m[(j * size) + i] != null && m[(j * size) + (i + 1)] != null)
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
        foreach (Edge e in edges) {
            e.UpdateEdge();
        }
    }

    //Creates a new menu (Examples) with a menu item (Create Prefab)
    [MenuItem("Examples/Create Prefab")]
    static void CreatePrefab()
    {
        //Keep track of the currently selected GameObject(s)
        GameObject[] objectArray = Selection.gameObjects;

        //Loop through every GameObject in the array above
        foreach (GameObject gameObject in objectArray)
        {
            //Set the path as within the Assets folder, and name it as the GameObject's name with the .prefab format
            string localPath = "Assets/" + gameObject.name + ".prefab";

            //Check if the Prefab and/or name already exists at the path
            if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
            {
                //Create dialog to ask if User is sure they want to overwrite existing prefab
                if (EditorUtility.DisplayDialog("Are you sure?",
                        "The prefab already exists. Do you want to overwrite it?",
                        "Yes",
                        "No"))
                //If the user presses the yes button, create the Prefab
                {
                    CreateNew(gameObject, localPath);
                }
            }
            //If the name doesn't exist, create the new Prefab
            else
            {
                Debug.Log(gameObject.name + " is not a prefab, will convert");
                CreateNew(gameObject, localPath);
            }
        }
    }

    // Disable the menu item if no selection is in place
    [MenuItem("Examples/Create Prefab", true)]
    static bool ValidateCreatePrefab()
    {
        return Selection.activeGameObject != null;
    }

    static void CreateNew(GameObject obj, string localPath)
    {
        //Create a new prefab at the path given
        Object prefab = PrefabUtility.CreatePrefab(localPath, obj);
        PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
    }

    /*[MenuItem("Tools/Write file")]
    static void WriteString()
    {
        string path = "Assets/Resources/test.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Test");
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(path); 
        TextAsset asset = Resources.Load("test");

        //Print the text from the file
        Debug.Log(asset.text);
    }

    [MenuItem("Tools/Read file")]
    static void ReadString()
    {
        string path = "Assets/Resources/test.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path); 
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }*/
}

/*
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Node {

    public int index;

    public bool active = true;
    
    public Vector3 position;

    public List<Edge> connectedList = new List<Edge>();
    
    private Graph graph;

    public void AddNode (Node n, float d) {
        bool canAdd = true;
        foreach (Neighbor ne in connectedList) {
            if (ne.node == n) {
                canAdd = false;
            }
        }
        if (canAdd) { 
            Neighbor aux;
            aux.node = n;
            aux.weight = d;
            connectedList.Add(aux);
        }
    }

    public void UpdateStats () {
        position = this.transform.position;
        this.gameObject.GetComponent<Renderer>().material = active ? green : red;
    }

    public void UpdateWeight (Node n, float value) {
        Neighbor aux;
        for (int i = 0; i < connectedList.Count; i++) {
            if (connectedList[i].node == n) {
                aux = connectedList[i];
                aux.weight = value;
                connectedList[i] = aux;
                return;
            }
        }
    }

    private void Graph () {
        this.graph = new Graph(this.active, this.position, this.connectedList) {
            status = this.active,
            position = this.position,
            connectedNodes = this.connectedList
        };
    }
}

-------------------------------------------------------------------------------------

using UnityEngine;
using System.Xml;
using System.Collections.Generic;

[System.Serializable]
public class Graph {   	
		  
    public bool status;
    public Vector3 position;
    public List<Node> nodes;

    public Graph (bool status, Vector3 position, List<Neighbor> connectedNodes) {
        this.status = status;
        this.position = position;
        this.connectedNodes = connectedNodes;
    }
}

-----------------------------------------------------------------------------------

using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

[CanEditMultipleObjects]
public class GraphEditor : EditorWindow {

    private int size;
    private GameObject initialVertex;
    private float granularity;

    [Range(0, 90)] public float maxSlope = 30; // Slope máximo do vértice para ele ser andável
    [Range(0, 10)] public float maxBound = 5;

    private GameObject nodesLocation;
    private GameObject edgesLocation;

    private GameObject node;
    private GameObject edge;
    private GameObject[] nodes = new GameObject[0]; // Vetor de nodes
    private List<Edge> edges = new List<Edge>(); // Vetor de arestas

    private GameObject[,] vertexMatrix;
    private Transform positions;

    //private Graph graph;
    
    private GameObject graph;

    [MenuItem("Graph/Create Graph...")]
    private static void Init () {
        EditorWindow.GetWindow<GraphEditor>().Show();
    }
    
    private void OnDrawGizmos(){
    		foreach(var node in graph.nodes){
        		Gizmos.DrawSphere();
            foreach(var edge in node.connectedList){
            	Gizmos.Drawline(this.position, edge.vertexB);
            }
        }
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
						graph = new GameObject("Graph");
            CreateGraph(size);
            DrawEdges(nodes);
        }

        if (GUILayout.Button("Delete Graph")) {
            DeleteGraph(size);
        }

        if (GUILayout.Button("SaveGraph")) {
            graph.SaveGraph();

            // SaveGraph(nodes);
            
            //    Save deve pegar: nodes.length
            //    Para cada node: Posição x,y,z   ativo/inativo      lista de nodos conectados { Index nodo, Peso da aresta }
        }
    }

    private void OnInspectorUpdate() {
        if (edges.Count > 0) {
            UpdateEdges();
        }
    }

    public void CreateGraph (int size) {
        nodesLocation = new GameObject("Nodes", graph);
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
                    if (!IsSlopeValid(hit)) {
                        newNode.GetComponent<Node>().active = false;
                    }
                    
                    // Desliga vértices próximos de paredes    
                    if (IsNearWall(newNode, hit)) {
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
    private bool IsSlopeValid (RaycastHit h) {

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
    private bool IsNearWall (GameObject n, RaycastHit h) {
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
        edgesLocation = new GameObject("Edges", graph);
        if (m == null) {
            return;
        }
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (i + 1 < size) {
                    if (m[(j * size) + i] != null && m[(j * size) + (i + 1)] != null)
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
        foreach (Edge e in edges) {
            e.UpdateEdge();
        }
    }
}

----------------------------------------------------------------------------------

using UnityEngine;

public class Edge {

    private float _value;
    public float value = 1;
    
    private Node vertexB;
  
    public void SetEdge (Node nodeTo) {
        _value = value;
        
        vertexB = b;
        vertexAActive = a.active;
        vertexBActive = b.active;
        vertexA.AddNode(b, value);
        vertexB.AddNode(a, value);
        UpdateEdge();
    }

    public void UpdateEdge () {
        offset = vertexB.transform.position - vertexA.transform.position;
        scale = new Vector3(0.05f, offset.magnitude, 0.05f);
        position = vertexA.transform.position + (offset / 2.0f);

        this.transform.up = offset;
        this.transform.position = position;
        this.gameObject.name = vertexA.name + " - " + vertexB.name + " Valor: " + value.ToString();
        
        this.transform.localScale = scale;

        vertexA.UpdateStats();
        vertexB.UpdateStats();

        if (!vertexA.active || !vertexB.active) {
            this.transform.localScale = Vector3.zero;
        } else {
            this.transform.localScale = scale;
        }
        if (_value != value) {
            vertexA.UpdateWeight(vertexB, value);
            vertexB.UpdateWeight(vertexA, value);
            _value = value;
        }   
    }
}

-------------------------------------------------------------------------------
//Edge
//connectedNode
//edge data

//Node
//node data
//list<Edges>

//Graph
//graph data
//List<Node>
//Add node
//Add edge
//Find node

//Graph editor
//Load graph
//save graph
//create new graph
//delete graph
  
//display
//for each node -> instantiate game object
//for each valid edge -> draw line

  
---------------------------------------------------
  
using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node> {
	
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;
	public int movementPenalty;

	public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;
	
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty) {
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		movementPenalty = _penalty;
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}

------------------------------------------------------------------------------
  
  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line {

	const float verticalLineGradient = 1e5f;

	float gradient;
	float y_intercept;
	Vector2 pointOnLine_1;
	Vector2 pointOnLine_2;

	float gradientPerpendicular;

	bool approachSide;

	public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine) {
		float dx = pointOnLine.x - pointPerpendicularToLine.x;
		float dy = pointOnLine.y - pointPerpendicularToLine.y;

		if (dx == 0) {
			gradientPerpendicular = verticalLineGradient;
		} else {
			gradientPerpendicular = dy / dx;
		}

		if (gradientPerpendicular == 0) {
			gradient = verticalLineGradient;
		} else {
			gradient = -1 / gradientPerpendicular;
		}

		y_intercept = pointOnLine.y - gradient * pointOnLine.x;
		pointOnLine_1 = pointOnLine;
		pointOnLine_2 = pointOnLine + new Vector2 (1, gradient);

		approachSide = false;
		approachSide = GetSide (pointPerpendicularToLine);
	}

	bool GetSide(Vector2 p) {
		return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
	}

	public bool HasCrossedLine(Vector2 p) {
		return GetSide (p) != approachSide;
	}

	public float DistanceFromPoint(Vector2 p) {
		float yInterceptPerpendicular = p.y - gradientPerpendicular * p.x;
		float intersectX = (yInterceptPerpendicular - y_intercept) / (gradient - gradientPerpendicular);
		float intersectY = gradient * intersectX + y_intercept;
		return Vector2.Distance (p, new Vector2 (intersectX, intersectY));
	}

	public void DrawWithGizmos(float length) {
		Vector3 lineDir = new Vector3 (1, 0, gradient).normalized;
		Vector3 lineCentre = new Vector3 (pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3.up;
		Gizmos.DrawLine (lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);
	}

}

----------------------------------------------------------------------------------------------
  
using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T> {
	
	T[] items;
	int currentItemCount;
	
	public Heap(int maxHeapSize) {
		items = new T[maxHeapSize];
	}
	
	public void Add(T item) {
		item.HeapIndex = currentItemCount;
		items[currentItemCount] = item;
		SortUp(item);
		currentItemCount++;
	}

	public T RemoveFirst() {
		T firstItem = items[0];
		currentItemCount--;
		items[0] = items[currentItemCount];
		items[0].HeapIndex = 0;
		SortDown(items[0]);
		return firstItem;
	}

	public void UpdateItem(T item) {
		SortUp(item);
	}

	public int Count {
		get {
			return currentItemCount;
		}
	}

	public bool Contains(T item) {
		return Equals(items[item.HeapIndex], item);
	}

	void SortDown(T item) {
		while (true) {
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if (childIndexLeft < currentItemCount) {
				swapIndex = childIndexLeft;

				if (childIndexRight < currentItemCount) {
					if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}

				if (item.CompareTo(items[swapIndex]) < 0) {
					Swap (item,items[swapIndex]);
				}
				else {
					return;
				}

			}
			else {
				return;
			}

		}
	}
	
	void SortUp(T item) {
		int parentIndex = (item.HeapIndex-1)/2;
		
		while (true) {
			T parentItem = items[parentIndex];
			if (item.CompareTo(parentItem) > 0) {
				Swap (item,parentItem);
			}
			else {
				break;
			}

			parentIndex = (item.HeapIndex-1)/2;
		}
	}
	
	void Swap(T itemA, T itemB) {
		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
	
	
	
}

public interface IHeapItem<T> : IComparable<T> {
	int HeapIndex {
		get;
		set;
	}
}


////---------------------------------------------------------------------

namespace BehaviourTreeAI
{
    using Sirenix.OdinInspector;

    using UnityEngine;

    using XNode;

    [CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "AI/Behaviour Tree")]
    public class BehaviourTree : NodeGraph
    {

        [ReadOnly]
        public IBehaviourNode currentNode;

        /// <summary>
        /// The root node to go when all nodes end.
        /// If this node is not set when all nodes end, the map ends.
        /// </summary>
        [ReadOnly]
        public IBehaviourNode rootNode;

        /// <summary>
        /// This behaviour's data storage.
        /// Used to share variables between behaviours.
        /// </summary>
        [SerializeField]
        public Blackboard blackboard;

        /// <summary>
        /// The object that is running this AI.
        /// </summary>
        [ReadOnly]
        public GameObject owner;

        /// <summary>
        /// The animator that controls the owner animations.
        /// Can be null.
        /// If this is null and setAnimatorAutomatically is set,
        /// at Start this is set to owner.GetComponent<Animator>().
        /// That can also be null, so do your null checks.
        /// </summary>
        [Tooltip("The animator that is exposed to be leaf behaviours. Can be null.")]
        [DisableIf("setAnimatorAutomatically")]
        public Animator animator;

        /// <summary>
        /// Set to true to set the animator on start.
        /// </summary>
        public bool setAnimatorAutomatically = true;

        /// <summary>
        /// The time between think calls.
        /// </summary>
        [MinValue(0.01)]
        public float thinkInterval = 1f;

        /// <summary>
        /// The nextThink time stamp.
        /// </summary>
        private float nextThink;

        /// <summary>
        /// Delegate to call various node tick functions correctly.
        /// </summary>
        protected delegate void NodeTick();

        /// <summary>
        /// The controller that runs the tree.
        /// </summary>
        public IBehaviourTreeController Controller
        {
            get; private set;
        }

        /// <summary>
        /// Prepares a BT copy from a BT asset.
        /// The copy is ready to run.
        /// 
        /// The copy owner is set to the owner passed.
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="controller"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static BehaviourTree BuildTree(GameObject owner, IBehaviourTreeController controller, BehaviourTree from)
        {
            BehaviourTree tree = from.DeepCopy();
            tree.Controller = controller;
            tree.owner = owner;
            tree.currentNode = tree.rootNode;
            return tree;
        }

        /// <summary>
        /// Starts the first node in the BT.
        /// If the root is set, then it is started.
        /// </summary>
        public virtual void Start()
        {
            if (this.setAnimatorAutomatically)
            {
                this.animator = this.owner.GetComponent<Animator>();
            }

            if (this.rootNode == null)
            {
                Debug.LogWarning("No root node for behaviour tree in object " + this.owner.name + "!");
                return;
            }

            this.rootNode.Start();
        }

        /// <summary>
        /// Runs the map late update.
        /// </summary>
        public virtual void LateUpdate()
        {
            if (this.CanKeepRunning())
            {
                this.rootNode.LateUpdate();
            }
        }

        /// <summary>
        /// Runs the map fixed update.
        /// </summary>
        public virtual void FixedUpdate()
        {
            if (this.CanKeepRunning())
            {
                this.rootNode.FixedUpdate();
            }
        }

        /// <summary>
        /// Runs the map.
        /// </summary>
        public virtual void Update()
        {
            if (this.CanKeepRunning())
            {
                this.rootNode.Update();
            }
        }

        /// <summary>
        /// Runs the map slower.
        /// </summary>
        public virtual void Think()
        {
            if (Time.time < this.nextThink)
            {
                return;
            }

            //Start the think timer
            this.nextThink = Time.time + this.thinkInterval;

            if (this.CanKeepRunning())
            {
                this.rootNode.Think();
            }

        }

        /// <summary>
        /// Sets the root node for this map.
        /// </summary>
        /// <param name="root"></param>
        public void SetRoot(ABehaviourNode root)
        {
            this.rootNode = root;
        }

        /// <summary>
        /// Sets the behaviour blackboard.
        /// </summary>
        /// <param name="board"></param>
        public void SetBlackboard(Blackboard board)
        {
            this.blackboard = board;
        }

        /// <summary>
        /// Ticks the node and applies the correct state changing if needed.
        /// </summary>
        /// <param name="tickFunction"></param>
        protected virtual void TickNode(NodeTick tickFunction)
        {
            if (this.CanKeepRunning() && tickFunction != null)
            {
                tickFunction();
            }
        }

        protected virtual bool CanKeepRunning()
        {
            if (this.rootNode != null)
            {

                switch (this.rootNode.Status())
                {
                    case Bhvstatus.Running:
                        return true;
                    case Bhvstatus.Success:
                    case Bhvstatus.Failure:
                        this.rootNode.End();
                        return false;
                }
            }

            Debug.LogWarning("No root node for behaviour tree in object " + this.owner.name + "!");
            return false;
        }

        /// <summary> Create a new deep copy of this graph </summary>
        private BehaviourTree DeepCopy()
        {
            // Instantiate a new nodegraph instance
            BehaviourTree graph = Instantiate(this);
            // Instantiate all nodes inside the graph
            for (int i = 0; i < this.nodes.Count; i++)
            {
                if (this.nodes[i] == null)
                {
                    continue;
                }
                Node node = Instantiate(this.nodes[i]) as Node;
                node.graph = graph;
                graph.nodes[i] = node;

                if (this.rootNode.Equals(this.nodes[i]))
                {
                    graph.rootNode = node as IBehaviourNode;
                }
            }

            // Redirect all connections
            for (int i = 0; i < graph.nodes.Count; i++)
            {
                if (graph.nodes[i] == null)
                {
                    continue;
                }
                foreach (NodePort port in graph.nodes[i].Ports)
                {
                    port.Redirect(this.nodes, graph.nodes);
                }
            }

            return graph;
        }
    }
}


*/