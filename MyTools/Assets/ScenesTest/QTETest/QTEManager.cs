using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

//[RequireComponent(typeof(QTEOperationResult))]
//[RequireComponent(typeof(QTEOperationBase))]
//[RequireComponent(typeof(QTECondition))]
public class QTEManager : MonoSingleton<QTEManager>
{
    public Transform panel;
    public Text qteText;
    private List<QTECondition> conditionList;
    public QTECondition currentCondition;
    public QTEInfo currentQTE;
    private QTEInfo lastQTE;
    private KeyCode currentKey;
    [HideInInspector]
    public List<KeyCode> keyList;
    private QTEOperationBase operation;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        CheckCondition();
    }

    public override void Init()
    {
        keyList = new List<KeyCode>();
        conditionList = new List<QTECondition>();
        QTECondition[] conditions = FindObjectsOfType<QTECondition>();
        AddQTE(conditions);
    }

    public void AddQTE(QTECondition condition)
    {
        if (!conditionList.Contains(condition))
            conditionList.Add(condition);
    }

    public void AddQTE(QTECondition[] conditions)
    {
        conditionList.AddRange(conditions.FindAll(t => !conditionList.Contains(t)));
    }

    public void AddQTE(QTECondition condition, QTEInfo info)
    {
        if (!conditionList.Contains(condition))
            conditionList.Add(condition);
        else
            conditionList.Find((t) => t == condition).infoList.Add(info);
    }

    public void RemoveQTE(QTECondition condition)
    {
        if (conditionList.Contains(condition))
            conditionList.Remove(condition);
    }

    public void RemoveQTE(QTECondition condition, QTEInfo info)
    {
        if (conditionList.Contains(condition))
        {
            List<QTEInfo> infoList = conditionList.Find((t) => t == condition).infoList;
            if (infoList.Contains(info))
                infoList.Remove(info);
        }
    }

    public void ClearQTE()
    {
        conditionList.Clear();
    }

    public void CheckCondition()
    {
        foreach (var item in conditionList)
        {
            item.CheckIsTrue();
            if (item.isTrue)
            {
                currentQTE = item.currentQTEInfo;
                if (currentQTE != null && currentQTE.isActive == true)
                    ExcuteQTE(currentQTE);
            }
        }
    }

    public QTEInfo GetQTE(QTECondition condition, string description)
    {
        return conditionList.Find((t) => t == condition).infoList.Find((t) => t.description == description);
    }

    public QTEInfo GetQTE(QTECondition condition, int index)
    {
        return conditionList.Find((t) => t == condition).infoList[index];
    }

    public QTEInfo GetCurrentQTE()
    {
        return currentQTE;
    }

    public void ExcuteQTE(QTEInfo info, Action endCall = null)
    {
        if (info == null) return;
        ShowQTEPanel(info);
        operation = SelecteOperationType(info);
        operation.Excute(info);
        HideQTEPanel(info);
        if (info.result == QTEResult.Succed)
            endCall?.Invoke();
        PrintMessage(info);
        currentQTE = info;
        lastQTE = currentQTE;
    }

    private void PrintMessage(QTEInfo info)
    {
        if (info.result == QTEResult.Failure)
        {
            //Debug.LogError("QTE Operation Failure !    -- " + info.errorType);5
        }
    }

    private QTEOperationBase SelecteOperationType(QTEInfo info)
    {
        QTEOperationBase operationBase = null;
        switch (info.type)
        {
            case QTEType.None:
                break;

            case QTEType.QuickClick:
                operationBase = new QTEQuickClick();
                break;

            case QTEType.PreciseClick:
                operationBase = new QTEPreciseClick();
                break;

            case QTEType.MouseGestures:
                operationBase = new QTEMouseGestures();
                break;

            case QTEType.KeyCombination:
                operationBase = new QTEKeyCombination();
                break;

            case QTEType.Others:
                break;

            default:
                break;
        }
        return operationBase;
    }

    public void ShowQTEPanel(QTEInfo info)
    {
        panel.localPosition = info.UILocalPosition;
        qteText.text = info.description;
        panel.gameObject.SetActive(true);
    }

    public async void HideQTEPanel(QTEInfo info)
    {
        if (info.result == QTEResult.Succed)
        {
            panel.gameObject.SetActive(false);
        }
        else
        {
            await new WaitForSeconds(info.duration);
            panel.gameObject.SetActive(false);
        }
        info.ResetQTEInfo();
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (e != null && e.isKey)
        {
            if (e.keyCode != KeyCode.None && e.type == EventType.KeyDown)
                keyList.Add(e.keyCode);
        }
    }
}