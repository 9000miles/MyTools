using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

namespace MarsPC
{
    [Serializable]
    public class QTEInfo
    {
        public bool isAutomaticActive;
        public bool isActive;
        public int ID;
        public float duration;
        [HideInInspector]
        public float startTime;
        [HideInInspector]
        public float excuteTime;
        public string description;
        public EQTEType type;

        public SingleKeyContinue singleKeyContinue;
        public SingleKeyPhythm singleKeyPhythm;
        public DoubleKeyRepeat doubleKeyRepeat;
        public LinearClick linearClick;
        public LinearDirection linearDirection;
        public ScrollBarClick scrollBarClick;
        public PowerGauge powerGauge;
        public MouseGestures mouseGestures;
        public FocusPoint focusPoint;

        [HideInInspector]
        public Vector2 UILocalPosition;
        [HideInInspector]
        public EQTEResult result;
        [HideInInspector]
        public EQTEErrorType errorType;

        public QTEInfo()
        {
            duration = 10;
            description = "QTE is Null";
            type = EQTEType.None;
            UILocalPosition = new Vector2(-560, 475);
            singleKeyContinue = new SingleKeyContinue();
            singleKeyPhythm = new SingleKeyPhythm();
            doubleKeyRepeat = new DoubleKeyRepeat();
            linearClick = new LinearClick();
            linearDirection = new LinearDirection();
            scrollBarClick = new ScrollBarClick();
            powerGauge = new PowerGauge();
            mouseGestures = new MouseGestures();
            focusPoint = new FocusPoint();
        }

        /// <summary>
        /// 重置QTE
        /// </summary>
        /// <param name="isReuse">是否重复使用</param>
        /// <param name="intervalTime">再次启动的间隔时间</param>
        public /*async*/ void ResetQTEInfo(bool isReuse = true, float intervalTime = 2f)
        {
            if (isReuse)
            {
                this.isAutomaticActive = false;//是否需要间隔一定时间之后再启动
                this.isActive = false;
                this.startTime = 0;
                this.result = EQTEResult.None;
                this.errorType = EQTEErrorType.None;
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
                this.type = EQTEType.None;
                this.result = EQTEResult.None;
                this.errorType = EQTEErrorType.None;
            }
        }
    }

    /// <summary>
    /// 单键连续式
    /// </summary>
    [Serializable]
    public class SingleKeyContinue
    {
        //public int keyCount = 10;
        public List<EQTEKeyCode> keyList = new List<EQTEKeyCode>();
    }

    /// <summary>
    /// 单键节奏式
    /// </summary>
    [Serializable]
    public class SingleKeyPhythm
    {
        public float errorRange = 25;
        public List<EQTEKeyCode> keyList = new List<EQTEKeyCode>();
    }

    /// <summary>
    /// 双键反复式
    /// </summary>
    [Serializable]
    public class DoubleKeyRepeat
    {
        public int keyCount = 10;
        public float errorRange = 25;
        public List<EQTEKeyCode> keyList = new List<EQTEKeyCode>();
    }

    /// <summary>
    /// 线性点击式
    /// </summary>
    [Serializable]
    public class LinearClick
    {
        public List<EQTEKeyCode> keyList = new List<EQTEKeyCode>();
    }

    /// <summary>
    /// 线性方向式
    /// </summary>
    [Serializable]
    public class LinearDirection
    {
        public List<EQTEKeyCode> keyList = new List<EQTEKeyCode>();
    }

    /// <summary>
    /// 滚动条式
    /// </summary>
    [Serializable]
    public class ScrollBarClick
    {
        public float speed = 150;
        public EQTEMouseButton mouseButton = EQTEMouseButton.LeftButton;
    }

    /// <summary>
    /// 蓄力式
    /// </summary>
    [Serializable]
    public class PowerGauge
    {
        public bool isKeyboard = true;
        public float rotateDelta = 10;
        public List<EQTEKeyCode> keyList = new List<EQTEKeyCode>();
        public List<GameObject> targetList = new List<GameObject>();
        public EQTEMouseButton mouseButton = EQTEMouseButton.LeftButton;
    }

    /// <summary>
    /// 鼠标手势
    /// </summary>
    [Serializable]
    public class MouseGestures
    {
        public EMouseGesturesType gesturesType = EMouseGesturesType.None;
        public bool isShowCountdown;
        public float angleLimit = 30;
        public float length = 30;
        public EQTEMouseButton mouseButton = EQTEMouseButton.LeftButton;
    }

    /// <summary>
    /// 焦点指向式
    /// </summary>
    [Serializable]
    public class FocusPoint
    {
        public bool isUIObject = true;
        public float percentage = 1f;
        public List<GameObject> targetList = new List<GameObject>();
        public EQTEMouseButton mouseButton = EQTEMouseButton.LeftButton;
    }

    /// <summary>
    /// QTE按键种类
    /// </summary>
    public enum EQTEKeyCode
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

    public enum EQTEErrorType
    {
        None = 0,
        OverTime = 1,
        OperatingError = 2,
    }

    /// <summary>
    /// QTE类型
    /// </summary>
    public enum EQTEType
    {
        None = 0,
        SingleKeyContinue,
        SingleKeyPhythm,
        DoubleKeyRepeat,
        LinearClick,
        LinearDirection,
        ScrollBarClick,
        PowerGauge,
        MouseGestures,
        FocusPoint,
    }

    public enum EMouseGesturesType
    {
        LeftSlide,//←
        LeftUpSlide,//↖
        LeftDownSlide,//↙
        RightSlide,//→
        RightUpSilde,//↗
        RightDownSlide,//↘
        UpSlide,//↑
        DownSlide,//↓
        None,
    }

    public enum EQTEMouseButton
    {
        LeftButton = 0,
        RightButtton = 1,
        MiddleButton = 2,
    }

    public enum EQTEResult
    {
        None = 0,
        Failure = 1,
        SubOptimal = 2,
        Succed = 3,
    }
}