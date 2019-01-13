using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateEvent
{
    public string fullName;
    public float delayTime;
    public Action OnEnterCall;
    public Action OnUpdateCall;
    public Action OnExitCall;
    public Action OnDelayCall;
}