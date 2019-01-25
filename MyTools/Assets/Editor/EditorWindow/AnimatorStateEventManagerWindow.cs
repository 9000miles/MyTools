using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace MarsPC
{
    public class AnimatorStateEventManagerWindow : EditorWindow
    {
        private Vector2 scrollPos;
        private Animator curAnimator;
        private Dictionary<AnimatorStateEventInfo, bool> stateEventFoldoutDic = new Dictionary<AnimatorStateEventInfo, bool>();
        private Dictionary<Animator, List<AnimatorStateEventInfo>> animatorStateEventDic;

        private void Awake()
        {
            animatorStateEventDic = new Dictionary<Animator, List<AnimatorStateEventInfo>>();
        }

        private void OnGUI()
        {
            animatorStateEventDic = AnimatorStateEventManager.Singleton.AnimatorStateEventDic;
            if (animatorStateEventDic == null || animatorStateEventDic.Count == 0)
            {
                EditorGUILayout.HelpBox("AnimatorStateEventManager AnimatorStateEventDic There are no animator status events", MessageType.Info, true);
            }
            else
            {
                Dictionary<Animator, List<AnimatorStateEventInfo>>.KeyCollection.Enumerator animators = animatorStateEventDic.Keys.GetEnumerator();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                while (animators.MoveNext())
                {
                    if (animators.Current != null)
                    {
                        if (GUILayout.Button(animators.Current.name))
                        {
                            curAnimator = animators.Current;
                            stateEventFoldoutDic.Clear();
                            foreach (AnimatorStateEventInfo info in animatorStateEventDic[curAnimator])
                            {
                                SearchAnimationClip(curAnimator, info);
                                stateEventFoldoutDic.Add(info, false);
                            }
                        }
                    }
                }

                if (GUILayout.Button("Fold All", GUILayout.MaxWidth(80)))
                {
                    AnimatorStateEventInfo[] infos = new AnimatorStateEventInfo[stateEventFoldoutDic.Count];
                    stateEventFoldoutDic.Keys.CopyTo(infos, 0);
                    foreach (AnimatorStateEventInfo info in infos)
                    {
                        stateEventFoldoutDic[info] = false;
                    }
                }

                EditorGUILayout.EndHorizontal();
                if (curAnimator != null)
                    OpenAnimatorViewport(curAnimator);
            }
        }

        private void SearchAnimationClip(Animator animator, AnimatorStateEventInfo info)
        {
            AnimatorController animatorController = (AnimatorController)animator.runtimeAnimatorController;
            if (animatorController == null) return;

            AnimatorControllerLayer[] layers = animatorController.layers;
            string stateFullName = info.stateEvent.FullName;
            int indexOf = stateFullName.IndexOf('.');
            if (indexOf > 0)
            {
                string layerName = stateFullName.Substring(0, indexOf);
                AnimatorControllerLayer controllerLayer = layers.Find(t => t.name == layerName);
                if (controllerLayer != null)
                {
                    string stateName = stateFullName.Substring(stateFullName.IndexOf('.') + 1);
                    info.clip = GetAnimationClip(controllerLayer.stateMachine, stateName);
                }
            }
        }

        private AnimationClip GetAnimationClip(AnimatorStateMachine stateMachine, string stateName)
        {
            AnimationClip animationClip = null;
            if (stateName.IndexOf('.') > 0)
            {
                string stateMachineName = stateName.Substring(0, stateName.IndexOf('.'));
                ChildAnimatorStateMachine[] childStateMachine = stateMachine.stateMachines;
                ChildAnimatorStateMachine animatorStateMachine = childStateMachine.Find(t => t.stateMachine.name == stateMachineName);
                if (animatorStateMachine.stateMachine != null)
                {
                    string _stateName = stateName.Substring(stateName.IndexOf('.') + 1);
                    animationClip = GetAnimationClip(animatorStateMachine.stateMachine, _stateName);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                ChildAnimatorState animatorState = stateMachine.states.Find(t => t.state.name == stateName);
                if (animatorState.state != null)
                {
                    Motion motion = stateMachine.states.Find(t => t.state.name == stateName).state.motion;
                    if (motion as BlendTree)
                    {
                        animationClip = (AnimationClip)((motion as BlendTree).children[0].motion);
                    }
                    else if (motion as AnimationClip)
                    {
                        animationClip = (AnimationClip)motion;
                    }
                }
            }
            return animationClip;
        }

        private void OpenAnimatorViewport(Animator animator)
        {
            if (!animatorStateEventDic.ContainsKey(animator)) return;

            List<AnimatorStateEventInfo> stateEventInfos = animatorStateEventDic[animator];
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
            foreach (AnimatorStateEventInfo stateEvent in stateEventInfos)
            {
                DrawOneStateEvent(stateEvent);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawOneStateEvent(AnimatorStateEventInfo eventInfo)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            GUIStyle style = new GUIStyle(EditorStyles.foldout);
            style.fontStyle = FontStyle.Bold;
            if (stateEventFoldoutDic.ContainsKey(eventInfo))
            {
                stateEventFoldoutDic[eventInfo] = EditorGUILayout.Foldout(stateEventFoldoutDic[eventInfo], eventInfo.stateEvent.FullName, true, style);
                EditorGUILayout.ObjectField(eventInfo.clip, typeof(AnimationClip), false, GUILayout.MaxWidth(300));
                EditorGUILayout.EndHorizontal();

                if (stateEventFoldoutDic[eventInfo])
                {
                    DrawStateCall(eventInfo.stateEvent.OnEnterCall, "Enter Call");
                    DrawStateCall(eventInfo.stateEvent.OnUpdateCall, "Update Call");
                    DrawStateCall(eventInfo.stateEvent.OnExitCall, "Exit Call");
                    DrawDelayCall(eventInfo.delayCallDic, "Delay Call");
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawStateCall(Action action, string callName)
        {
            if (action != null)
            {
                GUIStyle callNameStyle = new GUIStyle(EditorStyles.boldLabel);
                callNameStyle.padding.left = 25;
                EditorGUILayout.LabelField(callName, callNameStyle);
                foreach (Delegate method in action.GetInvocationList())
                {
                    GUIStyle methodNameStyle = new GUIStyle(EditorStyles.textArea);
                    methodNameStyle.margin.left = 50;
                    string methodName = method.Target != null ? method.Target.ToString() + "." + method.Method.Name : "method of delegate";
                    EditorGUILayout.TextArea(methodName, methodNameStyle);
                }
            }
        }

        private void DrawDelayCall(Dictionary<float, List<AniamtorStateEventDelayCall>> delayCalls, string callName)
        {
            if (delayCalls.Count > 0)
            {
                EditorGUILayout.BeginVertical("box");
                GUIStyle callNameStyle = new GUIStyle(EditorStyles.boldLabel);
                callNameStyle.padding.left = 25;
                EditorGUILayout.LabelField(callName, callNameStyle);
                Dictionary<float, List<AniamtorStateEventDelayCall>>.KeyCollection.Enumerator delayTimes = delayCalls.Keys.GetEnumerator();
                while (delayTimes.MoveNext())
                {
                    GUIStyle delayTimeStyle = new GUIStyle(EditorStyles.label);
                    delayTimeStyle.padding.left = 25;
                    EditorGUILayout.LabelField("DelayTime: " + delayTimes.Current.ToString(), delayTimeStyle);
                    foreach (AniamtorStateEventDelayCall delayCallInfo in delayCalls[delayTimes.Current])
                    {
                        GUIStyle delayMethodNameStyle = new GUIStyle(EditorStyles.textArea);
                        delayMethodNameStyle.margin.left = 50;
                        string methodName = delayCallInfo.delayCall.Target != null ? delayCallInfo.delayCall.Target.ToString() + "." + delayCallInfo.delayCall.Method.Name : "method of delegate";
                        EditorGUILayout.TextArea(methodName, delayMethodNameStyle);
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }
    }
}