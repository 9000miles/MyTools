using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Common;

//[RequireComponent(typeof(QTEOperationResult))]
//[RequireComponent(typeof(QTEOperationBase))]
//[RequireComponent(typeof(QTECondition))]
public class QTEManager : SingletonBehaviour<QTEManager>
{
    [HideInInspector]
    public bool isManualQTE;
    private bool isStartTimeHasSet;
    public Text qteText;
    public Transform panel;
    [HideInInspector]
    public KeyCode keyCode;
    public Event keyCodeEvent;
    private List<QTEConditionBase> conditionList;
    private QTEConditionBase currentCondition;
    private QTEConditionBase lastCondition;
    private QTEInfo currentQTE;
    private QTEInfo lastQTE;
    private QTEOperationBase QTEOperator;
    public Action succedCall;
    public Action failureCall;
    public Action<QTEInfo> endCallAction;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        CheckCondition();
        if (isManualQTE == true)
            ManualExcuteQTE(currentQTE, endCallAction);
    }

    public void Init()
    {
        conditionList = new List<QTEConditionBase>();
        QTEConditionBase[] conditions = FindObjectsOfType<QTEConditionBase>();
        AddQTE(conditions);
    }

    public void ActiveQTE()
    {
        currentCondition.info.isActive = true;
        keyCode = KeyCode.None;
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
        condition.info = info;
    }

    public void RemoveQTECondition(QTEConditionBase condition)
    {
        if (conditionList.Contains(condition))
            conditionList.Remove(condition);
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
                currentCondition = item;
                if (item.currentQTEInfo != null && item.currentQTEInfo.isActive == true)
                    ExcuteQTE(item, item.currentQTEInfo);
            }
        }
        if (currentCondition != null && currentCondition.isTrue == false)
            panel.gameObject.SetActive(false);
    }

    public QTEInfo GetCurrentQTE()
    {
        return currentQTE;
    }

    public QTEConditionBase GetCurrentQTECondition()
    {
        return currentCondition;
    }

    public void ManualExcuteQTE(QTEInfo info, Action<QTEInfo> endCall = null)
    {
        if (info == null) return;
        isManualQTE = true;
        if (isStartTimeHasSet == false)
            info.startTime = Time.time;
        isStartTimeHasSet = true;
        currentQTE = info;
        endCallAction = endCall;
        ShowQTEPanel(info);
        QTEOperator = SelecteOperatorType(info);
        QTEOperator?.ExcuteAndCheck(info);
        QTEExecutiveOutcomes(info, true, endCall);
    }

    public void ExcuteQTE(QTEConditionBase condition, QTEInfo info, Action<QTEInfo> endCall = null)
    {
        if (info == null) return;
        if (info.type == QTEType.None)
        {
            Debug.LogError("请设置" + condition.owerTF.name + "物体身上的 QTEInfo 信息");
            return;
        }
        currentCondition = condition;
        currentQTE = info;
        ShowQTEPanel(info);
        QTEOperator = SelecteOperatorType(info);
        QTEOperator.ExcuteAndCheck(info);
        QTEExecutiveOutcomes(info, false, endCall);
    }

    private QTEOperationBase SelecteOperatorType(QTEInfo info)
    {
        QTEOperationBase operation = null;
        switch (info.type)
        {
            case QTEType.QuickClick:
                operation = QTEQuickClick.Singleton;
                break;

            case QTEType.PreciseClick:
                operation = QTEPreciseClick.Singleton;
                break;

            case QTEType.MouseGestures:
                operation = QTEMouseGestures.Singleton;
                break;

            case QTEType.KeyCombination:
                operation = QTEKeyCombination.Singleton;
                break;
        }
        return operation;
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
    public void QTEExecutiveOutcomes(QTEInfo info, bool isManual, Action<QTEInfo> endCall)
    {
        if (info.result == QTEResult.None) return;
        isStartTimeHasSet = false;
        switch (info.result)
        {
            case QTEResult.Succed:
                succedCall?.Invoke();
                break;

            case QTEResult.Failure:
                failureCall?.Invoke();
                break;
        }
        endCall?.Invoke(info);
        Debug.Log("          操作结果：" + info.result + "             错误类型：" + info.errorType);
        if (info.result == QTEResult.Succed || info.result == QTEResult.Failure || info.errorType == QTEErrorType.OverTime)
            panel.gameObject.SetActive(false);

        isManualQTE = false;
        lastCondition = currentCondition;
        lastQTE = currentQTE;
        info.ResetQTEInfo(true);
        QTEOperator.ResetData();
    }

    private void OnGUI()
    {
        keyCodeEvent = Event.current;
        if (keyCodeEvent != null && keyCodeEvent.type == EventType.KeyDown &&
            keyCodeEvent.isKey && keyCodeEvent.keyCode != KeyCode.None &&
            ((int)keyCodeEvent.keyCode < 323 || (int)keyCodeEvent.keyCode > 329))
        {
            keyCode = keyCodeEvent.keyCode;
        }
    }
}