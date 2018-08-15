using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MarsPC
{
    internal class QTELinearClickOperator : QTEOperatorBase
    {
        private List<EQTEKeyCode> keyList;
        private static QTELinearClickOperator singleton = null;
        public static QTELinearClickOperator Singleton
        {
            get
            {
                if (singleton == null)
                    singleton = new QTELinearClickOperator();
                return singleton;
            }
        }

        public QTELinearClickOperator() : base()
        {
            keyList = new List<EQTEKeyCode>();
        }

        public override void ExcuteAndCheck(QTEInfo info)
        {
            base.ExcuteAndCheck(info);
            if (isInTime == false) return;

            if (keyList.Count == 0)
            {
                EQTEKeyCode[] keys = new EQTEKeyCode[info.linearClick.keyList.Count];
                info.linearClick.keyList.CopyTo(keys);
                keyList.AddRange(keys);
                QTETipPanel.Singleton.ShowLinearClick(true, false);
            }

            //检查按键操作是否正确
            if (((int)QTEManager.Singleton.keyCode != (int)keyList[0]) &&
                QTEManager.Singleton.keyCode != KeyCode.None)
            {
                info.excuteTime = Time.time - info.startTime;
                info.result = EQTEResult.Failure;
                info.errorType = EQTEErrorType.OperatingError;
                QTETipPanel.Singleton.ShowLinearClick(false);
            }

            if (Input.GetKeyDown(keyList[0].ToString().ToLower()))
            {
                QTETipPanel.Singleton.ShowLinearClick(true, true);
                keyList.RemoveAt(0);
                QTEManager.Singleton.keyCode = KeyCode.None;
            }

            //检查按键是否按完
            if (keyList.Count <= 0)
            {
                info.excuteTime = Time.time - info.startTime;
                info.result = EQTEResult.Succed;
                info.errorType = EQTEErrorType.None;
            }
        }

        public override void ResetData()
        {
            keyList.Clear();
        }
    }
}