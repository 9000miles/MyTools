using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookObjectTest : MonoBehaviour
{
    private void Start()
    {
    }

    private void Update()
    {
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.name);
    }
}