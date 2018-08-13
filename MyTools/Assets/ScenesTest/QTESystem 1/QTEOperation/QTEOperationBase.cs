using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QTEOperationBase
{
    protected bool isInTime;

    public QTEOperationBase()
    {
    }

    public abstract void ResetData();

    public virtual void ExcuteAndCheck(QTEInfo info)
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
            info.excuteTime = Time.time - info.startTime;
            info.result = QTEResult.Failure;
            info.errorType = QTEErrorType.OverTime;
            isInTime = false;
            QTETipPanel.Singleton.ShowSingleKeyContinue(false);
            QTETipPanel.Singleton.ShowSingleKeyRhythm(false);
            QTETipPanel.Singleton.ShowDoubleKeyRepeat(false);
            QTETipPanel.Singleton.ShowLinearDirection(false);
            QTETipPanel.Singleton.ShowLinearClick(false);
            QTETipPanel.Singleton.ShowScrollBar(false);
        }
    }
}