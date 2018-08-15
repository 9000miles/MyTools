using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MarsPC
{
    internal class QTEPowerGaugeOperator : QTEOperatorBase
    {
        private int index;
        private int needCount;
        private int keyDownCount;
        private List<EQTEKeyCode> keyList;
        private static QTEPowerGaugeOperator singleton = null;
        private int clickCount;

        public static QTEPowerGaugeOperator Singleton
        {
            get
            {
                if (singleton == null)
                    singleton = new QTEPowerGaugeOperator();
                return singleton;
            }
        }

        public QTEPowerGaugeOperator() : base()
        {
            keyList = new List<EQTEKeyCode>();
        }

        public override void ExcuteAndCheck(QTEInfo info)
        {
            base.ExcuteAndCheck(info);
            if (isInTime == false) return;

            if (keyList.Count == 0)
            {
                EQTEKeyCode[] keys = new EQTEKeyCode[info.powerGauge.keyList.Count];
                info.powerGauge.keyList.CopyTo(keys);
                keyList.AddRange(keys);
                QTETipPanel.Singleton.ShowPowerGauge(true, info.duration);
                needCount = (int)(360f / info.powerGauge.rotateDelta);
            }

            if (info.powerGauge.isKeyboard)
            {
                if (Input.GetKeyDown(keyList[index].ToString().ToLower()))
                {
                    QTETipPanel.Singleton.ShowPowerGauge(true, info.duration, true, info.powerGauge.rotateDelta, info.powerGauge.keyList[index].ToString());
                    keyDownCount++;
                    index++;
                    if (index >= info.powerGauge.keyList.Count)
                        index = 0;
                }

                if (keyDownCount >= needCount)
                {
                    info.excuteTime = Time.time - info.startTime;
                    info.result = EQTEResult.Succed;
                    info.errorType = EQTEErrorType.None;
                    QTETipPanel.Singleton.ShowPowerGauge(false);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown((int)info.powerGauge.mouseButton))
                {
                    GameObject go = GetClickUIGameObject();
                    if (go == info.powerGauge.targetList[index])
                    {
                        QTETipPanel.Singleton.ShowPowerGauge(true, info.duration, true, info.powerGauge.rotateDelta);
                        clickCount++;
                        index++;
                        if (index >= info.powerGauge.targetList.Count)
                            index = 0;
                    }
                }

                if (clickCount >= needCount)
                {
                    info.excuteTime = Time.time - info.startTime;
                    info.result = EQTEResult.Succed;
                    info.errorType = EQTEErrorType.None;
                    QTETipPanel.Singleton.ShowPowerGauge(false);
                }
            }
        }

        public override void ResetData()
        {
            keyDownCount = 0;
            index = 0;
            needCount = 0;
            keyList.Clear();
        }

        public GameObject GetClickUIGameObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0 ? results[0].gameObject : null;
        }
    }
}