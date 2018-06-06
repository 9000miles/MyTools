using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手电筒探照器
/// </summary>
public class FlashlightDetector : MonoBehaviour
{
    [Range(0, 200)]
    public int length;

    [Range(0, 360)]
    public int angle;

    private int mAngle;

    [Range(0, 10000)]
    public int number;

    private int mNumber;

    [Range(0, 500)]
    public int circleNumber;

    private int mCircleNumber;

    private int[] arr;
    private bool isCreate;
    private Color[] colors;
    private List<Transform> list = new List<Transform>();
    private float minAngle = 0;
    private float minDistance = 0;

    private void Start()
    {
        mAngle = angle - 1;
        mNumber = number - 1;
        mCircleNumber = circleNumber - 1;
        colors = new Color[circleNumber];
        for (int i = 0; i < circleNumber; i++)
        {
            colors[i] = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        }
    }

    private void Update()
    {
        //arr = CreateCirclePlanB();
        arr = CreateCirclePlanC();
        SetColors();
        DrawRay(arr, transform.forward, angle, true);
    }

    /// <summary>
    /// 随机生成颜色
    /// </summary>
    private void SetColors()
    {
        if (angle != mAngle || number != mNumber || circleNumber != mCircleNumber)//如果发生改变则从新生成颜色
        {
            mAngle = angle;
            mNumber = number;
            mCircleNumber = circleNumber;
            colors = new Color[circleNumber];
            for (int i = 0; i < circleNumber; i++)
            {
                colors[i] = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            }

            int mun = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                Debug.Log(string.Format("第{0}圈  {1}", i, arr[i].ToString()));
                mun += arr[i];
            }
            Debug.Log("总数  " + mun);
        }
    }

    /// <summary>
    /// 绘制射线
    /// </summary>
    /// <param name="arr"></param>
    /// <param name="dirction"></param>
    /// <param name="totalAngle"></param>
    private void DrawRay(int[] arr, Vector3 dirction, int totalAngle, bool isSingleObject)
    {
        if (tf != null)
            tf.GetComponent<Renderer>().material.color = Color.white;
        list.Clear();
        bool isCast = false;
        float angle = totalAngle / 2;
        dirction = Quaternion.Euler(new Vector3(0, angle, 0)) * dirction;//向Y轴旋转角度，从外侧开始绘制
        List<Transform> castList = new List<Transform>();
        for (int i = arr.Length - 1; i >= 0; i--)
        {
            for (int j = 0; j < arr[i]; j++)
            {
                Debug.DrawRay(transform.position, dirction * 10, colors[i]);
                if (PhysicsRayCast(transform.position, dirction, length, isSingleObject, out castList))
                    isCast = true;
                angle = 360f / arr[i];//平均分布
                dirction = Quaternion.Euler(new Vector3(0, 0, angle)) * dirction;//向Z轴旋转角度，旋转偏移角度
            }
            angle = totalAngle / 2f / (circleNumber - 1f);
            dirction = Quaternion.Euler(new Vector3(0, -angle, 0)) * dirction;//向Y轴旋转角度，计算向内偏移的角度
        }
        if (isSingleObject)
        {
            if (tf != null && isCast)
                tf.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            foreach (var item in list)
            {
                item.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }

    private Transform tf = null;

    /// <summary>
    /// 投射射线并将碰撞到的物体添加到集合，改变其颜色
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="length"></param>
    private bool PhysicsRayCast(Vector3 origin, Vector3 direction, float length, bool isSingleObject, out List<Transform> castList)
    {
        RaycastHit hit;
        bool isCast = Physics.Raycast(origin, direction, out hit, length, 1 << LayerMask.NameToLayer("Object"));
        if (isCast)
        {
            if (!list.Contains(hit.transform))
            {
                list.Add(hit.transform);
            }
            minAngle = Vector3.Dot(transform.forward, list[0].position);
            minDistance = Vector3.Distance(transform.position, list[0].position);
            if (isSingleObject)
            {
                foreach (var item in list)
                {
                    float forwardAngle = Vector3.Dot(transform.forward, item.position);
                    float twoDistance = Vector3.Distance(transform.position, item.position);
                    if (forwardAngle <= minAngle && twoDistance <= minDistance)
                    {
                        tf = item;
                    }
                }
            }
            else
            {
                castList = list;
            }
        }
        castList = null;
        return isCast;
    }

    /// <summary>
    /// 累计平分法
    /// </summary>
    /// <returns></returns>
    private int[] CreateCirclePlanC()
    {
        if (number <= 0)
            return null;
        int remain = number;//余数
        int quotient = 0;//商
        int arrLength = circleNumber == 0 ? 1 : circleNumber;
        int[] arr = new int[arrLength];//5
        int n = arrLength - 1;//4
        do
        {
            if (n == 0)
            {
                arr[n] = 1;
                break;
            }
            else
                quotient = remain / (circleNumber - 1) == 0 ? 1 : remain / (circleNumber - 1);//如果余数为0，则加上剩余的数
            arr[n] += quotient;
            remain -= quotient;
            n--;
            if (n == 0)
            {
                if (arr[n] != 1)//保证中间只有一个
                {
                    arr[n] = 1;
                    remain -= 1;
                }
                n = arrLength - 1;//实现数值循环
            }
        } while (remain > 0);
        return arr;
    }

    /// <summary>
    /// 一半相加法
    /// </summary>
    /// <returns></returns>
    private int[] CreateCirclePlanB()
    {
        if (number <= 0)
            return null;
        int remain = number;//余数
        int quotient = 0;//商
        int arrLength = circleNumber == 0 ? 1 : circleNumber;
        int[] arr = new int[arrLength];//5
        int n = arrLength - 1;//4
        do
        {
            if (n == 0)
            {
                arr[n] = 1;
                break;
            }
            else
                quotient = remain / 2 == 0 ? 1 : remain / 2;//如果余数为0，则加上剩余的数
            arr[n] += quotient;
            remain = remain - quotient;
            n--;
            if (n == 0)
            {
                if (arr[n] != 1)
                {
                    arr[n] = 1;
                    remain -= 1;
                }
                n = arrLength - 1;
            }
        } while (remain > 0);
        //保证只创建一次
        if (isCreate == false)
        {
            for (int i = 0; i < arr.Length; i++)//根据数组创建物体
            {
                GameObject go = new GameObject(string.Format("第{0}圈", i.ToString()));
                for (int j = 0; j < arr[i]; j++)
                {
                    GameObject go0 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go0.transform.parent = go.transform;
                }
            }
            isCreate = true;
        }
        return arr;
    }
}