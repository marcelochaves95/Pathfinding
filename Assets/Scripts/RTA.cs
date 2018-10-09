using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RTA : MonoBehaviour {

	public List<float> heuristics = new List<float>();
	
	//Initializes List with lenght of Nodes vector and value 'f'.
	public void InitHeuristics(float f){
		for(int i =0; i<GraphSingleton.Instance.n.Length;i++){
			heuristics.Add(f);
		}
	}

	public void CalculateHeuristics(Vector3 destiny){
		for(int i =0;i<GraphSingleton.Instance.n.Length;i++){
			heuristics[i] = Vector3.Distance(GraphSingleton.Instance.n[i].position,destiny);
		}
		print("Heurísticas calculadas");
	}

	public void UpdateHeuristic(float newH, Node n){
		heuristics[n.index] = newH;
		print("Heurísticas atualizadas");
	}
}
