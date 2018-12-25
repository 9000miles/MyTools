using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MarsPC
{
    ///<summary>
    /// Transform组件助手类
    ///</summary>
    public static class TransformHelper
    {
        /// <summary>A useful Epsilon</summary>
        public const float Epsilon = 0.0001f;

        public enum EDirection
        {
            Forward,
            Back,
            Left,
            Right,
        }

        internal static Vector3 GetPosition(Vector3 newPosition, float initialSpeed, float initialAngle, float totalTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据名字查找子物体。
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="childName">需要查找的后代物体名字</param>
        /// <returns>根据名字查找的物体</returns>
        public static Transform FindChildByName(this Transform tf, string childName)
        {
            Transform childTF = tf.Find(childName);
            if (childTF != null)
                return childTF;
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
            Transform childTF = tf.Find(childName);
            if (childTF == null)
                for (int i = 0; i < tf.childCount; i++)
                    childTF = tf.GetChild(i).FindChildByName(childName);
            if (childTF == null)
                return null;

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
            Quaternion dir = Quaternion.LookRotation(targetDir);
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
        /// 获取周边物体，在自身Direction方向，Angle角度，Distance距离内，自身到指定Tag标签的物体往投影平面投影
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="distance">距离</param>
        /// <param name="angle">角度</param>
        /// <param name="direction">方向</param>
        /// <param name="plane">角度投影平面</param>
        /// <param name="hasToSameDirection">必须同一方向</param>
        /// <param name="tags">物体的标签</param>
        /// <returns></returns>
        public static Transform[] GetAroundObject(this Transform currentTF, float distance, float angle, Vector3 direction, Vector3 plane, bool hasToSameDirection = true, params string[] tags)
        {
            List<Transform> list = new List<Transform>();
            for (int i = 0; i < tags.Length; i++)
            {
                GameObject[] allGo = GameObject.FindGameObjectsWithTag(tags[i]);
                list.AddRange(allGo.Select(o => o.transform));
            }
            if (hasToSameDirection)
            {
                list = list.FindAll(t => Vector3.Distance(t.position, currentTF.position) <= distance &&
                        ProjectPlaneAngle(direction, t.position - currentTF.position, plane) <= angle / 2f);
            }
            else
            {
                list = list.FindAll(t => Vector3.Distance(t.position, currentTF.position) <= distance &&
                            (ProjectPlaneAngle(direction, t.position - currentTF.position, plane) <= angle / 2f) ||
                            (180f - ProjectPlaneAngle(direction, t.position - currentTF.position, plane)) <= angle / 2f);
            }
            return list.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="direction"></param>
        /// <param name="plane"></param>
        /// <param name="distance"></param>
        /// <param name="angle"></param>
        /// <param name="isRangeDistance"></param>
        /// <param name="hasToSameDirection"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static Transform[] GetAroundObject(this Transform currentTF, Vector3 direction, Vector3 plane, float distance, float angle, bool isRangeDistance = true, bool hasToSameDirection = true, params string[] tags)
        {
            List<Transform> list = new List<Transform>();
            for (int i = 0; i < tags.Length; i++)
            {
                GameObject[] allGo = GameObject.FindGameObjectsWithTag(tags[i]);
                list.AddRange(allGo.Select(o => o.transform));
            }

            if (isRangeDistance)
            {
                if (hasToSameDirection)
                {
                    list = list.FindAll(t => Vector3.Distance(t.position, currentTF.position) <= distance &&
                            ProjectPlaneAngle(direction, t.position - currentTF.position, plane) <= angle / 2f);
                }
                else
                {
                    list = list.FindAll(t => Vector3.Distance(t.position, currentTF.position) <= distance &&
                                (ProjectPlaneAngle(direction, t.position - currentTF.position, plane) <= angle / 2f) ||
                                (180f - ProjectPlaneAngle(direction, t.position - currentTF.position, plane)) <= angle / 2f);
                }
            }
            else
            {
                if (hasToSameDirection)
                {
                    list = list.FindAll(t => ProjectPlaneAngle(direction, t.position - currentTF.position, plane) <= angle / 2f);
                }
                else
                {
                    list = list.FindAll(t => ProjectPlaneAngle(direction, t.position - currentTF.position, plane) <= angle / 2f ||
                                (180f - ProjectPlaneAngle(direction, t.position - currentTF.position, plane)) <= angle / 2f);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获取周边物体
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="distance">距离</param>
        /// <param name="angle">角度</param>
        /// <param name="tags">标签</param>
        /// <returns></returns>
        public static Transform[] GetAroundObject(this Transform currentTF, Vector3 direction, Vector3 plane, float distance, float angle, int layer, bool isRangeDistance = true, bool hasToSameDirection = true)
        {
            Collider[] colliders = Physics.OverlapSphere(currentTF.position, distance);
            List<Transform> list = new List<Transform>();
            foreach (var item in colliders)
            {
                if (item.gameObject.layer == layer && item.transform != currentTF)
                    list.Add(item.transform);
            }

            if (isRangeDistance)
            {
                if (hasToSameDirection)
                {
                    list = list.FindAll(t => Vector3.Distance(t.position, currentTF.position) <= distance &&
                            ProjectPlaneAngle(direction, t.position - currentTF.position, plane) <= angle / 2f);
                }
                else
                {
                    list = list.FindAll(t => Vector3.Distance(t.position, currentTF.position) <= distance &&
                                (ProjectPlaneAngle(direction, t.position - currentTF.position, plane) <= angle / 2f) ||
                                (180f - ProjectPlaneAngle(direction, t.position - currentTF.position, plane)) <= angle / 2f);
                }
            }
            else
            {
                if (hasToSameDirection)
                {
                    list = list.FindAll(t => ProjectPlaneAngle(direction, t.position - currentTF.position, plane) <= angle / 2f);
                }
                else
                {
                    list = list.FindAll(t => ProjectPlaneAngle(direction, t.position - currentTF.position, plane) <= angle / 2f ||
                                (180f - ProjectPlaneAngle(direction, t.position - currentTF.position, plane)) <= angle / 2f);
                }
            }
            return list.ToArray().Select(t => t.transform);
        }

        /// <summary>
        /// 查找指定范围内的物体
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="arrary">目标数组</param>
        /// <param name="distance">距离</param>
        /// <param name="angle">角度</param>
        /// <returns>匹配的数组</returns>
        public static Transform[] GetAroundObject(this Transform currentTF, Vector3 direction, Transform[] arrary, float distance, float angle, Vector3 plane, bool hasToSameDirection)
        {
            if (arrary == null || arrary.Length <= 0) return null;
            List<Transform> list = new List<Transform>();

            if (hasToSameDirection)
            {
                list.AddRange(arrary.FindAll(t => Vector3.Distance(t.position, currentTF.position) <= distance &&
                       ProjectPlaneAngle(direction, t.position - currentTF.position, plane) <= angle / 2f));
            }
            else
            {
                list.AddRange(arrary.FindAll(t => Vector3.Distance(t.position, currentTF.position) <= distance &&
                            (ProjectPlaneAngle(direction, t.position - currentTF.position, plane) <= angle / 2f) ||
                            (180f - ProjectPlaneAngle(direction, t.position - currentTF.position, plane)) <= angle / 2f));
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获取最近的物体，在指定范围内查找，要求被查找物体有Collider组件
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="distance">距离</param>
        /// <param name="angle">角度</param>
        /// <param name="isPenetrate">是否穿透墙壁</param>
        /// <param name="layer">层级</param>
        /// <returns></returns>
        public static Transform GetMinDistanceObject(this Transform currentTF, Vector3 direction, float distance, float angle, bool isPenetrate, Vector3 plane, bool hasToSameDirection, int layer)
        {
            Collider[] colliders = Physics.OverlapSphere(currentTF.position, distance);
            List<Collider> list = new List<Collider>();
            foreach (var item in colliders)
            {
                if (item.gameObject.layer == layer && item.transform != currentTF)
                    list.Add(item);
            }
            if (list.Count <= 0) return null;

            Transform[] tfs = list.ToArray().Select((t) => t.GetComponent<Transform>());
            Transform[] matchTFs = currentTF.GetAroundObject(direction, tfs, distance, angle, plane, hasToSameDirection);
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
        /// 获取最近的物体，在指定范围内查找，要求被查找物体有Collider组件
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="distance">距离</param>
        /// <param name="angle">角度</param>
        /// <param name="isPenetrate">是否穿透墙壁</param>
        /// <param name="tags">标签</param>
        /// <returns></returns>
        public static Transform GetMinDistanceObject(this Transform currentTF, Vector3 direction, float distance, float angle, bool isPenetrate, Vector3 plane, bool hasToSameDirection, params string[] tags)
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
            Transform[] matchTFs = currentTF.GetAroundObject(direction, tfs, distance, angle, plane, hasToSameDirection);
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
            if (match.Success)
            {
                posStr = match.Value;
                string position = match.Value.Replace("(", "").Replace(")", "");
                string[] pos = position.Split(',');
                return new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
            }
            posStr = string.Empty;
            return Vector3.zero;
        }

        /// <summary>
        /// 将字符串转换成Vector3，输出该Vector3字符串
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="str">包含该格式的字符串"*(19.34, 1.5, 4.56)*"</param>
        /// <returns>返回Vector3值</returns>
        public static Vector3 ConverVector3(this Transform currentTF, string str)
        {
            Regex regex = new Regex(@"\([\d\.\,\ \-]*\)");
            Match match = regex.Match(str);
            if (match.Success)
            {
                string position = match.Value.Replace("(", "").Replace(")", "");
                string[] pos = position.Split(',');
                return new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
            }
            return Vector3.zero;
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
            if (match.Success)
            {
                rotStr = match.Value;
                string rotation = match.Value.Replace("(", "").Replace(")", "");
                string[] rot = rotation.Split(',');
                return new Quaternion(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]), float.Parse(rot[3]));
            }
            rotStr = string.Empty;
            return Quaternion.Euler(0, 0, 0);
        }

        /// <summary>
        /// 将字符串转换成Quaternion，输出该Quaternion字符串
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="str">包含该格式的字符串"*(0, 0.1961161, 0, 0.9805807)*"</param>
        /// <returns></returns>
        public static Quaternion ConvetQuaternion(this Transform currentTF, string str)
        {
            Regex regex = new Regex(@"\([\d\.\,\ \-]*\)");
            Match match = regex.Match(str);
            if (match.Success)
            {
                string rotation = match.Value.Replace("(", "").Replace(" ", "").Replace(")", "");
                string[] rot = rotation.Split(',');
                return new Quaternion(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]), float.Parse(rot[3]));
            }
            return new Quaternion(0, 0, 0, 0);
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
        /// 判断一个点是否在线段内
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool IsPointInLineSegment(this Vector3 point, Vector3 start, Vector3 end)
        {
            if (point == start || point == end) return true;
            float startDot = Vector3.Dot(end - start, point - start);
            float endDot = Vector3.Dot(start - end, point - end);
            Vector3 dirLine = (end - start).normalized;
            Vector3 dirPoint = (point - start).normalized;
            return startDot >= 0 && endDot >= 0 && dirLine == dirPoint;
        }

        /// <summary>
        /// 判断一个点是否在线段内
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool WhetherPointIsOnTheLineSegment(Vector2 start, Vector2 end, Vector2 point)
        {
            bool isInside =
                (point.x >= start.x && point.x <= end.x && point.y >= start.y && point.y <= end.y) ||
                (point.x >= end.x && point.x <= start.x && point.y >= end.y && point.y <= start.y) ||

                (point.x >= start.x && point.x <= end.x && point.y >= end.y && point.y <= start.y) ||
                (point.x >= end.x && point.x <= start.x && point.y >= start.y && point.y <= end.y);
            return isInside;
        }

        /// <summary>
        /// 判断一个点是否同时在2条线段内
        /// </summary>
        /// <param name="L1Start"></param>
        /// <param name="L1End"></param>
        /// <param name="L2Start"></param>
        /// <param name="L2End"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool WhetherPointIsOnTheLineSegment(Vector2 L1Start, Vector2 L1End, Vector2 L2Start, Vector2 L2End, Vector2 point)
        {
            bool isInsideL1 = WhetherPointIsOnTheLineSegment(L1Start, L1End, point);
            bool isInsideL2 = WhetherPointIsOnTheLineSegment(L2Start, L2End, point);

            #region 有误差存在

            //bool isInsideL1 = Vector2.Distance(point, L1Start) + Vector2.Distance(point, L1End) - Vector2.Distance(L1Start, L1End) < 0.01f;
            //bool isInsideL2 = Vector2.Distance(point, L2Start) + Vector2.Distance(point, L2End) - Vector2.Distance(L2Start, L2End) < 0.01f;

            //bool isInsideL1 = Vector2.SqrMagnitude(point - L1Start) + Vector2.SqrMagnitude(point - L1End) == Vector2.SqrMagnitude(L1Start - L1End);
            //bool isInsideL2 = Vector2.SqrMagnitude(point - L2Start) + Vector2.SqrMagnitude(point - L2End) == Vector2.SqrMagnitude(L2Start - L2End);

            #endregion 有误差存在

            return isInsideL1 && isInsideL2;
        }

        /// <summary>
        /// 计算交点坐标
        /// </summary>
        /// <param name="line1Start"></param>
        /// <param name="line1End"></param>
        /// <param name="line2Start"></param>
        /// <param name="line2End"></param>
        /// <returns></returns>
        public static Vector2 CalculateIntersectionCoordinates(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End)
        {
            Vector2 result = new Vector2();
            float left, right;
            left = (line1End.y - line1Start.y) * (line2End.x - line2Start.x) - (line2End.y - line2Start.y) * (line1End.x - line1Start.x);
            right = (line2Start.y - line1Start.y) * (line1End.x - line1Start.x) * (line2End.x - line2Start.x) +
                (line1End.y - line1Start.y) * (line2End.x - line2Start.x) * line1Start.x - (line2End.y - line2Start.y) * (line1End.x - line1Start.x) * line2Start.x;
            float x = right / left;
            result.x = Single.IsNaN(x) ? line1Start.x : x;

            left = (line1End.x - line1Start.x) * (line2End.y - line2Start.y) - (line2End.x - line2Start.x) * (line1End.y - line1Start.y);
            right = (line2Start.x - line1Start.x) * (line1End.y - line1Start.y) * (line2End.y - line2Start.y) +
                line1Start.y * (line1End.x - line1Start.x) * (line2End.y - line2Start.y) - line2Start.y * (line2End.x - line2Start.x) * (line1End.y - line1Start.y);
            float y = right / left;
            result.y = Single.IsNaN(y) ? line1Start.y : y;
            return result;
        }

        /// <summary>
        /// 计算交点坐标
        /// </summary>
        /// <param name="line1Start"></param>
        /// <param name="line1End"></param>
        /// <param name="line2Start"></param>
        /// <param name="line2End"></param>
        /// <returns></returns>
        public static bool CalculateIntersectionCoordinates(Vector3 line1Start, Vector3 line1End, Vector3 line2Start, Vector3 line2End, out Vector3 crossPoint)
        {
            Vector3 result = new Vector3();
            if (line1Start.x == line1End.x && line2Start.x == line2End.x)
            {
                Vector2 l1S = new Vector2(line1Start.y, line1Start.z);
                Vector2 l1E = new Vector2(line1End.y, line1End.z);
                Vector2 l2S = new Vector2(line2Start.y, line2Start.z);
                Vector2 l2E = new Vector2(line2End.y, line2End.z);
                Vector2 pos = CalculateIntersectionCoordinates(l1S, l1E, l2S, l2E);
                result = new Vector3(line1Start.x, pos.x, pos.y);
                crossPoint = result;
                return true;
            }
            else if (line1Start.y == line1End.y && line2Start.y == line2End.y)
            {
                Vector2 l1S = new Vector2(line1Start.x, line1Start.z);
                Vector2 l1E = new Vector2(line1End.x, line1End.z);
                Vector2 l2S = new Vector2(line2Start.x, line2Start.z);
                Vector2 l2E = new Vector2(line2End.x, line2End.z);
                Vector2 pos = CalculateIntersectionCoordinates(l1S, l1E, l2S, l2E);
                result = new Vector3(pos.x, line1Start.y, pos.y);
                crossPoint = result;
                return true;
            }
            else if (line1Start.z == line1End.z && line2Start.z == line2End.z)
            {
                Vector2 l1S = new Vector2(line1Start.x, line1Start.y);
                Vector2 l1E = new Vector2(line1End.x, line1End.y);
                Vector2 l2S = new Vector2(line2Start.x, line2Start.y);
                Vector2 l2E = new Vector2(line2End.x, line2End.y);
                Vector2 pos = CalculateIntersectionCoordinates(l1S, l1E, l2S, l2E);
                result = new Vector3(pos.x, pos.y, line1Start.z);
                crossPoint = result;
                return true;
            }
            else
            {
                //在某些情况下可能会为NaN
                float x = ((line2Start.y - line1Start.y) * (line1End.x - line1Start.x) * (line2End.x - line2Start.x) + line1Start.x * (line1End.y - line1Start.y) * (line2End.x - line2Start.x) - line2Start.x * (line1End.x - line1Start.x) * (line2End.y - line2Start.y)) /
                              ((line1End.y - line1Start.y) * (line2End.x - line2Start.x) - (line1End.x - line1Start.x) * (line2End.y - line2Start.y));
                //在某些情况下可能会为NaN
                float y = ((line1End.y - line1Start.y) * (line2End.y - line2Start.y) * (line1Start.x - line2Start.x) + line2Start.y * (line2End.x - line2Start.x) * (line1End.y - line1Start.y) - line1Start.y * (line1End.x - line1Start.x) * (line2End.y - line2Start.y)) /
                              ((line2End.x - line2Start.x) * (line1End.y - line1Start.y) - (line1End.x - line1Start.x) * (line2End.y - line2Start.y));

                float z = ((line2Start.x - line1Start.x) * (line1End.z - line1Start.z) * (line2End.z - line2Start.z) + line1Start.z * (line1End.x - line1Start.x) * (line2End.z - line2Start.z) - line2Start.z * (line2End.x - line2Start.x) * (line1End.z - line1Start.z)) /
                              ((line2End.x - line2Start.x) * (line1End.z - line1Start.z) - (line1End.x - line1Start.x) * (line2End.z - line2Start.z));
                result.x = Single.IsNaN(x) ? line1Start.x : x;
                result.y = Single.IsNaN(y) ? line1Start.y : y;
                result.z = Single.IsNaN(z) ? line1Start.z : -z;
                if (result.IsPointInLineSegment(line1Start, line1End) && result.IsPointInLineSegment(line2Start, line2End))
                {
                    crossPoint = result;
                    return true;
                }
            }
            crossPoint = Vector3.zero;
            return false;
        }

        /// <summary>
        /// 等距平分排列
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
        /// 等距平分排列，适用于直线段和折线段
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3[] IsometricArranged(Vector3[] points, int pointCount)
        {
            List<Vector3> linePointList = new List<Vector3>();
            if (pointCount > 0 && points.Length >= 2)
            {
                Vector3[] newPoints = new Vector3[points.Length];
                if (points.Length >= 3)
                {
                    Vector3 dir = (points[1] - points[0]).normalized;
                    newPoints[0] = points[0];
                    newPoints[1] = points[1];
                    for (int i = 2; i < points.Length; i++)
                    {
                        float dis = Vector3.Distance(points[i - 1], points[i]);
                        Vector3 newPoint = newPoints[i - 1] + dir * dis;
                        newPoints[i] = newPoint;
                    }
                }

                Vector3[] linePoints = IsometricArranged(newPoints[0], newPoints[newPoints.Length - 1], pointCount);

                for (int i = 0; i < linePoints.Length; i++)
                {
                    for (int j = 0; j < newPoints.Length - 1; j++)
                    {
                        bool isIn = linePoints[i].IsPointInLineSegment(newPoints[j], newPoints[j + 1]);
                        if (isIn == true)
                        {
                            float dis = Vector3.Distance(linePoints[i], newPoints[j]);
                            Vector3 resultPoint = points[j] + (points[j + 1] - points[j]).normalized * dis;
                            linePointList.Add(resultPoint);
                            break;
                        }
                    }
                }
            }
            return linePointList.ToArray();
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
            float totalLength = Vector3.Distance(from, to);//8.395981  13.00873  22.07345
            Vector3[] newPoints = new Vector3[(int)(totalLength / distance) + 1];
            Vector3 dir = (to - from).normalized;
            for (int i = 0; i < newPoints.Length; i++)
            {
                newPoints[i] = from + dir * distance * i;
            }
            return newPoints;
        }

        /// <summary>
        /// 定距排列
        /// </summary>
        /// <param name="points"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3[] DistanceArranged(Vector3[] points, float distance)
        {
            List<Vector3> resultList = new List<Vector3>();
            Vector3[] linearSegmentPoints = new Vector3[points.Length];//直线上的点
            if (points.Length >= 3)
            {
                Vector3 dir = (points[1] - points[0]).normalized;
                linearSegmentPoints[0] = points[0];
                linearSegmentPoints[1] = points[1];
                for (int i = 2; i < points.Length; i++)
                {
                    float dis = Vector3.Distance(points[i - 1], points[i]);
                    Vector3 newPoint = linearSegmentPoints[i - 1] + dir * dis;
                    linearSegmentPoints[i] = newPoint;
                }
            }

            Vector3[] linePoints = DistanceArranged(linearSegmentPoints[0], linearSegmentPoints[linearSegmentPoints.Length - 1], distance);

            int pointIndex = 0;
            for (int i = 0; i < linePoints.Length; i++)
            {
                bool isIn = linePoints[i].IsPointInLineSegment(linearSegmentPoints[pointIndex], linearSegmentPoints[pointIndex + 1]);
                pointIndex = isIn ? pointIndex : ++pointIndex;
                float dis = Vector3.Distance(linePoints[i], linearSegmentPoints[pointIndex]);
                Vector3 resultPoint = points[pointIndex] + (points[pointIndex + 1] - points[pointIndex]).normalized * dis;
                resultList.Add(resultPoint);
            }
            return resultList.ToArray();
        }

        /// <summary>
        /// 绘制抛物线
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="speed"></param>
        /// <param name="angle"></param>
        /// <param name="totalTime"></param>
        /// <returns></returns>
        public static Vector3 GetParabolaCoordinate(Vector3 startPoint, float speed, float angle, float totalTime)
        {
            Vector3 point = new Vector3();
            point.z = speed * Mathf.Cos(angle * Mathf.Deg2Rad) * totalTime + startPoint.z;
            point.y = speed * Mathf.Sin(angle * Mathf.Deg2Rad) * totalTime - 0.5f * Mathf.Abs(Physics.gravity.y) * totalTime * totalTime + startPoint.y;
            return point;
        }

        /// <summary>
        /// 获取中点坐标
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static Vector3 GetMidpoint(Vector3 pos1, Vector3 pos2)
        {
            return new Vector3((pos1.x + pos2.x) / 2f, (pos1.y + pos2.y) / 2f, (pos1.z + pos2.z) / 2f);
        }

        /// <summary>
        /// 获取指定坐标偏移角度和距离的点
        /// </summary>
        /// <param name="center"></param>
        /// <param name="innerDis"></param>
        /// <param name="outerDis"></param>
        /// <returns></returns>
        public static Vector2 GetPositionInAnnulus(Vector2 center, float innerDis, float outerDis)
        {
            float randomAngle = UnityEngine.Random.Range(0, 360);
            float randomDistance = UnityEngine.Random.Range(innerDis, outerDis);
            var x = center.x + randomDistance * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
            var y = center.y + randomDistance * Mathf.Sin(randomAngle * Mathf.Deg2Rad);
            return new Vector2(x, y);
        }

        /// <summary>
        /// 获取指定坐标偏移角度和距离的点
        /// </summary>
        /// <param name="center"></param>
        /// <param name="innerDis"></param>
        /// <param name="outerDis"></param>
        /// <returns></returns>
        public static Vector3 GetPositionInAnnulus(Vector3 center, float innerDis, float outerDis)
        {
            float randomAngle = UnityEngine.Random.Range(0, 360);
            float randomDistance = UnityEngine.Random.Range(innerDis, outerDis);
            var x = center.x + randomDistance * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
            var z = center.y + randomDistance * Mathf.Sin(randomAngle * Mathf.Deg2Rad);
            return new Vector3(x, 0, z);
        }

        /// <summary>
        /// 获取组件，如果没有，则自动添加该组件
        /// </summary>
        /// <typeparam name="ComponentT"></typeparam>
        /// <param name="mTF"></param>
        /// <returns></returns>
        public static ComponentT GetComponentNullAdd<ComponentT>(this Transform mTF) where ComponentT : Component
        {
            ComponentT temp = mTF.GetComponent<ComponentT>();
            if (temp == null)
            {
                temp = mTF.gameObject.AddComponent<ComponentT>();
            }
            return temp;
        }

        /// <summary>
        /// 获取子物体Transform
        /// </summary>
        /// <param name="mTF"></param>
        /// <param name="isIncludeCondition">是否是包含条件></param>
        /// <param name="condition">排除条件</param>
        /// <returns></returns>
        public static Transform[] GetChildrensTransform(this Transform mTF, bool isIncludeCondition = false, Predicate<Transform> condition = null)
        {
            Transform[] tfs = mTF.GetComponentsInChildren<Transform>();
            List<Transform> list = new List<Transform>();
            list = tfs.ArrayToList();
            list.Remove(mTF);
            if (condition != null)
            {
                if (isIncludeCondition)
                {
                    List<Transform> includeList = new List<Transform>();
                    includeList = list.FindAll(condition);
                    list = includeList;
                }
                else
                {
                    list.RemoveAll(condition);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获取目标相对于自身的方向和角度
        /// </summary>
        /// <param name="myself"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static EDirection GetTargetDirectionForMe(this Transform myself, Transform target, out float angle)
        {
            EDirection direction = EDirection.Forward;
            Vector3 dir = target.position - myself.position;
            angle = Mathf.Acos(Vector3.Dot(myself.forward.normalized, dir.normalized)) * Mathf.Rad2Deg;

            float dotForward = Vector3.Dot(myself.forward, dir.normalized);//点乘判断前后：dotForward >0在前，<0在后
            if (dotForward > 0 && angle <= 45f)
            {
                return EDirection.Forward;
            }
            else if (dotForward < 0 && angle > 135f)
            {
                return EDirection.Back;
            }

            float dotRight = Vector3.Dot(myself.right, dir.normalized);//点乘判断左右： dotRight>0在右，<0在左
            if (dotRight > 0 && angle <= 135f)
            {
                return EDirection.Right;
            }
            else if (dotRight < 0 && angle > 45f)
            {
                return EDirection.Left;
            }
            return direction;
        }

        /// <summary>
        /// 获取目标相对于自身的方向
        /// </summary>
        /// <param name="myself"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static EDirection GetTargetDirectionForMe(this Transform myself, Transform target)
        {
            EDirection direction = EDirection.Forward;
            Vector3 dir = target.position - myself.position;
            float angle = Mathf.Acos(Vector3.Dot(myself.forward.normalized, dir.normalized)) * Mathf.Rad2Deg;

            float dotForward = Vector3.Dot(myself.forward, dir.normalized);//点乘判断前后：dotForward >0在前，<0在后
            if (dotForward > 0 && angle <= 45f)
            {
                return EDirection.Forward;
            }
            else if (dotForward < 0 && angle > 135f)
            {
                return EDirection.Back;
            }

            float dotRight = Vector3.Dot(myself.right, dir.normalized);//点乘判断左右： dotRight>0在右，<0在左
            if (dotRight > 0 && angle <= 135f)
            {
                return EDirection.Right;
            }
            else if (dotRight < 0 && angle > 45f)
            {
                return EDirection.Left;
            }
            return direction;
        }

        /// <summary>
        /// 获取物体相对于自身正前方的角度
        /// </summary>
        /// <param name="myself"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float GetForMeAngle(this Transform myself, Transform target)
        {
            Vector3 dir = target.position - myself.position;
            return Vector3.Angle(myself.forward, dir);
        }

        /// <summary>
        /// 检查是否穿过Trigger，Exit Trigger 时调用，根据Trigger的前方向判断
        /// </summary>
        /// <param name="myself"></param>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public static bool CheckThroughTrigger(this Transform myself, Transform trigger)
        {
            Vector3 toMyselfDirection = myself.position - trigger.position;
            return ProjectPlaneAngle(toMyselfDirection, trigger.forward, Vector3.up) < 90f;
        }

        /// <summary>
        /// 检查是否穿过Trigger，Exit Trigger时调用，根据进入Trigger和退出时位置判断
        /// </summary>
        /// <param name="enterPos"></param>
        /// <param name="exitPos"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static bool CheckThroughTrigger(Vector3 enterPos, Vector3 exitPos, Vector3 center)
        {
            Vector3 dirEnter = enterPos - center;
            Vector3 dirExit = exitPos - center;
            return ProjectPlaneAngle(dirEnter, dirExit, Vector3.up) > 91f;
        }

        /// <summary>
        /// 获取2个向量投影在某一平面的夹角
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static float ProjectPlaneAngle(Vector3 dir1, Vector3 dir2, Vector3 plane)
        {
            Vector3 dirPlane1 = Vector3.ProjectOnPlane(dir1, plane);
            Vector3 dirPlane2 = Vector3.ProjectOnPlane(dir2, plane);
            float angle = Vector3.Angle(dirPlane1, dirPlane2);
            return angle;
        }

        /// <summary>
        /// Get the closest point on a line segment.
        /// </summary>
        /// <param name="p">A point in space</param>
        /// <param name="s0">Start of line segment</param>
        /// <param name="s1">End of line segment</param>
        /// <returns>The interpolation parameter representing the point on the segment, with 0==s0, and 1==s1</returns>
        public static float ClosestPointOnSegment(this Vector3 p, Vector3 s0, Vector3 s1)
        {
            Vector3 s = s1 - s0;
            float len2 = Vector3.SqrMagnitude(s);
            if (len2 < Epsilon)
                return 0; // degenrate segment
            return Mathf.Clamp01(Vector3.Dot(p - s0, s) / len2);
        }
    }
}