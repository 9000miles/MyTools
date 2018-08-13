using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OdinInspectorTest))]
public class OdinInspectorTestInspector : Editor
{
    //private OdinInspectorTest odin
    //{ get { return target as OdinInspectorTest; } }
    //public override void OnInspectorGUI()
    //{
    //    base.OnInspectorGUI();
    //    odin.EnemyName = EditorGUILayout.TextField("EnemyName", odin.EnemyName, EditorStyles.boldLabel);
    //}
}