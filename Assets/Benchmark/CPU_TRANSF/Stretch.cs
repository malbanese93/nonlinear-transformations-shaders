using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// STRETCH STRENGTH = 1
public class Stretch : Transformation {
    Vector3[] newVertices;
    Vector3[] newNormals;

    public float maxValue = 1.0f;

    public override void StartTransformation()
    {
        newVertices = new Vector3[mesh.vertexCount];
        newNormals = new Vector3[mesh.vertexCount];
    }

    public override void DoTransformation(float sineTime)
    {
        Vector3[] vertices = startVertices;
        Vector3[] normals = startNormals;

        float amount = sineTime * maxValue;

        for (int i = 0; i < mesh.vertexCount; ++i)
        {
            Vector3 v = vertices[i];
            Vector3 n = normals[i];

            v -= bounds.center;

            if (amount > 0.0f)
            {
                newVertices[i].x = v.x / (1.0f + amount);
                newVertices[i].y = v.y / (1.0f + amount);
                newVertices[i].z = v.z * (1.0f + amount);

                newVertices[i] += bounds.center;

                newNormals[i].x = n.x;
                newNormals[i].y = n.y;
                newNormals[i].z = n.z / ((1.0f + amount) * (1.0f + amount));
                newNormals[i].Normalize();
            } else
            {
                newVertices[i].x = v.x * (1.0f - amount);
                newVertices[i].y = v.y * (1.0f - amount);
                newVertices[i].z = -v.z / (amount - 1.0f);

                newVertices[i] += bounds.center;

                newNormals[i].x = n.x;
                newNormals[i].y = n.y;
                newNormals[i].z = n.z * (amount - 1.0f) * (amount - 1.0f);
                newNormals[i].Normalize();
            }

            
        }

        mesh.vertices = newVertices;
        mesh.normals = newNormals;
    }


}
