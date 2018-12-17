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
        public string placename;
        public string position;
    }
}