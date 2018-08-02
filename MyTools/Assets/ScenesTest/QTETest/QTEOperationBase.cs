using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.Experimental.UIElements;

public class QTEOperationBase : SingletonTemplate<QTEOperationBase>
{
    protected bool isInTime;
    public int clickCount;
    public float time;
    /// <summary>
    /// 计算按键按下次数
    /// </summary>
    protected static int keyDownCount;
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
            keyDownCount = 0;
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
    public override QTEOperationBase GetSingleton()
    {
        //return base.GetSingleton();
        if (singleton == null)
        {
            singleton = new QTEOperationBase();
        }
        return singleton;
    }

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
        if (Input.anyKeyDown &&//会检测鼠标点击操作，包括鼠标上的扩展按键
           QTEManager.Singleton.keyCode != KeyCode.None)
        {
            keyDownCount++;//鼠标操作第一次，会进入
            Debug.Log("InputCount           " + keyDownCount + "          keyCode    " + QTEManager.Singleton.keyCode);
            QTEManager.Singleton.keyCode = KeyCode.None;
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
            keyDownCount = 0;
        }
        //else
        //{
        //    info.result = QTEResult.Failure;
        //    info.errorType = QTEErrorType.OperatingError;
        //    return;
        //}
    }
}