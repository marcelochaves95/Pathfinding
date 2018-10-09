using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
	[SerializeField] float agentSpeed = .5f;
	[SerializeField] float minDistToNode = 0.25f;
	
	private bool move = false;
	
	public Queue<Node> path = new Queue<Node>();

	void Update()
	{
		ClickToMove();
		if (move)
		{
			Move(path.Peek());
		}
	}

	private void RTA(Vector3 destiny)
	{
		Graph.singleton.RTAScript.CalculateHeuristics(destiny);
		Node nCurrent = Graph.singleton.GetNode(transform.position);
		Node nDestiny = Graph.singleton.GetNode(destiny);
		EnqueueOnPath(nCurrent);
		Node aux = nCurrent;
		
		//Queues path
		while (aux != nDestiny)
		{
			Debug.DrawLine(aux.position, GetNextNode(aux,destiny).position, Color.red,2);
			aux = EnqueueOnPath(GetNextNode(aux,destiny));
			
		}
	}

	private Node EnqueueOnPath(Node n)
	{
		path.Enqueue(n);
		if (!move)
			move = true;
		return n;
	}

	private Node DequeueOnPath()
	{		
		if (path.Count == 1 && move)
			move = false;
		return path.Dequeue();
	}

	private Node GetNextNode(Node nCurrent, Vector3 d)
	{
		Node nNext = new Node();
		PriorityQueue<Node> queueNodes = new PriorityQueue<Node>();
		float newCost;
		foreach (Neighbor nei in nCurrent.connectedList)
		{
			if (!nei.node.active)
				continue;
			queueNodes.Enqueue(nei.node,GetCustoNeighbor(nei));
			
		}
		nNext = queueNodes.Dequeue();
		newCost = GetCustoNeighbor(nCurrent,nNext);
		Graph.singleton.RTAScript.UpdateHeuristic(newCost,nCurrent);
		return nNext;
	}

	/// <summary>
	/// f(n) = h(n) + g(n); new Heuristic (Dist to destiny) + actual node cost (edge weight)
	/// </summary>
	/// <param name="nei"></param>
	/// <returns>Final cost</returns>
	private float GetCustoNeighbor(Neighbor nei)
	{
		float finalCost = Graph.singleton.RTAScript.heuristics[nei.node.index] + nei.weight;
		return finalCost;
	}

	private float GetCustoNeighbor(Node o, Node d)
	{
		float finalCost = 9999;
		foreach (Neighbor n in o.connectedList)
		{
			if (n.node == d)
				finalCost = GetCustoNeighbor(n);
		}
		if (finalCost == 9999)
		{
			print("ERROR!");
		}
		return finalCost;
	}

	/// <summary>
	/// Input read to start LRTA calculations
	/// </summary>
	private void ClickToMove()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (move)
			{
				path.Clear();
				move = false;
			}
            RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				RTA(hit.point);
			}
        }
	}

	/// <summary>
	/// Moves agent to node-destiny
	/// </summary>
	/// <param name="nDestiny">Destiny of node</param>
	private void Move(Node nDestiny)
	{
		if (!HasReachedNode(nDestiny))
			transform.position += (nDestiny.position - transform.position) * agentSpeed * Time.deltaTime;
		else
			DequeueOnPath();
	}

	/// <summary>
	/// Checks if agent has already reached goal node
	/// </summary>
	/// <param name="n">Node</param>
	bool HasReachedNode(Node n)
	{
		if(Vector3.Distance(n.position, transform.position) < minDistToNode)
			return true;
		return false;			
	}
}
