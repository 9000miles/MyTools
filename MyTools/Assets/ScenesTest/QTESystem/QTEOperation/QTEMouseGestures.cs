using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEMouseGestures : QTEOperationBase
{
    private float angle;
    private Vector2 downPos;
    private Vector2 upPos;
    private List<Vector2> posList;
    private static QTEMouseGestures singleton = null;
    public static QTEMouseGestures Singleton
    {
        get
        {
            if (singleton == null)
                singleton = new QTEMouseGestures();
            return singleton;
        }
    }

    public QTEMouseGestures() : base()
    {
    }

    /// <summary>
    /// 记录鼠标移动过程中的位置数据，然后，将这些位置数据归一化
    /// 再和标准数据进行对比，可使用平面投影，求位置是否在规定的区域内
    /// 计算根据识别率，判断该手势是否正确
    ///
    /// 怎么构建数据模型
    /// 怎么进行数据对比
    /// 还需要判断出入点的先后顺序
    ///
    /// </summary>
    /// <param name="info"></param>
    public override void ExcuteAndCheck(QTEInfo info)
    {
        base.ExcuteAndCheck(info);
        MouseGesturesType gesturesType = HandGestureRecognition(info);
        if (gesturesType == info.mouseGestures.gesturesType)
        {
            info.excuteTime = Time.time - info.startTime;
            info.result = QTEResult.Succed;
            info.errorType = QTEErrorType.None;
        }
    }

    private MouseGesturesType HandGestureRecognition(QTEInfo info)
    {
        MouseGesturesType gesturesType = MouseGesturesType.None;
        if (Input.GetMouseButtonDown((int)info.mouseGestures.mouseButton))
        {
            downPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp((int)info.mouseGestures.mouseButton))
        {
            upPos = Input.mousePosition;
            switch (info.mouseGestures.gesturesType)
            {
                case MouseGesturesType.LeftSlide:
                    angle = Vector2.Angle(upPos - downPos, Vector2.left);
                    if (downPos.x > upPos.x && angle < info.mouseGestures.angleLimit)
                        gesturesType = MouseGesturesType.LeftSlide;
                    break;

                case MouseGesturesType.LeftUpSlide:
                    angle = Vector2.Angle(upPos - downPos, Quaternion.Euler(new Vector3(0, 0, -45)) * Vector3.left);
                    if (downPos.x > upPos.x && angle < info.mouseGestures.angleLimit)
                        gesturesType = MouseGesturesType.LeftUpSlide;
                    break;

                case MouseGesturesType.LeftDownSlide:
                    angle = Vector2.Angle(upPos - downPos, Quaternion.Euler(new Vector3(0, 0, 45)) * Vector3.left);
                    if (downPos.x > upPos.x && angle < info.mouseGestures.angleLimit)
                        gesturesType = MouseGesturesType.LeftDownSlide;
                    break;

                case MouseGesturesType.RightSlide:
                    angle = Vector2.Angle(upPos - downPos, Vector2.left);
                    if (downPos.x < upPos.x && angle < info.mouseGestures.angleLimit)
                        gesturesType = MouseGesturesType.RightSlide;
                    break;

                case MouseGesturesType.RightUpSilde:
                    angle = Vector2.Angle(upPos - downPos, Quaternion.Euler(new Vector3(0, 0, 45)) * Vector3.right);
                    if (downPos.x < upPos.x && angle < info.mouseGestures.angleLimit)
                        gesturesType = MouseGesturesType.RightUpSilde;
                    break;

                case MouseGesturesType.RightDownSlide:
                    angle = Vector2.Angle(upPos - downPos, Quaternion.Euler(new Vector3(0, 0, -45)) * Vector3.right);
                    if (downPos.x < upPos.x && angle < info.mouseGestures.angleLimit)
                        gesturesType = MouseGesturesType.RightDownSlide;
                    break;

                case MouseGesturesType.UpSlide:
                    if (downPos.y < upPos.y && angle < info.mouseGestures.angleLimit)
                        gesturesType = MouseGesturesType.UpSlide;
                    break;

                case MouseGesturesType.DownSlide:
                    if (downPos.y > upPos.y && angle < info.mouseGestures.angleLimit)
                        gesturesType = MouseGesturesType.DownSlide;
                    break;

                case MouseGesturesType.CheckMark:
                    break;

                case MouseGesturesType.Capital_C:
                    break;

                case MouseGesturesType.Capital_Z:
                    break;

                case MouseGesturesType.Capital_U:
                    break;

                case MouseGesturesType.Capital_O:
                    break;

                case MouseGesturesType.Capital_S:
                    break;

                case MouseGesturesType.Capital_L:
                    break;
            }
        }
        return gesturesType;
    }

    public override void ResetData()
    {
        posList = null;
    }
}