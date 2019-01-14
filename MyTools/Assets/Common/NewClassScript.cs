using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using MarsPC;

public class GameManager : SingletonBehaviour<GameManager>
{
    private Dictionary<object, Action> UpdateDic;
    private Dictionary<object, Action> FixUpdateDic;
    private Dictionary<object, Action> LateUpdateDic;
    private Dictionary<object, Action> OnGUIDic;

    private void Start()
    {
        Init();
    }

    public void Init(bool isShowStart = true)
    {
        UpdateDic = new Dictionary<object, Action>();
        FixUpdateDic = new Dictionary<object, Action>();
        LateUpdateDic = new Dictionary<object, Action>();
        OnGUIDic = new Dictionary<object, Action>();
    }

    public void InitGameManager()
    {
        AnimatorStateEventManager.Singleton.Init();
    }

    public void RegisterAction(EUpdateType eMonoType, object obj, Action action)
    {
        switch (eMonoType)
        {
            case EUpdateType.Update:
                UpdateDic[obj] = action;
                break;

            case EUpdateType.FixedUpdate:
                FixUpdateDic[obj] = action;
                break;

            case EUpdateType.LateUpdate:
                LateUpdateDic[obj] = action;
                break;

            case EUpdateType.OnGUI:
                OnGUIDic[obj] = action;
                break;
        }
    }

    private void Update()
    {
        if (UpdateDic == null)
        {
            return;
        }
        Dictionary<object, Action>.Enumerator dic = UpdateDic.GetEnumerator();
        while (dic.MoveNext())
        {
            dic.Current.Value();
        }
    }

    private void FixedUpdate()
    {
        if (FixUpdateDic == null)
        {
            return;
        }
        Dictionary<object, Action>.Enumerator dic = FixUpdateDic.GetEnumerator();
        while (dic.MoveNext())
        {
            dic.Current.Value();
        }
    }

    private void LateUpdate()
    {
        if (LateUpdateDic == null)
        {
            return;
        }
        Dictionary<object, Action>.Enumerator dic = LateUpdateDic.GetEnumerator();
        while (dic.MoveNext())
        {
            dic.Current.Value();
        }
    }

    private void OnGUI()
    {
        if (OnGUIDic == null)
        {
            return;
        }
        Dictionary<object, Action>.Enumerator dic = OnGUIDic.GetEnumerator();
        while (dic.MoveNext())
        {
            dic.Current.Value();
        }
    }

    public virtual void Clear()
    {
        UpdateDic = null;
        FixUpdateDic = null;
        LateUpdateDic = null;
        OnGUIDic = null;
    }
}

public enum EUpdateType
{
    Update,
    FixedUpdate,
    LateUpdate,
    OnGUI
}