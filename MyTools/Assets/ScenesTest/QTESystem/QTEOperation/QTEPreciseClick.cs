﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEPreciseClick : QTEOperationBase
{
    private RaycastHit hit;
    private static QTEPreciseClick singleton = null;
    public static QTEPreciseClick Singleton
    {
        get
        {
            if (singleton == null)
                singleton = new QTEPreciseClick();
            return singleton;
        }
    }

    public QTEPreciseClick() : base()
    {
    }

    public override void ExcuteCheck(QTEInfo info)
    {
        base.ExcuteCheck(info);
        if (Input.GetMouseButtonDown((int)info.preciseClick.mouseButton))
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            if (hit.transform.gameObject == info.preciseClick.targetList[0])
            {
                info.preciseClick.targetList.RemoveAt(0);
            }
            else
            {
                info.result = QTEResult.Failure;
                info.errorType = QTEErrorType.OperatingError;
            }
        }
        //if ((int)QTEManager.Singleton.keyCode >= 323 &&
        //    (int)QTEManager.Singleton.keyCode <= 329 &&
        //    (int)QTEManager.Singleton.keyCode != (int)info.preciseClick.mouseButton + 323)//按键操作错误
        //if ((int)QTEManager.Singleton.keyCode != (int)info.preciseClick.mouseButton + 323 && QTEManager.Singleton.eventType == EventType.MouseDown)
        //{
        //    info.result = QTEResult.Failure;
        //    info.errorType = QTEErrorType.OperatingError;
        //}
        if (info.preciseClick.targetList.Count <= 0)
        {
            info.result = QTEResult.Succed;
            info.errorType = QTEErrorType.None;
        }
    }

    public override void ResetData()
    {
    }
}