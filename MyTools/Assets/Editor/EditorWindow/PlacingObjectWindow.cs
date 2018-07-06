using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Common;

/// <summary>
/// 按照间距排列物体，可重置为初始位置，可快速将三维向量数值归零
/// </summary>
public class PlacingObjectWindow : EditorWindow
{
    private static Vector3 spaceV;
    private GameObject[] selecteObject;

    private void OnGUI()
    {
        PlacingObject();
    }

    private void PlacingObject()
    {
        EditorGUILayout.BeginHorizontal();
        spaceV = EditorGUILayout.Vector3Field("间距：", spaceV);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("开始排列"))
        {
            selecteObject = Selection.gameObjects;
            if (selecteObject.Length <= 0) return;
            selecteObject = selecteObject.QuickSortAscending(t => t.transform.GetSiblingIndex());
            GameObject minSibling = selecteObject.GetMin(t => t.transform.GetSiblingIndex());
            for (int i = 1; i < selecteObject.Length; i++)
            {
                if (minSibling != selecteObject[i])
                {
                    selecteObject[i].transform.position = selecteObject[i - 1].transform.position + spaceV;
                }
            }
        }
        if (GUILayout.Button("重置位置"))
        {
            selecteObject = Selection.gameObjects;
            if (selecteObject.Length <= 0) return;
            selecteObject = selecteObject.QuickSortAscending(t => t.transform.GetSiblingIndex());
            GameObject minSibling = selecteObject.GetMin(t => t.transform.GetSiblingIndex());
            for (int i = 0; i < selecteObject.Length; i++)
            {
                selecteObject[i].transform.position = minSibling.transform.position;
            }
        }
        if (GUILayout.Button("数值归零"))
        {
            spaceV = Vector3.zero;
            spaceV = EditorGUILayout.Vector3Field("", spaceV);
        }
        EditorGUILayout.EndHorizontal();
    }

    public bool StringToInt(string str, out int num)
    {
        if (!int.TryParse(str, out num))
        {
            Debug.LogError("输入不合法，请检查");
            return true;
        }
        return false;
    }
}