using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ref : MonoBehaviour {
    
    [SerializeField] int size;
    [SerializeField] GameObject initialVertex;
    [SerializeField] GameObject vertexPrefab;
    [SerializeField] float granularidade;

    public GameObject[,] vertexMatrix;

    private void Start()
    {
        InitializeMatrix(size);
        CreateVertex();
        DrawEdges(vertexMatrix);
    }
    void CreateVertex()
    {
        int iterations = (int)(size / granularidade);
        
        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < iterations; j++)
            {
                Vector3 relativePosToSeed = new Vector3(i * granularidade, 0, j * granularidade);
                GameObject vertex = Instantiate(vertexPrefab, initialVertex.transform.position + relativePosToSeed, Quaternion.identity);
                vertex.name = "v_" + i.ToString() + "," + j.ToString();
                vertexMatrix[i, j] = vertex;
            }
        }
    }

    void DrawEdges(GameObject[,] m)
    {
        for (int i = 0; i < m.Length-1; i++)
        {
            for (int j = 0; j < m.Length-1; j++)
            {
                if(j+1 < m.Length) Debug.DrawLine(m[i, j].transform.position, m[i, j + 1].transform.position, Color.blue,10000);
                if(i+1 < m.Length) Debug.DrawLine(m[i, j].transform.position, m[i+1, j].transform.position, Color.blue, 10000);
               //if (j + 1 < m.Length && i + 1 < m.Length) Debug.DrawLine(m[i, j].vObj.transform.position, m[i + 1, j+1].vObj.transform.position, Color.blue, 10000);
               //if (j - 1 > m.Length && i - 1 > m.Length) Debug.DrawLine(m[i, j].vObj.transform.position, m[i - 1, j - 1].vObj.transform.position, Color.blue, 10000);
            }   
        }
    } 

    void InitializeMatrix(int size)
    {
        vertexMatrix = new GameObject[size*size, size*size];
    }
}
