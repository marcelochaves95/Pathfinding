using UnityEngine;

namespace Graph
{
    public class Edge : MonoBehaviour
    {
        private float _value;

        public float Value
        {
            get { return _value; }
            set { _value = 1; }
        }

        public bool VertexAActive { get; set; }
        public bool VertexBActive { get; set; }

        public Vector3 Offset { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Position { get; set; }

        public Node VertexA { get; set; }
        public Node VertexB { get; set; }

        public void SetEdge(Node node1, Node node2)
        {
            VertexA = node1;
            VertexB = node2;
            VertexAActive = node1.active;
            VertexBActive = node2.active;
            _value = Value = VertexA.AddNode(node2, Value);
            VertexB.AddNode(node1, Value);
            UpdateEdge();
        }

        public void UpdateEdge()
        {
            Offset = VertexB.transform.position - VertexA.transform.position;
            Scale = new Vector3(0.05f, Offset.magnitude, 0.05f);
            Position = VertexA.transform.position + (Offset / 2f);

            transform.up = Offset;
            transform.position = Position;
            gameObject.name = VertexA.name + " - " + VertexB.name + " Value: " + Value.ToString();
            
            transform.localScale = Scale;

            VertexA.UpdateStats();
            VertexB.UpdateStats();

            if (!VertexA.active || !VertexB.active)
                transform.localScale = Vector3.zero;
            else
                transform.localScale = Scale;
            if (_value != Value)
            {
                VertexA.UpdateWeight(VertexB, Value);
                VertexB.UpdateWeight(VertexA, Value);
                _value = Value;
            }   
        }
    }
}