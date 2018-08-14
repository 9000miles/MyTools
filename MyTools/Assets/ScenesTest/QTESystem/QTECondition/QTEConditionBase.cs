using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public abstract class QTEConditionBase : SingletonBehaviour<QTEConditionBase>
{
    [HideInInspector]
    public bool isTrue;
    private bool isStartTimeHasSet;
    public QTEInfo info;
    [HideInInspector]
    public Transform owerTF;
    [HideInInspector]
    public QTEInfo currentQTEInfo;

    private void Start()
    {
        owerTF = transform;
    }

    protected abstract bool Check();

    public void CheckIsTrue()
    {
        isTrue = Check();
        if (isTrue)
        {
            if (info.isAutomaticActive)
            {
                info.isActive = true;
            }
            if (info != null && info.isActive)
            {
                if (isStartTimeHasSet == false)
                {
                    isStartTimeHasSet = true;
                    info.startTime = Time.time;
                }
                currentQTEInfo = info;
            }
            QTETipPanel.Singleton.ShowEnterQTEButton(!info.isAutomaticActive);
        }
        else
        {
            if (info != QTEManager.Singleton.GetCurrentQTE()) return;
            isStartTimeHasSet = false;
            QTETipPanel.Singleton.ShowEnterQTEButton(false);
            QTETipPanel.Singleton.ShowSingleKeyContinue(false);
            QTETipPanel.Singleton.ShowSingleKeyRhythm(false);
            QTETipPanel.Singleton.ShowDoubleKeyRepeat(false);
            QTETipPanel.Singleton.ShowLinearClick(false);
        }
    }
}