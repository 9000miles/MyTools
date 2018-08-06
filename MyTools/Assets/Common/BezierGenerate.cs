using UnityEngine;
using System.Collections;
using Common;

/// <summary>
/// 生成贝塞尔曲线
/// </summary>
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
    public void GenerateCurve()
    {
        //每段所占比例
        float ratio = 1f / (nodeCount - 1);
        float t = 0;
        for (int i = 0; i < nodeCount; i++)
        {
            nodePoints[i] = CreatePoint(
                startTF.position,
                startContrllor.position,
                endContrllor.position,
                endTF.position,
                t);
            t += ratio;
        }
    }

    //3. 绘制曲线
    public void DrawCurve()
    {
        line.SetPositions(nodePoints);
    }

    /// <summary>
    /// 根据数组位置绘制曲线
    /// </summary>
    /// <param name="nodePoints">点数组</param>
    public void DrawCurve(Vector3[] nodePoints)
    {
        GenerateCurve();
        line.SetPositions(nodePoints);
    }
}