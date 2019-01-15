using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestAsset))]
public class TestAssetEditor : Editor
{
    private TestAsset mBehaviour { get { return (target as TestAsset); } }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("点击"))
        {
            Debug.Log("45665645");
        }
    }
}