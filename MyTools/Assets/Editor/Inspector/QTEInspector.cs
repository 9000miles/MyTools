using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//显示不正常
[CustomEditor(typeof(QTEInfo))]
public class QTEInspector : Editor
{
    private bool isShow;
    private SerializedObject QTE;
    private SerializedProperty qteType;
    private SerializedProperty quickClick;
    private SerializedProperty preciseClick;
    private SerializedProperty mouseGestures;
    private SerializedProperty keyCombination;

    private void OnEnable()
    {
        QTE = new SerializedObject(target);
        qteType = QTE.FindProperty("type");//获取m_type
        quickClick = QTE.FindProperty("quickClick");//获取a_int
        preciseClick = QTE.FindProperty("preciseClick");//获取b_int
        mouseGestures = QTE.FindProperty("mouseGestures");//获取b_int
        keyCombination = QTE.FindProperty("keyCombination");//获取b_int
    }

    //public override void OnInspectorGUI()
    //{
    //    test.Update();//更新test
    //    EditorGUILayout.PropertyField(m_type);
    //    if (m_type.enumValueIndex == 0)
    //    {//当选择第一个枚举类型
    //        EditorGUILayout.PropertyField(a_int);
    //    }
    //    else if (m_type.enumValueIndex == 1)
    //    {
    //        EditorGUILayout.PropertyField(b_int);
    //    }
    //    //serializedObject.ApplyModifiedProperties();
    //    test.ApplyModifiedProperties();//应用
    //}

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        QTE.Update();
        int index = 0;
        //index = EditorGUILayout.Popup(index, new string[] { "quickClick", "preciseClick" });
        //switch (index)
        //{
        //    case 0:
        //        Debug.Log("55555");
        //        break;

        //    case 1:
        //        Debug.Log("6666666666");
        //        break;

        //    default:
        //        break;
        //}
        EditorGUILayout.PropertyField(qteType);
        switch (qteType.enumValueIndex)
        {
            case 0:
                break;

            case 1:
                Debug.Log("55555");
                EditorGUILayout.PropertyField(quickClick);
                break;

            case 2:
                Debug.Log("6666666666");
                EditorGUILayout.PropertyField(preciseClick);
                break;

            case 3:
                EditorGUILayout.PropertyField(mouseGestures);
                break;

            case 4:
                EditorGUILayout.PropertyField(keyCombination);
                break;

            case 999:
                break;
        }
        QTE.ApplyModifiedProperties();
    }
}