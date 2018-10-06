using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[CanEditMultipleObjects]
public class CreateGraphMenu : EditorWindow {

    int size;
    GameObject initialVertex;
    float granularity;

    [Range(0, 90)] public float maxSlope = 30;//slope máximo do vértice para ele ser andável
    [Range(0, 10)] public float maxBound = 5;

    GameObject node;
    GameObject[] nodes;//vetor de nodes
    //Material red, green;

    GameObject[,] vertexMatrix;
    Transform positions;

    bool graphCreated = false;

    [MenuItem("Graph/Create Graph...")]
    private static void Init () {
        EditorWindow.GetWindow<CreateGraphMenu>().Show();
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
        GUILayout.Label("");
        if (GUILayout.Button("Generate Graph"))
        {
            CreateGraph(size);
        }

        if (GUILayout.Button("Delete Graph"))
        {
            DeleteGraph(size);
        }
    }




    //private void OnInspectorUpdate()
    //{
    //    if (graphCreated)
    //    {
    //        //desenhar linhas
    //        if (nodes == null)//se o vetor não tiver sido preenchido ainda
    //            return;

    //        for (int i = 0; i < size; i++)
    //        {
    //            for (int j = 0; j < size; j++)
    //            {
    //                if (!nodes[j * size + i].GetComponent<Node>().active) //se o nó estiver desativo
    //                    nodes[j * size + i].GetComponent<MeshRenderer>().material.color = Color.red;//fica vermelho
    //                else
    //                    nodes[j * size + i].GetComponent<MeshRenderer>().material.color = Color.green;

    //                if (i + 1 < size)
    //                {
    //                    if (nodes[(j * size) + (i + 1)].GetComponent<Node>().active && nodes[(j * size) + (i)].GetComponent<Node>().active)
    //                        Gizmos.DrawLine(nodes[(j * size) + i].transform.position, nodes[(j * size) + (i + 1)].transform.position);

    //                }
    //                if (j + 1 < size)
    //                    if (nodes[((j + 1) * size) + (i)].GetComponent<Node>().active && nodes[(j * size) + (i)].GetComponent<Node>().active)
    //                        Gizmos.DrawLine(nodes[(j * size) + i].transform.position, nodes[((j + 1) * size) + (i)].transform.position);
    //                if (i + 1 < size && j + 1 < size)
    //                {
    //                    if (nodes[((j + 1) * size) + (i + 1)].GetComponent<Node>().active && nodes[(j * size) + (i)].GetComponent<Node>().active)
    //                        Gizmos.DrawLine(nodes[(j * size) + i].transform.position, nodes[((j + 1) * size) + (i + 1)].transform.position);
    //                    if (nodes[((j + 1) * size) + (i)].GetComponent<Node>().active && nodes[(j * size) + (i + 1)].GetComponent<Node>().active)
    //                        Gizmos.DrawLine(nodes[((j + 1) * size) + i].transform.position, nodes[((j) * size) + (i + 1)].transform.position);
    //                }
    //            }
    //        }
    //    }
    //}

    public void CreateGraph(int size)
    {
        //criar vértices
        nodes = new GameObject[size * size];
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
                    newNode.transform.SetParent(initialVertex.transform);

                    if (hit.collider.gameObject.layer == 23)//se colidir com a layer No Walk
                        newNode.GetComponent<Node>().active = false;

                    if (!isSlopeValid(hit))                  //desliga vértices muito inclinados
                        newNode.GetComponent<Node>().active = false;

                    if (isNearWall(newNode, hit))               //desliga vértices próximos de paredes
                        newNode.GetComponent<Node>().active = false;

                    nodes[i * size + j] = newNode;
                }
            }
        }

        graphCreated = true;
    }

    public void DeleteGraph(int size)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (nodes[i * size + j] != null) DestroyImmediate(nodes[i * size + j].gameObject);
            }
        }
        graphCreated = false;
    }

    /// <summary>
    /// Checks if the slope is valid
    /// </summary>
    /// <returns></returns>
    bool isSlopeValid(RaycastHit h)
    {
        float slope = Vector3.Angle(h.collider.gameObject.transform.TransformDirection(Vector3.up), h.normal);//calculates angle between hitPoint and hitNormal
        if (slope > 90)
            slope = 180 - slope;
        if (slope > maxSlope)//returns false if slope is bigger than maxSlope
            return false;
        return true;
    }

    /// <summary>
    /// Checks if vertex is near a blocking wall
    /// </summary>
    /// <param name="n"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    bool isNearWall(GameObject n, RaycastHit h)
    {
        if (Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.forward, out h, maxBound) ||
            Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.back, out h, maxBound) ||
            Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.right, out h, maxBound) ||
            Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.left, out h, maxBound))
            if (h.transform.gameObject.layer == 23)
                return true;

        return false;
    }

}