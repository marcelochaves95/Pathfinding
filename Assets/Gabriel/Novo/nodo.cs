using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nodo : MonoBehaviour {

public bool ativo;
    RaycastHit hit;
    RaycastHit sphereHit;

    Vector3 reajuste = new Vector3(0, 0.2f, 0);
    public float radius;
    public float slopeHit;
    float angle;

    private void Start()
    {
        if (Physics.Raycast(this.transform.position, -Vector3.up, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.layer == 20)
            {
                if (Physics.SphereCast(transform.position, this.GetComponent<MeshRenderer>().bounds.size.x + 1, transform.forward, out sphereHit, 1))
                {
                    ativo = false;
                    Debug.Log(sphereHit.transform.gameObject.layer);
                }
                else
                {
                    angle = Vector3.Angle(transform.up, hit.normal);
                    if (angle < slopeHit) ativo = true;
                    else ativo = false;
                }
            }
            else if (hit.transform.gameObject.layer == 21)
            {
                ativo = false;
            }
            this.transform.position = hit.point + reajuste;
        }
    }
}