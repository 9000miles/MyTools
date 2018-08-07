using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTETest : MonoBehaviour
{
    private QTEInfo info1;
    private QTEInfo info2;
    private QTEInfo info3;
    private QTEInfo info4;

    private void Start()
    {
        info1 = new QTEInfo();
        info1.description = "按A键";
        info1.type = QTEType.KeyCombination;
        info1.keyCombination = new KeyCombinationInfo();
        info1.keyCombination.keyList = new List<QTEKeyCode>();
        info1.keyCombination.keyList.Add(QTEKeyCode.A);

        info2 = new QTEInfo();
        info2.description = "按住左键下滑键";
        info2.type = QTEType.MouseGestures;
        info2.mouseGestures = new MouseGesturesInfo();
        info2.mouseGestures.gesturesType = MouseGesturesType.DownSlide;
        info2.mouseGestures.mouseButton = QTEMouseButton.LeftButton;

        //info3 = new QTEInfo();
        //info3.description = "按住左键下滑键";
        //info3.type = QTEType.MouseGestures;
        //info3.mouseGestures = new MouseGesturesInfo();
        //info3.mouseGestures.gesturesType = MouseGesturesType.DownSlide;
        //info3.mouseGestures.mouseButton = QTEMouseButton.LeftButton;

        //info4 = new QTEInfo();
        //info4.description = "按住左键下滑键";
        //info4.type = QTEType.MouseGestures;
        //info4.mouseGestures = new MouseGesturesInfo();
        //info4.mouseGestures.gesturesType = MouseGesturesType.DownSlide;
        //info4.mouseGestures.mouseButton = QTEMouseButton.LeftButton;
    }

    private void Update()
    {
    }

    public void QTEExcute_1()
    {
        QTEManager.Singleton.ManualExcuteQTE(info1, (info1) =>
        {
            Debug.Log(info1.type + "   " + info1.excuteTime.ToString() + "   " + info1.result);
        });
    }

    public void QTEExcute_2()
    {
        QTEManager.Singleton.ManualExcuteQTE(info2, (info2) =>
        {
            Debug.Log(info2.type + "   " + info2.excuteTime.ToString() + "   " + info2.result);
        });
    }
}