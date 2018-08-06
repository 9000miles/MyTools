using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Common;

//[RequireComponent(typeof(BezierGenerate))]
public class DrawTemplate : MonoBehaviour
{
    private List<Transform> allPoint;
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
        allPoint = new List<Transform>();
        lineRenderer = GetComponent<LineRenderer>();
        bezier = GetComponent<BezierGenerate>();
        Init();
    }

    private void Init()
    {
        allPoint.AddRange(GetComponentsInChildren<Transform>());
        allPoint.Remove(transform);
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
        lineRenderer.positionCount = allPoint.Count;
        lineRenderer.SetPositions(allPoint.ToArray().Select(t => t.position));
        for (int i = 0; i < allPoint.Count - 1; i++)
        {
            LineRenderer angleLine = allPoint[i].GetComponent<LineRenderer>();
            angleLine.positionCount = 3;
            Vector3 dir = allPoint[i + 1].position - allPoint[i].position;
            Vector3 angleOffset = Quaternion.Euler(new Vector3(0, 0, angle)) * dir;
            Vector3 start = allPoint[i].position + angleOffset.normalized * length;
            angleOffset = Quaternion.Euler(new Vector3(0, 0, -angle)) * dir;
            Vector3 end = allPoint[i].position + angleOffset.normalized * length;
            angleLine.SetPosition(0, start);
            angleLine.SetPosition(1, allPoint[i].position);
            angleLine.SetPosition(2, end);
        }
    }
}