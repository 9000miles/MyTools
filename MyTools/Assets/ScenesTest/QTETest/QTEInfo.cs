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
    public float duration;
    public float startTime;
    public string description;
    public QTEType type;
    public QTEResult result;
    public Vector2 UILocalPosition;
    public List<QTEKeyCode> keyList;
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
        this.keyList = null;
        this.errorType = QTEErrorType.None;
        this.animation = null;
        this.cinemachine = null;
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
}

public enum QTEBehaviorType
{
    PlayAnimation,
    ChangeCamera,
}

public enum QTEErrorType
{
    None,
    OverTime,
    OperatingError,
}

/// <summary>
/// QTE类型
/// </summary>
public enum QTEType
{
    None,
    /// <summary>
    /// 快速点击
    /// </summary>
    QuickClick,
    /// <summary>
    /// 精准点击
    /// </summary>
    PreciseClick,
    /// <summary>
    /// 鼠标手势
    /// </summary>
    MouseGestures,
    /// <summary>
    /// 按键组合
    /// </summary>
    KeyCombination,
    /// <summary>
    /// 其它
    /// </summary>
    Others,
}

public enum QTEResult
{
    None,
    Failure,
    Succed,
}