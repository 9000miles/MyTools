using UnityEngine;
using System.Collections;
using MarsPC;
using Common;
using System;
using System.Collections.Generic;

public class TriggerManager : SingletonBehaviour<TriggerManager>
{
    private TriggerBehaviour[] triggerBehaviours;

    public override void Init()
    {
        base.Init();
        triggerBehaviours = GameObject.FindObjectsOfType<TriggerBehaviour>();
        for (int i = 0; i < triggerBehaviours.Length; i++)
        {
            Dictionary<TriggerColliderBase, List<TriggerBase>>.ValueCollection.Enumerator triggerBases = triggerBehaviours[i].triggerDic.Values.GetEnumerator();
            while (triggerBases.MoveNext())
            {
                triggerBases.Current.ForEach((t) => t.Init());
            }
        }
    }

    public void AddTrigger(string targetTag, Collider collider, Collider2D collider2D, TriggerBase trigger)
    {
        TriggerBehaviour[] target = GameObject.FindGameObjectsWithTag(targetTag).Select(t => t.GetComponent<TriggerBehaviour>());
        for (int i = 0; i < target.Length; i++)
        {
            TriggerColliderBase[] colliders = new TriggerColliderBase[target[i].triggerDic.Count];
            target[i].triggerDic.Keys.CopyTo(colliders, 0);
            bool isCollider = collider != null;
            bool isExists = Array.Exists(colliders, t =>
            {
                if (t.collider == null && t.collider == null) return false;
                return isCollider ? t.collider == collider : t.collider2D == collider2D;
            });

            if (!isExists)
            {
                List<TriggerBase> list = new List<TriggerBase>();
                list.Add(trigger);
                if (isCollider)
                    target[i].triggerDic.Add(new TriggerColliderBase(collider, null), list);
                else
                    target[i].triggerDic.Add(new TriggerColliderBase(null, collider2D), list);
            }
            else
            {
                TriggerColliderBase colliderBase = colliders.Find(t =>
                {
                    return isCollider ? t.collider == collider : t.collider2D == collider2D;
                });
                target[i].triggerDic[colliderBase].Add(trigger);
                Debug.LogError(trigger.transform.GetSelfPath() + " there are multiple TriggerBase");
            }
        }
    }

    public void RemoveTrigger(string targetTag, Collider collider, Collider2D collider2D, TriggerBase trigger)
    {
        TriggerBehaviour[] target = GameObject.FindGameObjectsWithTag(targetTag).Select(t => t.GetComponent<TriggerBehaviour>());
        for (int i = 0; i < target.Length; i++)
        {
            TriggerColliderBase[] colliders = new TriggerColliderBase[target[i].triggerDic.Count];
            target[i].triggerDic.Keys.CopyTo(colliders, 0);
            bool isCollider = collider != null;
            bool isExists = Array.Exists(colliders, t =>
            {
                if (t.collider == null && t.collider == null) return false;
                return isCollider ? t.collider == collider : t.collider2D == collider2D;
            });

            //if (!isExists)
            //{
            //    List<TriggerBase> list = new List<TriggerBase>();
            //    list.Add(trigger);
            //    if (isCollider)
            //        target[i].triggerDic.Remove( new TriggerColliderBase(collider, null), list);
            //    else
            //        target[i].triggerDic.Add(new TriggerColliderBase(null, collider2D), list);
            //}
            //else
            //{
            //    TriggerColliderBase colliderBase = colliders.Find(t =>
            //    {
            //        return isCollider ? t.collider == collider : t.collider2D == collider2D;
            //    });
            //    target[i].triggerDic[colliderBase].Add(trigger);
            //    Debug.LogError(trigger.transform.GetSelfPath() + " there are multiple TriggerBase");
            //}
        }
    }
}