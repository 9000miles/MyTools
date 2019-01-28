using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve_Test : MonoBehaviour
{
    public float value;
    public AnimationCurve curve;
    public Transform point;
    private float timeX;
    private float valueY;
    private bool isStart;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isStart = true;
        }

        if (isStart)
        {
            point.position = new Vector3(timeX, curve.Evaluate(timeX), 0);
            timeX += Time.deltaTime;
        }
        //if (timeX >= curve[curve.length - 1].time)
        //{
        //    timeX = 0;
        //    isStart = false;
        //}
    }

    [ContextMenu("Test")]
    private void DebugCurve()
    {
        Debug.Log(curve.Evaluate(value));
        Debug.Log(curve.keys.Length);
        Debug.Log(curve.length);
    }
}