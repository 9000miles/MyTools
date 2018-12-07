using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    //public Vector3 dir;
    public float speed;
    public Transform point1;
    public Transform point2;
    public Transform point;

    private void Start()
    {
    }

    private void Update()
    {
        //transform.Translate(dir.normalized * speed);
        Vector3 dir = point2.position - point1.position;
        Debug.DrawLine(point1.position, point2.position);
        Vector3 wantDir = Vector3.zero;
        Vector3.OrthoNormalize(ref dir, ref wantDir);
        point.position = point1.position + wantDir * 2;
        Debug.DrawLine(point1.position, point.position);
    }
}