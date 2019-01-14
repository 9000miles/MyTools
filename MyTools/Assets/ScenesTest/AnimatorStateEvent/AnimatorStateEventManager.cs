using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsPC
{
    /// <summary>
    /// Animator动画事件管理类
    /// 添加，移除，更改事件
    /// Enter，Update，Exit，Delay延时事件
    /// 示例参考：AnimatorStateEvent_Test
    /// </summary>
    public class AnimatorStateEventManager : SingletonTemplate<AnimatorStateEventManager>
    {
        private Dictionary<Animator, List<AnimatorStateEventInfo>> animatorStateEventDic = new Dictionary<Animator, List<AnimatorStateEventInfo>>();

        /// <summary>
        /// 动画状态事件字典，可同时处理多个Animator状态机的动画事件
        /// </summary>
        public Dictionary<Animator, List<AnimatorStateEventInfo>> AnimatorStateEventDic
        {
            get { return animatorStateEventDic; }
        }

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
                animatorStateEventDic.Add(animator, new List<AnimatorStateEventInfo>());
            }
        }

        /// <summary>
        /// 给Animator添加StateEvent事件
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateEventInfo">状态事件</param>
        public void AddStateEvent(Animator animator, AnimatorStateEvent stateEvent)
        {
            if (animatorStateEventDic.ContainsKey(animator))
            {
                List<AnimatorStateEventInfo> list = animatorStateEventDic[animator];
                if (list == null) list = new List<AnimatorStateEventInfo>();

                AnimatorStateEventInfo stateEventInfo = new AnimatorStateEventInfo()
                {
                    stateEvent = stateEvent,
                };

                if (!list.Exists(t => t.stateEvent.fullName == stateEventInfo.stateEvent.fullName))
                {
                    list.Add(stateEventInfo);
                    if (stateEventInfo.stateEvent.delayTime != 0)
                    {
                        AniamtorStateEventDelayCall delayCall = new AniamtorStateEventDelayCall() { delayCalled = false, delayCall = stateEventInfo.stateEvent.OnDelayCall };
                        List<AniamtorStateEventDelayCall> delayCallList = new List<AniamtorStateEventDelayCall> { delayCall };
                        stateEventInfo.delayCallDic.Add(stateEventInfo.stateEvent.delayTime, delayCallList);
                    }
                }
                else
                {
                    Debug.LogWarning(stateEventInfo.stateEvent.fullName + " already existed");
                    AnimatorStateEventInfo _stateEventInfo = list.Find(t => t.stateEvent.fullName == stateEventInfo.stateEvent.fullName);

                    if (stateEventInfo.stateEvent.delayTime != 0)
                    {
                        if (!_stateEventInfo.delayCallDic.ContainsKey(stateEventInfo.stateEvent.delayTime))
                        {
                            AniamtorStateEventDelayCall delayCall = new AniamtorStateEventDelayCall() { delayCalled = false, delayCall = stateEventInfo.stateEvent.OnDelayCall };
                            List<AniamtorStateEventDelayCall> delayCallList = new List<AniamtorStateEventDelayCall> { delayCall };
                            _stateEventInfo.delayCallDic.Add(stateEventInfo.stateEvent.delayTime, delayCallList);
                        }
                        else
                        {
                            _stateEventInfo.delayCallDic[stateEventInfo.stateEvent.delayTime][0].delayCall += stateEventInfo.stateEvent.OnDelayCall;
                        }
                    }

                    _stateEventInfo.stateEvent.OnEnterCall += stateEventInfo.stateEvent.OnEnterCall;
                    _stateEventInfo.stateEvent.OnUpdateCall += stateEventInfo.stateEvent.OnUpdateCall;
                    _stateEventInfo.stateEvent.OnExitCall += stateEventInfo.stateEvent.OnExitCall;
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
                List<AnimatorStateEventInfo> list = animatorStateEventDic[animator];
                AnimatorStateEventInfo stateEvent = list.Find(t => t.stateEvent.fullName == stateFullName);
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
                List<AnimatorStateEventInfo> list = animatorStateEventDic[animator];
                AnimatorStateEventInfo stateEvent = list.Find(t => t.stateEvent.fullName == stateFullName);
                switch (callType)
                {
                    case EAnimatorStateCallType.EnterCall:
                        ChangeCall(ref stateEvent.stateEvent.OnEnterCall, changeCallType, onCall);
                        break;

                    case EAnimatorStateCallType.UpdateCall:
                        ChangeCall(ref stateEvent.stateEvent.OnUpdateCall, changeCallType, onCall);
                        break;

                    case EAnimatorStateCallType.ExitCall:
                        ChangeCall(ref stateEvent.stateEvent.OnExitCall, changeCallType, onCall);
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
        private static void ChangeCall(ref Action stateEventCall, EAnimatorStateChangeCallType changeCallType, Action onChangeCall)
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
                List<AnimatorStateEventInfo> list = animatorStateEventDic[animator];
                AnimatorStateEventInfo stateEvent = list.Find(t => t.stateEvent.fullName == stateFullName);
                if (stateEvent != null)
                {
                    if (stateEvent.delayCallDic.ContainsKey(delayTime))
                    {
                        Action delayCall = stateEvent.delayCallDic[delayTime].Find(t => t.delayCall == onCall).delayCall;
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

            Dictionary<Animator, List<AnimatorStateEventInfo>>.KeyCollection.Enumerator animators = animatorStateEventDic.Keys.GetEnumerator();
            while (animators.MoveNext())
            {
                Animator animator = animators.Current;
                if (animator.isActiveAndEnabled)
                {
                    List<AnimatorStateEventInfo> list = animatorStateEventDic[animator];
                    for (int i = 0; i < list.Count; i++)
                    {
                        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(list[i].stateEvent.layerIndex);
                        if (stateInfo.IsName(list[i].stateEvent.fullName))
                        {
                            InvokeOnEnterCall(list, i);
                            InvokeOnDelayCall(list, i, stateInfo);
                            InvokeOnUpdateCall(list, i);
                        }
                        else
                        {
                            if (list[i].isInState == true)
                            {
                                InvokeOnDelay_1f_Call(list, i);
                                InvokeOnExitCall(list, i);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行Enter事件
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        private static void InvokeOnEnterCall(List<AnimatorStateEventInfo> list, int index)
        {
            if (list[index].isInState == false)
            {
                list[index].stateEvent.OnEnterCall?.Invoke();
                list[index].isInState = true;
            }
        }

        /// <summary>
        /// 执行延时时间在【0,1）的事件，
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="stateInfo"></param>
        private static void InvokeOnDelayCall(List<AnimatorStateEventInfo> list, int index, AnimatorStateInfo stateInfo)
        {
            Dictionary<float, List<AniamtorStateEventDelayCall>>.KeyCollection.Enumerator delayTimes = list[index].delayCallDic.Keys.GetEnumerator();
            while (delayTimes.MoveNext())
            {
                float delayTime = delayTimes.Current;
                if (stateInfo.normalizedTime >= delayTime)
                {
                    foreach (AniamtorStateEventDelayCall delayCallItem in list[index].delayCallDic[delayTime])
                    {
                        if (delayCallItem.delayCalled == false)
                        {
                            delayCallItem.delayCalled = true;
                            delayCallItem.delayCall?.Invoke();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行Update事件
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        private static void InvokeOnUpdateCall(List<AnimatorStateEventInfo> list, int index)
        {
            list[index].stateEvent.OnUpdateCall?.Invoke();
        }

        /// <summary>
        /// 执行延时时间为1的事件，在Exit之前执行
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        private static void InvokeOnDelay_1f_Call(List<AnimatorStateEventInfo> list, int index)
        {
            float[] delayTime_1f = new float[list[index].delayCallDic.Count];
            list[index].delayCallDic.Keys.CopyTo(delayTime_1f, 0);
            for (int u = 0; u < delayTime_1f.Length; u++)
            {
                for (int h = 0; h < list[index].delayCallDic[u].Count; h++)
                {
                    list[index].delayCallDic[u][h].delayCalled = false;
                    list[index].delayCallDic[u][h].delayCall?.Invoke();
                }
            }
        }

        /// <summary>
        /// 执行Exit事件
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        private static void InvokeOnExitCall(List<AnimatorStateEventInfo> list, int index)
        {
            list[index].stateEvent.OnExitCall?.Invoke();
            list[index].isInState = false;
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
}