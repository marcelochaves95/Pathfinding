using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class No : MonoBehaviour {

    public bool active = true;

    void OnMouseClick()
    {
        active = !active;
    }
}
