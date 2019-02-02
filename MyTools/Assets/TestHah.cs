using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHah : MonoBehaviour
{
    public bool isTri;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (isTri)
        {
            GetComponent<Vector3Test>().TestVaule = 5;
        }
    }
}