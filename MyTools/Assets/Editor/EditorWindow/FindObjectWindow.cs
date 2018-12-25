using Common;
using MarsPC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 添加查找组件的功能，可以将脚本拖入到框中，进行查找
/// </summary>
public class FindObjectWindow : EditorWindow
{
    private bool isAllMatch = false;
    private bool isIgnoreCase = true;
    private int pageIndex = 0;
    private int layerField = 0;
    private string tagField = "Untagged";
    private string inputText = "";
    private string tipStr = "";
    private string[] pageNames = new string[] { "根据名字（Name）查找", "根据标签（Tag）查找", "根据层级（Layer）查找" };
    private Vector2 scrollPos;
    private GUIStyle buttonStyle;
    private GUIStyle labelStyle;
    private RectOffset offset;
    private List<string> namePathList = new List<string>();
    private List<string> tagPathList = new List<string>();
    private List<string> layerPathList = new List<string>();

    private void OnGUI()
    {
        DrawPage();
        DisplayResult();
    }

    private void DrawPage()
    {
        pageIndex = GUILayout.Toolbar(pageIndex, pageNames);
        switch (pageIndex)
        {
            case 0:
                DrawInterface("请输入需要查找物体的名字（Name）：", pageIndex);
                break;

            case 1:
                DrawInterface("请选择需要查找的标签（Tag）：", pageIndex);
                break;

            case 2:
                DrawInterface("请选择需要查找的层级（Layer）：", pageIndex);
                break;
        }
    }

    private void DrawInterface(string label, int pageIndex)
    {
        titleContent = new GUIContent("查找物体");
        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        SetLableStyle(label.Length * 11f, TextAnchor.MiddleLeft, 2, 5);
        GUILayout.Label(label, labelStyle);
        switch (pageIndex)
        {
            case 0:
                inputText = GUILayout.TextField(inputText);
                break;

            case 1:
                tagField = EditorGUILayout.TagField(tagField);
                break;

            case 2:
                layerField = EditorGUILayout.LayerField(layerField);
                break;
        }
        EditorGUILayout.EndHorizontal();

        if (pageIndex == 0)
        {
            EditorGUILayout.BeginHorizontal();
            isAllMatch = GUILayout.Toggle(isAllMatch, "全字匹配");
            isIgnoreCase = GUILayout.Toggle(isIgnoreCase, "忽略大小写");
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(5);
        if (GUILayout.Button("查找"))
        {
            switch (pageIndex)
            {
                case 0:
                    if (inputText == "") return;
                    namePathList.Clear();
                    break;

                case 1:
                    tagPathList.Clear();
                    break;

                case 2:
                    layerPathList.Clear();
                    break;
            }

            GameObject[] gos = Selection.gameObjects;
            if (gos.Length > 0) //在选中的物体中进行查找
            {
                FindInSelection(gos, pageIndex);
            }
            else//如果没有选中，在整个Hierarchy面板中查找
            {
                FindInHierarchy(pageIndex);
            }
        }
    }

    /// <summary>
    /// 在选中的物体中查找
    /// </summary>
    /// <param name="gos"></param>
    /// <param name="pageIndex">当前页面索引</param>
    private void FindInSelection(GameObject[] gos, int pageIndex)
    {
        tipStr = "在选中的物体中";
        string[] allPaths = null;
        switch (pageIndex)
        {
            case 0:
                allPaths = gos.GetSelfAndChildrenPaths();
                if (isAllMatch == true)
                {
                    string[] matchPath = allPaths.FindAll((str) =>
                    {
                        int index = str.LastIndexOf('/');
                        str = str.Substring(index + 1);
                        bool result = isIgnoreCase ? str.ToLower().Equals(inputText.ToLower()) : str.Equals(inputText);
                        return result;
                    });
                    namePathList.AddRange(matchPath);
                }
                else
                {
                    string[] matchPath = allPaths.FindAll((str) =>
                    {
                        int index = str.LastIndexOf('/');
                        str = str.Substring(index + 1);
                        bool result = isIgnoreCase ? str.ToLower().Contains(inputText.ToLower()) : str.Contains(inputText);
                        return result;
                    });
                    namePathList.AddRange(matchPath);
                }
                break;

            case 1:
                allPaths = gos.FindAll((t) => t.tag == tagField).GetSelfPaths();
                tagPathList.AddRange(allPaths);
                break;

            case 2:
                allPaths = gos.FindAll((t) => t.layer == layerField).GetSelfPaths();
                layerPathList.AddRange(allPaths);
                break;
        }
    }

    private void FindInHierarchy(int pageIndex)
    {
        tipStr = "在整个Hierarchy面板中";
        List<GameObject> goList = new List<GameObject>();
        goList = GetGamaObjectsInHierarchy("All");
        string[] allPaths = null;
        switch (pageIndex)
        {
            case 0:
                allPaths = goList.GetSelfPaths();
                if (isAllMatch == true)
                {
                    string[] matchPath = allPaths.FindAll((str) =>
                    {
                        int index = str.LastIndexOf('/');
                        str = str.Substring(index + 1);
                        bool result = isIgnoreCase ? str.ToLower().Equals(inputText.ToLower()) : str.Equals(inputText);
                        return result;
                    });
                    namePathList.AddRange(matchPath);
                }
                else
                {
                    string[] matchPath = allPaths.FindAll((str) =>
                    {
                        int index = str.LastIndexOf('/');
                        str = str.Substring(index + 1);
                        bool result = isIgnoreCase ? str.ToLower().Contains(inputText.ToLower()) : str.Contains(inputText);
                        return result;
                    });
                    namePathList.AddRange(matchPath);
                }
                break;

            case 1:
                allPaths = goList.FindAll((t) => t.tag == tagField).GetSelfPaths();
                tagPathList.AddRange(allPaths);
                break;

            case 2:
                allPaths = goList.FindAll((t) => t.layer == layerField).GetSelfPaths();
                layerPathList.AddRange(allPaths);
                break;
        }
    }

    private void DisplayResult()
    {
        List<string> result = new List<string>();
        switch (pageIndex)
        {
            //根据名字查找
            case 0:
                result = namePathList;
                break;
            //根据Tag查找
            case 1:
                result = tagPathList;
                break;
            //根据Layer查找
            case 2:
                result = layerPathList;
                break;
        }
        SetLableStyle(50, TextAnchor.MiddleRight, 2, 5);
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("[  {0}  ] 查找到的结果如下（单击显示查看位置）：", result.Count > 0 ? tipStr : ""));
        GUILayout.Label(string.Format("[ 共找到  {0}  个 ]", result.Count), labelStyle);
        EditorGUILayout.EndHorizontal();

        SetButtonStyle();
        SetLableStyle(position.width - 80, TextAnchor.MiddleLeft, 8, 10);

        //绘制查找到的结果按钮
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true));
        foreach (var item in result)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.TextArea(item, labelStyle);
            //EditorGUILayout.SelectableLabel(item/*, labelStyle*/);
            if (GUILayout.Button(new GUIContent("显示")))
            {
                List<GameObject> goList = new List<GameObject>();
                goList = GetGamaObjectsInHierarchy("All");
                GameObject go = goList.Find((t) => t.GetSelfPath() == item);
                Selection.activeGameObject = go;
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    /// <summary>
    /// 用于获取所有Hierarchy中的物体，包括被禁用的物体
    /// </summary>
    /// <param name="range">请输入"All"、"Active"或者"UnActive"</param>
    /// <returns></returns>
    private List<GameObject> GetGamaObjectsInHierarchy(string range)
    {
        var allTransforms = Resources.FindObjectsOfTypeAll(typeof(Transform));
        var previousSelection = Selection.objects;
        switch (range)
        {
            //获取在Hierarchy中所有的物体（包含禁用）"All"
            case "All":
                Selection.objects = allTransforms.Cast<Transform>().Where(x => x != null)
               .Select(x => x.gameObject)
               .Where(x => x != null || !x.activeInHierarchy)
               .Cast<UnityEngine.Object>().ToArray();
                break;

            //获取在Hierarchy中所有激活的物体（不包含禁用）"Active"
            case "Active":
                Selection.objects = allTransforms.Cast<Transform>().Where(x => x != null)
                    .Select(x => x.gameObject)
                    .Where(x => x != null && x.activeInHierarchy)
                    .Cast<UnityEngine.Object>().ToArray();
                break;

            //获取在Hierarchy中所有被禁用的物体（只包含禁用）  "UnActive"
            case "UnActive":
                Selection.objects = allTransforms.Cast<Transform>().Where(x => x != null)
                    .Select(x => x.gameObject)
                    .Where(x => x != null && !x.activeInHierarchy)
                    .Cast<UnityEngine.Object>().ToArray();
                break;

            default:
                Debug.LogError("type参数错误，请输入All 、Active或者UnActive");
                return null;
        }

        var selectedTransforms = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
        Selection.objects = previousSelection;
        return selectedTransforms.Select(tr => tr.gameObject).ToList();
    }

    private void SetButtonStyle()
    {
        buttonStyle = new GUIStyle();
        buttonStyle.fixedWidth = 50;
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        offset = new RectOffset();
        offset.top = 2;
        offset.right = 2;
        buttonStyle.padding = offset;
        buttonStyle.normal.background = null;
        buttonStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
        buttonStyle.onActive = new GUIStyleState();
    }

    private void SetLableStyle(float width, TextAnchor anchor, int top, int left)
    {
        labelStyle = new GUIStyle();
        labelStyle.fixedWidth = width;
        labelStyle.alignment = anchor;
        offset = new RectOffset();
        offset.top = top;
        offset.left = left;
        offset.right = 10;
        labelStyle.padding = offset;
        labelStyle.normal.background = null;
        labelStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
    }
}