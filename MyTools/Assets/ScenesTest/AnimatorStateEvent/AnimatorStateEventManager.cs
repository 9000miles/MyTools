using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateEventManager : SingletonTemplate<AnimatorStateEventManager>
{
    public Dictionary<Animator, List<AnimatorStateEventInfo>> animatorStateEventDic = new Dictionary<Animator, List<AnimatorStateEventInfo>>();

    public void Add(Animator animator, AnimatorStateEvent stateEvent)
    {
        AnimatorStateEventInfo stateEventInfo = new AnimatorStateEventInfo() { stateEvent = stateEvent, };
    }
}