using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class QTEOperationBase : MonoSingleton<QTEOperationBase>
{
    protected bool isInTime;
    public int clickCount;
    public float time;
    public List<KeyCode> keyList;
    public Vector2 clickPosition;
    //public MouseGestures mouseGestures;

    private void Start()
    {
    }

    public QTEOperationBase()
    {
        keyList = new List<KeyCode>();
    }

    public void EnptyResult()
    {
        clickCount = 0;
        time = 0;
        keyList.Clear();
        clickPosition = Vector2.zero;
        //mouseGestures = null;
    }

    public virtual void Excute(QTEInfo info)
    {
        CheckIsInTime(info);
    }

    protected virtual void CheckIsInTime(QTEInfo info)
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
            QTEManager.Instance.isGetResult = true;
            isInTime = false;
        }
    }
}

public class QTEQuickClick : QTEOperationBase
{
    public QTEQuickClick() : base()
    {
    }

    public override void Excute(QTEInfo info)
    {
        throw new System.NotImplementedException();
    }

    protected override void CheckIsInTime(QTEInfo info)
    {
        base.CheckIsInTime(info);
        throw new System.NotImplementedException();
    }
}

public class QTEPreciseClick : QTEOperationBase
{
    public QTEPreciseClick() : base()
    {
    }

    public override void Excute(QTEInfo info)
    {
        throw new System.NotImplementedException();
    }

    protected override void CheckIsInTime(QTEInfo info)
    {
        base.CheckIsInTime(info);
        throw new System.NotImplementedException();
    }
}

public class QTEMouseGestures : QTEOperationBase
{
    public QTEMouseGestures() : base()
    {
    }

    public override void Excute(QTEInfo info)
    {
        throw new System.NotImplementedException();
    }

    protected override void CheckIsInTime(QTEInfo info)
    {
        base.CheckIsInTime(info);
        throw new System.NotImplementedException();
    }
}

public class QTEKeyCombination : QTEOperationBase
{
    public QTEKeyCombination() : base()
    {
    }

    public override void Excute(QTEInfo info)
    {
        keyList = QTEManager.Instance.keyList;
        base.Excute(info);
    }

    protected override void CheckIsInTime(QTEInfo info)
    {
        base.CheckIsInTime(info);
        if (isInTime == false) return;
        //检查操作是否正确
        if (info.keyList != null && keyList.Count == info.keyList.Count && keyList.Count > 0)
        {
            for (int i = 0; i < keyList.Count; i++)
            {
                if (keyList[i] != info.keyList[i])
                {
                    info.result = QTEResult.Failure;
                    info.errorType = QTEErrorType.OperatingError;
                    return;
                }
            }
            info.result = QTEResult.Succed;
            info.errorType = QTEErrorType.None;
        }
        else
        {
            info.result = QTEResult.None;
            info.errorType = QTEErrorType.None;
            return;
        }
    }
}