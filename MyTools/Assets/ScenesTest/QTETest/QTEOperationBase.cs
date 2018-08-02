using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class QTEOperationBase : SingletonTemplate<QTEOperationBase>
{
    protected bool isInTime;
    public int clickCount;
    public float time;
    protected int inputCount;
    public Vector2 clickPosition;
    //public MouseGestures mouseGestures;

    private void Start()
    {
    }

    public QTEOperationBase()
    {
    }

    public void EmptyResult()
    {
        clickCount = 0;
        time = 0;
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
            isInTime = false;
            inputCount = 0;
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
        base.Excute(info);
    }

    protected override void CheckIsInTime(QTEInfo info)
    {
        base.CheckIsInTime(info);
        if (isInTime == false) return;
        if (Input.anyKeyDown)
        {
            inputCount++;
            Debug.Log("InputCount           " + inputCount);
        }
        //检查操作是否正确
        if (Input.GetKeyDown(info.keyList[0].ToString().ToLower()))
        {
            //Debug.Log("按下了：" + info.keyList[0]);
            info.keyList.RemoveAt(0);
        }
        if (info.keyList.Count <= 0)
        {
            info.result = QTEResult.Succed;
            info.errorType = QTEErrorType.None;
            inputCount = 0;
        }
        //else
        //{
        //    info.result = QTEResult.Failure;
        //    info.errorType = QTEErrorType.OperatingError;
        //    return;
        //}
    }
}