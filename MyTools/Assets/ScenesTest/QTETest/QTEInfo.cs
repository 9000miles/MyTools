using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

[Serializable]
public class QTEInfo
{
    public string description;
    public float startTime;
    public float duration;
    public bool isActive;
    public Vector2 UILocalPosition;
    public QTEType type;
    public QTEResult result;
    public List<KeyCode> keyList;
    public QTEErrorType errorType;
    public Animation animation;
    public CinemachineVirtualCameraBase cinemachine;

    public QTEInfo()
    {
    }

    public QTEInfo(string description, float time, Vector2 position, QTEType type)
    {
        this.description = description;
        this.duration = time;
        this.UILocalPosition = position;
        this.type = type;
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
    A,
    B,
    E,
    Q,
    R,
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
    Succed,
    Failure,
}