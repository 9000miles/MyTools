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
    private GUIStyle style;
    private RectOffset offset;
    private List<string> names = new List<string>();
    private List<string> oldNames = new List<string>();
    private bool isSetOldNames = false;

    private void SetStyle()
    {
        style = new GUIStyle();
        style.fixedWidth = 55;
        offset = new RectOffset();
        offset.top = 2;
        style.padding = offset;
        style.normal.background = null;
        style.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
    }

    /// <summary>
    /// 重命名
    /// </summary>
    private void ReName()
    {
        GUILayout.Space(5);
        GUILayout.Label("在基本名字后面添加编号或者后缀", style);
        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(5);
        //GUIStyle style = new GUIStyle();
        //style.fixedWidth = 55;
        //RectOffset offset = new RectOffset();
        //offset.top = 2;
        //style.padding = offset;
        //style.normal.background = null;
        //style.normal.textColor = new Color(0.7f, 0.7f, 0.7f);

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

    /// <summary>
    /// 只在后面添加后缀
    /// </summary>
    private void OnlyAddSuffix()
    {
        GUILayout.Space(10);
        GUILayout.Label("只添加后缀", style);
        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(5);
        style.fixedWidth = 30;
        GUILayout.Label("后缀：", style);
        strSuffix = GUILayout.TextField(strSuffix);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加后缀"))
        {
            names.Clear();
            //if (selecteObject != null)
            //{
            //    foreach (var item in Selection.gameObjects)
            //    {
            //        for (int i = 0; i < selecteObject.Length; i++)
            //        {
            //            if (item != selecteObject[i])
            //                oldNames.Clear();
            //        }
            //    }
            //}
            selecteObject = Selection.gameObjects;
            if (selecteObject.Length <= 0) return;
            for (int i = 0; i < selecteObject.Length; i++)
            {
                names.Add(selecteObject[i].name);
                if (isSetOldNames == false)
                {
                    oldNames.Add(selecteObject[i].name);
                }
                selecteObject[i].name += strSuffix;
            }
            isSetOldNames = true;
        }

        if (GUILayout.Button("撤销添加后缀"))
        {
            if (names.Count <= 0 || selecteObject.Length <= 0) return;
            for (int i = 0; i < selecteObject.Length; i++)
            {
                selecteObject[i].name = names[i];
            }
            names.Clear();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnGUI()
    {
        SetStyle();
        ReName();
        OnlyAddSuffix();
    }

    //if (GUILayout.Button("恢复原始名字"))
    //{
    //    if (oldNames.Count <= 0 || selecteObject.Length <= 0) return;
    //    for (int i = 0; i < selecteObject.Length; i++)
    //    {
    //        selecteObject[i].name = oldNames[i];
    //    }
    //}
}