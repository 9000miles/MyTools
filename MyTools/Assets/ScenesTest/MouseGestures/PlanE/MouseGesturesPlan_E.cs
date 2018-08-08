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
            JudgeTrend();
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

    /// 1、【判断走向】
    /// 存储模板的走向
    /// 然后从第一个点开始，依次判断后面的点是否在走向角度（60°）范围内
    /// 如果超出该范围，则转入下一个走向角度范围检测
    /// 根据模板的下一个方向，计算检测角度区域的边界（需要判断方向）
    /// 计算出边界后再向模板下一个方向偏转一半的角度（角度范围角平分线），找出离中心角度最近的点，作为下一个走向检测的起点
    /// 再从新起点开始向后检测
    /// 如果有点超出角度走向范围极限，则手势失败，识别率为0

    private bool JudgeTrend()
    {
        List<Vector2> trendList = DrawTemplate.Singleton.trendList;
        List<Transform> templateAllPoint = DrawTemplate.Singleton.allPoint;
        int trendListIndex = 0;
        Transform entrAngleTF;
        Transform outAngleTF;
        Transform outAngleLineTF = null;
        //最大角度，偏移检测角度
        Vector2 outsideAngleLine;
        float maxAngle = 0;

        List<Vector2> safePoint = new List<Vector2>();
        for (int pointIndex = 0; pointIndex < allPoint.Count - 1; pointIndex++)
        {
            //用于快速判断走向是否和模板路径的走向一致
            if (trendListIndex >= trendList.Count) return false;

            entrAngleTF = allPoint[0];
            outAngleLineTF = entrAngleTF;
            Vector2 dir = allPoint[pointIndex + 1].position - entrAngleTF.position;
            //           90度，超出了预期值，可能会因为鼠标微小的滑动而产生错误结果。需要避免
            float angle = Vector2.Angle(dir, trendList[trendListIndex]);//求每个点和当前模板走向的角度
            //angle = Vector2.Angle(dir, templateAllPoint[pointIndex + 1].position - templateAllPoint[pointIndex].position);
            if (maxAngle < angle)//求最大角度，用于确定走向最大反向边界角度
            {
                maxAngle = angle;
                //需要判断下一个的方向是否在正方向，当前点的方向和模板的外边方向一致的话
                //                                    本身      模板
                //外边方向：A（异侧）: <0         >0     True
                //[走向右侧]  B（同侧）: >0         >0

                //外边方向：A（异侧）: <0         <0
                //[走向左侧]  B（同侧）: >0         <0    True
                if ((Vector2.Dot(allPoint[pointIndex].position - entrAngleTF.position, trendList[trendListIndex]) < 0 &&
                   Vector2.Dot(trendList[trendListIndex + 1], trendList[trendListIndex]) > 0) ||
                   (Vector2.Dot(allPoint[pointIndex].position - entrAngleTF.position, trendList[trendListIndex]) > 0 &&
                   Vector2.Dot(trendList[trendListIndex + 1], trendList[trendListIndex]) < 0))
                    outAngleLineTF = allPoint[pointIndex];
            }

            //在角度范围内
            if (angle < DrawTemplate.Singleton.trendAngle)
            {
                safePoint.Add(allPoint[pointIndex].position);
            }

            //没在角度范围内
            else
            {
                //如果超出极限范围
                //if (angle > DrawTemplate.Singleton.trendAngleLimit)
                //    return false;

                trendListIndex++;//转入下一个角度范围检测
                outAngleTF = allPoint[pointIndex];//拿到超出边界的物体
                //找出最大边界点到当前超出范围点之间的所有点
                int outAngleLineTFIndex = allPoint.FindIndex(t => t == outAngleLineTF);
                List<Transform> enterOutTFList = allPoint.GetRange(outAngleLineTFIndex, pointIndex - outAngleLineTFIndex);

                //Transform newSatrtPoint = enterOutTFList.Find(t =>
                //{
                //    Transform newSatrt = null;
                //    float minAngle = DrawTemplate.Singleton.trendAngle;
                //    //求最大边界角度和当前物体的角度
                //    float angleTemp = Vector2.Angle(t.position - entrAngleTF.position, outAngleLineTF.position - entrAngleTF.position);
                //    if (angleTemp - DrawTemplate.Singleton.trendAngle < minAngle)
                //    {
                //        minAngle = angleTemp;
                //        newSatrt = t;
                //    }
                //    return newSatrt;
                //});
                //求离角平分线最近的物体
                entrAngleTF = enterOutTFList.ToArray().GetMin(t =>
                    Vector2.Angle(t.position - entrAngleTF.position, outAngleLineTF.position - entrAngleTF.position));
                pointIndex = allPoint.FindIndex(t => t == entrAngleTF);
            }
        }
        return true;
    }

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