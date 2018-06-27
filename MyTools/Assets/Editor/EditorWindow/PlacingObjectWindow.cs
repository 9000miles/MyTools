using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Common;

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
        //if (GUILayout.Button("归零"))
        //{
        //    spaceV = Vector3.zero;
        //    EditorGUILayout .Vector3Field()
        //}
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