using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

[Serializable]
public class QTEInfo
{
    public bool isAutomaticActive;
    //[HideInInspector]
    public bool isActive;
    [HideInInspector]
    public int ID;
    public float duration;
    [HideInInspector]
    public float startTime;
    [HideInInspector]
    public float excuteTime;
    public string description;
    public QTEType type;
    //[HideInInspector]
    public QuickClickInfo quickClick;
    //[HideInInspector]
    public PreciseClickInfo preciseClick;
    //[HideInInspector]
    public MouseGesturesInfo mouseGestures;
    //[HideInInspector]
    public KeyCombinationInfo keyCombination;

    [HideInInspector]
    public Vector2 UILocalPosition;
    [HideInInspector]
    public QTEResult result;
    [HideInInspector]
    public QTEErrorType errorType;
    [HideInInspector]
    public Animation animation;
    [HideInInspector]
    public CinemachineVirtualCameraBase cinemachine;

    public QTEInfo()
    {
        duration = 10;
        description = "QTE is Null";
        type = QTEType.None;
        UILocalPosition = new Vector2(-560, 475);
        quickClick = new QuickClickInfo();
        preciseClick = new PreciseClickInfo();
        mouseGestures = new MouseGesturesInfo();
        keyCombination = new KeyCombinationInfo();
    }

    /// <summary>
    /// 重置QTE
    /// </summary>
    /// <param name="isReuse">是否重复使用</param>
    /// <param name="intervalTime">再次启动的间隔时间</param>
    public async void ResetQTEInfo(bool isReuse = true, float intervalTime = 2f)
    {
        if (isReuse)
        {
            this.isAutomaticActive = false;//是否需要间隔一定时间之后再启动
            this.isActive = false;
            this.startTime = 0;
            this.result = QTEResult.None;
            this.errorType = QTEErrorType.None;
            //await new WaitForSeconds(intervalTime);
            //this.isActive = true;
        }
        else
        {
            this.isAutomaticActive = false;
            this.ID = 0;
            this.description = "QTE is Null";
            this.startTime = 0;
            this.duration = 0;
            this.isActive = false;
            this.UILocalPosition = Vector2.zero;
            this.type = QTEType.None;
            this.result = QTEResult.None;
            this.errorType = QTEErrorType.None;
            this.animation = null;
            this.cinemachine = null;
        }
    }
}

[Serializable]
public class QuickClickInfo
{
    public int clickCount = 5;
    public float speed = 150;
    //public float TimeLimit = 2;
    //public float IntervalTime = 0.5f;
    public QTEMouseButton mouseButton = QTEMouseButton.LeftButton;
}

[Serializable]
public class PreciseClickInfo
{
    public PreciseClickType preciseClickType;
    public float percentage = 1f;
    public float rotateDelta = 10;
    //[SerializeField]
    public List<GameObject> targetList = new List<GameObject>();
    public QTEMouseButton mouseButton = QTEMouseButton.LeftButton;

    public enum PreciseClickType
    {
        PowerGauge,
        FocusPoint,
    }
}

[Serializable]
public class MouseGesturesInfo
{
    public MouseGesturesType gesturesType = MouseGesturesType.None;
    //[Range(50, 100)]
    //public float recognitionRate = 90;
    public bool isShowCountdown;
    public float angleLimit = 30;
    public float length = 30;
    //public float augularOffset = 30;//角度偏移误差
    public QTEMouseButton mouseButton = QTEMouseButton.LeftButton;
}

[Serializable]
public class KeyCombinationInfo
{
    public int keyCount = 10;
    [Tooltip("误差范围百分比%")]
    public float errorRange = 25;
    public KeyCombinationType combinationType;
    public List<QTEKeyCode> keyList = new List<QTEKeyCode>();

    public enum KeyCombinationType
    {
        /// <summary>
        /// 单键连续式
        /// </summary>
        SingleKeyContinue,//OK
        /// <summary>
        /// 单键节奏式
        /// </summary>
        SingleKeyRhythm,//OK
        /// <summary>
        /// 双键反复式
        /// </summary>
        DoubleKeyRepeat,
        /// <summary>
        /// 线性点击式
        /// </summary>
        LinearClick,//与单键连续式相同
        /// <summary>
        /// 线性方向式
        /// </summary>
        LinearDirection,
    }
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
    Up = 273,
    Down = 274,
    Right = 275,
    Left = 276,
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
    SubOptimal = 2,
    Succed = 3,
}