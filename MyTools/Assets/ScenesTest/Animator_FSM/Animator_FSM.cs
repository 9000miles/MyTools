using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Playables;

public class Animator_FSM : MonoBehaviour
{
    public AnimatorController controller;

    private void Start()
    {
        PrintName();
    }

    public void PrintName()
    {
        var layers = controller.layers;
        for (int i = 0; i < layers.Length; i++)
        {
            for (int j = 0; j < layers[i].stateMachine.states.Length; j++)
            {
                string name = layers[i].stateMachine.states[j].state.name;
                Debug.Log(name);
            }
        }
    }

    private void Update()
    {
    }
}