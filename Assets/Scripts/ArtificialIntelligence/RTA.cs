namespace ArtificialIntelligence
{
    using System.Collections.Generic;

	using UnityEngine;

    using Graph;

    public class RTA : MonoBehaviour
	{
		public List<float> heuristics = new List<float>();

		/// <summary>
		/// Initializes List with lenght of Nodes vector and value 'f'.
		/// </summary>
		/// <param name="f">f</param>
		public void InitHeuristics(float f)
		{
			for (int i = 0; i < GraphManager.singleton.n.Length; i++)
			{
				heuristics.Add(f);
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

		public void UpdateHeuristic(float newH, Node n)
		{
			heuristics[n.index] = newH;
			print("Heuristics updated");
		}
	}
}