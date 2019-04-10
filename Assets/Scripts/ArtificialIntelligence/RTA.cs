using System.Collections.Generic;

using UnityEngine;

using Graph;

namespace ArtificialIntelligence
{
    public class RTA : MonoBehaviour
	{
		public List<float> heuristics = new List<float>();

        /// <summary>
        /// Initializes List with lenght of Nodes vector and value 'f'.
        /// </summary>
        /// <param name="heuristic">Heuristics</param>
        public void InitHeuristics(float heuristic)
		{
			for (int i = 0; i < GraphManager.singleton.n.Length; i++)
			{
				heuristics.Add(heuristic);
			}
		}

		public void CalculateHeuristics(Vector3 destiny)
		{
			for (int i = 0; i < GraphManager.singleton.n.Length; i++)
			{
				heuristics[i] = Vector3.Distance(GraphManager.singleton.n[i].position, destiny);
			}

			print("Calculated heuristics");
		}

		public void UpdateHeuristic(float newHeuristic, Node node)
		{
			heuristics[node.index] = newHeuristic;
			print("Heuristics updated");
		}
	}
}