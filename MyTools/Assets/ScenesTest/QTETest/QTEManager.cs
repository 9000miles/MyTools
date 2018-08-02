using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
using System.Threading.Tasks;

//[RequireComponent(typeof(QTEOperationResult))]
//[RequireComponent(typeof(QTEOperationBase))]
//[RequireComponent(typeof(QTECondition))]
public class QTEManager : SingletonBehaviour<QTEManager>
{
    public bool isNewQTE;
    public Transform panel;
    public Text qteText;
    private List<QTEConditionBase> conditionList;
    public QTEConditionBase currentCondition;
    public QTEConditionBase lastCondition;
    public QTEInfo currentQTE;
    private QTEInfo lastQTE;
    private QTEOperationBase operation;
    public Action succedCall;
    public Action failureCall;

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
        conditionList = new List<QTEConditionBase>();
        QTEConditionBase[] conditions = FindObjectsOfType<QTEConditionBase>();
        AddQTE(conditions);
    }

    public void AddQTE(QTEConditionBase condition)
    {
        if (!conditionList.Contains(condition))
            conditionList.Add(condition);
    }

    public void AddQTE(QTEConditionBase[] conditions)
    {
        conditionList.AddRange(conditions.FindAll(t => !conditionList.Contains(t)));
    }

    public void AddQTE(QTEConditionBase condition, QTEInfo info)
    {
        if (!conditionList.Contains(condition))
            conditionList.Add(condition);
        else
            conditionList.Find((t) => t == condition).infoList.Add(info);
    }

    public void RemoveQTE(QTEConditionBase condition)
    {
        if (conditionList.Contains(condition))
            conditionList.Remove(condition);
    }

    public void RemoveQTE(QTEConditionBase condition, QTEInfo info)
    {
        if (conditionList.Contains(condition))
        {
            List<QTEInfo> infoList = conditionList.Find((t) => t == condition).infoList;
            if (infoList.Contains(info))
            {
                infoList.Remove(info);
            }
        }
    }

    private void AutoActiveNextQTE(QTEConditionBase condition)
    {
        List<QTEInfo> infoList = conditionList.Find((t) => t == condition).infoList;
        if (infoList.Count > 0)
        {
            infoList[0].isActive = true;
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
            if (item.isTrue && (lastCondition != currentCondition || currentCondition == null))
            {
                if (item.currentQTEInfo != null && item.currentQTEInfo.isActive == true)
                    ExcuteQTE(item, item.currentQTEInfo);
            }
        }
    }

    public QTEInfo GetQTE(QTEConditionBase condition, string description)
    {
        return conditionList.Find((t) => t == condition).infoList.Find((t) => t.description == description);
    }

    public QTEInfo GetQTE(QTEConditionBase condition, int index)
    {
        return conditionList.Find((t) => t == condition).infoList[index];
    }

    public QTEInfo GetCurrentQTE()
    {
        return currentQTE;
    }

    public QTEConditionBase GetCurrentQTECondition()
    {
        return currentCondition;
    }

    public void ExcuteQTE(QTEConditionBase condition, QTEInfo info, Action endCall = null)
    {
        if (info == null) return;
        isNewQTE = true;
        currentCondition = condition;
        currentQTE = info;
        ShowQTEPanel(info);
        operation = SelecteOperationType(info);
        operation.Excute(info);
        HideQTEPanel(info, endCall);
        PrintMessage(info);
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

    public void HideQTEPanel(QTEInfo info, Action endCall)
    {
        if (info.result == QTEResult.None) return;
        switch (info.result)
        {
            case QTEResult.Succed:
                succedCall?.Invoke();
                break;

            case QTEResult.Failure:
                failureCall?.Invoke();
                break;
        }
        if (info.result == QTEResult.Succed || info.errorType == QTEErrorType.OverTime)
            panel.gameObject.SetActive(false);
        //AutoActiveNextQTE(currentCondition);
        isNewQTE = false;
        Debug.Log("          操作结果：" + info.result + "             错误类型：" + info.errorType);

        lastCondition = currentCondition;
        RemoveQTE(currentCondition, info);
        info.ResetQTEInfo();
        currentQTE = null;
        QTEOperationBase.Singleton.EmptyResult();
    }
}