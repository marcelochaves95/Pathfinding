using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public static Graph singleton;
	
	public RTA RTAScript;

	public Node[] n;

	private void Awake()
	{
		if (singleton == null)
		{
			singleton = this;
		}

		RTAScript = this.GetComponent<RTA>();
	}

	public void SetNodes(Node[] newNodes)
	{
		n = newNodes;
		print("Initialized nodes");
		RTAScript.InitHeuristics(9999);
	}

	/// <summary>
	/// Find the nearest Node of a position
	/// </summary>
	/// <param name="pos">Postion</param>
	/// <returns>Near node</returns>
    public Node GetNode(Vector3 pos)
	{
        Node nearNode = new Node();
        float lowerDist = 99999;

        for (int i = 0; i < n.Length - 1; i++)
		{
            if (Vector3.Distance(pos, n[i].position) < lowerDist)
			{
				if (n[i].active)
				{
                    lowerDist = Vector3.Distance(pos, n[i].position);
                    nearNode = n[i];
				}
            }
        }
        
        return nearNode;
    }
}