using Assets.Elenesski.Camera.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour {

	public void ChangeCamera(Bounds b)
    {
        var e = b.extents;
        var bc = b.center;

        transform.position = new Vector3(0, bc.y, 2.5f * e.z);
        transform.rotation = Quaternion.Euler(0, 180, 0);

        // adapt movements to mesh size
        var cameraScript = GetComponent<GenericMoveCamera>();
        cameraScript.MovementSpeedMagnification = (e.x + e.z) / 2.0f;
        cameraScript.PanLeftRightSensitivity = (e.x + e.z) / 2.0f;
        cameraScript.PanUpDownSensitivity = e.y;
    }
}

