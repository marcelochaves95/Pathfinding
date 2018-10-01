using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[CanEditMultipleObjects]
public class CreateGraph : EditorWindow {

    private float granularity;
    private int scale;
    private GameObject nodo;
    private GameObject semente;
    private GameObject[] vetorVertices;

    [MenuItem("Matrix/Create Matrix...")]
    private static void Init () {
        EditorWindow.GetWindow<CreateGraph>().Show();
    }

    private void OnGUI () {
        GUILayout.Label("Granularidade: ");
        granularity = EditorGUILayout.FloatField(granularity);
        GUILayout.Label("Escala: ");
        scale = EditorGUILayout.IntField(scale);
        GUILayout.Label("Seed: ");
        semente = (GameObject)EditorGUILayout.ObjectField(semente, typeof(GameObject), true);
        GUILayout.Label("Prefab Nó: ");
        nodo = (GameObject)EditorGUILayout.ObjectField(nodo, typeof(GameObject), true);
        GUILayout.Label("");
        if (GUILayout.Button("GenerateMatrix")) {
            vetorVertices = new GameObject[scale * scale];
            GenerateMatrix(scale);
        }
        if (GUILayout.Button("UpdateMatrix")) {
            UpdateMatrix();
        }
        if (GUILayout.Button("DeleteMatrix")) {
            DeleteMatrix();
        }

    }

    public void GenerateMatrix (int n) {
        Vector3 pos = semente.transform.position;
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                GameObject novoNodo = Instantiate(nodo, semente.transform.position + new Vector3(i * granularity, 0, j * granularity), Quaternion.identity);
                novoNodo.name = "v_" + (i * n + j);
                vetorVertices[n * i + j] = novoNodo;
            }
        }
    }

    public void UpdateMatrix () {
        if (vetorVertices == null) {
            return;
        }
        for (int i = 0; i < scale; i++) {
            for (int j=0 ;i<scale; j++) {
                if (j + 1 < scale) { 
                    Debug.DrawLine(vetorVertices[(i) * scale + (j)].transform.position, vetorVertices[(i) * scale + (j + 1)].transform.position, Color.green);
                    }
                    if (i + 1 < scale) {
                        Debug.DrawLine(vetorVertices[(i) * scale + (j)].transform.position, vetorVertices[(i+1) * scale + (j)].transform.position, Color.green);
                    }
                    if (i + 1 < scale && j + 1 < scale) { 
                        Debug.DrawLine(vetorVertices[(i) * scale + (j)].transform.position, vetorVertices[(i + 1) * scale + (j + 1)].transform.position, Color.green);
                        Debug.DrawLine(vetorVertices[(i+1) * scale + (j)].transform.position, vetorVertices[(i) * scale + (j + 1)].transform.position, Color.green);
                    }
                }
            }
        }

    public void DeleteMatrix () {
        for (int i = 0; i < vetorVertices.Length; i++) {
            if (vetorVertices[i] != null) {
                Destroy(vetorVertices[i].gameObject);
                vetorVertices[i] = null;
            }
        }
    }
}