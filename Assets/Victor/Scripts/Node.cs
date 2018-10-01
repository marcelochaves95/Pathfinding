using UnityEngine;

public class Node : MonoBehaviour {

    public bool active = true;

    private void OnMouseClick () {
        active = !active;
    }
}