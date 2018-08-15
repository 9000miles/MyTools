using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MarsPC
{
    internal class QTESingleKeyPhythmOperator : QTEOperatorBase
    {
        private float time;
        private List<EQTEKeyCode> keyList;
        private static QTESingleKeyPhythmOperator singleton = null;
        public static QTESingleKeyPhythmOperator Singleton
        {
            get
            {
                if (singleton == null)
                    singleton = new QTESingleKeyPhythmOperator();
                return singleton;
            }
        }

        public QTESingleKeyPhythmOperator() : base()
        {
            keyList = new List<EQTEKeyCode>();
        }

        public override void ExcuteAndCheck(QTEInfo info)
        {
            base.ExcuteAndCheck(info);
            if (isInTime == false) return;

            time = Time.time - info.startTime;
            if (keyList.Count == 0)
            {
                EQTEKeyCode[] keys = new EQTEKeyCode[info.singleKeyPhythm.keyList.Count];
                info.singleKeyPhythm.keyList.CopyTo(keys);
                keyList.AddRange(keys);
                QTETipPanel.Singleton.ShowSingleKeyRhythm(true, false, info.duration);
            }

            //检查按键操作是否正确
            if (((int)QTEManager.Singleton.keyCode != (int)keyList[0]) &&
                QTEManager.Singleton.keyCode != KeyCode.None)
            {
                info.excuteTime = Time.time - info.startTime;
                info.result = EQTEResult.Failure;
                info.errorType = EQTEErrorType.OperatingError;
                QTETipPanel.Singleton.ShowSingleKeyRhythm(false);
            }

            if (Input.GetKeyDown(keyList[0].ToString().ToLower()))
            {
                if (time > 0 && 100f - (time / info.duration * 100f) < info.singleKeyPhythm.errorRange)
                {
                    QTETipPanel.Singleton.ShowSingleKeyRhythm(true, true, info.duration);
                    info.startTime = Time.time;
                    keyList.RemoveAt(0);
                    QTEManager.Singleton.keyCode = KeyCode.None;
                }
                else
                {
                    info.result = EQTEResult.Failure;
                    info.errorType = EQTEErrorType.OperatingError;
                    QTETipPanel.Singleton.ShowSingleKeyRhythm(false);
                }
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
            time = 0;
            keyList.Clear();
        }
    }
}