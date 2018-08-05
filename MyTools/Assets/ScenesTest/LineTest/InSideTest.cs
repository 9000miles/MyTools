using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

/// <summary>
/// 求出所有点最高距离、最宽距离
/// 按照最高和最宽分别做一次比例适配，然后计算命中率
/// 取其中命中率最大值，作为结果
///
/// 求质心点与所有点父物体的偏移距离
/// 然后将所有的子物体移动一个偏移距离，这样父物体的位置便是质心点
/// 将父物体移动到模板上，进行比例适配，计算结果
///
/// 需要增加对标准模板长和宽的相似程度
/// </summary>
public class InSideTest : MonoBehaviour
{
    public Image point;
    public List<Vector3> pointList;
    //public List<Transform> inSidePointTF;
    //public List<Vector3> inSidePoint;
    private List<Transform> allPoint;
    private List<Transform> inSidePoint;
    public RectTransform pointsPanel;
    public RectTransform gravityCenter;
    public float templateLength;
    public float templateWidth;
    /// <summary>
    /// 长度方向适配识别率
    /// </summary>
    public float lengthRecognitionRate;
    /// <summary>
    /// 宽度方向适配识别率
    /// </summary>
    public float widthRecognitionRate;
    /// <summary>
    /// 长宽方向整体识别率
    /// </summary>
    public float wholeRecognitionRate;
    /// <summary>
    /// 最终的识别率 = （lengthRecognitionRate + widthRecognitionRate + wholeRecognitionRate）/ 3
    /// </summary>
    public float recognitionRate;
    private PolygonCollider2D templateCollider;

    // Use this for initialization
    private void Start()
    {
        allPoint = new List<Transform>();
        inSidePoint = new List<Transform>();
        templateCollider = GetComponent<PolygonCollider2D>();
        GetTemplateWidthAndHeight();
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
            FitTemplateProportions();
            Debug.Log("所有的：" + allPoint.Count + "   在内部：" + inSidePoint.Count);
            inSidePoint.TrimExcess();
            if (inSidePoint.Count > 0)
                Debug.Log("比例：" + CalculateRecongnitionRate() + "%");
        }
    }

    private void Clear()
    {
        pointList.Clear();
        if (Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < allPoint.Count; i++)
            {
                Destroy(allPoint[i].gameObject);
            }
            allPoint.Clear();
            inSidePoint.Clear();
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!inSidePoint.Contains(collision.transform))
        {
            inSidePoint.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inSidePoint.Remove(collision.transform);
    }

    public Vector3 GetCenterOfGravity(Vector3[] points)
    {
        if (points.Length <= 0) return Vector3.zero;

        float minX = points[0].x;
        float maxX = points[0].x;
        float minY = points[0].y;
        float maxY = points[0].y;
        float minZ = points[0].z;
        float maxZ = points[0].z;
        minX = points.GetMin(t => t.x).x;
        maxX = points.GetMax(t => t.x).x;
        minY = points.GetMin(t => t.y).y;
        maxY = points.GetMax(t => t.y).y;
        minZ = points.GetMin(t => t.z).z;
        maxZ = points.GetMax(t => t.z).z;
        //for (int i = 0; i < vect3s.Length; i++)
        //{
        //    if (minx > vect3s[i].x)
        //    {
        //        minx = vect3s[i].x;
        //    }
        //    if (maxx < vect3s[i].x)
        //    {
        //        maxx = vect3s[i].x;
        //    }
        //    if (miny > vect3s[i].y)
        //    {
        //        miny = vect3s[i].y;
        //    }
        //    if (maxy < vect3s[i].y)
        //    {
        //        maxy = vect3s[i].y;
        //    }
        //    if (minz > vect3s[i].z)
        //    {
        //        minz = vect3s[i].z;
        //    }
        //    if (maxz < vect3s[i].z)
        //    {
        //        maxz = vect3s[i].z;
        //    }
        //}
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

    private async void FitTemplateProportions()
    {
        float length = 0;
        float width = 0;
        float height = 0;
        CalculateLengthWidthHigh(allPoint.ToArray().Select(t => t.position), out length, out width, out height);

        Vector3 originalScale = pointsPanel.localScale;

        float lengthRatio = templateLength / length;
        pointsPanel.localScale = new Vector3(originalScale.x * lengthRatio, originalScale.y, originalScale.z);
        await new WaitForFixedUpdate();
        lengthRecognitionRate = CalculateRecongnitionRate();

        float widthRatio = templateWidth / width;
        pointsPanel.localScale = new Vector3(originalScale.x, originalScale.y * widthRatio, originalScale.z);
        await new WaitForFixedUpdate();
        widthRecognitionRate = CalculateRecongnitionRate();

        pointsPanel.localScale = new Vector3(originalScale.x * lengthRatio, originalScale.y * widthRatio, originalScale.z);
        await new WaitForFixedUpdate();
        wholeRecognitionRate = CalculateRecongnitionRate();

        recognitionRate = (lengthRecognitionRate + widthRecognitionRate + wholeRecognitionRate) / 3f;
    }

    private float CalculateRecongnitionRate()
    {
        return (float)inSidePoint.Count / (float)allPoint.Count * 100f;
    }
}