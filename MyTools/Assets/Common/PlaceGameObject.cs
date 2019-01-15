using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsPC
{
    public class PlaceGameObject : MonoBehaviour
    {
        [Tooltip("True：法线偏移，False：Y方向偏移")]
        public bool isNormalDirection = true;
        [Tooltip("离地间隙")]
        public float groundClearance;
    }
}