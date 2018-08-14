using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

///// <summary>
///// 不行，没有效果
///// </summary>
//[CustomEditor(typeof(QTEInfo))]
//public class QTECoillisionTriggerInspector : Editor
//{
//    private SerializedProperty isActive;
//    private SerializedProperty type;

//    private void OnEnable()
//    {
//        isActive = serializedObject.FindProperty("isActive");
//        type = serializedObject.FindProperty("type");
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        serializedObject.Update();
//        if (type.enumValueIndex == (int)QTEType.Others)
//            EditorGUILayout.PropertyField(isActive);
//        serializedObject.ApplyModifiedProperties();
//    }
//}

[CustomEditor(typeof(QTECollisionTrigger))]
public class QTECoillisionTriggerInspector : Editor
{
    private SerializedProperty angleLimit;
    private SerializedProperty augularOffset;
    private SerializedProperty mouseButton;
    private SerializedProperty gesturesType;
    private SerializedProperty result;
    private SerializedProperty quickClick;
    private SerializedProperty preciseClick;
    private SerializedProperty mouseGestures;
    private SerializedProperty keyCombination;
    private QTECollisionTrigger condition;

    private void Awake()
    {
        condition = target as QTECollisionTrigger;
    }

    private void OnEnable()
    {
        quickClick = serializedObject.FindProperty("info.quickClick");
        preciseClick = serializedObject.FindProperty("info.preciseClick");
        mouseGestures = serializedObject.FindProperty("info.mouseGestures");
        keyCombination = serializedObject.FindProperty("info.keyCombination");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();
        condition.info.type = (QTEType)EditorGUILayout.EnumPopup("Type", condition.info.type);
        switch (condition.info.type)
        {
            case QTEType.None:
                break;
            case QTEType.QuickClick:
                EditorGUILayout.PropertyField(quickClick, true);
                //EditorGUILayout.BeginVertical("box");
                //condition.info.quickClick.clickCount = EditorGUILayout.IntField("Click Count", condition.info.quickClick.clickCount);
                //condition.info.quickClick.speed = EditorGUILayout.FloatField("Speed", condition.info.quickClick.speed);
                //condition.info.quickClick.mouseButton = (QTEMouseButton)EditorGUILayout.EnumPopup("Mouse Button", condition.info.quickClick.mouseButton);
                //EditorGUILayout.EndVertical();
                break;
            case QTEType.PreciseClick:
                //EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(preciseClick, true);

                //SerializedProperty property = serializedObject.FindProperty("info.preciseClick.targetList");
                //condition.info.preciseClick.preciseClickType = (PreciseClickInfo.PreciseClickType)EditorGUILayout.EnumPopup("Precise Click Type", condition.info.preciseClick.preciseClickType);
                //condition.info.preciseClick.percentage = EditorGUILayout.FloatField("Percentage", condition.info.preciseClick.percentage);
                //condition.info.preciseClick.rotateDelta = EditorGUILayout.FloatField("Rotate Delta", condition.info.preciseClick.rotateDelta);
                ////EditorGUILayout.PropertyField(property, true);
                ////condition.info.preciseClick.targetList = EditorGUILayout.PropertyField(property, new List<GameObject>());
                //condition.info.preciseClick.mouseButton = (QTEMouseButton)EditorGUILayout.EnumPopup("Mouse Button", condition.info.preciseClick.mouseButton);
                //EditorGUILayout.EndVertical();
                break;
            case QTEType.MouseGestures:
                EditorGUILayout.PropertyField(mouseGestures, true);
                //EditorGUILayout.PropertyField(angleLimit);
                //EditorGUILayout.PropertyField(augularOffset);
                //EditorGUILayout.PropertyField(mouseButton);
                //EditorGUILayout.PropertyField(gesturesType);
                //EditorGUILayout.PropertyField(result);
                break;
            case QTEType.KeyCombination:
                EditorGUILayout.PropertyField(keyCombination, true);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}