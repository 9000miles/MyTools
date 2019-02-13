using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Test2 : TriggerBase
{
    protected override void Start()
    {
        TargetTag = ETriggerTargetTag.Player;
        base.Awake();
    }

    public override bool OnTriggerEnterCall(Transform intruder)
    {
        Debug.Log("2 -- Enter -- Test");
        if (base.OnTriggerEnterCall(intruder))
        {
            Debug.Log("2 -- Enter");
            TriggerType = ETriggerType.Actived;
        }
        return true;
    }

    public override bool OnTriggerStayCall(Transform intruder)
    {
        if (base.OnTriggerStayCall(intruder))
        {
            Debug.Log("2 -- Stay");
        }
        return true;
    }

    public override bool OnTriggerExitCall(Transform intruder)
    {
        if (base.OnTriggerExitCall(intruder))
        {
            Debug.Log("2 -- Exit");
        }
        return true;
    }
}