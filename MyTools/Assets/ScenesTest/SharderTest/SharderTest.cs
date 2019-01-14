using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharderTest : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private SkinnedMeshRenderer skinned;
    private Action action;

    // Use this for initialization
    [ContextMenu("fff")]
    private void Start()
    {
        action += () => { };
        action += () => { };
        action += Oowei;
        if (action != null)
        {
            Delegate[] delegates = action.GetInvocationList();
            Debug.Log(delegates.Length);
        }
    }

    private void Oowei()
    {
        Debug.Log("Oowei");
    }

    // Update is called once per frame
    private void Update()
    {
    }
}