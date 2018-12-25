using Common;
using System.Collections;
using System.Collections.Generic;
using MarsPC;
using UnityEngine;

public class CurveTest : MonoBehaviour
{
    public Transform Point;
    public Vector3 originalPosition;
    public float initialSpeed = 5;
    public float initialAngle = 45;
    private Vector3 _newPosition;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public Vector3 GetPosition(float totalTime)
    {
        return TransformHelper.GetPosition(_newPosition, initialSpeed, initialAngle, totalTime);
    }

    public void UpdateOriginalPosition()
    {
        originalPosition = transform.position;
        _newPosition = originalPosition;
    }

    public float GetTotalTime()
    {
        return 2 * initialSpeed * Mathf.Sin(initialAngle * Mathf.Deg2Rad) / (GetGravity());
    }

    public float GetGravity()
    {
        return Mathf.Abs(Physics.gravity.y);
    }
}