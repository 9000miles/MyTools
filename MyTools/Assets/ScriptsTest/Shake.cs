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
        void Start()
        {
            transform.DOShakePosition(1);
        }
    }
}