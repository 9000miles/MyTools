using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 求出所有点最高距离、最宽距离
/// 按照最高和最宽分别做一次比例适配，然后计算命中率
/// 取其中命中率最大值，作为结果
///
/// 求质心点与所有点父物体的偏移距离
/// 然后将所有的子物体移动一个偏移距离，这样父物体的位置便是质心点
/// 将父物体移动到模板上，进行比例适配，计算结果
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

    // Use this for initialization
    private void Start()
    {
        allPoint = new List<Transform>();
        inSidePoint = new List<Transform>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Image newPoint = GameObject.Instantiate(point);
            newPoint.rectTransform.localPosition = Input.mousePosition;
            newPoint.transform.SetAsLastSibling();
            newPoint.transform.SetParent(pointsPanel);
            allPoint.Add(newPoint.transform);
        }
        var enumator = allPoint.GetEnumerator();
        while (enumator.MoveNext())
        {
            pointList.Add(enumator.Current.position);
        }
        Vector3 center = GetCenterOfGravity(pointList.ToArray());
        gravityCenter.position = center;
        if (Input.GetMouseButtonUp(0))
            MoveOffsetDistance();
        Debug.Log("所有的：" + allPoint.Count + "   在内部：" + inSidePoint.Count);
        inSidePoint.TrimExcess();
        if (inSidePoint.Count > 0)
            Debug.Log("比例：" + (float)inSidePoint.Count / (float)allPoint.Count * 100f + "%");
        pointList.Clear();

        if (Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < allPoint.Count; i++)
            {
                Destroy(allPoint[i].gameObject);
            }
            allPoint.Clear();
            inSidePoint.Clear();
            //Destroy(gravityCenter.gameObject);
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

    public Vector2 GetCenterOfGravity(Vector2[] points)
    {
        Vector2 gravityCenter = new Vector2(0, 0);

        float minx = points[0].x;
        float maxx = points[0].x;
        float miny = points[0].y;
        float maxy = points[0].y;

        for (int i = 0; i < points.Length; i++)
        {
            if (minx > points[i].x)
            {
                minx = points[i].x;
            }
            if (maxx < points[i].x)
            {
                maxx = points[i].x;
            }
            if (miny > points[i].y)
            {
                miny = points[i].y;
            }
            if (maxy < points[i].y)
            {
                maxy = points[i].y;
            }
        }
        return gravityCenter = new Vector2((minx + maxx) / 2, (miny + maxy) / 2);
    }

    public Vector3 GetCenterOfGravity(Vector3[] vect3s)
    {
        if (vect3s.Length <= 0) return Vector3.zero;
        Vector3 gravityCenter = new Vector3(0, 0, 0);

        float minx = vect3s[0].x;
        float maxx = vect3s[0].x;
        float miny = vect3s[0].y;
        float maxy = vect3s[0].y;
        float minz = vect3s[0].z;
        float maxz = vect3s[0].z;

        for (int i = 0; i < vect3s.Length; i++)
        {
            if (minx > vect3s[i].x)
            {
                minx = vect3s[i].x;
            }
            if (maxx < vect3s[i].x)
            {
                maxx = vect3s[i].x;
            }
            if (miny > vect3s[i].y)
            {
                miny = vect3s[i].y;
            }
            if (maxy < vect3s[i].y)
            {
                maxy = vect3s[i].y;
            }
            if (minz > vect3s[i].z)
            {
                minz = vect3s[i].z;
            }
            if (maxz < vect3s[i].z)
            {
                maxz = vect3s[i].z;
            }
        }
        return gravityCenter = new Vector3((minx + maxx) / 2, (miny + maxy) / 2, (minz + maxz) / 2);
    }
}