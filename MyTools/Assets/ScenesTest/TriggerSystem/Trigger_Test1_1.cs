using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Test1_1 : TriggerBase
{
    protected override void Start()
    {
        TargetTag = ETriggerTargetTag.Enemy | ETriggerTargetTag.Player;
        base.Awake();
    }

    public override bool OnTriggerEnterCall(Transform intruder)
    {
        if (base.OnTriggerEnterCall(intruder))
        {
            Debug.Log("1_1 -- Enter");
        }
        return true;
    }

    public override bool OnTriggerStayCall(Transform intruder)
    {
        if (base.OnTriggerStayCall(intruder))
        {
            //Debug.Log("1_1 -- Stay");
        }
        return true;
    }

    public override bool OnTriggerExitCall(Transform intruder)
    {
        if (base.OnTriggerExitCall(intruder))
        {
            //Debug.Log("1_1 -- Exit");
        }
        return true;
    }
}