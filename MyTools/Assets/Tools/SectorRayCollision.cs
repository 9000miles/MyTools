using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTools
{
    public class SectorRayCollision : MonoBehaviour
    {
        public string myLayerName;
        public float length;
        public float angle;
        public int number;
        private bool mIsGetTarget;
        private Transform lastTarget;
        private Transform targetRaycast;
        private Vector3 lastPosition;
        private bool isToLeftMove;

        private void Update()
        {
            if (transform.position.x < lastPosition.x)
                isToLeftMove = true;
            else if (transform.position.x > lastPosition.x)
                isToLeftMove = false;
            mIsGetTarget = SectorRayCollison(transform, transform.forward, length, angle, number,
                1 << LayerMask.NameToLayer(myLayerName), lastTarget, out targetRaycast, isToLeftMove);
            lastPosition = transform.position;
            lastTarget = targetRaycast;
        }

        /// <summary>
        /// 扇形碰撞
        /// </summary>
        /// <param name="myLayerName">指定层级</param>
        /// <param name="length">射线长度</param>
        /// <param name="angle">射线角度</param>
        /// <param name="number">射线数量</param>
        /// <param name="hitPoint">碰撞点位置</param>
        private bool SectorRayCollison(Transform origin, Vector3 direction, float length, float totalAngle, int number,
            LayerMask layerMask, Transform lastTransform, out Transform target, bool isToLeftMove /*,Vector3 plan, Vector3 direction = Vector3.forward*/)
        {
            float angle = totalAngle / (number - 1);
            bool isGetTarget = false;
            RaycastHit hit;
            //旋转初始角度
            direction = isToLeftMove ? Quaternion.Euler(new Vector3(0, totalAngle / 2, 0)) * direction : Quaternion.Euler(new Vector3(0, -totalAngle / 2, 0)) * direction;
            //循环所有的射线
            for (int i = 0; i < number; i++)
            {
                isGetTarget = Physics.Raycast(origin.position, direction, out hit, length, layerMask);
                Debug.DrawRay(origin.position, direction * length, Color.yellow);//绘制射线
                Debug.DrawLine(origin.position, hit.point, Color.black);//绘制绘制碰撞点连线
                if (lastTransform != null)
                    lastTransform.GetComponent<Renderer>().material.color = Color.white;
                if (isGetTarget)
                {
                    if (Vector3.Distance(origin.position, hit.point) <= length)//如果在一定范围内则执行
                    {
                        hit.transform.GetComponent<Renderer>().material.color = Color.red;
                        lastTransform = hit.transform;//碰撞到的物体
                    }
                    target = hit.transform;//碰撞到的物体
                    return isGetTarget; //如果碰撞到，则退出循环，减少射线次数
                }
                direction = isToLeftMove ? Quaternion.Euler(new Vector3(0, -angle, 0)) * direction : Quaternion.Euler(new Vector3(0, angle, 0)) * direction;//旋转到下一个角度
            }
            target = null;
            return isGetTarget;
        }
    }
}