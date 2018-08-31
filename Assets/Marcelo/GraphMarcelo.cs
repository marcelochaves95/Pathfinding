using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marcelo : MonoBehaviour {

	public GameObject v;
	[Range(1, 100)] public float granularidade = 1;
	public int tamanhoMapa = 100;

	private void Start () {
		Draw();
	}

	private void Draw () {
		float n = tamanhoMapa / granularidade;
		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				GameObject g = Instantiate(v, v.transform.position + new Vector3(i, 0, j), Quaternion.identity);
				g.name = "v" + (n * i + j);
			}
		}
	}

	/*void OnDrawGizmos () {
		float n = tamanhoMapa / granularidade;
		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				Gizmos.DrawSphere(this.transform.position + new Vector3(i * granularidade, 0, j * granularidade), 0.5f);
			}
		}
	}*/
}
