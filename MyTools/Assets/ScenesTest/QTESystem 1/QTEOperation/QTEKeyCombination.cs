using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEKeyCombination : QTEOperationBase
{
    private bool hasKeyDown;
    private int keyDownCount;
    private float time;
    private float lastKeyDownTime;
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
        hasKeyDown = false;
        keyList = new List<QTEKeyCode>();
    }

    public override void ResetData()
    {
        keyDownCount = 0;
        lastKeyDownTime = 0;
        keyList.Clear();
        hasKeyDown = false;
    }

    public override void ExcuteAndCheck(QTEInfo info)
    {
        base.ExcuteAndCheck(info);
        if (isInTime == false) return;

        time = Time.time - info.startTime;
        if (keyList.Count == 0)
        {
            QTEKeyCode[] keys = new QTEKeyCode[info.keyCombination.keyList.Count];
            info.keyCombination.keyList.CopyTo(keys);
            keyList.AddRange(keys);
            //显示操作提示
            switch (info.keyCombination.combinationType)
            {
                case KeyCombinationInfo.KeyCombinationType.SingleKeyContinue:
                    break;

                case KeyCombinationInfo.KeyCombinationType.SingleKeyRhythm:
                    QTETipPanel.Singleton.ShowSingleKeyRhythm(true, false, info.duration);
                    break;

                case KeyCombinationInfo.KeyCombinationType.DoubleKeyRepeat:
                    QTETipPanel.Singleton.ShowDoubleKeyRepeat(true, true, info.keyCombination.keyList[keyDownCount % 2]);
                    break;

                case KeyCombinationInfo.KeyCombinationType.LinearClick:
                    QTETipPanel.Singleton.ShowLinearClick(true, false);
                    break;

                case KeyCombinationInfo.KeyCombinationType.LinearDirection:
                    QTETipPanel.Singleton.ShowLinearDirection(true, false, keyList[0].ToString());
                    break;
            }
        }

        //执行相应操作检查
        switch (info.keyCombination.combinationType)
        {
            case KeyCombinationInfo.KeyCombinationType.SingleKeyContinue:
                CheckSingleKeyContinue(info);
                break;

            case KeyCombinationInfo.KeyCombinationType.SingleKeyRhythm:
                hasKeyDown = true;
                CheckKeyIsRight(info);
                CheckSingleKeyRhythm(info);
                break;

            case KeyCombinationInfo.KeyCombinationType.DoubleKeyRepeat:
                CheckDoubleKeyRepeat(info);
                break;

            case KeyCombinationInfo.KeyCombinationType.LinearClick:
                hasKeyDown = true;
                CheckKeyIsRight(info);
                CheckLinearClick(info);
                break;

            case KeyCombinationInfo.KeyCombinationType.LinearDirection:
                hasKeyDown = true;
                CheckKeyIsRight(info);
                CheckLinearDirection(info);
                break;
        }

        //检查按键是否按完
        if (keyList.Count <= 0)
        {
            info.excuteTime = Time.time - info.startTime;
            info.result = QTEResult.Succed;
            info.errorType = QTEErrorType.None;
        }
    }

    private void CheckKeyIsRight(QTEInfo info)
    {
        //检查按键操作是否正确
        if (hasKeyDown && ((int)QTEManager.Singleton.keyCode != (int)keyList[0]) &&
            QTEManager.Singleton.keyCode != KeyCode.None)
        {
            info.excuteTime = Time.time - info.startTime;
            info.result = QTEResult.Failure;
            info.errorType = QTEErrorType.OperatingError;
            //操作错误则关闭提示面板
            switch (info.keyCombination.combinationType)
            {
                case KeyCombinationInfo.KeyCombinationType.SingleKeyContinue:
                    break;

                case KeyCombinationInfo.KeyCombinationType.SingleKeyRhythm:
                    QTETipPanel.Singleton.ShowSingleKeyRhythm(false);
                    break;

                case KeyCombinationInfo.KeyCombinationType.DoubleKeyRepeat:
                    break;

                case KeyCombinationInfo.KeyCombinationType.LinearClick:
                    QTETipPanel.Singleton.ShowLinearClick(false);
                    break;

                case KeyCombinationInfo.KeyCombinationType.LinearDirection:
                    QTETipPanel.Singleton.ShowLinearDirection(false);
                    break;
            }
        }
    }

    private void CheckSingleKeyContinue(QTEInfo info)
    {
        if (Input.GetKeyDown(keyList[0].ToString().ToLower()))
        {
            QTETipPanel.Singleton.ShowSingleKeyContinue(true);
            keyDownCount++;
            hasKeyDown = true;
            keyList.RemoveAt(0);
            QTEManager.Singleton.keyCode = KeyCode.None;
        }
    }

    private void CheckSingleKeyRhythm(QTEInfo info)
    {
        if (Input.GetKeyDown(keyList[0].ToString().ToLower()))
        {
            if (time > 0 && 100f - (time / info.duration * 100f) < info.keyCombination.errorRange)
            {
                QTETipPanel.Singleton.ShowSingleKeyRhythm(true, true, info.duration);
                hasKeyDown = true;
                info.startTime = Time.time;
                keyList.RemoveAt(0);
                QTEManager.Singleton.keyCode = KeyCode.None;
            }
            else
            {
                info.result = QTEResult.Failure;
                info.errorType = QTEErrorType.OperatingError;
                QTETipPanel.Singleton.ShowSingleKeyRhythm(false);
            }
        }
    }

    private void CheckDoubleKeyRepeat(QTEInfo info)
    {
        if (Input.GetKeyDown(keyList[keyDownCount % 2].ToString().ToLower()))
        {
            hasKeyDown = true;
            bool isLeft = keyDownCount % 2 == 0;
            QTETipPanel.Singleton.ShowDoubleKeyRepeat(true, isLeft, info.keyCombination.keyList[keyDownCount % 2]);
            keyDownCount++;
            info.startTime = Time.time;
            QTEManager.Singleton.keyCode = KeyCode.None;
        }

        if (keyDownCount > 0 && 100f - (keyDownCount / info.keyCombination.keyCount * 100f) < info.keyCombination.errorRange)
        {
            info.result = QTEResult.Failure;
            info.errorType = QTEErrorType.OperatingError;
            QTETipPanel.Singleton.ShowDoubleKeyRepeat(false);
        }

        //检查是否达到按键次数
        if (keyDownCount >= info.keyCombination.keyCount)
        {
            info.excuteTime = Time.time - info.startTime;
            info.result = QTEResult.Succed;
            info.errorType = QTEErrorType.None;
        }
    }

    private void CheckLinearClick(QTEInfo info)
    {
        if (Input.GetKeyDown(keyList[0].ToString().ToLower()))
        {
            QTETipPanel.Singleton.ShowLinearClick(true, true);
            hasKeyDown = true;
            keyList.RemoveAt(0);
            QTEManager.Singleton.keyCode = KeyCode.None;
        }
    }

    private void CheckLinearDirection(QTEInfo info)
    {
        if (Input.GetKeyDown(keyList[0].ToString().ToLower()))
        {
            QTETipPanel.Singleton.ShowLinearDirection(true, true, keyList[0].ToString());
            hasKeyDown = true;
            keyList.RemoveAt(0);
            QTEManager.Singleton.keyCode = KeyCode.None;
        }
    }
}