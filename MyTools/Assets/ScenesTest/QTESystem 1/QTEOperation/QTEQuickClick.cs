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

    public override void ExcuteAndCheck(QTEInfo info)
    {
        base.ExcuteAndCheck(info);
        QTETipPanel.Singleton.ShowScrollBar(true, info.quickClick.speed);
        switch (info.quickClick.mouseButton)
        {
            case QTEMouseButton.LeftButton:
                if (Input.GetMouseButtonDown(0))
                {
                    lastClickTime = Time.time;
                    clickCount++;
                    leftClickCount++;
                    info.excuteTime = Time.time - info.startTime;
                    info.result = QTETipPanel.Singleton.ShowScrollBar(false);
                }
                break;

            case QTEMouseButton.RightButtton:
                if (Input.GetMouseButtonDown(1))
                {
                    lastClickTime = Time.time;
                    clickCount++;
                    rightClickCount++;
                    info.excuteTime = Time.time - info.startTime;
                    info.result = QTETipPanel.Singleton.ShowScrollBar(false);
                }
                break;

            case QTEMouseButton.MiddleButton:
                if (Input.GetMouseButtonDown(2))
                {
                    lastClickTime = Time.time;
                    clickCount++;
                    middleClickCount++;
                    info.excuteTime = Time.time - info.startTime;
                    info.result = QTETipPanel.Singleton.ShowScrollBar(false);
                }
                break;
        }

        //if (clickCount >= info.quickClick.clickCount)
        //{
        //    info.excuteTime = Time.time - info.startTime;
        //    info.result = QTEResult.Succed;
        //    info.errorType = QTEErrorType.None;
        //}
    }
}