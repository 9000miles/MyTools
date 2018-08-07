using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEKeyCombination : QTEOperationBase
{
    /// <summary> 记录按键按下次数 </summary>
    private int keyDownCount;
    private List<QTEKeyCode> keyList;
    private static QTEKeyCombination singleton = null;
    public static QTEKeyCombination Singleton
    {
        get
        {
            if (singleton == null)
                singleton = new QTEKeyCombination();
            return singleton;
        }
    }

    public QTEKeyCombination() : base()
    {
        keyList = new List<QTEKeyCode>();
    }

    public override void ResetData()
    {
        keyList.Clear();
        keyDownCount = 0;
    }

    public override void ExcuteAndCheck(QTEInfo info)
    {
        base.ExcuteAndCheck(info);
        if (isInTime == false) return;

        if (keyList.Count == 0)
        {
            QTEKeyCode[] keys = new QTEKeyCode[info.keyCombination.keyList.Count];
            info.keyCombination.keyList.CopyTo(keys);
            keyList.AddRange(keys);
        }

        if (Input.anyKeyDown &&//会检测鼠标点击操作，包括鼠标上的扩展按键
           QTEManager.Singleton.keyCode != KeyCode.None &&
         ((int)QTEManager.Singleton.keyCode < 323 || (int)QTEManager.Singleton.keyCode > 329))
        {
            keyDownCount++;//鼠标操作第一次，会进入
            //Debug.Log("InputCount：" + keyDownCount + "          keyCode：" + QTEManager.Singleton.keyCode);
            QTEManager.Singleton.keyCode = KeyCode.None;
        }
        //检查操作是否正确
        if (Input.GetKeyDown(keyList[0].ToString().ToLower()))
        {
            //Debug.Log("按下了：" + info.keyList[0]);
            //info.keyCombination.keyList.RemoveAt(0);
            keyList.RemoveAt(0);
        }
        if (keyList.Count <= 0)
        {
            info.excuteTime = Time.time - info.startTime;
            info.result = QTEResult.Succed;
            info.errorType = QTEErrorType.None;
        }
    }
}