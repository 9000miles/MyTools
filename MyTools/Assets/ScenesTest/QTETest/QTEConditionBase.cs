using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QTEConditionBase : MonoBehaviour
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

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        owerTF = transform;
        //infoList = new List<QTEInfo>();
    }

    protected abstract bool Check();

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