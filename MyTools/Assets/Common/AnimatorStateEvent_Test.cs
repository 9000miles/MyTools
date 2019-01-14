﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsPC
{
    public class AnimatorStateEvent_Test : MonoBehaviour
    {
        public Animator animator_00;
        public Animator animator_01;
        public AnimatorStateEventManager manager;
        private Action UpdateCall;
        private Action action;

        private void Start()
        {
            AnimatorStateEvent_TestFunction();
            manager = AnimatorStateEventManager.Singleton;

            UpdateCall = AnimatorStateEventManager.Singleton.Update;
        }

        private void AnimatorStateEvent_TestFunction()
        {
            //---------------------------  animator_00  ----------------------------
            action = () => { Debug.Log("animator_00 -- OnEnterCall_00------------------------------"); };
            AnimatorStateEvent stateEvent00_00 = new AnimatorStateEvent()
            {
                fullName = "Base Layer.RunFwd",
                layerIndex = 0,
                delayTime = 0.4f,
                OnEnterCall = action,
                OnUpdateCall = () => { Debug.Log("animator_00 -- OnUpdateCall_00"); },
                OnDelayCall = () => { Debug.Log("animator_00 -- OnDelayCall_00     --  " + Time.time); },
                OnExitCall = () => { Debug.Log("animator_00 -- OnExitCall_00"); },
            };
            AnimatorStateEventManager.Singleton.AddStateEvent(animator_00, stateEvent00_00);
            AnimatorStateEvent stateEvent00_10 = new AnimatorStateEvent()
            {
                fullName = "Base Layer.RunFwd",
                layerIndex = 0,
                delayTime = 0.6f,
                OnDelayCall = () => { Debug.Log("animator_00 -- OnDelayCall_00     --  " + Time.time); },
            };
            AnimatorStateEventManager.Singleton.AddStateEvent(animator_00, stateEvent00_10);
            AnimatorStateEvent stateEvent00_11 = new AnimatorStateEvent()
            {
                fullName = "Base Layer.RunFwd",
                layerIndex = 0,
                delayTime = 0.8f,
                OnDelayCall = () => { Debug.Log("animator_00 -- OnDelayCall_00     --  " + Time.time); },
            };
            AnimatorStateEventManager.Singleton.AddStateEvent(animator_00, stateEvent00_11);

            AnimatorStateEvent stateEvent00_01 = new AnimatorStateEvent()
            {
                fullName = "New Layer.RunLeft",
                layerIndex = 1,
                delayTime = 0.6f,
                OnEnterCall = () => { Debug.Log("animator_00 -- OnEnterCall_01"); },
                OnUpdateCall = () => { Debug.Log("animator_00 -- OnUpdateCall_01"); },
                OnDelayCall = () => { Debug.Log("animator_00 -- OnDelayCall_01"); },
                OnExitCall = () => { Debug.Log("animator_00 -- OnExitCall_01"); },
            };
            AnimatorStateEventManager.Singleton.AddStateEvent(animator_00, stateEvent00_01);

            //---------------------------  animator_01  ----------------------------
            AnimatorStateEvent stateEvent01_00 = new AnimatorStateEvent()
            {
                fullName = "Base Layer.RunFwdStart",
                layerIndex = 0,
                delayTime = 0.4f,
                OnEnterCall = () => { Debug.Log("animator_01 -- OnEnterCall_00"); },
                OnUpdateCall = () => { Debug.Log("animator_01 -- OnUpdateCall_00"); },
                OnDelayCall = () => { Debug.Log("animator_01 -- OnDelayCall_00"); },
                OnExitCall = () => { Debug.Log("animator_01 -- OnExitCall_00"); },
            };
            AnimatorStateEventManager.Singleton.AddStateEvent(animator_01, stateEvent01_00);

            AnimatorStateEvent stateEvent01_01 = new AnimatorStateEvent()
            {
                fullName = "Base Layer.New StateMachine.RunLt135",
                layerIndex = 0,
                delayTime = 0.2f,
                OnEnterCall = () => { Debug.Log("animator_01 -- OnEnterCall_00"); },
                OnUpdateCall = () => { Debug.Log("animator_01 -- OnUpdateCall_00"); },
                OnDelayCall = () => { Debug.Log("animator_01 -- OnDelayCall_00"); },
                OnExitCall = () => { Debug.Log("animator_01 -- OnExitCall_00"); },
            };
            AnimatorStateEventManager.Singleton.AddStateEvent(animator_00, stateEvent01_01);
        }

        private void Update()
        {
            UpdateCall();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AnimatorStateEventManager.Singleton.RemoveStateEvent(animator_00, "Base Layer.RunFwd");
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                AnimatorStateEventManager.Singleton.RemoveAnimator(animator_00);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                AnimatorStateEventManager.Singleton.ChangeStateEvent(animator_01, "Base Layer.New StateMachine.RunLt135", EAnimatorStateCallType.EnterCall, EAnimatorStateChangeCallType.Append,
                    () =>
                    {
                        Debug.Log("animator_01 EnterCall  Changed");
                        ChangeEnterCall();
                    });
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                AnimatorStateEventManager.Singleton.ChangeStateEvent(animator_01, "Base Layer.New StateMachine.RunLt135", EAnimatorStateCallType.UpdateCall, EAnimatorStateChangeCallType.Cover, null);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                AnimatorStateEvent stateEvent00_00 = new AnimatorStateEvent()
                {
                    fullName = "Base Layer.RunFwd",
                    layerIndex = 0,
                    OnEnterCall = () => { Debug.Log("animator_00 -- OnEnterCall_00 [ Add ]"); },
                };
                AnimatorStateEventManager.Singleton.AddStateEvent(animator_00, stateEvent00_00);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                AnimatorStateEventManager.Singleton.ChangeStateEvent(animator_00, "Base Layer.RunFwd", EAnimatorStateCallType.EnterCall, EAnimatorStateChangeCallType.Remove, action);
            }
        }

        private void ChangeEnterCall()
        {
            Debug.Log("animator_01 EnterCall  Changed 2");
        }
    }
}