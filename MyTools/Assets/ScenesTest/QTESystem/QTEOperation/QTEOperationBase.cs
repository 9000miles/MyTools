using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.Experimental.UIElements;

public abstract class QTEOperationBase/* : SingletonTemplate<QTEOperationBase>*/
{
    protected bool isInTime;
    //public MouseGestures mouseGestures;

    public QTEOperationBase()
    {
    }

    public abstract void ResetData();

    public virtual void ExcuteCheck(QTEInfo info)
    {
        CheckIsInTime(info);
    }

    private void CheckIsInTime(QTEInfo info)
    {
        //检查是否在时间范围内
        if (Time.time <= info.startTime + info.duration)
        {
            info.result = QTEResult.None;
            info.errorType = QTEErrorType.None;
            isInTime = true;
        }
        else
        {
            info.result = QTEResult.Failure;
            info.errorType = QTEErrorType.OverTime;
            isInTime = false;
        }
    }
}