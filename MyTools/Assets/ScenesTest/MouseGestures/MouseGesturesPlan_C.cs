using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MouseGesturesPlan_C : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Image point;
    public Transform plane;
    private List<Vector3> pointList;
    private List<Vector3> linePointList;
    private List<Transform> allPoint;
    public RectTransform pointsPanel;
    public RectTransform gravityCenter;
    public float templateLength;
    public float templateWidth;

    [Range(5, 100)]
    public float recognitionAngle = 60;

    [Space(5)]
    /// <summary>
    /// 最终的识别率 = （lengthRecognitionRate + widthRecognitionRate + wholeRecognitionRate）/ 3
    /// </summary>
    public float recognitionRate;
    //private PolygonCollider2D templateCollider;

    private List<Transform> fixedPointList;

    // Use this for initialization
    private void Start()
    {
        allPoint = new List<Transform>();
        lineRenderer = GetComponent<LineRenderer>();
        linePointList = new List<Vector3>();

        fixedPointList = new List<Transform>();
        fixedPointList.AddRange(transform.GetComponentsInChildren<Transform>());
        fixedPointList.Remove(transform);
    }

    // Update is called once per frame
    private void Update()
    {
        CreatePoint();
        //SetGravityCenterPosition();
        //RecognitionMouseGestures();
        Clear();
    }

    private void CreatePoint()
    {
        if (Input.GetMouseButton(0))
        {
            Image newPoint = GameObject.Instantiate(point);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                newPoint.rectTransform.localPosition = Input.mousePosition;
                newPoint.transform.SetAsLastSibling();
                newPoint.transform.SetParent(pointsPanel);
                allPoint.Add(newPoint.transform);
                //linePointList.Add(Camera.main.ScreenToWorldPoint(newPoint.transform.position));
                linePointList.Add(hit.point);
                lineRenderer.positionCount = linePointList.Count;
                lineRenderer.SetPositions(linePointList.ToArray());
            }
        }

        plane.transform.localScale = new Vector3(Screen.width / 10f, 1, Screen.height / 10f);
    }

    private void SetGravityCenterPosition()
    {
        Vector3 center = GetCenterOfGravity(pointList.ToArray());
        gravityCenter.position = center;
    }

    private async void RecognitionMouseGestures()
    {
        if (Input.GetMouseButtonUp(0))
        {
            MoveOffsetDistance();
            await new WaitForUpdate();
            //if (inSidePointList.Count > 0)
            //    Debug.Log("比例：" + CalculateRecongnitionRate() + "%");
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
            linePointList.Clear();
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
        if (points.Length <= 0) return Vector3.zero;
        float minX = points.GetMin(t => t.x).x;
        float maxX = points.GetMax(t => t.x).x;
        float minY = points.GetMin(t => t.y).y;
        float maxY = points.GetMax(t => t.y).y;
        float minZ = points.GetMin(t => t.z).z;
        float maxZ = points.GetMax(t => t.z).z;
        return new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
    }

    private void CalculateLengthWidthHigh(Vector2[] vectors, out float length, out float width)
    {
        float minX = vectors.GetMin(t => t.x).x;
        float maxX = vectors.GetMax(t => t.x).x;
        float minY = vectors.GetMin(t => t.y).y;
        float maxY = vectors.GetMax(t => t.y).y;
        length = maxX - minX;
        width = maxY - minY;
    }

    private void CalculateLengthWidthHigh(Vector3[] vectors, out float length, out float width, out float height)
    {
        float minX = vectors.GetMin(t => t.x).x;
        float maxX = vectors.GetMax(t => t.x).x;
        float minY = vectors.GetMin(t => t.y).y;
        float maxY = vectors.GetMax(t => t.y).y;
        float minZ = vectors.GetMin(t => t.z).z;
        float maxZ = vectors.GetMax(t => t.z).z;
        length = maxX - minX;
        width = maxY - minY;
        height = maxZ - minZ;
    }

    private float CalculateRecongnitionRate(ref float fixedPointCount, ref float fixedPointRate)
    {
        foreach (var item in fixedPointList)
        {
        }

        float rate = 0;
        if (fixedPointCount != 0)
            fixedPointRate = fixedPointCount / fixedPointList.Count * 100f;
        return rate;
    }
}