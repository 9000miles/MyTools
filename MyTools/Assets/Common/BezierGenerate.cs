using UnityEngine;
using System.Collections;
using Common;

/// <summary>
/// 生成贝塞尔曲线
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class BezierGenerate : SingletonBehaviour<BezierGenerate>
{
    #region 变量

    [Tooltip("曲线上节点的数量")]
    public int nodeCount = 20;
    [Tooltip("起点")]
    public Transform startTF;
    [Tooltip("控制点1")]
    public Transform startContrllor;
    [Tooltip("控制点2")]
    public Transform endContrllor;
    [Tooltip("终点")]
    public Transform endTF;
    [Tooltip("中间点数组")]
    public Vector3[] nodePoints;
    [Tooltip("线段组件")]
    public LineRenderer line;

    #endregion 变量

    private void Start()
    {
        nodePoints = new Vector3[nodeCount];
        line = GetComponent<LineRenderer>();
        line.positionCount = nodeCount;
        startTF = transform.Find("Start");
        startContrllor = transform.Find("StartContrllor");
        endContrllor = transform.Find("EndContrllor");
        endTF = transform.Find("End");
    }

    //公式
    public static Vector3 CreatePoint(Vector3 beginPos, Vector3 controlPos01, Vector3 controlPos02, Vector3 endPos, float t)
    {
        return beginPos * Mathf.Pow(1 - t, 3) + 3 * controlPos01 * t * Mathf.Pow(1 - t, 2) + 3 * controlPos02 * Mathf.Pow(t, 2) * (1 - t) + endPos * Mathf.Pow(t, 3);
    }

    //生成曲线坐标
    public void GenerateCurve(Vector3 startPos, Vector3 startContrllor, Vector3 endContrllor, Vector3 endPos)
    {
        float distance = Vector3.Distance(startPos, endPos);
        int nodeCount = (int)distance > 1 ? (int)distance : 2;// 2;
        int startIndex = line.positionCount;
        line.positionCount = startIndex + nodeCount;
        //每段所占比例
        float ratio = 1f / (nodeCount - 1);
        float t = 0;
        for (int i = 0; i < nodeCount; i++)
        {
            Vector3 pos = CreatePoint(startPos, startContrllor, endContrllor, endPos, t);
            line.SetPosition(startIndex + i, pos);
            t += ratio;
        }
    }

    /// <summary>
    /// 根据数组位置绘制曲线
    /// </summary>
    /// <param name="nodePoints">点数组</param>
    public void DrawCurve(Vector3[] nodePoints)
    {
        for (int i = 0; i < nodePoints.Length - 1; i++)
        {
            Vector3 startContrllor = SetBezierStartControlTF(nodePoints[i], nodePoints[i + 1]);
            Vector3 endContrllor = SetBezierEndControlTF(nodePoints[i], nodePoints[i + 1]);
            GenerateCurve(nodePoints[i], startContrllor, endContrllor, nodePoints[i + 1]);
        }
        line.SetPositions(nodePoints);
    }

    /// <summary>
    /// 设置贝赛尔曲线起点控制点
    /// </summary>
    private Vector3 SetBezierStartControlTF(Vector3 first, Vector3 scend)
    {
        float distance = Vector3.Distance(first, scend);
        return new Vector3(first.x + distance / 2f, first.y + distance / 1f, first.z);
    }

    /// <summary>
    /// 设置贝赛尔曲线终点控制点
    /// </summary>
    private Vector3 SetBezierEndControlTF(Vector3 first, Vector3 scend)
    {
        float distance = Vector3.Distance(first, scend);
        return new Vector3(first.x, first.y + distance, first.z);
    }
}