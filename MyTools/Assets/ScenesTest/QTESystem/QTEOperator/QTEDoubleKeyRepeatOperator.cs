using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MarsPC
{
    internal class QTEDoubleKeyRepeatOperator : QTEOperatorBase
    {
        private int keyDownCount;
        private List<EQTEKeyCode> keyList;
        private static QTEDoubleKeyRepeatOperator singleton = null;
        public static QTEDoubleKeyRepeatOperator Singleton
        {
            get
            {
                if (singleton == null)
                    singleton = new QTEDoubleKeyRepeatOperator();
                return singleton;
            }
        }

        public QTEDoubleKeyRepeatOperator() : base()
        {
            keyList = new List<EQTEKeyCode>();
        }

        public override void ExcuteAndCheck(QTEInfo info)
        {
            base.ExcuteAndCheck(info);
            if (isInTime == false) return;

            if (keyList.Count == 0)
            {
                EQTEKeyCode[] keys = new EQTEKeyCode[info.doubleKeyRepeat.keyList.Count];
                info.doubleKeyRepeat.keyList.CopyTo(keys);
                keyList.AddRange(keys);
                QTETipPanel.Singleton.ShowDoubleKeyRepeat(true, true, info.doubleKeyRepeat.keyList[keyDownCount % 2]);
            }

            if (Input.GetKeyDown(keyList[keyDownCount % 2].ToString().ToLower()))
            {
                keyDownCount++;
                info.startTime = Time.time;
                bool isLeft = keyDownCount % 2 == 0;
                QTEManager.Singleton.keyCode = KeyCode.None;
                QTETipPanel.Singleton.ShowDoubleKeyRepeat(true, isLeft, info.doubleKeyRepeat.keyList[keyDownCount % 2]);
            }

            if (keyDownCount > 0 && 100f - (keyDownCount / info.doubleKeyRepeat.keyCount * 100f) < info.doubleKeyRepeat.errorRange)
            {
                info.result = EQTEResult.Failure;
                info.errorType = EQTEErrorType.OperatingError;
                QTETipPanel.Singleton.ShowDoubleKeyRepeat(false);
            }

            if (keyDownCount >= info.doubleKeyRepeat.keyCount)
            {
                info.excuteTime = Time.time - info.startTime;
                info.result = EQTEResult.Succed;
                info.errorType = EQTEErrorType.None;
            }

            if (keyList.Count <= 0)
            {
                info.excuteTime = Time.time - info.startTime;
                info.result = EQTEResult.Succed;
                info.errorType = EQTEErrorType.None;
            }
        }

        public override void ResetData()
        {
            keyDownCount = 0;
            keyList.Clear();
        }
    }
}