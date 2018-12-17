using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardAngleTest : MonoBehaviour
{
    public float angle;
    public Transform tf;

    private void Start()
    {
    }

    private void Update()
    {
        Vector3 mtfForProject = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        Vector3 otherForProject = Vector3.ProjectOnPlane(tf.forward, Vector3.up);
        angle = Vector3.Angle(mtfForProject, otherForProject);
    }
}