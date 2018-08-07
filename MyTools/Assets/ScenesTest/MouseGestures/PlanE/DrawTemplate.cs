﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Common;

//[RequireComponent(typeof(BezierGenerate))]
public class DrawTemplate : MonoBehaviour
{
    private bool isDraw;
    private Transform pointsPanel;
    public Transform crossPoint;
    public Transform crossPointPanel;
    private List<Transform> allPoint;
    private List<AngleLine> angleLines;
    private List<Vector2> crossingPoint;
    private LineRenderer lineRenderer;
    private BezierGenerate bezier;
    public float length = 400;
    public float angle = 10;
    public float angleLimit = 30;
    public Material material;
    private float width = 3;

    // Use this for initialization
    private void Start()
    {
        pointsPanel = transform.Find("PointsPanel");
        allPoint = new List<Transform>();
        angleLines = new List<AngleLine>();
        crossingPoint = new List<Vector2>();
        lineRenderer = GetComponent<LineRenderer>();
        bezier = GetComponent<BezierGenerate>();
        Init();
    }

    private void Init()
    {
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

            angleLines.Add(new AngleLine(allPoint[i].position, start));
            angleLines.Add(new AngleLine(allPoint[i].position, end));

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

                angleLine.SetPosition(0, start);
                angleLine.SetPosition(1, allPoint[i].position);
                angleLine.SetPosition(2, end);
            }
            else
            {
                angleLine.positionCount = 7;
                angleLine.SetPosition(3, allPoint[i].position);

                Vector3 dirNext = allPoint[i - 1].position - allPoint[i].position;
                Vector3 angleOffsetNext = Quaternion.Euler(new Vector3(0, 0, angle)) * dirNext;
                Vector3 startNext = allPoint[i].position + angleOffsetNext.normalized * length;
                angleOffsetNext = Quaternion.Euler(new Vector3(0, 0, -angle)) * dirNext;
                Vector3 endNext = allPoint[i].position + angleOffsetNext.normalized * length;

                angleLines.Add(new AngleLine(allPoint[i].position, startNext));
                angleLines.Add(new AngleLine(allPoint[i].position, endNext));

                angleLine.SetPosition(4, startNext);
                angleLine.SetPosition(5, allPoint[i].position);
                angleLine.SetPosition(6, endNext);
            }
        }
    }

    private void DrawAngleLineCoordinates()
    {
        if (isDraw == true) return;
        foreach (var item in angleLines)
        {
            foreach (var inItem in angleLines)
            {
                if (item != inItem)
                {
                    Vector2 crossPoint = CalculateIntersectionCoordinates(item.start, item.end, inItem.start, inItem.end);
                    if (!crossingPoint.Contains(crossPoint))
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
        isDraw = true;
    }

    /// <summary>
    /// 计算交点坐标
    /// </summary>
    private Vector2 CalculateIntersectionCoordinates(Vector2 P1, Vector2 P2, Vector2 P3, Vector2 P4)
    {
        Vector2 result = new Vector2();
        float left, right;
        left = (P2.y - P1.y) * (P4.x - P3.x) - (P4.y - P3.y) * (P2.x - P1.x);
        right = (P3.y - P1.y) * (P2.x - P1.x) * (P4.x - P3.x) + (P2.y - P1.y) * (P4.x - P3.x) * P1.x - (P4.y - P3.y) * (P2.x - P1.x) * P3.x;
        result.x = (int)(right / left);
        left = (P2.x - P1.x) * (P4.y - P3.y) - (P4.x - P3.x) * (P2.y - P1.y);
        right = (P3.x - P1.x) * (P2.y - P1.y) * (P4.y - P3.y) + P1.y * (P2.x - P1.x) * (P4.y - P3.y) - P3.y * (P4.x - P3.x) * (P2.y - P1.y);
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