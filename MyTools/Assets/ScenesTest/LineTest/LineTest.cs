using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTest : MonoBehaviour
{
    private ArrayList lines = new ArrayList();
    // Use this for initialization

    private void Start()
    {
        MeshFilter meshfilter = GetComponent("MeshFilter") as MeshFilter;
        Mesh mesh = meshfilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length / 3; i++)
        {
            lines.Add(vertices[triangles[i]]);
            lines.Add(vertices[triangles[i * 3 + 1]]);
            lines.Add(vertices[triangles[i * 3 + 2]]);
        }
    }

    private void OnRenderObject()
    {
        if (true)
        {
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.LINES);
            GL.Color(Color.blue);
            for (int i = 0; i < lines.Count / 3; i++)
            {
                GL.Vertex((Vector3)lines[i * 3]);
                GL.Vertex((Vector3)lines[i * 3 + 1]);
                GL.Vertex((Vector3)lines[i * 3 + 1]);
                GL.Vertex((Vector3)lines[i * 3 + 2]);
                GL.Vertex((Vector3)lines[i * 3 + 2]);
                GL.Vertex((Vector3)lines[i * 3]);
            }

            GL.End();
            GL.PopMatrix();
        }
    }
}