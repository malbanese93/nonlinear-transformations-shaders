using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBoundDebug : MonoBehaviour {

    Bounds bounds; // Cache starting bounds since adding cubes later modify them!

    private void Start()
    {
        bounds = GetComponent<MeshFilter>().mesh.bounds;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // NB: transform.position because we are first translating all vertices in the vertex shader by bounds.center.
        // In this way we always have the bounds center centered in trasform position (the origin is always in the AABB center)
        Gizmos.DrawWireCube(transform.position, bounds.size);
    }
}
