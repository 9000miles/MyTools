using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEQuickClick : QTEOperationBase
{
    private int clickCount;
    private int leftClickCount;
    private int rightClickCount;
    private int middleClickCount;
    private float lastClickTime;
    private static QTEQuickClick singleton = null;
    public static QTEQuickClick Singleton
    {
        get
        {
            if (singleton == null)
                singleton = new QTEQuickClick();
            return singleton;
        }
    }

    public QTEQuickClick() : base()
    {
    }

    public override void ResetData()
    {
        clickCount = 0;
        leftClickCount = 0;
        rightClickCount = 0;
        middleClickCount = 0;
        lastClickTime = 0;
    }

    public override void ExcuteCheck(QTEInfo info)
    {
        base.ExcuteCheck(info);
        if (clickCount > 0 && Time.time > lastClickTime + info.quickClick.IntervalTime)
        {
            info.result = QTEResult.Failure;
            info.errorType = QTEErrorType.OperatingError;
        }
        switch (info.quickClick.mouseButton)
        {
            case QTEMouseButton.LeftButton:
                if (Input.GetMouseButtonDown(0))
                {
                    lastClickTime = Time.time;
                    clickCount++;
                    leftClickCount++;
                }
                break;

            case QTEMouseButton.RightButtton:
                if (Input.GetMouseButtonDown(1))
                {
                    lastClickTime = Time.time;
                    clickCount++;
                    rightClickCount++;
                }
                break;

            case QTEMouseButton.MiddleButton:
                if (Input.GetMouseButtonDown(2))
                {
                    lastClickTime = Time.time;
                    clickCount++;
                    middleClickCount++;
                }
                break;
        }
        Debug.Log(info.quickClick.mouseButton + "单击次数：  " + clickCount);
        if (clickCount == info.quickClick.clickCount)
        {
            info.result = QTEResult.Succed;
            info.errorType = QTEErrorType.None;
        }
    }
}