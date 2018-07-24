using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SelectedGameObjectWindow : EditorWindow
{
    /// <summary> 存储的容量大小 </summary>
    private static string count = "10";

    /// <summary> 选中的物体集合 </summary>
    private static List<GameObject> objectList = new List<GameObject>();

    /// <summary> 选中的物体 </summary>
    private GameObject selecte;

    private void OnGUI()
    {
        ShowWindow();
    }

    private void OnSelectionChange()
    {
        selecte = Selection.activeGameObject;
        AddGameObject();
    }

    /// <summary>
    /// 往集合中添加元素，如果没有则添加，如果数量满了则移除第一个
    /// </summary>
    /// <param name="go"></param>
    private void AddGameObject()
    {
        if (selecte != null && objectList.Contains(selecte) == false)
        {
            int num = 0;
            if (string.IsNullOrEmpty(count) || string.IsNullOrWhiteSpace(count))
                return;
            if (int.TryParse(count, out num))
            {
                if (objectList.Count >= num)
                {
                    int n = objectList.Count - num;
                    for (int i = 0; i <= n; i++)
                    {
                        objectList.RemoveAt(0);
                    }
                }
                objectList.Add(selecte);
            }
            else
                Debug.LogError("-----  存储容量必须输入整数  -----");
        }
    }

    /// <summary>
    /// 选中前一个物体
    /// </summary>
    private void SelectedPreviouObject()
    {
        int index = objectList.FindIndex((t) => t == selecte);
        if (index > 0)
        {
            selecte = objectList[index - 1];
        }
        Selection.activeGameObject = selecte;
    }

    /// <summary>
    /// 选中后一个物体
    /// </summary>
    private void SelectedNextObject()
    {
        int index = objectList.FindIndex((t) => t == selecte);
        if (index < objectList.Count - 1)
        {
            selecte = objectList[index + 1];
        }
        Selection.activeGameObject = selecte;
    }

    /// <summary>
    /// 窗口显示图元
    /// </summary>
    private void ShowWindow()
    {
        titleContent = new GUIContent("选择物体历史记录窗口");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(8);
        GUIStyle style = new GUIStyle();
        style.fixedWidth = 55;
        RectOffset offset = new RectOffset();
        offset.top = 2;
        style.padding = offset;
        style.normal.background = null;
        style.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
        GUILayout.Label("存储容量：", style);
        count = GUILayout.TextField(count);
        GUILayout.Space(5);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("前一个物体"))
        {
            SelectedPreviouObject();
        }
        if (GUILayout.Button("后一个物体"))
        {
            SelectedNextObject();
        }
    }
}