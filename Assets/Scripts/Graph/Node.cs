namespace Graph
{
    using System.Collections.Generic;

    using UnityEngine;
    
    public struct Neighbor
    {
        public Node node;
        public float weight;
    }

    public class Node : MonoBehaviour
    {
        public int index;

        [SerializeField]
        private bool _active = true;
        public bool active
        {
            get
            {
                return this._active;
            }
            set
            {
                this._active = value;
                UpdateStats();
            }
        }


        public Vector3 position;

        public List<Neighbor> connectedList = new List<Neighbor>();

        public Material green;
        public Material red;

        public float AddNode(Node n, float d)
        {
            foreach (Neighbor ne in connectedList)
            {
                if (ne.node == n)
                {
                    return ne.weight;
                }
            }
            Neighbor aux;
            aux.node = n;
            aux.weight = d;
            connectedList.Add(aux);
            return d;
        }

        void FixedUpdate(){
            UpdateStats();
        }

        public void UpdateStats()
        {
            position = this.transform.position;
            this.gameObject.GetComponent<Renderer>().material = active ? green : red;
        }

        public void UpdateWeight(Node n, float value)
        {
            Neighbor aux;
            for (int i = 0; i < connectedList.Count; i++)
            {
                if (connectedList[i].node == n)
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