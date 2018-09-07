using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grafo : MonoBehaviour {

	public float Granularidade = 1;
	public int N = 10;
	public GameObject Nodo;
	public GameObject[] Nodos;
	public Material ativo, desativo;

	public void Start(){
		Nodos = new GameObject[N*N];
		for (int i = 0; i < N; i++){
			for (int j = 0; j < N; j++)
			{
				GameObject NovoNodo = Instantiate(Nodo, this.transform.position + new Vector3(i*Granularidade,0,j*Granularidade), Quaternion.identity);
				NovoNodo.name = "v_" + (i*N+j);
				Nodos[i*N + j] = NovoNodo;
			}
		}
	}
    public void Update() {
        if (Nodos == null) {
            return;
        }
        for (int i = 0; i < N; i++) {
            for (int j = 0; j < N; j++)
            {
                if (!Nodos[i * N + j].GetComponent<nodo>().ativo)
                    Nodos[i * N + j].GetComponent<MeshRenderer>().material = desativo;
                else
                    Nodos[i * N + j].GetComponent<MeshRenderer>().material = ativo;

                    if (i + 1 < N)
                    {
                        if (Nodos[(j) * N + (i + 1)].GetComponent<nodo>().ativo && Nodos[(j) * N + (i)].GetComponent<nodo>().ativo)
                        {
                            Debug.DrawLine(Nodos[(j) * N + (i)].transform.position, Nodos[(j) * N + (i + 1)].transform.position, Color.green);
                        }
                    }

                    if (j + 1 < N)
                    {
                        if (Nodos[(j + 1) * N + (i)].GetComponent<nodo>().ativo && Nodos[(j) * N + (i)].GetComponent<nodo>().ativo)
                        {
                            Debug.DrawLine(Nodos[(j) * N + (i)].transform.position, Nodos[(j + 1) * N + (i)].transform.position, Color.green);
                        }
                    }

                    if (i + 1 < N && j + 1 < N)
                    {
                        if (Nodos[(j + 1) * N + (i + 1)].GetComponent<nodo>().ativo && Nodos[(j) * N + (i)].GetComponent<nodo>().ativo)
                        {
                            Debug.DrawLine(Nodos[(j) * N + (i)].transform.position, Nodos[(j + 1) * N + (i + 1)].transform.position, Color.green);
                        }
                        if (Nodos[(j + 1) * N + (i)].GetComponent<nodo>().ativo && Nodos[(j) * N + (i + 1)].GetComponent<nodo>().ativo)
                        {
                            Debug.DrawLine(Nodos[(j + 1) * N + (i)].transform.position, Nodos[(j) * N + (i + 1)].transform.position, Color.green);
                        }

                    }
                }
            }
        }

    }