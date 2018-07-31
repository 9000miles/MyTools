using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTEManager : MonoBehaviour
{
    public Transform panel;
    public Text qteText;
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
        foreach (var item in qteDic.Keys)
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

    public void ExcuteQTE(QTEInfo info, Action endCall = null)
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
        currentQTE.result = CheckQTEResult(info);
        HideQTE();
        if (currentQTE.result == QTEResult.Succed)
        {
            endCall?.Invoke();
        }
        else
        {
            Debug.LogError("QTE Operation Failure !");
        }
        lastQTE = currentQTE;
    }

    public void ShowQTE()
    {
        panel.localPosition = currentQTE.position;
        qteText.text = currentQTE.description;
        panel.gameObject.SetActive(true);
    }

    public QTEResult CheckQTEResult(QTEInfo info)
    {
        bool isSucced = GetOperationIsSucced(info);
        bool isInTime = GetOperationIsInTime(info);
        if (isSucced == true && isInTime == true)
            info.result = QTEResult.Succed;
        else
            info.result = QTEResult.Failure;
        return info.result;
    }

    /// <summary>
    /// 检测操作是否正确
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public bool GetOperationIsSucced(QTEInfo info)
    {
        return false;
    }

    /// <summary>
    /// 检测是否在指定时间内完成操作
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public bool GetOperationIsInTime(QTEInfo info)
    {
        //检测在时间范围内，是否作出正确操作
        //如果失败了，打印错误类型
        return false;
    }

    public void HideQTE()
    {
        panel.gameObject.SetActive(false);
    }
}

[Serializable]
public class QTEInfo
{
    public string description;
    public float startTime;
    public float time;
    public Vector2 position;
    private QTECondition condition;
    public QTEType type;
    public QTEResult result;

    public QTEInfo()
    {
        startTime = Time.time;
    }

    public QTEInfo(string description, float time, Vector2 position, QTEType type)
    {
        this.description = description;
        this.time = time;
        this.startTime = Time.time;
        this.position = position;
        this.type = type;
        this.result = QTEResult.None;
    }
}

public enum QTEErrorType
{
    None,
    OverTime,
    OperatingError,
}

public enum QTEType
{
    None,
    SingleKey,
    MultiKey,
    Other,
}

public enum QTEResult
{
    None,
    Succed,
    Failure,
}