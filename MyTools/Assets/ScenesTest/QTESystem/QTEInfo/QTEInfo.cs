using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

[Serializable]
public class QTEInfo
{
    public bool isActive;
    public int ID;
    public float duration = 10;
    public float startTime;
    public float excuteTime;
    public string description = "";
    public QTEType type = QTEType.None;
    public QuickClickInfo quickClick = null;
    public PreciseClickInfo preciseClick = null;
    public MouseGesturesInfo mouseGestures = null;
    public KeyCombinationInfo keyCombination = null;

    public Vector2 UILocalPosition = new Vector2(-560, 475);
    public QTEResult result;
    public QTEErrorType errorType;
    public Animation animation;
    public CinemachineVirtualCameraBase cinemachine;

    public QTEInfo()
    {
    }

    public QTEInfo(string description, int id, float time, Vector2 position, QTEType type)
    {
        this.ID = id;
        this.type = type;
        this.duration = time;
        this.description = description;
        this.UILocalPosition = position;
        this.result = QTEResult.None;
    }

    public void ResetQTEInfo()
    {
        this.ID = 0;
        this.description = "";
        this.startTime = 0;
        this.duration = 0;
        this.isActive = false;
        this.UILocalPosition = Vector2.zero;
        this.type = QTEType.None;
        this.result = QTEResult.None;
        //this.keyList = null;
        this.errorType = QTEErrorType.None;
        this.animation = null;
        this.cinemachine = null;
    }

    public void ResetQTEInfo(bool isManual)
    {
        this.ID = 0;
        this.startTime = 0;
        this.isActive = false;
        this.result = QTEResult.None;
        //this.keyList = null;
        this.errorType = QTEErrorType.None;
        this.animation = null;
        this.cinemachine = null;
    }
}

[Serializable]
public class QuickClickInfo
{
    public int clickCount = 2;
    public float IntervalTime = 0.5f;
    public QTEMouseButton mouseButton = QTEMouseButton.LeftButton;
}

[Serializable]
public class PreciseClickInfo
{
    //public Vector2 clickScreenPosition;
    public List<GameObject> targetList = null;
    public QTEMouseButton mouseButton = QTEMouseButton.LeftButton;
}

[Serializable]
public class MouseGesturesInfo
{
    //[Range(50, 100)]
    //public float recognitionRate = 90;
    public float angleLimit = 30;
    public float augularOffset = 30;//角度偏移误差
    public QTEMouseButton mouseButton = QTEMouseButton.LeftButton;
    public MouseGesturesType gesturesType = MouseGesturesType.None;
}

[Serializable]
public class KeyCombinationInfo
{
    public List<QTEKeyCode> keyList = new List<QTEKeyCode>();
}

/// <summary>
/// QTE按键种类
/// </summary>
public enum QTEKeyCode
{
    Space = 32,
    A = 97,
    B = 98,
    E = 101,
    H = 104,
    P = 112,
    Q = 113,
    R = 114,
    Y = 121,
}

public enum QTEBehaviorType
{
    PlayAnimation,
    ChangeCamera,
}

public enum QTEErrorType
{
    None = 0,
    OverTime = 1,
    OperatingError = 2,
}

/// <summary>
/// QTE类型
/// </summary>
public enum QTEType
{
    None = 0,
    /// <summary>
    /// 快速点击
    /// </summary>
    QuickClick = 1,
    /// <summary>
    /// 精准点击
    /// </summary>
    PreciseClick = 2,
    /// <summary>
    /// 鼠标手势
    /// </summary>
    MouseGestures = 3,
    /// <summary>
    /// 按键组合
    /// </summary>
    KeyCombination = 4,
    /// <summary>
    /// 其它
    /// </summary>
    Others = 999,
}

public enum MouseGesturesType
{
    LeftSlide,//←
    LeftUpSlide,//↖
    LeftDownSlide,//↙
    RightSlide,//→
    RightUpSilde,//↗
    RightDownSlide,//↘
    UpSlide,//↑
    DownSlide,//↓
    CheckMark,
    Capital_C,
    Capital_Z,
    Capital_U,
    Capital_O,
    Capital_S,
    Capital_L,
    None,
}

public enum QTEMouseButton
{
    //None,
    LeftButton = 0,
    RightButtton = 1,
    MiddleButton = 2,
}

public enum QTEResult
{
    None = 0,
    Failure = 1,
    Succed = 2,
}