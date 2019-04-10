using System.Collections.Generic;

using UnityEngine;

using Graph;

namespace ArtificialIntelligence
{
    public class Agent : MonoBehaviour
	{
		[SerializeField] private float agentSpeed = 0.5f;
		[SerializeField] private float minDistToNode = 0.25f;
		
		private bool move = false;
		
		public Queue<Node> path = new Queue<Node>();

		void Update()
		{
			ClickToMove();
			if (move)
                Move(path.Peek());
		}

		private void RTA(Vector3 destiny)
		{
			GraphManager.singleton.rta.CalculateHeuristics(destiny);
			Node nodeCurrent = GraphManager.singleton.GetNode(transform.position);
			Node nodeDestiny = GraphManager.singleton.GetNode(destiny);
			EnqueueOnPath(nodeCurrent);
			Node aux = nodeCurrent;

			while (aux != nodeDestiny)
			{
				Debug.DrawLine(aux.position, GetNextNode(aux).position, Color.red, 2);
				aux = EnqueueOnPath(GetNextNode(aux));
				
			}
		}

		private Node EnqueueOnPath(Node n)
		{
			path.Enqueue(n);
			if (!move)
			{
				move = true;
			}
			return n;
		}

		private Node DequeueOnPath()
		{		
			if (path.Count == 1 && move)
				move = false;

			return path.Dequeue();
		}

		private Node GetNextNode(Node nodeCurrent)
		{
			Node nodeNext = new Node();
			PriorityQueue<Node> queueNodes = new PriorityQueue<Node>();
			float newCost;

            foreach (Neighbor neighbor in nodeCurrent.connectedList)
			{
				if (!neighbor.node.Active)
                    continue;

				queueNodes.Enqueue(neighbor.node,GetCustoNeighbor(neighbor));
			}

			nodeNext = queueNodes.Dequeue();
			newCost = GetCustoNeighbor(nodeCurrent, nodeNext);
			GraphManager.singleton.rta.UpdateHeuristic(newCost,nodeCurrent);

			return nodeNext;
		}

		/// <summary>
		/// f(n) = h(n) + g(n); new Heuristic (Dist to destiny) + actual node cost (edge weight)
		/// </summary>
		/// <param name="neighbor"></param>
		/// <returns>Final cost</returns>
		private float GetCustoNeighbor(Neighbor neighbor)
		{
			return GraphManager.singleton.rta.heuristics[neighbor.node.index] + neighbor.weight;
        }

		private float GetCustoNeighbor(Node origin, Node destiny)
		{
			float finalCost = 9999;

            foreach (Neighbor neighbor in origin.connectedList)
			{
				if (neighbor.node == destiny)
					finalCost = GetCustoNeighbor(neighbor);
			}

			if (finalCost == 9999)
                print("ERROR!");

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
                    RTA(hit.point);
			}
		}

		/// <summary>
		/// Moves agent to node-destiny
		/// </summary>
		/// <param name="nodeDestiny">Destiny of node</param>
		private void Move(Node nodeDestiny)
		{
            Vector3 direction = (nodeDestiny.position - transform.position).normalized;

			if (!HasReachedNode(nodeDestiny))
                transform.position += direction * agentSpeed * Time.deltaTime;
            else
                DequeueOnPath();
		}

		/// <summary>
		/// Checks if agent has already reached goal node
		/// </summary>
		/// <param name="node">Node</param>
		private bool HasReachedNode(Node node)
		{
            return Vector3.Distance(node.position, transform.position) < minDistToNode;
		}
	}
}