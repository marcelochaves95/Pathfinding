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
    int escalaX;
    int escalaY;
    GameObject semente;
    GameObject[,] matrizGameObjects;

    [MenuItem("Matrix/Create Matrix...")]
    static void Init()
    {
        EditorWindow.GetWindow<Criacao>().Show();
    }

    public void OnGUI()
    {
        GUILayout.Label("Granularidade: ");
        granularidade = EditorGUILayout.FloatField(granularidade);
        GUILayout.Label("EscalaX: ");
        escalaX = EditorGUILayout.IntField(escalaX);
        GUILayout.Label("EscalaY: ");
        escalaY = EditorGUILayout.IntField(escalaY);
        GUILayout.Label("Seed: ");
        semente = (GameObject)EditorGUILayout.ObjectField(semente, typeof(GameObject));
        GUILayout.Label("");
        if (GUILayout.Button("GenerateMatrix"))
        {
            matrizGameObjects = new GameObject[escalaX,escalaY];
            GenerateMatrix(escalaX, escalaY);
        }

    }

    public void GenerateMatrix(int m, int n)
    {
        matrizGameObjects[0, 0] = semente;
        Vector3 pos = semente.transform.position;
        for(int i=0; i<m; i++)
        {
            for(int j=0; j<n; j++)
            {
                Vector3 aux = pos + new Vector3(-granularidade * j, -granularidade * i , 0);
                if (i != 0 || j != 0)
                {
                    matrizGameObjects[i, j] = Instantiate(semente, aux, Quaternion.Euler(0, 0, 0));
                }
                matrizGameObjects[i, j].name = "V_" + (m * i + j);
            }
        }
    }
}