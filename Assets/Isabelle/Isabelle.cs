using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Isabelle : MonoBehaviour {

public int Granularidade = 1;
public int TamanhoMapa = 10;
public GameObject vertex;
GameObject vertex2;
public GameObject [,] matriz ;
	void Start () {
		matriz = new GameObject [TamanhoMapa, TamanhoMapa];
		desenhaBolinhas();
		desenhaArestas();
	}
	void Update () {
			
	}
	void desenhaBolinhas(){
		float n = TamanhoMapa/Granularidade;
		for (int i = 0; i < n; i++){
			for (int j = 0; j < n; j++){
				 GameObject obj = Instantiate(vertex, vertex.transform.position + new Vector3(i,0,j), Quaternion.identity);
				 obj.name = "v_"+ (n*i+j);
				matriz[i,j] = obj;
			}
		}
	}

	void desenhaArestas(){
		float n = TamanhoMapa/Granularidade;
		
		for (int i = 0; i < n-1; i++){
			for (int j = 0; j < n-1; j++)
			{
				Debug.DrawLine(matriz[i,j].transform.position, matriz[i+1,j].transform.position, Color.magenta, 10000000);
				Debug.DrawLine(matriz[i,j].transform.position, matriz[i,j+1].transform.position, Color.magenta, 10000000);
				Debug.DrawLine(matriz[i,j].transform.position, matriz[i+1,j+1].transform.position, Color.magenta, 10000000);
				Debug.DrawLine(matriz[i+1,j].transform.position, matriz[i+1,j+1].transform.position, Color.magenta, 10000000);
				Debug.DrawLine(matriz[i,j+1].transform.position, matriz[i+1,j+1].transform.position, Color.magenta, 10000000);
				Debug.DrawLine(matriz[i+1,j].transform.position, matriz[i,j+1].transform.position, Color.magenta, 10000000);
			}			
		}
		

	}

	/*void OnDrawGizmos(){
		float n = TamanhoMapa / Granularidade;
		for (int i = 0; i < n; i++){
			for(int j = 0; j < n; j++){
				Gizmos.DrawSphere(this.transform.position + new Vector3(i * Granularidade, 0, j * Granularidade), 0.5f);
			}
		}
	}*/
}
