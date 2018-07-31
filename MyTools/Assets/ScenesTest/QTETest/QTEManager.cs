using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTEManager : MonoBehaviour
{
    public Transform panel;
    public Text qteText;
    public List<QTECondition> conditionList;
    private Dictionary<QTECondition, List<QTEInfo>> qteDic;
    public QTEInfo currentQTE;
    public QTEInfo lastQTE;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        CheckCondition();
    }

    public void Init()
    {
        conditionList = new List<QTECondition>();
        qteDic = new Dictionary<QTECondition, List<QTEInfo>>();
        QTECollisionTrigger trigger = FindObjectOfType<QTECollisionTrigger>();
        AddQTE(trigger, trigger.info);
    }

    public void AddQTE(QTECondition condition)
    {
        if (!qteDic.ContainsKey(condition))
            qteDic.Add(condition, new List<QTEInfo>());
        else
            qteDic[condition] = new List<QTEInfo>();
    }

    public void AddQTE(QTECondition condition, QTEInfo info)
    {
        if (!qteDic.ContainsKey(condition))
        {
            qteDic.Add(condition, new List<QTEInfo>());
            if (!qteDic[condition].Contains(info))
                qteDic[condition].Add(info);
        }
        else
        {
            if (!qteDic[condition].Contains(info))
                qteDic[condition].Add(info);
        }
    }

    public void RemoveQTE(QTECondition condition)
    {
        qteDic.Remove(condition);
    }

    public void RemoveQTE(QTECondition condition, QTEInfo info)
    {
        qteDic[condition].Remove(info);
    }

    public void ClearQTE()
    {
        qteDic.Clear();
    }

    public void CheckCondition()
    {
        foreach (var item in conditionList)
        {
            item.CheckIsTrue();
            if (item.isTrue)
            {
                currentQTE = GetQTE(item, item.description);
                ExcuteQTE(currentQTE);
            }
        }
    }

    public QTEInfo GetQTE(QTECondition condition, string description)
    {
        return qteDic[condition].Find((t) => t.description == description);
    }

    public QTEInfo GetQTE(QTECondition condition, int index)
    {
        return qteDic[condition][index];
    }

    public QTEInfo GetCurrentQTE()
    {
        return currentQTE;
    }

    public void ShowQTE()
    {
        panel.position = currentQTE.position;
        qteText.text = currentQTE.description;
        panel.gameObject.SetActive(true);
    }

    public void HideQTE()
    {
        panel.gameObject.SetActive(false);
    }

    public void ExcuteQTE(QTEInfo info, Action onCall = null)
    {
        switch (currentQTE.type)
        {
            case QTEType.None:
                break;

            case QTEType.SingleKey:
                break;

            case QTEType.MultiKey:
                break;

            default:
                break;
        }
        ShowQTE();
        onCall?.Invoke();
        currentQTE.result = CheckQTEResult();
    }

    public QTEResult CheckQTEResult()
    {
        return QTEResult.None;
    }
}

[Serializable]
public class QTEInfo
{
    public string description;
    public float time;
    public Vector2 position;
    private QTECondition condition;
    public QTEType type;
    public QTEResult result;

    public QTEInfo()
    {
    }

    public QTEInfo(string description, float time, Vector2 position, QTEType type)
    {
        this.description = description;
        this.time = time;
        this.position = position;
        this.type = type;
        this.result = QTEResult.None;
    }
}

public enum QTEType
{
    None,
    SingleKey,
    MultiKey,
}

public enum QTEResult
{
    None,
    Succed,
    Failure,
}