using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MarsPC
{
    internal class QTESingleKeyContinueOperator : QTEOperatorBase
    {
        private List<EQTEKeyCode> keyList;
        private static QTESingleKeyContinueOperator singleton = null;
        public static QTESingleKeyContinueOperator Singleton
        {
            get
            {
                if (singleton == null)
                    singleton = new QTESingleKeyContinueOperator();
                return singleton;
            }
        }

        public QTESingleKeyContinueOperator() : base()
        {
            keyList = new List<EQTEKeyCode>();
        }

        public override void ExcuteAndCheck(QTEInfo info)
        {
            base.ExcuteAndCheck(info);
            if (isInTime == false) return;

            if (keyList.Count == 0)
            {
                EQTEKeyCode[] keys = new EQTEKeyCode[info.singleKeyContinue.keyList.Count];
                info.singleKeyContinue.keyList.CopyTo(keys);
                keyList.AddRange(keys);
            }

            if (Input.GetKeyDown(keyList[0].ToString().ToLower()))
            {
                QTETipPanel.Singleton.ShowSingleKeyContinue(true);
                keyList.RemoveAt(0);
                QTEManager.Singleton.keyCode = KeyCode.None;
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
            keyList.Clear();
        }
    }
}