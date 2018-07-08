using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twist : Transformation {
    Vector3[] newVertices;
    Vector3[] newNormals;

    public float maxAngle = 90.0f;

    public override void StartTransformation()
    {
        newVertices = new Vector3[mesh.vertexCount];
        newNormals = new Vector3[mesh.vertexCount];
    }

    public override void DoTransformation()
    {
        Vector3[] vertices = startVertices;
        Vector3[] normals = startNormals;

        for (int i = 0; i < mesh.vertexCount; ++i)
        {
            Vector3 v = vertices[i];
            Vector3 n = normals[i];

            v -= bounds.center;

            float theta = (v.z / extents.z) * Mathf.Deg2Rad * Mathf.Sin(Time.time) * maxAngle;
            float dtheta = (1.0f / extents.z) * Mathf.Deg2Rad * Mathf.Sin(Time.time) * maxAngle;

            float c = Mathf.Cos(theta);
            float s = Mathf.Sin(theta);

            newVertices[i].x = v.x * c - v.y * s;
            newVertices[i].y = v.x * s + v.y * c;
            newVertices[i].z = v.z;

            newVertices[i] += bounds.center;

            newNormals[i].x = c * n.x - s * n.y;
            newNormals[i].y = s * n.x + c * n.y;
            newNormals[i].z = v.y * dtheta * n.x - v.x * dtheta * n.y + n.z;
            newNormals[i].Normalize();
        }

        mesh.vertices = newVertices;
        mesh.normals = newNormals;
    }


}
