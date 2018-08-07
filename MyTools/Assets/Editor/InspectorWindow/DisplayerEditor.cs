using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Displayer))]
public class DisplayerEditor : Editor
{
    private Displayer obj;
    private void Awake()
    {
        obj = target as Displayer;
        if (obj.instanceA == null)
            obj.instanceA = new A();
        if (obj.instanceB == null)
            obj.instanceB = new B(); 
    }
     
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (obj.type == TestType.A)
        {
            EditorGUILayout.LabelField("A对象");
            obj.instanceA.a = EditorGUILayout.TagField(obj.instanceA.a);
        }
        else
        {
            EditorGUILayout.LabelField("B对象");
            obj.instanceB.b = EditorGUILayout.TextField(obj.instanceB.b);
        }
    }
}
