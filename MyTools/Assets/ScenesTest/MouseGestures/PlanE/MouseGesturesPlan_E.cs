using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MouseGesturesPlan_E : MonoBehaviour
{
    public Image point;
    private List<Transform> allPoint;
    private List<Transform> inAreaList;
    public RectTransform pointsPanel;
    public RectTransform gravityCenter;
    public Transform gravityCenterTemplate;
    [Space(5)]
    [Range(0, 100)]
    /// <summary>
    /// 最终的识别率 = （lengthRecognitionRate + widthRecognitionRate + wholeRecognitionRate）/ 3
    /// </summary>
    public float recognitionRate;

    private void Start()
    {
        allPoint = new List<Transform>();
        inAreaList = new List<Transform>();
    }

    private void Update()
    {
        CreatePoint();
        SetGravityCenterPosition();
        RecognitionMouseGestures();
        Clear();
    }

    private void CreatePoint()
    {
        if (Input.GetMouseButton(0))
        {
            Image newPoint = GameObject.Instantiate(point);
            newPoint.rectTransform.localPosition = Input.mousePosition;
            newPoint.transform.SetAsLastSibling();
            newPoint.transform.SetParent(pointsPanel);
            allPoint.Add(newPoint.transform);
        }
    }

    private void SetGravityCenterPosition()
    {
        Vector3 center = GetCenterOfGravity(allPoint.ToArray().Select(t => t.position));
        gravityCenter.position = center;
        pointsPanel.position = gravityCenterTemplate.position;
    }

    private async void RecognitionMouseGestures()
    {
        if (Input.GetMouseButtonUp(0))
        {
            MoveOffsetDistance();
            CalculateRecongnitionRate();
        }
    }

    private void Clear()
    {
        if (Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < allPoint.Count; i++)
            {
                Destroy(allPoint[i].gameObject);
            }

            allPoint.Clear();
            inAreaList.Clear();
            pointsPanel.localScale = Vector3.one;
        }
    }

    private void MoveOffsetDistance()
    {
        Vector3 offset = pointsPanel.position - gravityCenter.position;
        foreach (var item in allPoint)
        {
            item.position += offset;
        }
    }

    public Vector3 GetCenterOfGravity(Vector3[] points)
    {
        if (points == null || points.Length <= 0) return Vector3.zero;
        float minX = points.GetMin(t => t.x).x;
        float maxX = points.GetMax(t => t.x).x;
        float minY = points.GetMin(t => t.y).y;
        float maxY = points.GetMax(t => t.y).y;
        float minZ = points.GetMin(t => t.z).z;
        float maxZ = points.GetMax(t => t.z).z;
        return new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
    }

    private float CalculateRecongnitionRate()
    {
        float rate = 0;

        return rate;
    }
}