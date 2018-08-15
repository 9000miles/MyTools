using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MarsPC
{
    internal class QTEScrollBarClickOperator : QTEOperatorBase
    {
        private static QTEScrollBarClickOperator singleton = null;
        public static QTEScrollBarClickOperator Singleton
        {
            get
            {
                if (singleton == null)
                    singleton = new QTEScrollBarClickOperator();
                return singleton;
            }
        }

        public QTEScrollBarClickOperator() : base()
        {
        }

        public override void ExcuteAndCheck(QTEInfo info)
        {
            base.ExcuteAndCheck(info);

            QTETipPanel.Singleton.ShowScrollBar(true, info.scrollBarClick.speed);
            switch (info.scrollBarClick.mouseButton)
            {
                case EQTEMouseButton.LeftButton:
                    if (Input.GetMouseButtonDown(0))
                    {
                        info.excuteTime = Time.time - info.startTime;
                        info.result = QTETipPanel.Singleton.ShowScrollBar(false);
                    }
                    break;

                case EQTEMouseButton.RightButtton:
                    if (Input.GetMouseButtonDown(1))
                    {
                        info.excuteTime = Time.time - info.startTime;
                        info.result = QTETipPanel.Singleton.ShowScrollBar(false);
                    }
                    break;

                case EQTEMouseButton.MiddleButton:
                    if (Input.GetMouseButtonDown(2))
                    {
                        info.excuteTime = Time.time - info.startTime;
                        info.result = QTETipPanel.Singleton.ShowScrollBar(false);
                    }
                    break;
            }
        }

        public override void ResetData()
        {
        }
    }
}