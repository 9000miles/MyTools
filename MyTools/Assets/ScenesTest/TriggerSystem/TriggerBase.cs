﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarsPC;
using System;

public class TriggerBase : MonoBehaviour
{
    public bool isTestTrigger = false;//测试Trigger==true，会自动将triggerType设置为Active
    public bool isThroughTriggerOpen = false;//是否穿过Trigger开启
    public int triggerCount;
    [EnumFlags] public ETriggerTargetTag targetTag = ETriggerTargetTag.Player;
    [SerializeField] private ETriggerType triggerType;

    private Vector3 enterPos;
    private Vector3 exitPos;

    private new Collider collider;
    private new Collider2D collider2D;

    public ETriggerType TriggerType
    {
        get { return triggerType; }
        set
        {
            triggerType = value;
            if (triggerType == ETriggerType.InActive) Init();
        }
    }

    protected virtual void Awake()
    {
        collider = GetComponent<Collider>();
        collider2D = GetComponent<Collider2D>();
        if ((collider != null && collider2D == null) || (collider == null && collider2D != null))
        {
            if (collider != null)
            {
                collider.isTrigger = true;
                foreach (ETriggerTargetTag item in Enum.GetValues(typeof(ETriggerTargetTag)))
                {
                    if ((item & targetTag) != 0) TriggerManager.Singleton.AddTrigger(item.ToString(), collider, null, this);
                }
            }

            if (collider2D != null)
            {
                collider2D.isTrigger = true;
                foreach (ETriggerTargetTag item in Enum.GetValues(typeof(ETriggerTargetTag)))
                {
                    if ((item & targetTag) != 0) TriggerManager.Singleton.AddTrigger(item.ToString(), null, collider2D, this);
                }
            }
        }
        else
        {
            Debug.LogError("Please Check [ " + transform.GetSelfPath() + " ] Collider");
        }
    }

    public virtual void Init()
    {
        triggerType = ETriggerType.InActive;
    }

    public GameObject enterPoint;
    public GameObject exitPoint;

    public virtual bool OnTriggerEnterCall(Transform intruder)
    {
        enterPos = GetComponent<Collider>().bounds.ClosestPoint(intruder.position);
        if (collider == null || isThroughTriggerOpen || triggerType == ETriggerType.Actived) return false;
        //Debug.Log(intruder.name + " --> " + gameObject.name);
        return true;
    }

    public virtual bool OnTriggerStayCall(Transform intruder)
    {
        if (collider == null || isThroughTriggerOpen || triggerType == ETriggerType.Actived) return false;
        return true;
    }

    public virtual bool OnTriggerExitCall(Transform intruder)
    {
        exitPos = GetComponent<Collider>().bounds.ClosestPoint(intruder.position);
        if (collider == null || triggerType == ETriggerType.Actived) return false;
        if (isThroughTriggerOpen && !TransformHelper.CheckThroughTrigger(enterPos, exitPos, transform)) return false;
        if (isTestTrigger) TriggerType = ETriggerType.InActive;
        //Debug.Log(intruder.name + " <-- " + gameObject.name);
        return true;
    }

    public virtual bool OnTriggerEnter2DCall(Transform intruder)
    {
        if (collider2D == null || isThroughTriggerOpen || triggerType == ETriggerType.Actived) return false;
        enterPos = GetComponent<Collider>().bounds.ClosestPoint(intruder.position);
        return true;
    }

    public virtual bool OnTriggerStay2DCall(Transform intruder)
    {
        if (collider2D == null || isThroughTriggerOpen || triggerType == ETriggerType.Actived) return false;
        return true;
    }

    public virtual bool OnTriggerExit2DCall(Transform intruder)
    {
        if (collider2D == null || triggerType == ETriggerType.Actived) return false;
        exitPos = GetComponent<Collider>().bounds.ClosestPoint(intruder.position);
        if (isThroughTriggerOpen && !TransformHelper.CheckThroughTrigger(enterPos, exitPos, transform)) return false;
        if (isTestTrigger) TriggerType = ETriggerType.InActive;
        return true;
    }

    public virtual void Update()
    {
        if (isTestTrigger) Init();
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void LateUpdate()
    {
    }
}