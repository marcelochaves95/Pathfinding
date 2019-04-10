using UnityEngine;

using ArtificialIntelligence;

namespace Graph
{
    public class GraphManager : MonoBehaviour
	{
		public static GraphManager singleton;
		
		public RTA rta;

		public Node[] node;

		private void Awake()
		{
			if (singleton == null)
				singleton = this;

			rta = GetComponent<RTA>();
		}

        public void SetNodes(Node[] newNodes)
		{
			node = newNodes;
			print("Initialized nodes");
			rta.InitHeuristics(9999);
		}

		/// <summary>
		/// Find the nearest Node of a position
		/// </summary>
		/// <param name="position">Postion</param>
		/// <returns>Near node</returns>
		public Node GetNode(Vector3 position)
		{
			Node nearNode = new Node();
			float lowerDistance = 99999;

			for (int i = 0; i < node.Length - 1; i++)
			{
				if (Vector3.Distance(position, node[i].position) < lowerDistance)
				{
					if (node[i].Active)
					{
						lowerDistance = Vector3.Distance(position, node[i].position);
						nearNode = node[i];
					}
				}
			}

			return nearNode;
		}
	}
}