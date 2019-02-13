using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Test3 : TriggerBase
{
    protected override void Start()
    {
        TargetTag = ETriggerTargetTag.Player;
        base.Awake();
    }

    public override bool OnTriggerEnterCall(Transform intruder)
    {
        if (base.OnTriggerEnterCall(intruder))
        {
            Debug.Log("3 -- Enter");
        }
        return true;
    }

    public override bool OnTriggerStayCall(Transform intruder)
    {
        if (base.OnTriggerStayCall(intruder))
        {
            Debug.Log("3 -- Stay");
        }
        return true;
    }

    public override bool OnTriggerExitCall(Transform intruder)
    {
        if (base.OnTriggerExitCall(intruder))
        {
            Debug.Log("3 -- Exit");
            TriggerType = ETriggerType.Actived;
        }
        return true;
    }
}