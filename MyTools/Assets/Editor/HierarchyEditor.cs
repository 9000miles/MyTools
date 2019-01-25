using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[UnityEditor.InitializeOnLoad]
public class HierarchyEditor
{
    /// <summary> 存储的容量大小 </summary>
    private static string count = "10";

    /// <summary> 选中的物体集合 </summary>
    private static List<GameObject> objectList = new List<GameObject>();

    /// <summary> 选中的物体 </summary>
    private static GameObject selecte;

    static HierarchyEditor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyChange;
    }

    private static void HierarchyChange(int instanceID, Rect selectionRect)
    {
        selecte = Selection.activeGameObject;
        objectList.RemoveAll(t => t == null);
        AddGameObject();
    }

    /// <summary>
    /// 往集合中添加元素，如果没有则添加，如果数量满了则移除第一个
    /// </summary>
    /// <param name="go"></param>
    private static void AddGameObject()
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
    [MenuItem("MyTools/Selected Previou Object #W", false, 200)]
    public static void SelectedPreviouObject()
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
    [MenuItem("MyTools/Selected Next Object #S", false, 201)]
    public static void SelectedNextObject()
    {
        int index = objectList.FindIndex((t) => t == selecte);
        if (index < objectList.Count - 1)
        {
            selecte = objectList[index + 1];
        }
        Selection.activeGameObject = selecte;
    }

    /// <summary>
    /// 清空列表
    /// </summary>
    [MenuItem("MyTools/Clear Selected Object List #C", false, 202)]
    public static void ClearList()
    {
        objectList.Clear();
    }
}