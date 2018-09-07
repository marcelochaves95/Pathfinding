using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptZoado : MonoBehaviour {

    public float granularidade;
    public int escala;
    public GameObject nodo;
    GameObject[] vetorVertices;

    public void Start()
    {
        vetorVertices = new GameObject[escala * escala];
        for (int i = 0; i < escala; i++)
        {
            for (int j = 0; j < escala; j++)
            {
                GameObject novoNodo = Instantiate(nodo, this.transform.position + new Vector3(i * granularidade, 0, j * granularidade), Quaternion.identity);
                novoNodo.name = "v_" + (i * escala + j);
                vetorVertices[i*escala + j] = novoNodo;
            }
        }
    }

    public void Update()
    {
        if (vetorVertices == null) return;

        for (int i = 0; i < escala; i++)
        {
            for (int j = 0; i < escala; j++)
            {
                Debug.Log("i: " + i + "j: " + j);

                if (j + 1 < escala)
                {
                    Debug.DrawLine(vetorVertices[(i) * escala + (j)].transform.position, vetorVertices[(i) * escala + (j + 1)].transform.position, Color.green);
                }
                if (j + 1 < escala)
                {
                    Debug.DrawLine(vetorVertices[(i) * escala + (j)].transform.position, vetorVertices[(i + 1) * escala + (j)].transform.position, Color.green);
                }
                if (i + 1 < escala && j + 1 < escala)
                {
                    Debug.DrawLine(vetorVertices[(i) * escala + (j)].transform.position, vetorVertices[(i + 1) * escala + (j + 1)].transform.position, Color.green);
                    Debug.DrawLine(vetorVertices[(i + 1) * escala + (j)].transform.position, vetorVertices[(i) * escala + (j + 1)].transform.position, Color.green);
                }                
            }
        }
    }
}
