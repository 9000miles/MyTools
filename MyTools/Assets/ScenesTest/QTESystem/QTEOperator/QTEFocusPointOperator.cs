using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MarsPC
{
    internal class QTEFocusPointOperator : QTEOperatorBase
    {
        private int targetCount;
        private RaycastHit hit;
        private List<GameObject> targetList;
        private static QTEFocusPointOperator singleton = null;
        public static QTEFocusPointOperator Singleton
        {
            get
            {
                if (singleton == null)
                    singleton = new QTEFocusPointOperator();
                return singleton;
            }
        }

        public QTEFocusPointOperator() : base()
        {
            targetList = new List<GameObject>();
        }

        public override void ExcuteAndCheck(QTEInfo info)
        {
            base.ExcuteAndCheck(info);
            if (isInTime == false) return;

            if (targetList.Count == 0)
            {
                targetCount = info.focusPoint.targetList.Count;
                GameObject[] targets = new GameObject[info.focusPoint.targetList.Count];
                info.focusPoint.targetList.CopyTo(targets);
                targetList.AddRange(targets);
                if (info.focusPoint.isUIObject)
                    QTETipPanel.Singleton.ShowFocusPoint(true, info.duration, info.focusPoint.targetList.Count);
            }

            if (info.focusPoint.isUIObject)
            {
                info.focusPoint.targetList.RemoveAll(t => t == null);
            }
            else
            {
                if (Input.GetMouseButtonDown((int)info.focusPoint.mouseButton))
                {
                    Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
                    if (targetList.Count > 0 && hit.transform.gameObject == targetList[0])
                    {
                        targetList.RemoveAt(0);
                    }
                }
            }

            if (1f - (float)info.focusPoint.targetList.Count / targetCount >= info.focusPoint.percentage ||
              1f - (float)targetList.Count / (float)targetCount >= info.focusPoint.percentage)
            {
                info.excuteTime = Time.time - info.startTime;
                info.result = EQTEResult.Succed;
                info.errorType = EQTEErrorType.None;
                QTETipPanel.Singleton.ShowFocusPoint(false);
            }
        }

        public override void ResetData()
        {
            targetList.Clear();
            targetCount = 0;
        }
    }
}