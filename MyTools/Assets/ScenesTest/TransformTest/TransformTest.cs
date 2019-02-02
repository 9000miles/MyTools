using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTest : MonoBehaviour
{
    public bool isOpen;
    public Transform point;
    public Transform point1;
    public Transform result;
    public Vector3 pos;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (isOpen)
            result.localPosition = transform.TransformPoint(transform.InverseTransformDirection(point1.localPosition));
        //result.position = transform.TransformDirection(pos);
        //result.position = transform.InverseTransformDirection(pos);
    }
}