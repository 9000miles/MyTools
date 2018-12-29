using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    public Transform trigger;
    public float angle;

    private void Start()
    {
    }

    private void Update()
    {
    }

    private void OnTriggerExit(Collider other)
    {
        Vector3 dir = transform.position - trigger.position;
        Vector3 dirXZ = Vector3.ProjectOnPlane(dir, Vector3.up);
        Vector3 triggerXZ = Vector3.ProjectOnPlane(trigger.forward, Vector3.up);
        angle = Vector3.Angle(dirXZ, triggerXZ);
    }
}