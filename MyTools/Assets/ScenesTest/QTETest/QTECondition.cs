using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QTECondition : MonoBehaviour
{
    public bool isTrue;
    private bool isStartTimeHasSet;
    [HideInInspector]
    public Transform ower;
    public List<QTEInfo> infoList;
    public QTEInfo currentQTEInfo;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        ower = transform;
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
            currentQTEInfo = null;
            isStartTimeHasSet = false;
            QTEOperationBase.Instance.EnptyResult();
            QTEManager.Instance.keyList.Clear();
        }
    }
}