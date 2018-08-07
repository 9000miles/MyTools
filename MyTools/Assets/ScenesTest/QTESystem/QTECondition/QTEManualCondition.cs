using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class QTEManualCondition : SingletonTemplate<QTEManualCondition>
{
    /// <summary>
    /// 存储该条件下的所有QTE，通过编辑器赋值，后面考虑读档赋值
    /// </summary>
    public List<QTEInfo> infoList = new List<QTEInfo>();
}