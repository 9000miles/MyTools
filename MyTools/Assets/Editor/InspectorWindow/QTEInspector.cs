using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QTEInfo))]
public class QTEInspector : Editor
{
    private bool isShow;
    private QTEInfo QTE;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        isShow = EditorGUILayout.Foldout(isShow, "Set QTE");
        if (isShow)
        {
            switch (QTE.type)
            {
                case QTEType.None:
                    break;
                case QTEType.QuickClick:
                    QTE.quickClick.clickCount = EditorGUILayout.IntField(QTE.quickClick.clickCount);
                    break;
                case QTEType.PreciseClick:
                    break;
                case QTEType.MouseGestures:
                    break;
                case QTEType.KeyCombination:
                    break;
                case QTEType.Others:
                    break;
                default:
                    break;
            }
        }
    }
}