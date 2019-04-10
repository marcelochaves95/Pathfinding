using System.Collections.Generic;

using UnityEngine;

namespace Graph
{
    public struct Neighbor
    {
        public Node node;
        public float weight;
    }

    public class Node : MonoBehaviour
    {
        public int index;

        private bool active;
        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                UpdateStats();
            }
        }

        public Vector3 position;

        public List<Neighbor> connectedList = new List<Neighbor>();

        public Material green;
        public Material red;

        public float AddNode(Node node, float distance)
        {
            foreach (Neighbor ne in connectedList)
            {
                if (ne.node == node)
                {
                    return ne.weight;
                }
            }

            Neighbor aux;
            aux.node = node;
            aux.weight = distance;
            connectedList.Add(aux);
            return distance;
        }

        private void FixedUpdate()
        {
            UpdateStats();
        }

        public void UpdateStats()
        {
            position = transform.position;
            gameObject.GetComponent<Renderer>().material = Active ? green : red;
        }

        public void UpdateWeight(Node node, float value)
        {
            Neighbor aux;

            for (int i = 0; i < connectedList.Count; i++)
            {
                if (connectedList[i].node == node)
                {
                    aux = connectedList[i];
                    aux.weight = value;
                    connectedList[i] = aux;
                    return;
                }
            }
        }
    }
}