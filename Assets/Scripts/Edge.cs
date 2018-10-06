using UnityEngine;

public class Edge : MonoBehaviour {

    private Node vertexA;
    private Node vertexB;
    public float value = 1;

    public void SetEdge (Node a, Node b) {
        vertexA = a;
        vertexB = b;
        UpdateEdge();
    }

    public void UpdateEdge () {
        Vector3 offset = vertexB.transform.position - vertexA.transform.position;
        Vector3 scale = new Vector3(0.05f, offset.magnitude, 0.05f);
        Vector3 position = vertexA.transform.position + (offset / 2.0f);
        this.transform.up = offset;
        this.transform.position = position;
        this.transform.localScale = scale;
        this.gameObject.name = vertexA.name + " - " + vertexB.name + " Valor: " + value.ToString();
    }
}