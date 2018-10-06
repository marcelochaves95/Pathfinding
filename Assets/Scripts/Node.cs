using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour {

    public bool active = true;
    public List<Node> connectedList = new List<Node>();

    public void AddNode (Node n) {
        connectedList.Add(n);
    }
}