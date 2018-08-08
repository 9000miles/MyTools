using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MouseGesturesPlan_E : MonoBehaviour
{
    private int pointIndex;
    public Image point;
    private List<Transform> allPoint;
    private Dictionary<Transform, float> notInAreaDic;
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
        notInAreaDic = new Dictionary<Transform, float>();
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
            newPoint.name = "Point " + pointIndex++;
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
            FitTemplateProportions();
            await new WaitForUpdate();
            JudgePointsIncludedAngleLine();
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

            pointIndex = 0;
            recognitionRate = 0;
            allPoint.Clear();
            notInAreaDic.Clear();
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

    private void FitTemplateProportions()
    {
        float length = 0;
        float width = 0;
        float height = 0;
        CalculateLengthWidthHigh(allPoint.ToArray().Select(t => t.position), out length, out width, out height);

        float ratio = DrawTemplate.Singleton.templateHight / width * 0.9f;
        pointsPanel.localScale = new Vector3(pointsPanel.localScale.x * ratio, pointsPanel.localScale.y * ratio, 0);
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

    /// <summary>
    /// 判断每个点在对应的模板中哪些夹角内，
    /// 先顺向判断夹角是否在夹角内，如果在则放入一个集合中
    /// 然后再反向计算，如果在则放入一个集合中
    /// 然后然后提取2个集合中的公共部分，作为识别率的主要参考
    ///
    /// 判断邻接的下一个点角度，在不在模板的下一个点的夹角内
    ///
    /// 1、【判断走向】
    /// 存储模板的走向
    /// 然后从第一个点开始，依次判断后面的点是否在走向角度（60°）范围内
    /// 如果超出该范围，则转入下一个走向角度范围检测
    /// 取开始点的走向角度范围角平分线最近的点作为下一个走向检测的起点
    /// 再从新起点开始向后检测
    /// 如果有点超出角度走向范围，则手势失败，识别率为0
    ///
    /// 2、【计算点是否在夹角范围内，计算识别率】
    /// 计算模板夹角上下最高和最低相交点，求2点之间的距离
    ///                 计算所有的夹角线的相交点，然后算出最高点和最低点
    /// 然后将Panel缩放到最大高度
    /// 先顺向判断每一个点是否在其夹角内  ==
    /// 如果有点没在夹角范围内，则加入一个Dic中，存储该点和偏移角度  ==
    /// 计算每一个点到每个夹角的法向量，再计算2个法向量的夹角，如果是钝角则在夹角内
    ///                 （改用判断该点和模板点的夹角大小，是否在模板规定的夹角范围内）==
    /// 然后再逆向判断每一个点  ==
    /// 逆向判断时如果在范围内，则从Dic中剔除  ==
    /// 将角度进行累加，计算整体识别率
    /// </summary>
    /// <returns></returns>
    private void JudgePointsIncludedAngleLine()
    {
        float tempLateAngle = DrawTemplate.Singleton.angle;
        List<Transform> tempLateAllPoint = DrawTemplate.Singleton.allPoint;

        //顺向判断
        foreach (var item in allPoint)
        {
            for (int index = 0; index < tempLateAllPoint.Count - 1; index++)
            {
                Vector2 templateDir = tempLateAllPoint[index + 1].position - tempLateAllPoint[index].position;
                float selfAngle = Vector2.Angle(item.position - tempLateAllPoint[index].position, templateDir);
                bool isInAngle = selfAngle <= tempLateAngle;
                if (isInAngle == false)
                {
                    if (!notInAreaDic.ContainsKey(item))
                    {
                        float offsetAngle = selfAngle - tempLateAngle;
                        notInAreaDic.Add(item, offsetAngle);
                    }
                }
                else
                {
                    notInAreaDic.Remove(item);
                    break;
                }
            }
        }

        //逆向判断
        allPoint.Reverse();
        foreach (var item in allPoint)
        {
            for (int index = tempLateAllPoint.Count - 1; index > 0; index--)
            {
                Vector2 templateDir = tempLateAllPoint[index - 1].position - tempLateAllPoint[index].position;
                float selfAngle = Vector2.Angle(item.position - tempLateAllPoint[index].position, templateDir);
                bool isInAngle = selfAngle <= tempLateAngle;
                if (isInAngle == true)
                {
                    notInAreaDic.Remove(item);
                    break;
                }
            }
        }
    }

    private void CalculateRecongnitionRate()
    {
        recognitionRate = 0;

        foreach (var item in notInAreaDic.Values)
        {
            recognitionRate += item;
        }
        foreach (var item in notInAreaDic.Keys)
        {
            Debug.Log(item.name);
        }
    }
}