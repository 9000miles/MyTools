using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Test : MonoBehaviour
{
    public Transform p1;
    public Transform p2;
    public Transform p3;
    public Transform p4;
    public Transform p5;

    private void Start()
    {
    }

    private void Update()
    {
        Vector3 pos = Vector3.Project(p3.position - p1.position, (p2.position - p1.position).normalized);
        p5.position = pos;
        Vector3 corssDir = Vector3.Cross(p2.position - p1.position, Vector3.up);
        p4.position = Vector3.ProjectOnPlane(p3.position - p1.position, corssDir);
    }
}