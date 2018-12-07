using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System;

public class ArrayTest : MonoBehaviour
{
    public float para;
    public int[] arrs = { 15, 31, 616, 31, 5 };

    private void Start()
    {
        float[] arr2s = new float[arrs.Length];
        arr2s = arrs.ConvertAll(t => t + para);
        //arr2s = Array.ConvertAll(arrs, t => t + para);
        foreach (var item in arr2s)
        {
            Debug.Log(item);
            //Array.
        }
        //Array.ForEach(transform.GetComponentsInChildren<Collider>(), t => t.isTrigger = true);
        transform.GetComponentsInChildren<Collider>().ForEach((t) => t.isTrigger = true);
    }

    private void Update()
    {
    }
}