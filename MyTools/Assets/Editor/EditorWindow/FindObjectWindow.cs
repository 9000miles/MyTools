using Common;
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
    private static string inputText = "";
    private static bool isAllMatch;
    private static bool isIgnoreCase = true;
    private static bool isByName;
    private static bool isByTag;
    private static bool isByLayer;
    private static bool isToggleGroup;
    private Vector2 scrollPos;
    private Vector2 size;
    private GUIStyle buttonStyle;
    private GUIStyle labelStyle;
    private RectOffset offset;
    private List<string> pathList = new List<string>();
    private string tipStr = "";

    private void OnGUI()
    {
        titleContent = new GUIContent("查找物体");
        GUILayout.Space(5);
        GUILayout.Label("请输入需要查找物体的名字：");
        inputText = GUILayout.TextField(inputText);

        EditorGUILayout.BeginHorizontal();
        isAllMatch = GUILayout.Toggle(isAllMatch, "全字匹配");
        isIgnoreCase = GUILayout.Toggle(isIgnoreCase, "忽略大小写");
        EditorGUILayout.EndHorizontal();

        isToggleGroup = EditorGUILayout.BeginToggleGroup("根据什么查找", isToggleGroup);
        isByName = GUILayout.Toggle(isByName, "根据名字查找");
        isByTag = GUILayout.Toggle(isByTag, "根据标签查找");
        isByLayer = GUILayout.Toggle(isByLayer, "根据层查找");
        EditorGUILayout.EndToggleGroup();

        if (GUILayout.Button("查找"))
        {
            if (inputText == "") return;
            pathList.Clear();
            GameObject[] gos = Selection.gameObjects;
            string findWay = null;
            if (isByName == true)
                findWay = "ByName_" + inputText;
            if (isByTag == true)
                findWay = "ByTag_" + inputText;
            if (isByLayer == true)
                findWay = "ByLayer" + inputText;

            //在选中的物体中进行查找
            if (gos.Length > 0)
            {
                FindInSelection(gos, findWay);
            }
            //如果没有选中，在整个Hierarchy面板中查找
            else
            {
                FindInHierarchy(findWay);
            }
        }

        SetLableStyle(50, TextAnchor.MiddleRight, 2, 5);
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("[  {0}  ] 查找到的结果如下（单击显示查看位置）：", tipStr));
        GUILayout.Label(string.Format("[ 共找到  {0}  个 ]", pathList.Count), labelStyle);
        EditorGUILayout.EndHorizontal();
        SetButtonStyle();
        SetLableStyle(position.width - 80, TextAnchor.MiddleLeft, 8, 10);
        //绘制查找到的结果按钮
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true));
        foreach (var item in pathList)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.TextArea(item, labelStyle);
            if (GUILayout.Button(new GUIContent("显示")))
            //if (GUILayout.Button("显示", buttonStyle))
            {
                List<GameObject> goList = new List<GameObject>();
                goList = GetGamaObjectInHierarchy("All");
                GameObject go = goList.Find((t) => t.GetSelfPath() == item);
                Selection.activeGameObject = go;
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="gos"></param>
    /// <param name="findWay">ByTag_Player</param>
    private void FindInSelection(GameObject[] gos, string findWay)
    {
        tipStr = "在选中的物体中";
        string way = findWay.Substring(0, findWay.IndexOf('_'));
        string text = findWay.Substring(findWay.IndexOf('_') + 1);
        string[] allPaths = null;
        switch (way)
        {
            case "ByName":
                allPaths = gos.GetSelfAndChildrenPaths();
                break;

            case "ByTag":
                allPaths = gos.FindAll((t) =>
                {
                    bool result = false;
                    result = isIgnoreCase ? t.tag.ToLower() == text.ToLower() : t.tag == text;
                    return result;
                }).ToArray().GetSelfAndChildrenPaths();
                break;

            case "ByLayer":
                allPaths = gos.FindAll((t) => t.layer == LayerMask.NameToLayer(text)).GetSelfAndChildrenPaths();
                break;
        }
        if (isAllMatch == true)
        {
            string[] matchPath = allPaths.FindAll((str) =>
            {
                int index = str.LastIndexOf('/');
                str = str.Substring(index + 1);
                bool result = isIgnoreCase ? str.ToLower().Equals(inputText.ToLower()) : str.Equals(inputText);
                return result;
            });
            pathList.AddRange(matchPath);
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
            pathList.AddRange(matchPath);
        }
    }

    private void FindInHierarchy(string findWay)
    {
        tipStr = "在整个Hierarchy面板中";
        string way = findWay.Substring(0, findWay.IndexOf('_'));
        string text = findWay.Substring(findWay.IndexOf('_') + 1);
        List<GameObject> goList = new List<GameObject>();
        goList = GetGamaObjectInHierarchy("All");
        string[] allPaths = null;
        switch (way)
        {
            case "ByName":
                allPaths = goList.GetSelfPaths();
                break;

            case "ByTag":
                allPaths = goList.FindAll((t) =>
                {
                    bool result = false;
                    result = isIgnoreCase ? t.tag.ToLower() == text.ToLower() : t.tag == text;
                    return result;
                }).ToArray().GetSelfAndChildrenPaths();
                break;

            case "ByLayer":
                //allPaths = goList.FindAll((t) =>
                //{
                //    bool result = false;
                //    result = isIgnoreCase ? t.layer == LayerMask.NameToLayer(text) : t.layer == LayerMask.NameToLayer(text);
                //    return result;
                //}).ToArray().GetSelfAndChildrenPaths();
                break;
        }

        if (isAllMatch == true)
        {
            string[] matchPath = allPaths.FindAll((str) =>
            {
                int index = str.LastIndexOf('/');
                str = str.Substring(index + 1);
                bool result = isIgnoreCase ? str.ToLower().Equals(inputText.ToLower()) : str.Equals(inputText);
                return result;
            });
            pathList.AddRange(matchPath);
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
            pathList.AddRange(matchPath);
        }
    }

    /// <summary>
    /// 用于获取所有Hierarchy中的物体，包括被禁用的物体
    /// </summary>
    /// <param name="type">请输入"All"、"Active"或者"UnActive"</param>
    /// <returns></returns>
    private List<GameObject> GetGamaObjectInHierarchy(string type)
    {
        var allTransforms = Resources.FindObjectsOfTypeAll(typeof(Transform));
        var previousSelection = Selection.objects;
        switch (type)
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