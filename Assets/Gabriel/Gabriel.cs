using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[CanEditMultipleObjects]
public class Criacao : EditorWindow
{
    float granularidade;
    int escala;
    GameObject nodo;
    GameObject semente;
    GameObject[] vetorVertices;

    [MenuItem("Matrix/Create Matrix...")]
    static void Init()
    {
        EditorWindow.GetWindow<Criacao>().Show();
    }

    public void OnGUI()
    {
        GUILayout.Label("Granularidade: ");
        granularidade = EditorGUILayout.FloatField(granularidade);
        GUILayout.Label("Escala: ");
        escala = EditorGUILayout.IntField(escala);
        GUILayout.Label("Seed: ");
        semente = (GameObject)EditorGUILayout.ObjectField(semente, typeof(GameObject));
        GUILayout.Label("Prefab Nó: ");
        nodo = (GameObject)EditorGUILayout.ObjectField(nodo, typeof(GameObject));
        GUILayout.Label("");
        if (GUILayout.Button("GenerateMatrix"))
        {
            vetorVertices = new GameObject[escala*escala];
            GenerateMatrix(escala);
        }
        if (GUILayout.Button("UpdateMatrix"))
        {
            UpdateMatrix();
        }
        if (GUILayout.Button("DeleteMatrix"))
        {
            DeleteMatrix();
        }

    }

    public void GenerateMatrix(int n)
    {
        Vector3 pos = semente.transform.position;
        for(int i=0; i<n; i++)
        {
            for(int j=0; j<n; j++)
            {
                GameObject novoNodo = Instantiate(nodo, semente.transform.position + new Vector3(i * granularidade, 0, j * granularidade), Quaternion.identity);
                novoNodo.name = "v_" + (i * n + j);
                vetorVertices[n * i + j] = novoNodo;
            }
        }
    }

    public void UpdateMatrix()
    {
        if (vetorVertices == null)
        {
            return;
        }
        for (int i = 0; i < escala; i++)
        {
            for(int j=0 ;i<escala; j++)
            {                
                    if(j+1 < escala) { 
                        Debug.DrawLine(vetorVertices[(i) * escala + (j)].transform.position, vetorVertices[(i) * escala + (j + 1)].transform.position, Color.green);
                    }
                    if(i+1 < escala) { 
                        Debug.DrawLine(vetorVertices[(i) * escala + (j)].transform.position, vetorVertices[(i+1) * escala + (j)].transform.position, Color.green);
                    }
                    if(i+1 < escala && j+1 < escala) { 
                        Debug.DrawLine(vetorVertices[(i) * escala + (j)].transform.position, vetorVertices[(i+1) * escala + (j + 1)].transform.position, Color.green);
                        Debug.DrawLine(vetorVertices[(i+1) * escala + (j)].transform.position, vetorVertices[(i) * escala + (j + 1)].transform.position, Color.green);
                    }
            }
        }
    }

    public void DeleteMatrix()
    {
        for(int i=0;i<vetorVertices.Length; i++)
        {
            if(vetorVertices[i] != null)
            {
                Destroy(vetorVertices[i].gameObject);
                vetorVertices[i] = null;
            }
        }
    }
}