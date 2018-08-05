using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
using System.Threading.Tasks;

/// <summary>
/// 在模板路径放置识别点，
/// 每个识别点获取识别范围内的数据点
/// 计算总的数据点在识别点范围内比例
/// </summary>
public class MouseGesturesPlan_B : MonoBehaviour
{
    public Image point;
    public List<Vector3> pointList;
    //public List<Transform> inSidePointTF;
    //public List<Vector3> inSidePoint;
    private List<Transform> allPoint;
    private List<Transform> inSidePointList;
    public RectTransform pointsPanel;
    public RectTransform gravityCenter;
    public float templateLength;
    public float templateWidth;

    [Range(5, 100)]
    public float recognitionRange = 60;

    [Space(5)]
    /// <summary>
    /// 长度方向适配识别率
    /// </summary>
    public float lengthRecognitionRate;
    public float lengthFixedPointCount;
    public float lengthFixedPointRate;

    [Space(5)]
    /// <summary>
    /// 宽度方向适配识别率
    /// </summary>
    public float widthRecognitionRate;
    public float widthFixedPointCount;
    public float widthFixedPointRate;

    [Space(5)]
    /// <summary>
    /// 长宽方向整体识别率
    /// </summary>
    public float wholeRecognitionRate;
    public float wholeFixedPointCount;
    public float wholeFixedPointRate;

    [Space(5)]
    /// <summary>
    /// 最终的识别率 = （lengthRecognitionRate + widthRecognitionRate + wholeRecognitionRate）/ 3
    /// </summary>
    public float recognitionRate;
    private PolygonCollider2D templateCollider;

    private List<Transform> fixedPointList;

    // Use this for initialization
    private void Start()
    {
        allPoint = new List<Transform>();
        inSidePointList = new List<Transform>();
        templateCollider = GetComponent<PolygonCollider2D>();
        GetTemplateWidthAndHeight();

        fixedPointList = new List<Transform>();
        fixedPointList.AddRange(transform.GetComponentsInChildren<Transform>());
        fixedPointList.Remove(transform);
    }

    // Update is called once per frame
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
        foreach (var item in allPoint)
        {
            pointList.Add(item.position);
        }
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
            await FitTemplateProportions();
            Debug.Log("所有的：" + allPoint.Count + "   在内部：" + inSidePointList.Count);
            inSidePointList.TrimExcess();
            //if (inSidePointList.Count > 0)
            //    Debug.Log("比例：" + CalculateRecongnitionRate() + "%");
        }
    }

    private void Clear()
    {
        if (Input.GetMouseButtonDown(1))
        {
            pointList.Clear();
            lengthFixedPointCount = 0;
            widthFixedPointCount = 0;
            wholeFixedPointCount = 0;
            for (int i = 0; i < allPoint.Count; i++)
            {
                Destroy(allPoint[i].gameObject);
            }

            allPoint.Clear();
            inSidePointList.Clear();
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

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (!inSidePoint.Contains(collision.transform))
    //    {
    //        inSidePoint.Add(collision.transform);
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    inSidePoint.Remove(collision.transform);
    //}

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

    private void GetTemplateWidthAndHeight()
    {
        Vector2[] points = templateCollider.points;
        float minX = points.GetMin(t => t.x).x;
        float maxX = points.GetMax(t => t.x).x;
        float minY = points.GetMin(t => t.y).y;
        float maxY = points.GetMax(t => t.y).y;
        templateLength = maxX - minX;
        templateWidth = maxY - minY;
        CalculateLengthWidthHigh(templateCollider.points, out templateLength, out templateWidth);
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

    private async Task FitTemplateProportions()
    {
        float length = 0;
        float width = 0;
        float height = 0;
        CalculateLengthWidthHigh(allPoint.ToArray().Select(t => t.position), out length, out width, out height);

        Vector3 originalScale = pointsPanel.localScale;

        float lengthRatio = templateLength / length;
        pointsPanel.localScale = new Vector3(originalScale.x * lengthRatio, originalScale.y, originalScale.z);
        await new WaitForFixedUpdate();
        lengthRecognitionRate = CalculateRecongnitionRate(ref lengthFixedPointCount, ref lengthFixedPointRate);

        float widthRatio = templateWidth / width;
        pointsPanel.localScale = new Vector3(originalScale.x, originalScale.y * widthRatio, originalScale.z);
        await new WaitForFixedUpdate();
        widthRecognitionRate = CalculateRecongnitionRate(ref widthFixedPointCount, ref widthFixedPointRate);

        pointsPanel.localScale = new Vector3(originalScale.x * lengthRatio, originalScale.y * widthRatio, originalScale.z);
        await new WaitForFixedUpdate();
        wholeRecognitionRate = CalculateRecongnitionRate(ref wholeFixedPointCount, ref wholeFixedPointRate);

        recognitionRate = (lengthRecognitionRate + widthRecognitionRate + wholeRecognitionRate) / 3f;
    }

    private float CalculateRecongnitionRate(ref float fixedPointCount, ref float fixedPointRate)
    {
        foreach (var item in fixedPointList)
        {
            Transform[] inSidePoint = item.GetAroundObject(recognitionRange, 360, "Point");
            if (inSidePoint.Length > 0) fixedPointCount++;
            foreach (var inSideItem in inSidePoint)
            {
                if (!inSidePointList.Contains(inSideItem))
                    inSidePointList.Add(inSideItem);
            }
        }

        float rate = 0;
        if (fixedPointCount != 0)
            fixedPointRate = fixedPointCount / fixedPointList.Count * 100f;
        if (inSidePointList.Count != 0)
            rate = (float)inSidePointList.Count / (float)allPoint.Count * 100f;
        return rate;
    }
}