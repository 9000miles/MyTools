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
    public KeyCode keyCode;
    private List<QTEConditionBase> conditionList;
    private QTEConditionBase currentCondition;
    private QTEConditionBase lastCondition;
    private QTEInfo currentQTE;
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
            conditionList.Find(t => t == condition).infoList.Add(info);
    }

    public void RemoveQTECondition(QTEConditionBase condition)
    {
        if (conditionList.Contains(condition))
            conditionList.Remove(condition);
    }

    public void RemoveQTE(QTEConditionBase condition, QTEInfo info)
    {
        if (conditionList.Contains(condition))
        {
            List<QTEInfo> infoList = conditionList.Find(t => t == condition).infoList;
            if (infoList.Contains(info))
            {
                infoList.Remove(info);
            }
        }
    }

    private void AutoActiveNextQTE(QTEConditionBase condition)
    {
        List<QTEInfo> infoList = conditionList.Find(t => t == condition).infoList;
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
            if (item.isTrue)
            {
                if (item.currentQTEInfo != null && item.currentQTEInfo.isActive == true)
                    ExcuteQTE(item, item.currentQTEInfo);
            }
        }
    }

    public QTEInfo GetQTE(QTEConditionBase condition, string description)
    {
        return conditionList.Find(t => t == condition).infoList.Find(t => t.description == description);
    }

    public QTEInfo GetQTE(QTEConditionBase condition, int id)
    {
        return conditionList.Find(t => t == condition).infoList.Find(t => t.ID == id);
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
        QTEExecutiveOutcomes(info, endCall);
    }

    private QTEOperationBase SelecteOperationType(QTEInfo info)
    {
        QTEOperationBase operationBase = null;
        switch (info.type)
        {
            case QTEType.None:
                break;

            case QTEType.QuickClick:
                operationBase = QTEQuickClick.Singleton;
                break;

            case QTEType.PreciseClick:
                operationBase = new QTEPreciseClick();
                break;

            case QTEType.MouseGestures:
                operationBase = new QTEMouseGestures();
                break;

            case QTEType.KeyCombination:
                operationBase = new QTEKeyCombination();
                //operationBase = QTEKeyCombination.Singleton;
                //operationBase = QTEKeyCombination.;
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

    /// <summary>
    /// QTE执行结果
    /// </summary>
    /// <param name="info"></param>
    /// <param name="endCall"></param>
    public void QTEExecutiveOutcomes(QTEInfo info, Action endCall)
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
        lastQTE = currentQTE;
        RemoveQTE(currentCondition, info);
        info.ResetQTEInfo();
        currentQTE = null;
        QTEOperationBase.Singleton.EmptyResult();
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (e != null && e.type == EventType.KeyDown && e.isKey)
        {
            keyCode = e.keyCode;
        }
    }
}