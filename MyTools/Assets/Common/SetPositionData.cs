using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Position Data")]
public class SetPositionData : ScriptableObject
{
    public PositionData[] positionDatas;

    [System.Serializable]
    public class PositionData
    {
        [HideInInspector]
        public bool isFoldout;

        public string userName;
        public Data[] data;
    }

    [System.Serializable]
    public class Data
    {
        public string placename;
        public string position;
    }
}