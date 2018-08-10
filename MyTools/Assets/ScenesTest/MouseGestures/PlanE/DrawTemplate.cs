using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Common;

//[RequireComponent(typeof(BezierGenerate))]
public class DrawTemplate : SingletonBehaviour<DrawTemplate>
{
    private bool isDrawed;
    private Transform pointsPanel;
    public Transform crossPoint;
    public Transform crossPointPanel;
    public List<Transform> allPoint;
    private List<AngleLine> angleLines;
    private List<Vector2> crossingPoint;
    public List<Vector2> trendList;
    private LineRenderer lineRenderer;
    private BezierGenerate bezier;
    public Dictionary<int, AngleLine[]> angleLineDic;
    public float length = 1000;
    public float angle = 15;
    public float trendAngleLimit = 50;
    public float betweenMinDistance = 20;
    public Material material;
    private float width = 3;
    private Vector2 highest;
    private Vector2 lowest;
    public float templateHight;
    private int dicIndex;
    public int outOfAngleCount = 3;

    // Use this for initialization
    private void Start()
    {
        pointsPanel = transform.Find("PointsPanel");
        allPoint = new List<Transform>();
        angleLines = new List<AngleLine>();
        crossingPoint = new List<Vector2>();
        angleLineDic = new Dictionary<int, AngleLine[]>();
        trendList = new List<Vector2>();
        lineRenderer = GetComponent<LineRenderer>();
        bezier = GetComponent<BezierGenerate>();
        Init();
    }

    public new void Init()
    {
        base.Init();
        allPoint.AddRange(pointsPanel.GetComponentsInChildren<Transform>());
        allPoint.Remove(pointsPanel);
        foreach (var item in allPoint)
        {
            LineRenderer angleLine = item.gameObject.AddComponent<LineRenderer>();
            angleLine.material = material;
            angleLine.startWidth = width;
            angleLine.endWidth = width;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        DrawAngleLine();
        DrawAngleLineCoordinates();
    }

    private void DrawAngleLine()
    {
        angleLines.Clear();
        angleLineDic.Clear();
        trendList.Clear();
        dicIndex = 0;
        lineRenderer.positionCount = allPoint.Count;
        lineRenderer.SetPositions(allPoint.ToArray().Select(t => t.position));

        for (int i = 0; i < pointsPanel.childCount; i++)
        {
            Transform childTF = pointsPanel.GetChild(i);
            if (!allPoint.Contains(childTF))
            {
                allPoint.Add(childTF);
            }
        }

        //顺向绘制
        for (int i = 0; i < allPoint.Count - 1; i++)
        {
            LineRenderer angleLine = allPoint[i].GetComponent<LineRenderer>();
            angleLine.positionCount = 3;

            Vector3 dir = allPoint[i + 1].position - allPoint[i].position;
            Vector3 angleOffset = Quaternion.Euler(new Vector3(0, 0, angle)) * dir;
            Vector3 start = allPoint[i].position + angleOffset.normalized * length;
            angleOffset = Quaternion.Euler(new Vector3(0, 0, -angle)) * dir;
            Vector3 end = allPoint[i].position + angleOffset.normalized * length;

            //if (trendList.Count < allPoint.Count && !trendList.Contains(dir))
            trendList.Add(dir);

            angleLines.Add(new AngleLine(allPoint[i].position, start));
            angleLines.Add(new AngleLine(allPoint[i].position, end));
            angleLineDic.Add(dicIndex, new AngleLine[] { new AngleLine(allPoint[i].position, start), new AngleLine(allPoint[i].position, end) });
            dicIndex++;

            angleLine.SetPosition(0, start);
            angleLine.SetPosition(1, allPoint[i].position);
            angleLine.SetPosition(2, end);
        }

        //反向绘制
        for (int i = allPoint.Count - 1; i > 0; i--)
        {
            LineRenderer angleLine = allPoint[i].GetComponent<LineRenderer>();
            if (i == allPoint.Count - 1)
            {
                angleLine.positionCount = 3;

                Vector3 dir = allPoint[i - 1].position - allPoint[i].position;
                Vector3 angleOffset = Quaternion.Euler(new Vector3(0, 0, angle)) * dir;
                Vector3 start = allPoint[i].position + angleOffset.normalized * length;
                angleOffset = Quaternion.Euler(new Vector3(0, 0, -angle)) * dir;
                Vector3 end = allPoint[i].position + angleOffset.normalized * length;

                angleLines.Add(new AngleLine(allPoint[i].position, start));
                angleLines.Add(new AngleLine(allPoint[i].position, end));
                angleLineDic.Add(dicIndex, new AngleLine[] { new AngleLine(allPoint[i].position, start), new AngleLine(allPoint[i].position, end) });
                dicIndex++;

                angleLine.SetPosition(0, start);
                angleLine.SetPosition(1, allPoint[i].position);
                angleLine.SetPosition(2, end);
            }
            else
            {
                angleLine.positionCount = 7;
                angleLine.SetPosition(3, allPoint[i].position);

                Vector3 dir = allPoint[i - 1].position - allPoint[i].position;
                Vector3 angleOffset = Quaternion.Euler(new Vector3(0, 0, angle)) * dir;
                Vector3 start = allPoint[i].position + angleOffset.normalized * length;
                angleOffset = Quaternion.Euler(new Vector3(0, 0, -angle)) * dir;
                Vector3 end = allPoint[i].position + angleOffset.normalized * length;

                angleLines.Add(new AngleLine(allPoint[i].position, start));
                angleLines.Add(new AngleLine(allPoint[i].position, end));
                angleLineDic.Add(dicIndex, new AngleLine[] { new AngleLine(allPoint[i].position, start), new AngleLine(allPoint[i].position, end) });
                dicIndex++;

                angleLine.SetPosition(4, start);
                angleLine.SetPosition(5, allPoint[i].position);
                angleLine.SetPosition(6, end);
            }
        }
    }

    private void DrawAngleLineCoordinates()
    {
        crossingPoint.Clear();
        for (int i = 0; i < crossPointPanel.childCount; i++)
        {
            if (crossPointPanel.GetChild(i).name != "CrossPoint")
                Destroy(crossPointPanel.GetChild(i).gameObject);
        }
        foreach (var item in angleLines)
        {
            foreach (var inItem in angleLines)
            {
                if (item != inItem)
                {
                    Vector2 crossPoint = CalculateIntersectionCoordinates(item.start, item.end, inItem.start, inItem.end);
                    if (!crossingPoint.Contains(crossPoint) && WhetherPointIsOnTheLineSegment(item.start, item.end, inItem.start, inItem.end, crossPoint))
                    {
                        crossingPoint.Add(crossPoint);
                    }
                }
            }
        }
        foreach (var item in crossingPoint)
        {
            Instantiate(crossPoint, item, Quaternion.identity, crossPointPanel);
        }
        Vector2 highest = crossingPoint.ToArray().GetMax(t => t.y);
        Vector2 lowest = crossingPoint.ToArray().GetMin(t => t.y);
        templateHight = Mathf.Abs(highest.y - lowest.y);
        isDrawed = true;
    }

    /// <summary>
    /// 判断一个点是否在线段内
    /// </summary>
    /// <param name="L1Start"></param>
    /// <param name="L1End"></param>
    /// <param name="L2Start"></param>
    /// <param name="L2End"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private bool WhetherPointIsOnTheLineSegment(Vector2 L1Start, Vector2 L1End, Vector2 L2Start, Vector2 L2End, Vector2 point)
    {
        bool isInsideL1 =
            (point.x >= L1Start.x && point.x <= L1End.x && point.y >= L1Start.y && point.y <= L1End.y) ||
            (point.x >= L1End.x && point.x <= L1Start.x && point.y >= L1End.y && point.y <= L1Start.y) ||

             (point.x >= L1Start.x && point.x <= L1End.x && point.y >= L1End.y && point.y <= L1Start.y) ||
              (point.x >= L1End.x && point.x <= L1Start.x && point.y >= L1Start.y && point.y <= L1End.y);

        bool isInsideL2 =
            (point.x >= L2Start.x && point.x <= L2End.x && point.y >= L2Start.y && point.y <= L2End.y) ||
            (point.x >= L2End.x && point.x <= L2Start.x && point.y >= L2End.y && point.y <= L2Start.y) ||

            (point.x >= L2Start.x && point.x <= L2End.x && point.y >= L2End.y && point.y <= L2Start.y) ||
            (point.x >= L2End.x && point.x <= L2Start.x && point.y >= L2Start.y && point.y <= L2End.y);

        #region 有误差存在

        //bool isInsideL1 = Vector2.Distance(point, L1Start) + Vector2.Distance(point, L1End) - Vector2.Distance(L1Start, L1End) < 0.01f;
        //bool isInsideL2 = Vector2.Distance(point, L2Start) + Vector2.Distance(point, L2End) - Vector2.Distance(L2Start, L2End) < 0.01f;

        //bool isInsideL1 = Vector2.SqrMagnitude(point - L1Start) + Vector2.SqrMagnitude(point - L1End) == Vector2.SqrMagnitude(L1Start - L1End);
        //bool isInsideL2 = Vector2.SqrMagnitude(point - L2Start) + Vector2.SqrMagnitude(point - L2End) == Vector2.SqrMagnitude(L2Start - L2End);

        #endregion 有误差存在

        return isInsideL1 && isInsideL2;
    }

    /// <summary>
    /// 计算交点坐标
    /// </summary>
    /// <param name="line1Start"></param>
    /// <param name="line1End"></param>
    /// <param name="line2Start"></param>
    /// <param name="line2End"></param>
    /// <returns></returns>
    private Vector2 CalculateIntersectionCoordinates(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End)
    {
        Vector2 result = new Vector2();
        float left, right;
        left = (line1End.y - line1Start.y) * (line2End.x - line2Start.x) - (line2End.y - line2Start.y) * (line1End.x - line1Start.x);
        right = (line2Start.y - line1Start.y) * (line1End.x - line1Start.x) * (line2End.x - line2Start.x) +
            (line1End.y - line1Start.y) * (line2End.x - line2Start.x) * line1Start.x - (line2End.y - line2Start.y) * (line1End.x - line1Start.x) * line2Start.x;
        result.x = (int)(right / left);

        left = (line1End.x - line1Start.x) * (line2End.y - line2Start.y) - (line2End.x - line2Start.x) * (line1End.y - line1Start.y);
        right = (line2Start.x - line1Start.x) * (line1End.y - line1Start.y) * (line2End.y - line2Start.y) +
            line1Start.y * (line1End.x - line1Start.x) * (line2End.y - line2Start.y) - line2Start.y * (line2End.x - line2Start.x) * (line1End.y - line1Start.y);
        result.y = (int)(right / left);
        return result;
    }
}

/// <summary>
/// 角度线条
/// </summary>
public class AngleLine
{
    public Vector2 start;
    public Vector2 end;

    public AngleLine(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }
}