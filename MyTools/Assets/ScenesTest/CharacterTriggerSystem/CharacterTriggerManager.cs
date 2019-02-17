using UnityEngine;
using System.Collections;
using MarsPC;
using Common;
using System;
using System.Collections.Generic;

public class CharacterTriggerManager : SingletonBehaviour<CharacterTriggerManager>
{
    private CharacterTriggerBehaviour[] triggerBehaviours;

    public override void Init()
    {
        base.Init();
        triggerBehaviours = GameObject.FindObjectsOfType<CharacterTriggerBehaviour>();
        for (int i = 0; i < triggerBehaviours.Length; i++)
        {
            Dictionary<TriggerColliderBase, List<TriggerBehaviourBase>>.ValueCollection.Enumerator triggerBases = triggerBehaviours[i].triggerDic.Values.GetEnumerator();
            while (triggerBases.MoveNext())
            {
                triggerBases.Current.ForEach((t) => t.Init());
            }
        }
    }

    public void AddTrigger(string targetTag, Collider collider, Collider2D collider2D, TriggerBehaviourBase trigger)
    {
        CharacterTriggerBehaviour[] target = GameObject.FindGameObjectsWithTag(targetTag).Select(t => t.GetComponent<CharacterTriggerBehaviour>());
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
                List<TriggerBehaviourBase> list = new List<TriggerBehaviourBase>();
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
                if (!target[i].triggerDic[colliderBase].Contains(trigger))
                    target[i].triggerDic[colliderBase].Add(trigger);
                if (target[i].triggerDic[colliderBase].Count > 1)
                    Debug.LogError(trigger.transform.GetSelfPath() + " there are multiple TriggerBase");
            }
        }
    }

    public void RemoveTrigger(string targetTag, Collider collider, Collider2D collider2D, TriggerBehaviourBase trigger)
    {
        CharacterTriggerBehaviour[] target = GameObject.FindGameObjectsWithTag(targetTag).Select(t => t.GetComponent<CharacterTriggerBehaviour>());
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

            if (isExists)
            {
                TriggerColliderBase colliderBase = colliders.Find(t =>
                {
                    return isCollider ? t.collider == collider : t.collider2D == collider2D;
                });
                target[i].triggerDic[colliderBase].Remove(trigger);
            }
        }
    }

    public void ClearTrigger(string targetTag, Collider collider, Collider2D collider2D, TriggerBehaviourBase trigger)
    {
        CharacterTriggerBehaviour[] target = GameObject.FindGameObjectsWithTag(targetTag).Select(t => t.GetComponent<CharacterTriggerBehaviour>());
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

            if (isExists)
            {
                TriggerColliderBase colliderBase = colliders.Find(t =>
                {
                    return isCollider ? t.collider == collider : t.collider2D == collider2D;
                });
                target[i].triggerDic[colliderBase].Clear();
            }
        }
    }
}