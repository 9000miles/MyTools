using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class QTEOperationBase : MonoSingleton<QTEOperationBase>
{
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
        Check(info);
    }

    protected virtual void Check(QTEInfo info)
    {
        //检查是否在时间范围内
        if (Time.time <= info.startTime + info.duration)
        {
            info.result = QTEResult.Succed;
            info.errorType = QTEErrorType.None;
        }
        else
        {
            info.result = QTEResult.Failure;
            info.errorType = QTEErrorType.OverTime;
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

    protected override void Check(QTEInfo info)
    {
        base.Check(info);
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

    protected override void Check(QTEInfo info)
    {
        base.Check(info);
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

    protected override void Check(QTEInfo info)
    {
        base.Check(info);
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

    protected override void Check(QTEInfo info)
    {
        base.Check(info);
        //检查操作是否正确
        if (keyList.Count == info.keyList.Count)
        {
            for (int i = 0; i < keyList.Count; i++)
            {
                if (keyList[i] != info.keyList[i])
                {
                    info.result = QTEResult.Failure;
                    return;
                }
            }
            info.result = QTEResult.Succed;
            info.errorType = QTEErrorType.None;
        }
        else
        {
            info.result = QTEResult.Failure;
            info.errorType = QTEErrorType.OperatingError;
            return;
        }
    }
}