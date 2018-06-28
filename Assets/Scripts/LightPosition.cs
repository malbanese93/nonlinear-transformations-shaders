using Assets.Elenesski.Camera.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPosition : MonoBehaviour {

	public void ChangeLight(Bounds b)
    {
        var e = b.extents;
        var bc = b.center;

        transform.position = new Vector3(0, bc.y + e.y, 2.5f * e.z);
        GetComponent<Light>().range = (2.5f * e.z) * 0.90f;
    }
}
