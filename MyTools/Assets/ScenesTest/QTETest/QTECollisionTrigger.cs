﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTECollisionTrigger : QTECondition
{
    private bool isCollisionTarget;

    protected override bool Check()
    {
        return isCollisionTarget;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            isCollisionTarget = true;
    }
}