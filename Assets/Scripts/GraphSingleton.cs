using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphSingleton : MonoBehaviour {

    public static GraphSingleton Instance;
	
	public RTA RTAScript;

	public Node[] n;

	void Awake(){
		if(Instance == null){
			Instance = this;
		}

		RTAScript = this.GetComponent<RTA>();
	}
	
	public void SetNodes(Node[] newNodes){		
		n = newNodes;
		print("Nodes inicializados");
		RTAScript.InitHeuristics(9999);
	}

	

	//Encontra o Node mais próximo de uma posição
    public Node GetNode(Vector3 pos){
        Node nearNode = new Node();
        float menorDist = 99999;

        for(int i = 0; i<n.Length-1;i++){

            if(Vector3.Distance(pos, n[i].position)<menorDist){
				if(n[i].active){
                    menorDist = Vector3.Distance(pos, n[i].position);
                    nearNode = n[i];
				}
            }
        }
        
        return nearNode;
    }
}
