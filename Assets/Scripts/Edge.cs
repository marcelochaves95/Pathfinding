using UnityEngine;

public class Edge : MonoBehaviour
{

    private float _value;
    public float value = 1;

    private bool vertexAActive;
    private bool vertexBActive;
    
    private Vector3 offset;
    private Vector3 scale;
    private Vector3 position;

    private Node vertexA;
    private Node vertexB;

    public void SetEdge(Node a, Node b)
    {
        _value = value;
        vertexA = a;
        vertexB = b;
        vertexAActive = a.active;
        vertexBActive = b.active;
        vertexA.AddNode(b, value);
        vertexB.AddNode(a, value);
        UpdateEdge();
    }

    public void UpdateEdge()
    {
        offset = vertexB.transform.position - vertexA.transform.position;
        scale = new Vector3(0.05f, offset.magnitude, 0.05f);
        position = vertexA.transform.position + (offset / 2.0f);

        this.transform.up = offset;
        this.transform.position = position;
        this.gameObject.name = vertexA.name + " - " + vertexB.name + " Valor: " + value.ToString();
        
        this.transform.localScale = scale;

        vertexA.UpdateStats();
        vertexB.UpdateStats();

        if (!vertexA.active || !vertexB.active)
        {
            this.transform.localScale = Vector3.zero;
        } else
        {
            this.transform.localScale = scale;
        }
        if (_value != value)
        {
            vertexA.UpdateWeight(vertexB, value);
            vertexB.UpdateWeight(vertexA, value);
            _value = value;
        }   
    }
}