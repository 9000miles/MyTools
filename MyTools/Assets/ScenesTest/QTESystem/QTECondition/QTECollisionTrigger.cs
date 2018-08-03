using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTECollisionTrigger : QTEConditionBase
{
    private bool isCollisionTarget;

    protected override bool Check()
    {
        if (base.Check() == false) return false;
        return isCollisionTarget;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            isCollisionTarget = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
            isCollisionTarget = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            isCollisionTarget = false;
    }
}