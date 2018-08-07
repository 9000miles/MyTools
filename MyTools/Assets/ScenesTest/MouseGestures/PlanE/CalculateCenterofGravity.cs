using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class CalculateCenterofGravity : MonoBehaviour
{
    public Transform pointsPanel;
    private List<Transform> list;

    private void Start()
    {
        list = new List<Transform>();
    }

    private void Update()
    {
        list.Clear();
        list.AddRange(pointsPanel.GetComponentsInChildren<Transform>());
        list.Remove(pointsPanel);
        Vector3[] points = list.ToArray().Select(t => t.position);
        transform.position = GetCenterOfGravity(points);
    }

    public Vector3 GetCenterOfGravity(Vector3[] points)
    {
        if (points.Length <= 0) return Vector3.zero;
        float minX = points.GetMin(t => t.x).x;
        float maxX = points.GetMax(t => t.x).x;
        float minY = points.GetMin(t => t.y).y;
        float maxY = points.GetMax(t => t.y).y;
        float minZ = points.GetMin(t => t.z).z;
        float maxZ = points.GetMax(t => t.z).z;
        return new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
    }
}