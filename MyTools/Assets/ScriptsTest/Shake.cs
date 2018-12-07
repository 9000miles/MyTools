using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace YouLe
{
    ///  <summary>
    ///
    ///  </summary>
    public class Shake : MonoBehaviour
    {
        public Transform A;
        public Transform B;
        public Transform C;

        private void Start()
        {
            transform.DOShakePosition(1);
        }

        private void Update()
        {
            Vector3 dir = A.position - B.position;
            Debug.DrawRay(A.position, -dir);
            Vector3 corssDir = Vector3.Cross(dir, Vector3.up);
            Debug.DrawRay(A.position, corssDir);
            C.rotation = Quaternion.LookRotation(corssDir);
        }
    }
}