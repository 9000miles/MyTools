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
            //animatorStateEventDic = AnimatorStateEventManager.Singleton.AnimatorStateEventDic;
            animatorStateEventDic = new Dictionary<Animator, List<AnimatorStateEventInfo>>();
            titleContent = new GUIContent("Animator State Event Manager Window");
            GameObject animator_01 = GameObject.Find("Animator_01");
            if (animator_01 != null)
            {
                AddAnimator_Test(animator_01);
            }

            GameObject animator_02 = GameObject.Find("Animator_02");
            if (animator_02 != null)
            {
                AddAnimator02_Test();
            }

            stateEventFoldoutDic.Clear();
            if (curAnimator != null)
            {
                foreach (AnimatorStateEventInfo info in animatorStateEventDic[curAnimator])
                {
                    stateEventFoldoutDic.Add(info, false);
                }
            }
        }

        private void AddAnimator02_Test()
        {
            Animator animator02 = GameObject.Find("Animator_02").GetComponent<Animator>();
            animator02.name = "Animator_02";
            List<AnimatorStateEventInfo> animator_02_Event = new List<AnimatorStateEventInfo>();
            animator_02_Event.Add(new AnimatorStateEventInfo()
            {
                //clip = animator02.GetComponent<Animation>().clip,
                stateEvent = new AnimatorStateEvent()
                {
                    fullName = "Animator_02_FullName_01",
                    delayTime = 0.3f,
                    OnEnterCall = () => { },
                    OnUpdateCall = () => { },
                    OnExitCall = () => { },
                    OnDelayCall = () => { },
                }
            });
            animatorStateEventDic.Add(animator02, animator_02_Event);
        }

        private void AddAnimator_Test(GameObject animator_01)
        {
            Animator animator01 = animator_01.GetComponent<Animator>();
            animator01.name = "Animator_01";
            List<AnimatorStateEventInfo> animator_01_Event = new List<AnimatorStateEventInfo>();
            animator_01_Event.Add(new AnimatorStateEventInfo()
            {
                stateEvent = new AnimatorStateEvent()
                {
                    fullName = "Base Layer.Fall",
                    delayTime = 0.1f,
                    OnEnterCall = () => { },
                    OnUpdateCall = () => { },
                    OnExitCall = () => { },
                    OnDelayCall = () => { },
                }
            });
            animator_01_Event.Add(new AnimatorStateEventInfo()
            {
                stateEvent = new AnimatorStateEvent()
                {
                    fullName = "Base Layer.Wall_Cfelimb.Wall_Climb_Idljofwee",
                    delayTime = 0.1f,
                    OnEnterCall = () => { },
                    OnUpdateCall = () => { },
                    OnExitCall = () => { },
                    OnDelayCall = () => { },
                }
            });
            animator_01_Event.Add(new AnimatorStateEventInfo()
            {
                stateEvent = new AnimatorStateEvent()
                {
                    fullName = "Base Layer.Wall_Run.Wall_Run_Lf.Wall_Run_Lf",
                    delayTime = 0.1f,
                    OnEnterCall = () => { },
                    //OnUpdateCall = () => { },
                    OnExitCall = () => { },
                    OnDelayCall = () => { },
                }
            });
            animator_01_Event.Add(new AnimatorStateEventInfo()
            {
                stateEvent = new AnimatorStateEvent()
                {
                    fullName = "Animator_01_FullName_04",
                    delayTime = 0.1f,
                    OnEnterCall = () => { },
                    OnUpdateCall = () => { },
                    OnExitCall = () => { },
                    OnDelayCall = () => { },
                }
            });
            animator_01_Event.Add(new AnimatorStateEventInfo()
            {
                stateEvent = new AnimatorStateEvent()
                {
                    fullName = "Animator_01_FullName_05",
                    delayTime = 0.1f,
                    OnEnterCall = () => { },
                    OnUpdateCall = () => { },
                    OnExitCall = () => { },
                    OnDelayCall = () => { },
                }
            });
            AnimatorStateEventInfo info_06 = new AnimatorStateEventInfo()
            {
                //clip = animator01.GetComponent<Animation>().clip,
                stateEvent = new AnimatorStateEvent()
                {
                    fullName = "Base Layer.Wall_Run.Wall_Run_Lf.Air_ToWallRun_Lf",
                    delayTime = 0.5f,
                    OnEnterCall = () => { },
                    OnUpdateCall = () => { },
                    OnExitCall = () => { },
                    OnDelayCall = () => { },
                },
            };
            info_06.stateEvent.OnEnterCall += Add_06_EnterCall;
            List<AniamtorStateEventDelayCall> delayAction1 = new List<AniamtorStateEventDelayCall>();
            delayAction1.Add(new AniamtorStateEventDelayCall() { delayCall = () => { } });
            delayAction1.Add(new AniamtorStateEventDelayCall() { delayCall = () => { } });
            delayAction1.Add(new AniamtorStateEventDelayCall() { delayCall = Add_06_DelayCall });
            info_06.delayCallDic.Add(0.85f, delayAction1);

            List<AniamtorStateEventDelayCall> delayAction2 = new List<AniamtorStateEventDelayCall>();
            delayAction2.Add(new AniamtorStateEventDelayCall() { delayCall = Add_06_Scende_DelayCall });
            info_06.delayCallDic.Add(0.65f, delayAction2);

            animator_01_Event.Add(info_06);

            animatorStateEventDic.Clear();
            animatorStateEventDic.Add(animator01, animator_01_Event);

            curAnimator = animator01;
        }

        private void Add_06_EnterCall()
        {
        }

        private void Add_06_DelayCall()
        {
        }

        private void Add_06_Scende_DelayCall()
        {
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
            string stateFullName = info.stateEvent.fullName;
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
                stateEventFoldoutDic[eventInfo] = EditorGUILayout.Foldout(stateEventFoldoutDic[eventInfo], eventInfo.stateEvent.fullName, true, style);
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