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
    /// Animator State Event该窗口可以查看有哪些AnimatorState哪些状态添加了事件，如果没有动画文件，则fullName有误
    /// </summary>
    public class AnimatorStateEventManager : SingletonTemplate<AnimatorStateEventManager>
    {
        private Dictionary<Animator, List<AnimatorStateEventInfo>> animatorStateEventDic = new Dictionary<Animator, List<AnimatorStateEventInfo>>();

        /// <summary>
        /// 动画状态事件字典，可同时处理多个Animator状态机的动画事件
        /// </summary>
        public Dictionary<Animator, List<AnimatorStateEventInfo>> AnimatorStateEventDic { get { return animatorStateEventDic; } }

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
        /// <param name="isRemoveAfterCompletion">该状态完成后是否移除，不在初始化中添加，就需要设置为true</param>
        public void AddStateEvent(Animator animator, AnimatorStateEvent stateEvent, bool isRemoveAfterCompletion = false)
        {
            if (animatorStateEventDic.ContainsKey(animator))
            {
                List<AnimatorStateEventInfo> list = animatorStateEventDic[animator];
                if (list == null) list = new List<AnimatorStateEventInfo>();
                if (list.Find(t => t.stateEvent == stateEvent) != null) return;

                AnimatorStateEventInfo stateEventInfo = new AnimatorStateEventInfo();
                stateEventInfo.isSingleUse = isRemoveAfterCompletion;
                stateEventInfo.stateEvent = stateEvent;
                int index = stateEvent.FullName.IndexOf('.');
                if (index <= 0) return;

                string layerNameStr = stateEvent.FullName.Substring(0, index);
                stateEventInfo.layerIndex = animator.GetLayerIndex(layerNameStr);

                if (!list.Exists(t => t.stateEvent.FullName == stateEventInfo.stateEvent.FullName))
                {
                    list.Add(stateEventInfo);
                    if (stateEventInfo.stateEvent.DelayTime != 0)
                    {
                        AniamtorStateEventDelayCall delayCall = new AniamtorStateEventDelayCall() { delayCalled = false, delayCall = stateEventInfo.stateEvent.OnDelayCall };
                        List<AniamtorStateEventDelayCall> delayCallList = new List<AniamtorStateEventDelayCall> { delayCall };
                        stateEventInfo.delayCallDic.Add(stateEventInfo.stateEvent.DelayTime, delayCallList);
                    }
                }
                else
                {
                    Debug.LogWarning(stateEventInfo.stateEvent.FullName + " already existed");
                    AnimatorStateEventInfo _stateEventInfo = list.Find(t => t.stateEvent.FullName == stateEventInfo.stateEvent.FullName);

                    if (stateEventInfo.stateEvent.DelayTime != 0)
                    {
                        if (!_stateEventInfo.delayCallDic.ContainsKey(stateEventInfo.stateEvent.DelayTime))
                        {
                            AniamtorStateEventDelayCall delayCall = new AniamtorStateEventDelayCall() { delayCalled = false, delayCall = stateEventInfo.stateEvent.OnDelayCall };
                            List<AniamtorStateEventDelayCall> delayCallList = new List<AniamtorStateEventDelayCall> { delayCall };
                            _stateEventInfo.delayCallDic.Add(stateEventInfo.stateEvent.DelayTime, delayCallList);
                        }
                        else
                        {
                            _stateEventInfo.delayCallDic[stateEventInfo.stateEvent.DelayTime][0].delayCall += stateEventInfo.stateEvent.OnDelayCall;
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
                AddStateEvent(animator, stateEvent, isRemoveAfterCompletion);
            }
        }

        /// <summary>
        /// 添加只是状态全名不一样的同类事件
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateFullNames">状态全名数组</param>
        /// <param name="stateEvent">状态事件</param>
        /// <param name="isRemoveAfterCompletion">该状态完成后是否移除，不在初始化中添加，就需要设置为true</param>
        public void AddStateEvent(Animator animator, string[] stateFullNames, AnimatorStateEvent stateEvent, bool isRemoveAfterCompletion = false)
        {
            for (int i = 0; i < stateFullNames.Length; i++)
            {
                AnimatorStateEvent _stateEvent = new AnimatorStateEvent()
                {
                    FullName = stateFullNames[i],
                    DelayTime = stateEvent.DelayTime,
                    OnEnterCall = stateEvent.OnEnterCall,
                    OnUpdateCall = stateEvent.OnUpdateCall,
                    OnDelayCall = stateEvent.OnDelayCall,
                    OnExitCall = stateEvent.OnExitCall,
                };
                AddStateEvent(animator, _stateEvent, isRemoveAfterCompletion);
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
                AnimatorStateEventInfo stateEvent = list.Find(t => t.stateEvent.FullName == stateFullName);
                if (stateEvent == null) return;

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
                List<AnimatorStateEventInfo> list = animatorStateEventDic[animator];
                AnimatorStateEventInfo stateEvent = list.Find(t => t.stateEvent.FullName == stateFullName);
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
                        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(list[i].layerIndex);
                        if (stateInfo.IsName(list[i].stateEvent.FullName))
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
        private void InvokeOnEnterCall(List<AnimatorStateEventInfo> list, int index)
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
        private void InvokeOnDelayCall(List<AnimatorStateEventInfo> list, int index, AnimatorStateInfo stateInfo)
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
        private void InvokeOnUpdateCall(List<AnimatorStateEventInfo> list, int index)
        {
            list[index].stateEvent.OnUpdateCall?.Invoke();
        }

        /// <summary>
        /// 执行延时时间为1的事件，在Exit之前执行
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        private void InvokeOnDelay_1f_Call(List<AnimatorStateEventInfo> list, int index)
        {
            if (list[index].delayCallDic.Keys.Count > 0)
            {
                float[] delayTime_1f = new float[list[index].delayCallDic.Count];
                list[index].delayCallDic.Keys.CopyTo(delayTime_1f, 0);
                float[] delayTimes = delayTime_1f.FindAll(t => t == 1f);
                foreach (float time_1f in delayTimes)
                {
                    foreach (AniamtorStateEventDelayCall delayCall in list[index].delayCallDic[time_1f])
                    {
                        delayCall.delayCalled = false;
                        delayCall.delayCall?.Invoke();
                    }
                }
            }

            Dictionary<float, List<AniamtorStateEventDelayCall>>.KeyCollection.Enumerator delayCallEnumerator = list[index].delayCallDic.Keys.GetEnumerator();
            while (delayCallEnumerator.MoveNext())
            {
                float delayTime = delayCallEnumerator.Current;
                foreach (AniamtorStateEventDelayCall delayCall in list[index].delayCallDic[delayTime])
                {
                    delayCall.delayCalled = false;
                }
            }
        }

        /// <summary>
        /// 执行Exit事件
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        private void InvokeOnExitCall(List<AnimatorStateEventInfo> list, int index)
        {
            list[index].stateEvent.OnExitCall?.Invoke();
            list[index].isInState = false;
            if (list[index].isSingleUse)
            {
                list.RemoveAt(index);
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
                AnimatorStateEventInfo stateEvent = list.Find(t => t.stateEvent.FullName == stateFullName);
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
        /// 移除指定状态事件
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateEvent">状态事件</param>
        public void RemoveStateEvent(Animator animator, AnimatorStateEvent stateEvent)
        {
            if (animatorStateEventDic.ContainsKey(animator))
            {
                AnimatorStateEventInfo eventInfo = animatorStateEventDic[animator].Find(t => t.stateEvent == stateEvent);
                animatorStateEventDic[animator].Remove(eventInfo);
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
}