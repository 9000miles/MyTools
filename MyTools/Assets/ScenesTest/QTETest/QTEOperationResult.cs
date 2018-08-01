//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Common;

//public abstract class QTEOperationResult : MonoSingleton<QTEOperationResult>
//{
//    public int clickCount;
//    public float time;
//    public List<KeyCode> keyList;
//    public Vector2 clickPosition;
//    public MouseGestures mouseGestures;

//    public abstract void GetResult();

//    public void EnptyResult()
//    {
//        clickCount = 0;
//        time = 0;
//        keyList.Clear();
//        clickPosition = Vector2.zero;
//        mouseGestures = null;
//    }
//}

//public class QuickClickResult : QTEOperationResult
//{
//    public override void GetResult()
//    {
//        throw new System.NotImplementedException();
//    }
//}

//public class PreciseClickResult : QTEOperationResult
//{
//    public override void GetResult()
//    {
//        throw new System.NotImplementedException();
//    }
//}

///// <summary>
///// 鼠标手势
///// </summary>
//public class MouseGestures : QTEOperationResult
//{
//    public override void GetResult()
//    {
//        throw new System.NotImplementedException();
//    }
//}

//public class KeyCombinationResult : QTEOperationResult
//{
//    public override void GetResult()
//    {
//        keyList = QTEManager.Instance.keyList;
//    }
//}