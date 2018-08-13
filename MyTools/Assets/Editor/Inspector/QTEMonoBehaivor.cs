using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 不行，没有效果
/// </summary>
[CustomEditor(typeof(QTEInfo))]
public class QTEInfoEditor : Editor
{
    private SerializedProperty isActive;
    private SerializedProperty type;

    private void OnEnable()
    {
        isActive = serializedObject.FindProperty("isActive");
        type = serializedObject.FindProperty("type");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        if (type.enumValueIndex == (int)QTEType.Others)
            EditorGUILayout.PropertyField(isActive);
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(QTECollisionTrigger))]
public class QTEMonoBehaivor : Editor
{
    private SerializedProperty angleLimit;
    private SerializedProperty augularOffset;
    private SerializedProperty mouseButton;
    private SerializedProperty gesturesType;
    private SerializedProperty result;
    private QTECollisionTrigger conditionBase;

    private void Awake()
    {
        conditionBase = target as QTECollisionTrigger;
        //if (conditionBase.infoList.quickClick == null)
        //{
        //    conditionBase.infoList.quickClick = new QuickClickInfo();
        //}
    }

    private void OnEnable()
    {
        angleLimit = serializedObject.FindProperty("angleLimit");
        augularOffset = serializedObject.FindProperty("augularOffset");
        mouseButton = serializedObject.FindProperty("mouseButton");
        gesturesType = serializedObject.FindProperty("gesturesType");
        result = serializedObject.FindProperty("isTrue");
    }

    private bool isOpen = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        //switch (conditionBase.infoList.type)
        //{
        //    case QTEType.None:
        //        break;
        //    case QTEType.QuickClick:
        //        isOpen = EditorGUILayout.Foldout(isOpen, "QuickClick");
        //        if (isOpen)
        //        {
        //            EditorGUILayout.BeginVertical("box");
        //            conditionBase.infoList.quickClick.clickCount = EditorGUILayout.IntField("clickCount", conditionBase.infoList.quickClick.clickCount);
        //            conditionBase.infoList.quickClick.IntervalTime = EditorGUILayout.FloatField("IntervalTime", conditionBase.infoList.quickClick.IntervalTime);
        //            conditionBase.infoList.quickClick.mouseButton = (QTEMouseButton)EditorGUILayout.EnumPopup("mouseButton", conditionBase.infoList.quickClick.mouseButton);
        //            EditorGUILayout.EndVertical();
        //        }
        //        break;
        //    case QTEType.PreciseClick:
        //        break;
        //    case QTEType.MouseGestures:
        //        //EditorGUILayout.PropertyField(angleLimit);
        //        //EditorGUILayout.PropertyField(augularOffset);
        //        //EditorGUILayout.PropertyField(mouseButton);
        //        //EditorGUILayout.PropertyField(gesturesType);
        //        //EditorGUILayout.PropertyField(result);
        //        break;
        //    case QTEType.KeyCombination:
        //        break;
        //    case QTEType.Others:
        //        break;
        //    default:
        //        break;
        //}
        //serializedObject.ApplyModifiedProperties();
    }
}