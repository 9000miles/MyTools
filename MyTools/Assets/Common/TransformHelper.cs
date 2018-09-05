using Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Common
{
    ///<summary>
    /// Transform组件助手类
    ///</summary>
    public static class TransformHelper
    {
        /// <summary>
        /// 根据名字查找子物体。
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="childName">需要查找的后代物体名字</param>
        /// <returns>根据名字查找的物体</returns>
        public static Transform FindChildByName(this Transform tf, string childName)
        {
            Transform childTF = tf.Find(childName);
            //如果找到 则退出
            if (childTF != null)
                return childTF;
            //如果没有找到，递归子物体进行查找。
            for (int i = 0; i < tf.childCount; i++)
            {
                childTF = tf.GetChild(i).FindChildByName(childName);
                if (childTF != null)
                    return childTF;
            }
            return null;
        }

        /// <summary>
        /// 根据子名字和孙子名字，查找孙子物体。
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="childName">子物体名字</param>
        /// <param name="grandsonChild">孙子物体名字</param>
        /// <returns>根据名字查找的物体</returns>
        public static Transform FindChildByName(this Transform tf, string childName, string grandsonChild)
        {
            //根据查找子物体
            Transform childTF = tf.Find(childName);
            if (childTF == null)
                for (int i = 0; i < tf.childCount; i++)
                    childTF = tf.GetChild(i).FindChildByName(childName);
            if (childTF == null)
                return null;

            //根据子物体，查找孙子物体
            Transform grandChildTF = childTF.Find(grandsonChild);
            if (grandChildTF != null)
                return grandChildTF;
            for (int i = 0; i < childTF.childCount; i++)
            {
                grandChildTF = childTF.GetChild(i).FindChildByName(grandsonChild);
                if (grandChildTF != null)
                    return grandChildTF;
            }
            return null;
        }

        /// <summary>
        /// 缓动注视目标方向旋转
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="targetDir">目标方向</param>
        /// <param name="rotateSpeed">旋转速度</param>
        public static void LookDirection(this Transform tf, Vector3 targetDir, float rotateSpeed)
        {
            if (targetDir == Vector3.zero) return;
            //注视旋转到目标方向
            Quaternion dir = Quaternion.LookRotation(targetDir);
            //插值旋转（由快到慢）
            tf.rotation = Quaternion.Lerp(tf.rotation, dir, Time.deltaTime * rotateSpeed);
        }

        /// <summary>
        /// 缓动注视目标点旋转
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="pos">目标位置</param>
        /// <param name="rotateSpeed">旋转速度</param>
        public static void LookPosition(this Transform tf, Vector3 pos, float rotateSpeed)
        {
            Vector3 dir = pos - tf.position;
            tf.LookDirection(dir, rotateSpeed);
        }

        /// <summary>
        /// 获取周边物体
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="distance">距离</param>
        /// <param name="angle">角度</param>
        /// <param name="tags">标签</param>
        /// <returns></returns>
        public static Transform[] GetAroundObject(this Transform currentTF, float distance, float angle, params string[] tags)
        {
            List<Transform> list = new List<Transform>();
            //根据所有标签查找物体
            for (int i = 0; i < tags.Length; i++)
            {
                GameObject[] allGo = GameObject.FindGameObjectsWithTag(tags[i]);
                list.AddRange(allGo.Select(o => o.transform));
            }
            //判断物体是否在攻击范围(距离、角度)
            list = list.FindAll(t =>
                 Vector3.Distance(t.position, currentTF.position) <= distance &&
                 Vector3.Angle(currentTF.forward, t.position - currentTF.position) <= angle / 2f
            );
            return list.ToArray();
        }

        /// <summary>
        /// 查找指定范围内的物体
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="arrary">目标数组</param>
        /// <param name="distance">距离</param>
        /// <param name="angle">角度</param>
        /// <returns>匹配的数组</returns>
        public static Transform[] GetAroundObject(this Transform currentTF, Transform[] arrary, float distance, float angle)
        {
            if (arrary == null || arrary.Length <= 0) return null;
            //返回指定范围内的物体(距离、角度)
            //return arrary.FindAll(t =>
            //     Vector3.Distance(t.position, currentTF.position) <= distance &&
            //     Vector3.Angle(currentTF.forward, t.position - currentTF.position) <= angle / 2f
            //);
            return arrary.FindAll(t =>
                 Vector3.Distance(t.position, currentTF.position) <= distance &&
                 Vector3.Angle(currentTF.forward, Vector3.ProjectOnPlane(t.position - currentTF.position, currentTF.up)) <= angle / 2f
            );
        }

        /// <summary>
        /// 获取最近的物体，在指定范围内查找，要求被查找物体有Collider组件
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="distance">距离</param>
        /// <param name="angle">角度</param>
        /// <param name="isPenetrate">是否穿透墙壁</param>
        /// <param name="tags">标签</param>
        /// <returns></returns>
        public static Transform GetMinDistanceObject(this Transform currentTF, float distance, float angle, bool isPenetrate, params string[] tags)
        {
            Collider[] colliders = Physics.OverlapSphere(currentTF.position, distance);
            List<Collider> list = new List<Collider>();
            foreach (var item in colliders)
            {
                if (Array.Exists(tags, (t) => item.tag == t) && item.transform != currentTF)
                    list.Add(item);
            }
            if (list.Count <= 0) return null;

            Transform[] tfs = list.ToArray().Select((t) => t.GetComponent<Transform>());
            Transform[] matchTFs = currentTF.GetAroundObject(tfs, distance, angle);
            Transform minDistanceTF = null;
            if (isPenetrate)
            {
                List<Transform> getList = new List<Transform>();
                foreach (var item in matchTFs)
                {
                    RaycastHit hit = new RaycastHit();
                    bool isGet = Physics.Raycast(currentTF.position, (item.position - currentTF.position).normalized, out hit, distance);
                    Debug.DrawLine(currentTF.position, hit.point);
                    if (isGet == true && hit.transform == item)
                    {
                        getList.Add(item);
                    }
                }
                minDistanceTF = getList.ToArray().GetMin((t) => Vector3.Distance(t.position, currentTF.position));
            }
            else
            {
                minDistanceTF = matchTFs.GetMin((t) => Vector3.Distance(t.position, currentTF.position));
            }
            return minDistanceTF;
        }

        /// <summary>
        /// 将字符串转换成Vector3，输出该Vector3字符串
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="str">包含该格式的字符串"*(19.34, 1.5, 4.56)*"</param>
        /// <param name="posStr">"输出该格式的字符串"(19.34, 1.5, 4.56)"</param>
        /// <returns>返回Vector3值</returns>
        public static Vector3 ConverVector3(this Transform currentTF, string str, out string posStr)
        {
            Regex regex = new Regex(@"\([\d\.\,\ \-]*\)");
            Match match = regex.Match(str);
            posStr = match.Value;
            string position = match.Value.Replace("(", "").Replace(")", "");
            string[] pos = position.Split(',');
            return new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
        }

        /// <summary>
        /// 将字符串转换成Quaternion，输出该Quaternion字符串
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="str">包含该格式的字符串"*(0, 0.1961161, 0, 0.9805807)*"</param>
        /// <param name="rotStr">"输出该格式的字符串"(0, 0.1961161, 0, 0.9805807)"</param>
        /// <returns></returns>
        public static Quaternion ConvetQuaternion(this Transform currentTF, string str, out string rotStr)
        {
            Regex regex = new Regex(@"\([\d\.\,\ \-]*\)");
            Match match = regex.Match(str);
            rotStr = match.Value;
            string rotation = match.Value.Replace("(", "").Replace(")", "");
            string[] rot = rotation.Split(',');
            return new Quaternion(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]), float.Parse(rot[3]));
        }

        /// <summary>
        /// 获取该物体的路径
        /// </summary>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static string GetSelfPath(this Transform currentTF)
        {
            if (currentTF == null) return null;
            string path = currentTF.name;
            while (currentTF.transform.parent != null)
            {
                currentTF = currentTF.parent;
                path = currentTF.name + "/" + path;
            }
            return path;
        }

        /// <summary>
        /// 获取选中的多个物体路径
        /// </summary>
        /// <param name="tfs"></param>
        /// <returns></returns>
        public static string[] GetSelfPaths(this Transform[] tfs)
        {
            List<string> pathList = new List<string>();
            foreach (var item in tfs)
            {
                pathList.Add(item.GetSelfPath());
            }
            return pathList.ToArray();
        }

        /// <summary>
        /// 获取选中的多个物体路径
        /// </summary>
        /// <param name="tfList"></param>
        /// <returns></returns>
        public static string[] GetSelfPaths(this List<Transform> tfList)
        {
            return tfList.ToArray().GetSelfPaths();
        }

        /// <summary>
        /// 获取本身和孩子的路径
        /// </summary>
        /// <param name="currentTF"></param>
        /// <returns></returns>
        public static string[] GetSelfAndChildrenPaths(this Transform currentTF)
        {
            List<string> pathList = new List<string>();
            Transform[] tfs = currentTF.GetComponentsInChildren<Transform>();
            foreach (var item in tfs)
            {
                pathList.Add(item.GetSelfPath());
            }
            return pathList.ToArray();
        }

        /// <summary>
        /// 获取本身和孩子的路径
        /// </summary>
        /// <param name="tfs"></param>
        /// <returns></returns>
        public static string[] GetSelfAndChildrenPaths(this Transform[] tfs)
        {
            List<string> pathList = new List<string>();
            foreach (var item in tfs)
            {
                string[] itemPaths = item.GetSelfAndChildrenPaths();
                pathList.AddRange(itemPaths);
            }
            return pathList.ToArray();
        }

        /// <summary>
        /// 等距排列
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pointObject"></param>
        /// <param name="pointCount">包含起点终点的点数</param>
        /// <returns></returns>
        public static Vector3[] IsometricArranged(Vector3 from, Vector3 to, int pointCount)
        {
            float spacingRatio = 0;
            List<Vector3> list = new List<Vector3>();
            if (pointCount > 0)
            {
                for (int i = 0; i < pointCount; i++)
                {
                    Vector3 point = Vector3.Lerp(from, to, spacingRatio);
                    spacingRatio += 1f / ((float)pointCount - 1f);
                    list.Add(point);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 定距排列，适用于直线段和折线段
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3[] IsometricArranged(Vector3[] points, int pointCount)
        {
            List<Vector3> list = new List<Vector3>();
            if (pointCount > 0 && points.Length >= 2)
            {
                Vector3[] newPoints = new Vector3[points.Length];
                if (points.Length >= 3)
                {
                    Vector3 dir = (points[1] - points[0]).normalized;
                    Debug.DrawLine(points[1], points[0]);
                    newPoints[0] = points[0];
                    newPoints[1] = points[1];
                    for (int i = 2; i < points.Length; i++)
                    {
                        float dis = Vector3.Distance(points[i - 1], points[i]);
                        Vector3 newPoint = newPoints[i - 1] + dir * dis;
                        newPoints[i] = newPoint;
                    }
                }
                list.AddRange(IsometricArranged(newPoints[0], newPoints[newPoints.Length - 1], pointCount));
            }
            return list.ToArray();
        }

        /// <summary>
        /// 定距排列
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3[] DistanceArranged(Vector3 from, Vector3 to, float distance)
        {
            return null;
        }

        /// <summary>
        /// 定距排列
        /// </summary>
        /// <param name="points"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3[] DistanceArranged(Vector3[] points, float distance)
        {
            return null;
        }
    }
}