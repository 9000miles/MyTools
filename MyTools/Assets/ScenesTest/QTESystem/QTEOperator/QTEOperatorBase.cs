using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsPC
{
    public abstract class QTEOperatorBase
    {
        protected bool isInTime;

        public QTEOperatorBase()
        {
        }

        public abstract void ResetData();

        public virtual void ExcuteAndCheck(QTEInfo info)
        {
            CheckIsInTime(info);
        }

        private void CheckIsInTime(QTEInfo info)
        {
            //检查是否在时间范围内
            if (Time.time <= info.startTime + info.duration)
            {
                info.result = EQTEResult.None;
                info.errorType = EQTEErrorType.None;
                isInTime = true;
            }
            else
            {
                info.excuteTime = Time.time - info.startTime;
                info.result = EQTEResult.Failure;
                info.errorType = EQTEErrorType.OverTime;
                isInTime = false;
                QTETipPanel.Singleton.ShowSingleKeyContinue(false);
                QTETipPanel.Singleton.ShowSingleKeyRhythm(false);
                QTETipPanel.Singleton.ShowDoubleKeyRepeat(false);
                QTETipPanel.Singleton.ShowLinearDirection(false);
                QTETipPanel.Singleton.ShowLinearClick(false);
                QTETipPanel.Singleton.ShowScrollBar(false);
                QTETipPanel.Singleton.ShowPowerGauge(false);
                QTETipPanel.Singleton.ShowMouseGestures(false);
                QTETipPanel.Singleton.ShowFocusPoint(false);
            }
        }
    }
}