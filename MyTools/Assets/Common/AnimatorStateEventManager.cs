using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsPC
{
    public class AnimatorStateEventManager : SingletonTemplate<AnimatorStateEventManager>
    {
        /// <summary> 动画事件添加，请查看该脚本示例 </summary>
        private AnimatorStateEvent_Test animatorStateEvent_Test;
        /// <summary>
        /// 动画状态事件字典，可同时处理多个Animator状态机的动画事件
        /// </summary>
        private Dictionary<Animator, List<AnimatorStateEvent>> animatorStateEventDic = new Dictionary<Animator, List<AnimatorStateEvent>>();

        public override void Init()
        {
            base.Init();
            GameManager.Singleton.RegisterAction(EUpdateType.Update, this, Update);
        }

        /// <summary>
        /// 添加Animator
        /// </summary>
        /// <param name="animator"></param>
        private void AddAnimator(Animator animator)
        {
            if (!animatorStateEventDic.ContainsKey(animator))
            {
                animatorStateEventDic.Add(animator, new List<AnimatorStateEvent>());
            }
        }

        /// <summary>
        /// 给Animator添加StateEvent事件
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateEvent">状态事件</param>
        public void AddStateEvent(Animator animator, AnimatorStateEvent stateEvent)
        {
            if (animatorStateEventDic.ContainsKey(animator))
            {
                List<AnimatorStateEvent> list = animatorStateEventDic[animator];
                if (list == null) list = new List<AnimatorStateEvent>();

                if (!list.Exists(t => t.fullName == stateEvent.fullName))
                {
                    list.Add(stateEvent);
                    if (stateEvent.delayTime != 0)
                    {
                        AniamtorStateEventDelayCall delayCall = new AniamtorStateEventDelayCall() { delayCalled = false, delayCall = stateEvent.OnDelayCall };
                        stateEvent.delayCallDic.Add(stateEvent.delayTime, delayCall);
                    }
                }
                else
                {
                    Debug.LogError(stateEvent.fullName + " already existed");
                    AnimatorStateEvent _stateEvent = list.Find(t => t.fullName == stateEvent.fullName);

                    if (stateEvent.delayTime != 0)
                    {
                        if (!_stateEvent.delayCallDic.ContainsKey(stateEvent.delayTime))
                        {
                            AniamtorStateEventDelayCall delayCall = new AniamtorStateEventDelayCall() { delayCalled = false, delayCall = stateEvent.OnDelayCall };
                            _stateEvent.delayCallDic.Add(stateEvent.delayTime, delayCall);
                        }
                        else
                        {
                            _stateEvent.delayCallDic[stateEvent.delayTime].delayCall += stateEvent.OnDelayCall;
                        }
                    }

                    _stateEvent.OnEnterCall += stateEvent.OnEnterCall;
                    _stateEvent.OnUpdateCall += stateEvent.OnUpdateCall;
                    _stateEvent.OnExitCall += stateEvent.OnExitCall;
                }
            }
            else
            {
                AddAnimator(animator);
                AddStateEvent(animator, stateEvent);
            }
        }

        /// <summary>
        /// 添加只是状态全名不一样的同类事件
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateFullNames"></param>
        /// <param name="stateEvent"></param>
        public void AddStateEvent(Animator animator, string[] stateFullNames, AnimatorStateEvent stateEvent)
        {
            for (int i = 0; i < stateFullNames.Length; i++)
            {
                AnimatorStateEvent _stateEvent = new AnimatorStateEvent()
                {
                    fullName = stateFullNames[i],
                    layerIndex = stateEvent.layerIndex,
                    delayTime = stateEvent.delayTime,
                    OnEnterCall = stateEvent.OnEnterCall,
                    OnUpdateCall = stateEvent.OnUpdateCall,
                    OnDelayCall = stateEvent.OnDelayCall,
                    OnExitCall = stateEvent.OnExitCall,
                };
                AddStateEvent(animator, _stateEvent);
            }
        }

        /// <summary>
        /// 移除该Animator对应的所有StateEvent事件
        /// </summary>
        /// <param name="animator"></param>
        public void RemoveAnimator(Animator animator)
        {
            if (animatorStateEventDic.ContainsKey(animator))
            {
                animatorStateEventDic[animator].Clear();
            }
            else
            {
                Debug.LogError(animator.name + " animator excluded");
            }
        }

        /// <summary>
        /// 移除该Animator状态全名为StateFullName的事件
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateFullName">全名</param>
        public void RemoveStateEvent(Animator animator, string stateFullName)
        {
            if (animatorStateEventDic.ContainsKey(animator))
            {
                List<AnimatorStateEvent> list = animatorStateEventDic[animator];
                AnimatorStateEvent stateEvent = list.Find(t => t.fullName == stateFullName);
                if (stateEvent != null)
                {
                    animatorStateEventDic[animator].Remove(stateEvent);
                }
                else
                {
                    Debug.LogError(stateFullName + " inexistence");
                }
            }
            else
            {
                Debug.LogError(animator.name + " animator excluded");
            }
        }

        /// <summary>
        /// 改变Animator状态全名为StateFullName对应事件的回调，可以更改某一个回调，或者取消某一个回调
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateFullName">全名</param>
        /// <param name="callType">回调类型</param>
        /// <param name="changeCallType">回调改变方式</param>
        /// <param name="onCall">回调函数</param>
        public void ChangeStateEvent(Animator animator, string stateFullName, EAnimatorStateCallType callType, EAnimatorStateChangeCallType changeCallType, Action onCall)
        {
            if (animatorStateEventDic.ContainsKey(animator))
            {
                List<AnimatorStateEvent> list = animatorStateEventDic[animator];
                AnimatorStateEvent stateEvent = list.Find(t => t.fullName == stateFullName);
                switch (callType)
                {
                    case EAnimatorStateCallType.EnterCall:
                        ChangeCall(ref stateEvent.OnEnterCall, changeCallType, onCall);
                        break;

                    case EAnimatorStateCallType.UpdateCall:
                        ChangeCall(ref stateEvent.OnUpdateCall, changeCallType, onCall);
                        break;

                    case EAnimatorStateCallType.ExitCall:
                        ChangeCall(ref stateEvent.OnExitCall, changeCallType, onCall);
                        break;
                }
            }
        }

        /// <summary>
        /// 更改回调
        /// </summary>
        /// <param name="stateEventCall">原事件回调</param>
        /// <param name="changeCallType">更改回调方式</param>
        /// <param name="onChangeCall">更改的回调</param>
        private void ChangeCall(ref Action stateEventCall, EAnimatorStateChangeCallType changeCallType, Action onChangeCall)
        {
            switch (changeCallType)
            {
                case EAnimatorStateChangeCallType.Append:
                    stateEventCall += onChangeCall;
                    break;

                case EAnimatorStateChangeCallType.Remove:
                    stateEventCall -= onChangeCall;
                    break;

                case EAnimatorStateChangeCallType.Cover:
                    stateEventCall = onChangeCall;
                    break;
            }
        }

        /// <summary>
        /// 更改延时调用函数
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateFullName"></param>
        /// <param name="delayTime"></param>
        /// <param name="changeCallType"></param>
        /// <param name="onCall"></param>
        public void ChangeStateEventDelayCall(Animator animator, string stateFullName, float delayTime, EAnimatorStateChangeCallType changeCallType, Action onCall)
        {
            if (animatorStateEventDic.ContainsKey(animator))
            {
                List<AnimatorStateEvent> list = animatorStateEventDic[animator];
                AnimatorStateEvent stateEvent = list.Find(t => t.fullName == stateFullName);
                if (stateEvent != null)
                {
                    if (stateEvent.delayCallDic.ContainsKey(delayTime))
                    {
                        Action delayCall = stateEvent.delayCallDic[delayTime].delayCall;
                        ChangeCall(ref delayCall, changeCallType, onCall);
                    }
                    else
                    {
                        Debug.LogError(stateFullName + " " + delayTime + " delayCall not found");
                    }
                }
                else
                {
                    Debug.LogError(stateFullName + " not found");
                }
            }
        }

        public void Update()
        {
            if (animatorStateEventDic == null) return;

            Dictionary<Animator, List<AnimatorStateEvent>>.KeyCollection.Enumerator animators = animatorStateEventDic.Keys.GetEnumerator();
            while (animators.MoveNext())
            {
                Animator animator = animators.Current;
                if (animator.isActiveAndEnabled)
                {
                    List<AnimatorStateEvent> list = animatorStateEventDic[animator];
                    for (int i = 0; i < list.Count; i++)
                    {
                        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(list[i].layerIndex);
                        if (stateInfo.IsName(list[i].fullName))
                        {
                            if (list[i].isInState == false)
                            {
                                list[i].OnEnterCall?.Invoke();
                                list[i].isInState = true;
                            }

                            Dictionary<float, AniamtorStateEventDelayCall>.KeyCollection.Enumerator delayTimes = list[i].delayCallDic.Keys.GetEnumerator();
                            while (delayTimes.MoveNext())
                            {
                                float delayTime = delayTimes.Current;
                                if (stateInfo.normalizedTime >= delayTime && list[i].delayCallDic[delayTime].delayCalled == false)
                                {
                                    list[i].delayCallDic[delayTime].delayCalled = true;
                                    list[i].delayCallDic[delayTime].delayCall?.Invoke();
                                }
                            }

                            list[i].OnUpdateCall?.Invoke();
                        }
                        else
                        {
                            if (list[i].isInState == true)
                            {
                                float[] delayTime_1f = new float[list[i].delayCallDic.Count];
                                list[i].delayCallDic.Keys.CopyTo(delayTime_1f, 0);
                                for (int u = 0; u < delayTime_1f.Length; u++)
                                {
                                    list[i].delayCallDic[u].delayCalled = false;
                                    list[i].delayCallDic[u].delayCall?.Invoke();
                                }

                                list[i].OnExitCall?.Invoke();
                                list[i].isInState = false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清空所有Animator
        /// </summary>
        public void RemoveAll()
        {
            animatorStateEventDic.Clear();
        }

        public override void Clear()
        {
            base.Clear();
            RemoveAll();
        }
    }

    public class AnimatorStateEvent
    {
        /// <summary> 是否在状态内，不需要外部设置 </summary>
        public bool isInState;

        /// <summary> 状态对应的layerIndex </summary>
        public int layerIndex;

        private float _delayTime;
        /// <summary> 延时时间，整个时间为1，具体参见AnimationState.normalizedTime </summary>
        public float delayTime
        {
            get { return _delayTime; }
            set { _delayTime = Mathf.Clamp01(value); }
        }

        /// <summary> 动画状态完整路径 </summary>
        public string fullName;

        /// <summary> 进入该状态时调用 </summary>
        public Action OnEnterCall;

        /// <summary> 播放该状态时一直调用 </summary>
        public Action OnUpdateCall;

        /// <summary> 离开该状态调用 </summary>
        public Action OnExitCall;

        /// <summary> 进入该状态之后延时调用，延时时间为delayNormalizedTime，整个时间为1，具体参见AnimationState.normalizedTime </summary>
        public Action OnDelayCall;

        /// <summary> 延时调用集合，可添加多个时间对应的事件函数 </summary>
        public Dictionary<float, AniamtorStateEventDelayCall> delayCallDic = new Dictionary<float, AniamtorStateEventDelayCall>();
    }

    public class AniamtorStateEventDelayCall
    {
        /// <summary> 是否已经延时调用了，不需要外部设置 </summary>
        public bool delayCalled;

        /// <summary> 延时调用函数 </summary>
        public Action delayCall;
    }

    public enum EAnimatorStateCallType
    {
        EnterCall,
        UpdateCall,
        ExitCall,
    }

    public enum EAnimatorStateChangeCallType
    {
        Append,
        Remove,
        Cover,
    }
}