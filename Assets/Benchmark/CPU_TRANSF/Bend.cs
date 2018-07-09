using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Range: 0 - 1, start: 0.5
public class Bend : Transformation {
    Vector3[] newVertices;
    Vector3[] newNormals;

    public float maxAngle = 60.0f;

    public override void StartTransformation()
    {
        newVertices = new Vector3[mesh.vertexCount];
        newNormals = new Vector3[mesh.vertexCount];
    }

    public override void DoTransformation(float sineTime)
    {
        Vector3[] vertices = startVertices;
        Vector3[] normals = startNormals;

        float angle = sineTime * maxAngle * Mathf.Deg2Rad;

        float ymin = -extents.y;
        float ymax = extents.y;
        float y0 = 0;
        float k = angle / (ymin - y0);

        if (angle > 0.001f || angle < -0.001f)
        {
            for (int i = 0; i < mesh.vertexCount; ++i)
            {
                Vector3 v = vertices[i];
                Vector3 n = normals[i];

                v -= bounds.center;

                float yhat = v.y;
                float theta = k * (yhat - y0);
                float c = Mathf.Cos(theta), s = Mathf.Sin(theta);

                float ik = 1.0f / k;

                newVertices[i].x = v.x;
                newVertices[i].y = -s * (v.z - ik) + y0;
                newVertices[i].z = c * (v.z - ik) + ik;

                newVertices[i] += bounds.center;

                float khat = 0.0f;
                if (ymin <= v.y && v.y <= ymax) khat = k;
                float khat_coeff = 1 - khat * v.z;

                newNormals[i].x = khat_coeff * n.x;
                newNormals[i].y = c * n.y - s * khat_coeff * n.z;
                newNormals[i].z = s * n.y + c * khat_coeff * n.z;
                newNormals[i].Normalize();
            }

            mesh.vertices = newVertices;
            mesh.normals = newNormals;
        } else
        {
            mesh.vertices = startVertices;
            mesh.normals = startNormals;
        }

        
    }


}
