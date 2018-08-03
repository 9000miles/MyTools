using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEMouseGestures : QTEOperationBase
{
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
    public override void ExcuteCheck(QTEInfo info)
    {
        base.ExcuteCheck(info);
        if (Event.current.type == EventType.MouseDown)
        {
            //记录鼠标按下的位置 　　
            posList.Add(Event.current.mousePosition);
        }
        if (Event.current.type == EventType.MouseDrag)
        {
            //记录鼠标拖动的位置 　　
            //second = Event.current.mousePosition;

            //if (second.x < first.x)
            //{
            //    //拖动的位置的x坐标比按下的位置的x坐标小时,响应向左事件 　　
            //}
            //if (second.x > first.x)
            //{
            //    //拖动的位置的x坐标比按下的位置的x坐标大时,响应向右事件
            //}
        }
    }

    public override void ResetData()
    {
        posList = null;
    }
}