using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {
    public float granularidade = 1;//distância entre vértices
    public int nVertex = 10;//número de vértices no lado
    public GameObject No;
    public GameObject[] Nos;
    public Material red, green;

    void Start()
    {   //criar vértices
        Nos = new GameObject[nVertex * nVertex];
        for (int i = 0; i < nVertex; i++)
        {
            for (int j = 0; j < nVertex; j++)
            {
                GameObject newNo = Instantiate(No, this.transform.position + new Vector3(i * granularidade, 0, j * granularidade), Quaternion.identity);
                newNo.name = "v_" + (i * nVertex + j);
                Nos[i * nVertex + j] = newNo;
            }
        }
    }

    void Update()
    {   //desenhar linhas
        if (Nos == null)//se o vetor não tiver sido preenchido ainda
            return;

        for (int i = 0; i < nVertex; i++)
        {
            for (int j = 0; j < nVertex; j++)
            {
                if(!Nos[j*nVertex+i].GetComponent<No>().active) //se o nó estiver desativo
                    Nos[j * nVertex + i].GetComponent<MeshRenderer>().material = red;//fica vermelho
                else
                    Nos[j * nVertex + i].GetComponent<MeshRenderer>().material = green;

                if (i + 1 < nVertex)
                {
                    if (Nos[(j * nVertex) + (i + 1)].GetComponent<No>().active && Nos[(j * nVertex) + (i)].GetComponent<No>().active)
                        Debug.DrawLine(Nos[(j * nVertex) + i].transform.position, Nos[(j * nVertex) + (i + 1)].transform.position, Color.green);
                }
                if (j + 1 < nVertex)
                    if ( Nos[((j + 1) * nVertex) + (i)].GetComponent<No>().active && Nos[(j * nVertex) + (i)].GetComponent<No>().active)
                        Debug.DrawLine(Nos[(j * nVertex) + i].transform.position, Nos[((j + 1) * nVertex) + (i)].transform.position, Color.green);
                if (i + 1 < nVertex && j + 1 < nVertex)
                {
                    if (Nos[((j + 1) * nVertex) + (i+1)].GetComponent<No>().active && Nos[(j * nVertex) + (i)].GetComponent<No>().active)
                        Debug.DrawLine(Nos[(j * nVertex) + i].transform.position, Nos[((j + 1) * nVertex) + (i + 1)].transform.position, Color.green);
                    if (Nos[((j + 1) * nVertex) + (i)].GetComponent<No>().active && Nos[(j * nVertex) + (i+1)].GetComponent<No>().active)
                        Debug.DrawLine(Nos[((j + 1) * nVertex) + i].transform.position, Nos[((j) * nVertex) + (i + 1)].transform.position, Color.green);
                    }
                }              
            }
    }
}
