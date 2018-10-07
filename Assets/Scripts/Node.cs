using UnityEngine;
using System.Collections.Generic;

public struct Neighbor {
    public Node node;
    public float weight;
    //public List<Edge> edge;
}

public class Node : MonoBehaviour {

    public int index;

    public bool active = true;
    
    public Vector3 position;

    public Material green;
    public Material red;

    public List<Neighbor> connectedList = new List<Neighbor>();
    
    private Graph graph;

    public void AddNode (Node n, float d) {
        bool canAdd = true;
        foreach (Neighbor ne in connectedList) {
            if (ne.node == n) {
                canAdd = false;
            }
        }
        if (canAdd) { 
            Neighbor aux;
            aux.node = n;
            aux.weight = d;
            connectedList.Add(aux);
        }
    }

    public void UpdateStats () {
        position = this.transform.position;
        this.gameObject.GetComponent<Renderer>().material = active ? green : red;
    }

    public void UpdateWeight (Node n, float value) {
        Neighbor aux;
        for (int i = 0; i < connectedList.Count; i++) {
            if (connectedList[i].node == n) {
                aux = connectedList[i];
                aux.weight = value;
                connectedList[i] = aux;
                return;
            }
        }
    }

    private void Graph () {
        this.graph = new Graph(this.active, this.position, this.connectedList) {
            status = this.active,
            position = this.position,
            connectedNodes = this.connectedList
        };
    }
}