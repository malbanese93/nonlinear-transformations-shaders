using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBoundDebug : MonoBehaviour {

    Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (rend != null)
            Gizmos.DrawWireCube(transform.position, rend.bounds.size);
    }
}
