using UnityEngine;

public class GraphMarcelo : MonoBehaviour {

	public float granularity = 1;
	public int n = 10;
	public GameObject node;
	public GameObject[] nodes;
	public Material active, desactive;

	private void Start () {
		nodes = new GameObject[n * n];

		for (int i = 0; i < n; i++){
			for (int j = 0; j < n; j++) {

				RaycastHit hit;
				Vector3 posNode = this.transform.position + new Vector3(i * granularity, 0, j * granularity);
				if (Physics.Raycast(posNode, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity)) {
					GameObject newNode = Instantiate(node, this.transform.position + new Vector3(i * granularity, 0, j * granularity), Quaternion.identity);
					newNode.name = "v" + (i * n + j);
					nodes[i * n + j] = newNode;
					newNode.transform.parent = this.gameObject.transform;

					// Desliga vértices que ficaram em cima de regiões não caminháveis
					if (hit.collider.gameObject.layer == 30) {
						newNode.GetComponent<Node>().active = false;
					}

					// Desliga vértices próximos de paredes

					// Desliga vértices com inclinação muito forte
					/*if (Vector3.Angle(hit.point, hit.normal)) {

					}*/
				}
			}
		}
	}
    private void Update () {
        if (nodes == null) {
            return;
        }
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                if (!nodes[i * n + j].GetComponent<Node>().active) {
					nodes[i * n + j].GetComponent<MeshRenderer>().material = desactive;
				} else {
					nodes[i * n + j].GetComponent<MeshRenderer>().material = active;
                    if (i + 1 < n) {
                        if (nodes[(j) * n + (i + 1)].GetComponent<Node>().active && nodes[(j) * n + (i)].GetComponent<Node>().active) {
                            Debug.DrawLine(nodes[(j) * n + (i)].transform.position, nodes[(j) * n + (i + 1)].transform.position, Color.green);
                        }
                    }
                    if (j + 1 < n) {
                        if (nodes[(j + 1) * n + (i)].GetComponent<Node>().active && nodes[(j) * n + (i)].GetComponent<Node>().active) {
                            Debug.DrawLine(nodes[(j) * n + (i)].transform.position, nodes[(j + 1) * n + (i)].transform.position, Color.green);
                        }
                    }
                    if (i + 1 < n && j + 1 < n) {
                        if (nodes[(j + 1) * n + (i + 1)].GetComponent<Node>().active && nodes[(j) * n + (i)].GetComponent<Node>().active) {
                            Debug.DrawLine(nodes[(j) * n + (i)].transform.position, nodes[(j + 1) * n + (i + 1)].transform.position, Color.green);
                        }
                        if (nodes[(j + 1) * n + (i)].GetComponent<Node>().active && nodes[(j) * n + (i + 1)].GetComponent<Node>().active) {
                            Debug.DrawLine(nodes[(j + 1) * n + (i)].transform.position, nodes[(j) * n + (i + 1)].transform.position, Color.green);
                        }
                    }
				}   
            }
        }
	}
}