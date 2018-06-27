using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChangeNameWindow : EditorWindow
{
    private static string firstName = "";
    private static string strNo = "0";
    private static GameObject[] selecteObject;

    /// <summary>
    /// 重命名
    /// </summary>
    private void ReName()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("基本名字：");
        firstName = GUILayout.TextField(firstName);

        GUILayout.Label("起始编号：");
        strNo = GUILayout.TextField(strNo);
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
                    selecteObject[i].name = firstName + number.ToString();
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