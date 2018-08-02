using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

[Serializable]
public class QTEInfo
{
    public bool isActive;
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

    public QTEInfo(string description, float time, Vector2 position, QTEType type)
    {
        this.type = type;
        this.duration = time;
        this.description = description;
        this.UILocalPosition = position;
        this.result = QTEResult.None;
    }

    public void ResetQTEInfo()
    {
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

public enum QTEType
{
    None,
    QuickClick,
    PreciseClick,
    MouseGestures,
    KeyCombination,
    Others,
}

public enum QTEResult
{
    None,
    Failure,
    Succed,
}