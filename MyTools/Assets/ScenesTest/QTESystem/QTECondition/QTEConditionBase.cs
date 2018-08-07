using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public abstract class QTEConditionBase : SingletonBehaviour<QTEConditionBase>
{
    public bool isTrue;
    private bool isStartTimeHasSet;
    /// <summary>
    /// 存储该条件下的所有QTE，通过编辑器赋值，后面考虑读档赋值
    /// </summary>
    public List<QTEInfo> infoList;
    [HideInInspector]
    public Transform owerTF;
    [HideInInspector]
    public QTEInfo currentQTEInfo;

    public QTEConditionBase()
    {
        infoList = new List<QTEInfo>();
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        owerTF = transform;
        //infoList = new List<QTEInfo>();
    }

    protected virtual bool Check()
    {
        bool isActiveCondition = true;
        List<QTEInfo> activeQTE = infoList.FindAll(t => t.isActive);
        if (activeQTE.Count > 1)
        {
            isActiveCondition = false;
            Debug.LogError("请保证同时只有一个QTE被激活");
        }
        return isActiveCondition;
    }

    public void CheckIsTrue()
    {
        isTrue = Check();
        if (isTrue)
        {
            QTEInfo info = infoList.Find((t) => t.isActive);
            //if (info == null)
            //    Debug.LogError("NO QTE is Activated");
            if (info != null)
            {
                if (isStartTimeHasSet == false)
                {
                    isStartTimeHasSet = true;
                    info.startTime = Time.time;
                }
                currentQTEInfo = info;
            }
        }
        else
        {
            isStartTimeHasSet = false;
        }
    }
}