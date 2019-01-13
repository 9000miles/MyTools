using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Common;

public class AnimatorStateEventManagerWindow : EditorWindow
{
    private Vector2 scrollPos;
    private Animator curAnimator;

    private Dictionary<AnimatorStateEventInfo, bool> stateEventFoldoutDic =
        new Dictionary<AnimatorStateEventInfo, bool>();

    public Dictionary<Animator, List<AnimatorStateEventInfo>> animatorStateEventDic =
        new Dictionary<Animator, List<AnimatorStateEventInfo>>();

    private void Awake()
    {
        animatorStateEventDic = AnimatorStateEventManager.Singleton.animatorStateEventDic;
        titleContent = new GUIContent("Animator State Event Manager Window");
        Animator animator01 = GameObject.Find("Animator_01").GetComponent<Animator>();
        animator01.name = "Animator_01";
        List<AnimatorStateEventInfo> animator_01_Event = new List<AnimatorStateEventInfo>();
        animator_01_Event.Add(new AnimatorStateEventInfo()
        {
            stateEvent = new AnimatorStateEvent()
            {
                fullName = "Animator_01_FullName_01",
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
                fullName = "Animator_01_FullName_02",
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
                fullName = "Animator_01_FullName_03",
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
            clip = animator01.GetComponent<Animation>().clip,
            stateEvent = new AnimatorStateEvent()
            {
                fullName = "Animator_01_FullName_019191919196",
                delayTime = 0.5f,
                OnEnterCall = () => { },
                OnUpdateCall = () => { },
                OnExitCall = () => { },
                OnDelayCall = () => { },
            },
        };
        info_06.stateEvent.OnEnterCall += Add_06_EnterCall;
        List<Action> delayAction1 = new List<Action>();
        delayAction1.Add(() => { });
        delayAction1.Add(() => { });
        delayAction1.Add(Add_06_DelayCall);
        info_06.onDelayCallDic.Add(0.5f, delayAction1);

        List<Action> delayAction2 = new List<Action>();
        delayAction2.Add(Add_06_Scende_DelayCall);
        info_06.onDelayCallDic.Add(0.65f, delayAction2);

        animator_01_Event.Add(info_06);

        animatorStateEventDic.Clear();
        animatorStateEventDic.Add(animator01, animator_01_Event);

        Animator animator02 = GameObject.Find("Animator_02").GetComponent<Animator>();
        animator02.name = "Animator_02";
        List<AnimatorStateEventInfo> animator_02_Event = new List<AnimatorStateEventInfo>();
        animator_02_Event.Add(new AnimatorStateEventInfo()
        {
            clip = animator02.GetComponent<Animation>().clip,
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
        curAnimator = animator01;
        stateEventFoldoutDic.Clear();
        foreach (AnimatorStateEventInfo info in animatorStateEventDic[curAnimator])
        {
            stateEventFoldoutDic.Add(info, false);
        }
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
                    stateEventFoldoutDic.Add(info, false);
                }
            }
        }

        if (GUILayout.Button("Foldout All", GUILayout.MaxWidth(80)))
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
        stateEventFoldoutDic[eventInfo] = EditorGUILayout.Foldout(stateEventFoldoutDic[eventInfo], eventInfo.stateEvent.fullName, true, style);
        EditorGUILayout.ObjectField(eventInfo.clip, typeof(AnimationClip), false, GUILayout.MaxWidth(300));
        EditorGUILayout.EndHorizontal();

        if (stateEventFoldoutDic[eventInfo])
        {
            GUIStyle callNameStyle = new GUIStyle(EditorStyles.boldLabel);
            callNameStyle.margin.left = 25;
            DrawStateCall(eventInfo.stateEvent.OnEnterCall, "Enter Call");
            DrawStateCall(eventInfo.stateEvent.OnUpdateCall, "Update Call");
            DrawStateCall(eventInfo.stateEvent.OnExitCall, "Exit Call");
            DrawDelayCall(eventInfo.onDelayCallDic, "Delay Call");
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawStateCall(Action action, string callName)
    {
        if (action != null)
        {
            EditorGUILayout.LabelField(callName, EditorStyles.boldLabel);
            foreach (Delegate method in action.GetInvocationList())
            {
                GUIStyle methodNameStyle = new GUIStyle(EditorStyles.textArea);
                methodNameStyle.margin.left = 50;
                EditorGUILayout.TextArea((method.Target != null ? method.Target.ToString() + "." + method.Method.Name : "method of delegate"), methodNameStyle);
            }
        }
    }

    private void DrawDelayCall(Dictionary<float, List<Action>> delayCalls, string callName)
    {
        if (delayCalls.Count > 0)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(callName, EditorStyles.boldLabel);
            Dictionary<float, List<Action>>.KeyCollection.Enumerator delayTimes = delayCalls.Keys.GetEnumerator();
            while (delayTimes.MoveNext())
            {
                GUIStyle delayTimeStyle = new GUIStyle(EditorStyles.label);
                delayTimeStyle.padding.left = 25;
                EditorGUILayout.LabelField("DelayTime: " + delayTimes.Current.ToString(), delayTimeStyle);
                foreach (Action method in delayCalls[delayTimes.Current])
                {
                    GUIStyle delayMethodNameStyle = new GUIStyle(EditorStyles.textArea);
                    delayMethodNameStyle.margin.left = 50;
                    EditorGUILayout.TextArea((method.Target != null ? method.Target.ToString() + "." + method.Method.Name : "method of delegate"), delayMethodNameStyle);
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}