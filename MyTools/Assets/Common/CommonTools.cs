using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common
{
    ///  <summary>
    /// 自定义工具类
    ///  </summary>
    public class CommonTools
    {
        #region 字符串处理相关

        /// <summary>
        /// 求2个字符串最长公共字符串
        /// </summary>
        /// <param name="longString">长字符串</param>
        /// <param name="shortString">短字符串</param>
        /// <returns>最长公共字符串</returns>
        public static string LongestCommonString(string longString, string shortString)
        {
            string resultString = ""; //结果字符串
            if (longString.Length < shortString.Length)//交换字符串
            {
                string strTemp = "";//转换字符串
                strTemp = shortString;
                shortString = longString;
                longString = strTemp;
            }
            for (int i = 0; i < longString.Length; i++)//循环最大长度，保证每个字符都进行比对
            {
                for (int j = 0; j < shortString.Length; j++)//循环较短的字符串
                {
                    if (longString[i] == shortString[j])//2个字符串的每个字符进行比较，如果相同
                    {
                        int n = 0;
                        while (longString[i + n] == shortString[j + n])//从相同的位置向后进行比较
                        {
                            n++;//如果相同则加1，记录相同的字符个数
                            if (i + n > longString.Length - 1 || j + n > shortString.Length - 1)//判断是否超出字符串长度范围
                                break;
                        }
                        if (n > resultString.Length)//如果新的相同字符串长度大于之前的
                            resultString = longString.Substring(i, n);//从相同的位置向后截取n个字符，即是最长公共字符串
                    }
                }
            }
            return resultString;//最长公共字符串：  fgfgfgfgfgfgf5h45h
        }

        #endregion 字符串处理相关

        #region 数组处理

        /// <summary>
        /// 合并2个数组，根据源数组的排序方式排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayA">A数组</param>
        /// <param name="arrayB">B数组</param>
        /// <returns></returns>
        public static T[] MergeSortedArray<T>(T[] arrayA, T[] arrayB) where T : IComparable
        {
            T[] mergeArray = new T[arrayA.Length + arrayB.Length];
            Array.Copy(arrayA, mergeArray, arrayA.Length);
            Array.Copy(arrayB, 0, mergeArray, arrayA.Length, arrayB.Length);
            Array.Sort(mergeArray);
            if (arrayA[0].CompareTo(arrayA[1]) > 0)//判断源数组是什么排序方式，如果是从大到小就反转数组
                Array.Reverse(mergeArray);
            return mergeArray;
        }

        /// <summary>
        /// 数组万能排序，冒泡排序，传入2个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">数组</param>
        /// <param name="handler">排序规则</param>
        public static void Sort<T>(T[] array, Func<T, T, bool> handler) /*where T : IComparable*/
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (handler(array[i], array[j]))
                    {
                        T temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// 数组万能排序，冒泡排序，可指定升序还是降序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">数组</param>
        /// <param name="handler">根据什么排序</param>
        /// <param name="isAscending">默认为TRUE升序</param>
        public static void Sort<T, Q>(T[] array, Func<T, Q> handler, bool isAscending = true) where Q : IComparable
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                    {
                        T temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
            if (!isAscending)
                Array.Reverse(array);
        }

        /// <summary>
        /// 判断数组是否具有指定数量的重复个数元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">数组</param>
        /// <param name="n">指定数量</param>
        /// <returns></returns>
        private bool IsAssignReiterationElement<T>(T[] array, int n) /*where T : IComparable*/
        {
            for (int i = 0; i < array.Length; i++)
            {
                int num = 0;
                for (int j = 0; j < array.Length; j++)
                {
                    if (array[i].Equals(array[j])/*.CompareTo(array[j]) == 0*/)
                    {
                        num++;
                        if (num == n)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断数组是否具有指定数量的重复个数元素，并输出重复元素数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">数组</param>
        /// <param name="n">指定数量</param>
        /// <param name="repeatElementArray">重复元素</param>
        /// <returns></returns>
        private bool IsAssignReiterationElement<T>(T[] array, int n, out T[] repeatElementArray) /*where T : IComparable*/
        {
            for (int i = 0; i < array.Length; i++)
            {
                int num = 0;
                for (int j = 0; j < array.Length; j++)
                {
                    if (array[i].Equals(array[j])/*.CompareTo(array[j]) == 0*/)
                    {
                        num++;
                        if (num == n)
                        {
                            repeatElementArray = new T[n];
                            for (int k = 0; k < n; k++)
                                repeatElementArray[k] = array[i];
                            return true;
                        }
                    }
                }
            }
            repeatElementArray = null;
            return false;
        }

        /// <summary>
        /// 从数组中获取指定数量的元素，是否可重复
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="many">获取个数</param>
        /// <param name="isRepetition">是否可重复</param>
        /// <returns></returns>
        public static T[] GetManyRandomElement<T>(T[] array, int many, bool isRepetition = true)
        {
            if (many > array.Length || array.Length <= 0 || many <= 0)
                return null;
            List<T> list = new List<T>(many);
            //不允许重复
            if (isRepetition == false)
            {
                for (int i = 0; i < many; i++)
                {
                    int index = UnityEngine.Random.Range(0, array.Length);
                    if (list.Contains(array[index]))
                        i--;
                    else
                        list.Add(array[index]);
                }
            }
            //允许重复
            else
            {
                for (int i = 0; i < many; i++)
                {
                    int index = UnityEngine.Random.Range(0, array.Length);
                    list.Add(array[index]);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 从数组中随机抽取一个物体
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="array">物体数组</param>
        /// <returns></returns>
        public T GetRandomObjcet<T>(T[] array)
        {
            int index = UnityEngine.Random.Range(0, array.Length);
            return array[index];
        }

        #endregion 数组处理

        #region 声音播放

        /// <summary>
        /// 根据名字播放声音
        /// </summary>
        /// <param name="audioSource">播放器</param>
        /// <param name="audioClips">声音片段数组</param>
        /// <param name="audioName">声音名称</param>
        public void PlayeAudioWithName(AudioSource audioSource, AudioClip[] audioClips, string audioName)
        {
            if (audioSource == null || audioClips.Length == 0)
                return;
            AudioClip audioClip = null;
            for (int i = 0; i < audioClips.Length; i++)
                if (audioClips[i].name == audioName)
                    audioClip = audioClips[i];
            if (audioClip == null)
                return;
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        #endregion 声音播放

        #region 曲线运动

        /// <summary>
        /// 沿曲线运动
        /// </summary>
        /// <param name="moveTF">移动的物体</param>
        /// <param name="points">移动的点数组</param>
        /// <param name="lerpValue">Lerp数值</param>
        /// <param name="moveSpeed">到下一个点的移动速度</param>
        /// <returns></returns>
        public IEnumerator MoveFollowCurve(Transform moveTF, Vector3[] points, float minDistance, float lerpValue, float moveSpeed)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                while (Vector3.Distance(moveTF.position, points[i + 1]) > minDistance)
                    moveTF.position = Vector3.Lerp(moveTF.position, points[i + 1], lerpValue);
                //yield return new WaitForSeconds(moveSpeed);
                yield return null;
            }
        }

        #endregion 曲线运动

        #region 距离位置

        /// <summary>
        /// 获取指定方向指定距离的位置
        /// </summary>
        /// <param name="startPos">起始点</param>
        /// <param name="dir">方向</param>
        /// <param name="distance">距离</param>
        /// <returns></returns>
        public Vector3 GetToDirectionDistancePosition(Vector3 startPos, Vector3 dir, float distance)
        {
            return startPos + dir.normalized * distance;
        }

        /// <summary>
        /// 计算2点之间分割的所有点
        /// </summary>
        /// <param name="pos1">终点</param>
        /// <param name="pos2">起点</param>
        /// <param name="pointCount">总点数</param>
        /// <returns>包含起点和终点的所有分割点数组</returns>
        public Vector3[] TwoPointBetweenPoints(Vector3 pos1, Vector3 pos2, int pointCount)
        {
            Vector3[] nodePoints = new Vector3[pointCount];
            float ratio = 1f / (pointCount - 1);
            float t = 0;
            for (int i = 0; i < pointCount; i++)
            {
                nodePoints[i] = Vector3.Lerp(pos1, pos2, t);
                t += ratio;
            }
            return nodePoints;
        }

        #endregion 距离位置

        #region 方法处理

        /// <summary>
        /// 委托调用方法名
        /// </summary>
        public delegate void MethodName();

        /// <summary>
        /// 延时调用方法
        /// </summary>
        /// <param name="obj">游戏物体</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="timer">延时时间</param>
        /// CommonTools.DelayTimeCallMethod(gameObject, Print, 2f);
        public static void DelayTimeCallMethod(GameObject obj, MethodName methodName, float timer)
        {
            obj.GetComponent<MonoBehaviour>().Invoke(methodName.Method.Name, timer);
        }

        #endregion 方法处理

        #region UI界面相关

        /// <summary>
        /// 刷新时间以00：00 格式显示
        /// </summary>
        public string RefreshTime(DateTime startTime)
        {
            TimeSpan currentTimeSpan = DateTime.Now.Subtract(startTime);
            return currentTimeSpan.Minutes.ToString().PadLeft(2, '0') + " : " + currentTimeSpan.Seconds.ToString().PadLeft(2, '0');
        }

        /// <summary>
        /// 获取鼠标点击的UI对象
        /// </summary>
        /// <returns>点击的UI对象</returns>
        public GameObject OnePointColliderObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0 ? results[0].gameObject : null;
        }

        /// <summary>
        /// UI点击子物体隐藏父物体
        /// </summary>
        public void UIClickSonHideParent()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            GameObject go = results.Count > 0 ? results[0].gameObject : null;
            go.gameObject.transform.parent.gameObject.SetActive(false);
        }

        /// <summary>
        /// 检查鼠标是否点击在UI上面
        /// </summary>
        /// <returns></returns>
        public static bool CheckGuiRaycastObjects()
        {
            EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            GraphicRaycaster graphicRaycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
            PointerEventData eventData = new PointerEventData(eventSystem);
            eventData.pressPosition = Input.mousePosition;
            eventData.position = Input.mousePosition;
            List<RaycastResult> list = new List<RaycastResult>();
            graphicRaycaster.GetComponent<GraphicRaycaster>().Raycast(eventData, list);
            return list.Count > 0;
        }

        /// <summary>
        ///随机更换颜色Graphic，UI
        /// </summary>
        /// <param name="graphic">UI颜色组件</param>
        /// <param name="colorValue">颜色值，结构体</param>
        /// <returns></returns>
        private IEnumerator RandomChangeColor(Graphic graphic, RandomChangeColorValue colorValue)
        {
            while (true)
            {
                float R = UnityEngine.Random.Range(colorValue.minR, colorValue.maxR);
                float G = UnityEngine.Random.Range(colorValue.minG, colorValue.maxG);
                float B = UnityEngine.Random.Range(colorValue.minB, colorValue.maxB);
                float changeTimer = UnityEngine.Random.Range(colorValue.minChangeTimer, colorValue.maxChangeTimer);
                float nextTime = Time.time;
                while (Time.time < nextTime + changeTimer)
                {
                    //更换颜色
                    graphic.color = Color.Lerp(graphic.color, new Color(R, G, B), colorValue.changeSpeed);
                    yield return null;
                }
                //更换颜色间隔时间
                yield return new WaitForSeconds(changeTimer);
            }
        }

        /// <summary>
        ///随机更换颜色Material，GameObject
        /// </summary>
        /// <param name="material">游戏物体颜色组件</param>
        /// <param name="colorValue">颜色值，结构体</param>
        /// <returns></returns>
        private IEnumerator RandomChangeColor(Material material, RandomChangeColorValue colorValue)
        {
            while (true)
            {
                float R = UnityEngine.Random.Range(colorValue.minR, colorValue.maxR);
                float G = UnityEngine.Random.Range(colorValue.minG, colorValue.maxG);
                float B = UnityEngine.Random.Range(colorValue.minB, colorValue.maxB);
                float changeTimer = UnityEngine.Random.Range(colorValue.minChangeTimer, colorValue.maxChangeTimer);
                float nextTime = Time.time;
                while (Time.time < nextTime + changeTimer)
                {
                    //更换颜色
                    material.color = Color.Lerp(material.color, new Color(R, G, B), colorValue.changeSpeed);
                    yield return null;
                }
                //更换颜色间隔时间
                yield return new WaitForSeconds(changeTimer);
            }
        }

        /// <summary>
        ///随机更换颜色Shadow，GameObject
        /// </summary>
        /// <param name="material">游戏物体颜色组件</param>
        /// <param name="colorValue">颜色值，结构体</param>
        /// <returns></returns>
        private IEnumerator RandomChangeColor(Shadow shadow, RandomChangeColorValue colorValue)
        {
            while (true)
            {
                float R = UnityEngine.Random.Range(colorValue.minR, colorValue.maxR);
                float G = UnityEngine.Random.Range(colorValue.minG, colorValue.maxG);
                float B = UnityEngine.Random.Range(colorValue.minB, colorValue.maxB);
                float changeTimer = UnityEngine.Random.Range(colorValue.minChangeTimer, colorValue.maxChangeTimer);
                float nextTime = Time.time;
                while (Time.time < nextTime + changeTimer)
                {
                    //更换颜色
                    shadow.effectColor = Color.Lerp(shadow.effectColor, new Color(R, G, B), colorValue.changeSpeed);
                    yield return null;
                }
                //更换颜色间隔时间
                yield return new WaitForSeconds(changeTimer);
            }
        }

        #endregion UI界面相关
    }

    /// <summary>
    /// 随机改变颜色设置值，结构体
    /// </summary>
    public struct RandomChangeColorValue
    {
        public float minR;
        public float maxR;
        public float minG;
        public float maxG;
        public float minB;
        public float maxB;
        public float minChangeTimer;
        public float maxChangeTimer;
        public float changeSpeed;
    }
}