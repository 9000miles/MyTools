using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class LookObjectTest : MonoBehaviour
{
    public float distance = 5;
    public float angle = 60;
    private Transform lastTF;
    private Transform area;

    private void Start()
    {
        area = transform.Find("Sphere");
    }

    private void Update()
    {
        area.localScale = new Vector3(distance * 2, distance * 2, distance * 2);
        Transform tf = transform.GetMinDistanceObject(distance, angle, true, "Respawn", "Finish");
        if (tf != lastTF)
        {
            if (tf != null)
                tf.GetComponent<MeshRenderer>().material.color = Color.red;
            if (lastTF != null)
                lastTF.GetComponent<MeshRenderer>().material.color = Color.white;
            lastTF = tf;
        }
    }
}