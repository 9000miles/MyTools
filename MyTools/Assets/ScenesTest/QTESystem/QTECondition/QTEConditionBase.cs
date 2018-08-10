using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class QTEConditionBase : SingletonBehaviour<QTEConditionBase>
{
    public bool isTrue;
    private bool isStartTimeHasSet;
    public QTEInfo infoList;
    [HideInInspector]
    public Transform owerTF;
    [HideInInspector]
    public QTEInfo currentQTEInfo;

    public QTEConditionBase()
    {
        infoList = new QTEInfo();
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
        return isActiveCondition;
    }

    public void CheckIsTrue()
    {
        isTrue = Check();
        if (isTrue)
        {
            QTEInfo info = null;
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