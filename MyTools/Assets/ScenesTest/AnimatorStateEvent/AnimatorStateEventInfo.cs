using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateEventInfo
{
    public bool isInState;
    public bool isDelayCalled;
    public AnimationClip clip;
    public AnimatorStateEvent stateEvent;
    public Dictionary<float, List<Action>> onDelayCallDic = new Dictionary<float, List<Action>>();
}