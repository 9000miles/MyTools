using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.Events;

public class PlayerTrigger2DManager : SingletonBehaviour<PlayerTrigger2DManager>
{
    public UnityAction OnEnterCall2D;
    public UnityAction OnStayCall2D;
    public UnityAction OnExitCall2D;
    public Dictionary<Collider2D, TriggerBase> triggerDic_2D = new Dictionary<Collider2D, TriggerBase>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerDic_2D.ContainsKey(other))
        {
        }
        OnEnterCall2D?.Invoke();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OnStayCall2D?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnExitCall2D?.Invoke();
    }
}