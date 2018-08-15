using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MarsPC
{
    internal class QTEMouseGesturesOperator : QTEOperatorBase
    {
        private bool isShow;
        private bool isMouseDown;
        private bool isMouseUp;
        private float angle;
        private Vector2 downPos;
        private Vector2 upPos;
        private static QTEMouseGesturesOperator singleton = null;
        public static QTEMouseGesturesOperator Singleton
        {
            get
            {
                if (singleton == null)
                    singleton = new QTEMouseGesturesOperator();
                return singleton;
            }
        }

        public QTEMouseGesturesOperator() : base()
        {
        }

        public override void ExcuteAndCheck(QTEInfo info)
        {
            base.ExcuteAndCheck(info);
            if (isShow == false)
            {
                QTETipPanel.Singleton.ShowMouseGestures(true, info.duration, info.mouseGestures.isShowCountdown, info.mouseGestures.gesturesType);
                isShow = true;
            }
            EMouseGesturesType gesturesType = HandGestureRecognition(info);

            if (isMouseDown && isMouseUp && gesturesType != info.mouseGestures.gesturesType)
            {
                info.excuteTime = Time.time - info.startTime;
                info.result = EQTEResult.Failure;
                info.errorType = EQTEErrorType.OperatingError;
                QTETipPanel.Singleton.ShowMouseGestures(false);
            }

            float distance = Vector2.Distance(downPos, upPos);
            if (gesturesType == info.mouseGestures.gesturesType && gesturesType != EMouseGesturesType.None && distance >= info.mouseGestures.length)
            {
                info.excuteTime = Time.time - info.startTime;
                info.result = EQTEResult.Succed;
                info.errorType = EQTEErrorType.None;
                QTETipPanel.Singleton.ShowMouseGestures(false);
            }
        }

        private EMouseGesturesType HandGestureRecognition(QTEInfo info)
        {
            EMouseGesturesType gesturesType = EMouseGesturesType.None;
            if (Input.GetMouseButtonDown((int)info.mouseGestures.mouseButton))
            {
                isMouseDown = true;
                isMouseUp = false;
                downPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp((int)info.mouseGestures.mouseButton))
            {
                isMouseUp = true;
                upPos = Input.mousePosition;
                switch (info.mouseGestures.gesturesType)
                {
                    case EMouseGesturesType.LeftSlide:
                        angle = Vector2.Angle(upPos - downPos, Vector2.left);
                        if (downPos.x > upPos.x && angle < info.mouseGestures.angleLimit)
                            gesturesType = EMouseGesturesType.LeftSlide;
                        break;

                    case EMouseGesturesType.LeftUpSlide:
                        angle = Vector2.Angle(upPos - downPos, Quaternion.Euler(new Vector3(0, 0, -45)) * Vector3.left);
                        if (downPos.x > upPos.x && angle < info.mouseGestures.angleLimit)
                            gesturesType = EMouseGesturesType.LeftUpSlide;
                        break;

                    case EMouseGesturesType.LeftDownSlide:
                        angle = Vector2.Angle(upPos - downPos, Quaternion.Euler(new Vector3(0, 0, 45)) * Vector3.left);
                        if (downPos.x > upPos.x && angle < info.mouseGestures.angleLimit)
                            gesturesType = EMouseGesturesType.LeftDownSlide;
                        break;

                    case EMouseGesturesType.RightSlide:
                        angle = Vector2.Angle(upPos - downPos, Vector2.left);
                        if (downPos.x < upPos.x && angle < info.mouseGestures.angleLimit)
                            gesturesType = EMouseGesturesType.RightSlide;
                        break;

                    case EMouseGesturesType.RightUpSilde:
                        angle = Vector2.Angle(upPos - downPos, Quaternion.Euler(new Vector3(0, 0, 45)) * Vector3.right);
                        if (downPos.x < upPos.x && angle < info.mouseGestures.angleLimit)
                            gesturesType = EMouseGesturesType.RightUpSilde;
                        break;

                    case EMouseGesturesType.RightDownSlide:
                        angle = Vector2.Angle(upPos - downPos, Quaternion.Euler(new Vector3(0, 0, -45)) * Vector3.right);
                        if (downPos.x < upPos.x && angle < info.mouseGestures.angleLimit)
                            gesturesType = EMouseGesturesType.RightDownSlide;
                        break;

                    case EMouseGesturesType.UpSlide:
                        if (downPos.y < upPos.y && angle < info.mouseGestures.angleLimit)
                            gesturesType = EMouseGesturesType.UpSlide;
                        break;

                    case EMouseGesturesType.DownSlide:
                        if (downPos.y > upPos.y && angle < info.mouseGestures.angleLimit)
                            gesturesType = EMouseGesturesType.DownSlide;
                        break;
                }
            }
            return gesturesType;
        }

        public override void ResetData()
        {
            isShow = false;
            isMouseDown = false;
            isMouseUp = false;
            angle = 0;
            downPos = Vector2.zero;
            upPos = Vector2.zero;
        }
    }
}