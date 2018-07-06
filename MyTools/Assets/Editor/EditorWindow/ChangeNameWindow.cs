using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChangeNameWindow : EditorWindow
{
    private static string firstName = "";
    private static string strNo = "0";
    private static string strSuffix = "";
    private static GameObject[] selecteObject;

    /// <summary>
    /// 重命名
    /// </summary>
    private void ReName()
    {
        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(5);
        GUIStyle style = new GUIStyle();
        style.fixedWidth = 55;
        RectOffset offset = new RectOffset();
        offset.top = 2;
        style.padding = offset;
        style.normal.background = null;
        style.normal.textColor = new Color(0.7f, 0.7f, 0.7f);

        GUILayout.Label("基本名字：", style);
        firstName = GUILayout.TextField(firstName);

        GUILayout.Space(10);
        GUILayout.Label("起始编号：", style);
        strNo = GUILayout.TextField(strNo);

        GUILayout.Space(10);
        style.fixedWidth = 30;
        GUILayout.Label("后缀：", style);
        strSuffix = GUILayout.TextField(strSuffix);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("改名"))
        {
            int number;
            if (int.TryParse(strNo, out number))
            {
                int firstIndex = 0;
                selecteObject = Selection.gameObjects;
                if (selecteObject.Length <= 0) return;
                firstIndex = selecteObject[0].transform.GetSiblingIndex();
                for (int i = 0; i < selecteObject.Length; i++)
                {
                    selecteObject[i].name = firstName + number.ToString() + strSuffix;
                    selecteObject[i].transform.SetSiblingIndex(firstIndex + i);
                    number++;
                }
            }
        }
    }

    private void OnGUI()
    {
        ReName();
    }
}