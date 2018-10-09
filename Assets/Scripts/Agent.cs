using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

	[SerializeField] float agentVel = .5f;
	[SerializeField] float minDistToNode = 0.25f;//distância mínima para considerar que chegou no node
	bool move = false;
	
	public Queue<Node> path = new Queue<Node>();

	// Update is called once per frame
	void Update () {
		ClickToMove();
		if(move){
			Move(path.Peek());
		}
	}

	void RTA(Vector3 destiny){
		GraphSingleton.Instance.RTAScript.CalculateHeuristics(destiny);//calculates heuristics based on destiny
		Node nCurrent = GraphSingleton.Instance.GetNode(transform.position);//gets agent's current node
		Node nDestiny = GraphSingleton.Instance.GetNode(destiny);//sets final node
		EnqueueOnPath(nCurrent);
		Node aux = nCurrent;
		//Queues path
		while(aux != nDestiny){
			aux = EnqueueOnPath(GetNextNode(aux,destiny));
		}
	}

	Node EnqueueOnPath(Node n){
		path.Enqueue(n);
		if(!move)//começa a mover caso já não esteja
			move = true;
		return n;
	}

	Node DequeueOnPath(){		
		if(path.Count == 1 && move)
			move = false;
		return path.Dequeue();
	}

	//Returns next node. RTA
	Node GetNextNode(Node nCurrent, Vector3 d){
		Node nNext = new Node();
		PriorityQueue<Node> queueNodes = new PriorityQueue<Node>();
		float novoCusto;
		foreach (Neighbor nei in nCurrent.connectedList)
		{
			if(!nei.node.active)
				continue;
			queueNodes.Enqueue(nei.node,GetCustoNeighbor(nei));
			
		}
		nNext = queueNodes.Dequeue();
		novoCusto = GetCustoNeighbor(nCurrent,nNext);
		GraphSingleton.Instance.RTAScript.UpdateHeuristic(novoCusto,nCurrent);
		return nNext;
	}

	//f(n) = h(n) + g(n); Heuristica(Dist to destiny) + custo real do node (peso da aresta)
	float GetCustoNeighbor(Neighbor nei){
		float custoFinal = GraphSingleton.Instance.RTAScript.heuristics[nei.node.index]+nei.weight;
		return custoFinal;
	}

	float GetCustoNeighbor(Node o, Node d){
		float custoFinal = 9999;
		foreach(Neighbor n in o.connectedList){
			if(n.node == d)
				custoFinal = GetCustoNeighbor(n);
		}
		if(custoFinal == 9999){
			print("ERRO!");
		}
		return custoFinal;
	}


	//Input read to start LRTA calculations
	void ClickToMove(){
		if (Input.GetMouseButtonDown(0)) {
				if(move){
					path.Clear();
					move = false;
				}
                RaycastHit hit;                
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
                    RTA(hit.point);
                }
            }
	}

	//Moves agent to node-destiny
	void Move(Node nDestiny){		
			if(!HasReachedNode(nDestiny))
				transform.position += (nDestiny.position - transform.position)*agentVel*Time.deltaTime;
			else
				DequeueOnPath();
	}

	//Checks if agent has already reached goal node
	bool HasReachedNode(Node n){
		if(Vector3.Distance(n.position, transform.position)<minDistToNode)
			return true;
		return false;			
	}
}
