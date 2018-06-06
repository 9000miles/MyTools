/*编辑器常用小功能整合
 * By:xiongjunyu
 *QQ:506994768@qq.com
 * 编辑器快捷键：%ctrl #shift &Alt _无
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class QuickEditor : EditorWindow
{
    public static int No = 0;
    public static int ChildNo = 0;
    public static string setNumber = string.Empty;
    public static string firstName = "A";
    public static string endName = "1";

    #region 快捷操作

    [MenuItem("Quick/快捷操作/物体激活关闭 %w")]
    private static void ToggleActiveState()
    {
        GameObject[] select = Selection.gameObjects;
        foreach (GameObject item in select)
        {
            item.SetActive(!item.activeSelf);
        }
    }

    [MenuItem("Quick/快捷操作/只激活当前选择(在同一个父物体下,如果父物体下所有物体已禁用则激活所有物体) %e")]
    private static void ToggleActiveStateOnChild()
    {
        GameObject select = Selection.activeGameObject;

        if (isAllNotActive(select.transform.parent))
        {
            foreach (Transform item in select.transform.parent)
            {
                item.gameObject.SetActive(true);
            }
            return;
        }

        select.SetActive(true);
        foreach (Transform item in select.transform.parent)
        {
            if (!item.gameObject.Equals(select))
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    private static bool isAllNotActive(Transform tf)
    {
        foreach (Transform item in tf)
        {
            if (item.gameObject.activeSelf) return false;
        }
        return true;
    }

    #endregion 快捷操作

    private static void SelectByTag()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag(tag);
        Selection.objects = gos;
    }

    [MenuItem("Quick/改名/手动递增改名 %q")]
    private static void ChangeNameOneByOne()
    {
        Selection.activeGameObject.name = firstName + No.ToString();
        No++;
    }

    private static void ChangeChidrensEndName()
    {
        ChangeChidrensEndNameFun(Selection.activeGameObject.transform);
    }

    private static void RemoveChidrensEndName()
    {
        RemoveChidrensEndNameFun(Selection.activeGameObject.transform);
    }

    private static void ChangeChidrensEndNameFun(Transform t)
    {
        foreach (Transform item in t)
        {
            item.name = item.name + endName;
            ChangeChidrensEndNameFun(item);
        }
    }

    private static void RemoveChidrensEndNameFun(Transform t)
    {
        foreach (Transform item in t)
        {
            item.name = item.name.Remove(item.name.Length - 1);
            RemoveChidrensEndNameFun(item);
        }
    }

    private static void ChangeChidrensName()
    {
        foreach (Transform item in Selection.activeGameObject.transform)
        {
            item.name = firstName + ChildNo.ToString();
            ChildNo++;
        }
    }

    ////创建工程文件夹
    //[MenuItem("Quick/其他功能/创建工程文件夹/确认/再次确认")]
    //private static void CreatFolders()
    //{
    //    string dataPath = Application.dataPath;

    //    string[] projectPaths = new string[] {
    //            "/Datas",
    //            "/Effects",
    //            "/Contents",
    //            "/Resources/Shaders",
    //            "/Scripts",
    //            "/StreamingAssets"
    //        };
    //    DirectoryInfo dirInfo;
    //    for (int i = 0; i < projectPaths.Length; ++i)
    //    {
    //        dirInfo = new DirectoryInfo(dataPath + projectPaths[i]);
    //        if (!dirInfo.Exists) dirInfo.Create();

    //        AssetDatabase.ImportAsset("Assets" + projectPaths[i]);
    //    }
    //}

    [MenuItem("Quick/其他功能/创建资源 ")]
    private static void creat()
    {
        try
        {
            GameObject g = GameObject.Instantiate(Resources.Load<GameObject>("A"));
            g.name = "CameraRender";
            var tex = new RenderTexture(1, 1, 1);
            if (!Directory.Exists(Path.Combine(Application.dataPath, "renderTexture"))) Directory.CreateDirectory((Path.Combine(Application.dataPath, "renderTexture")));

            AssetDatabase.CreateAsset(tex, "Assets/renderTexture/" + g.name + ".renderTexture");
            g.GetComponent<Camera>().targetTexture = tex;
        }
        catch (Exception)
        {
            Debug.LogWarning("此功能需要在代码中定制");
        }
    }

    //计数归零

    private static void NameTo_0()
    {
        No = 0;
        ChildNo = 0;
    }

    //打开工具窗口
    [MenuItem("Quick/工具窗口 ")]
    private static void OpenWindow()
    {
        EditorWindow windwo = GetWindow(typeof(QuickEditor));
        windwo.Show();
    }

    private static UnityEngine.Object flagGameObjcet = null;

    [MenuItem("GameObject/标记", false, 0)]
    private static void SetFlagGameObject()
    {
        flagGameObjcet = Selection.activeGameObject;
    }

    [MenuItem("Assets/复制名称")]
    [MenuItem("GameObject/复制名称", false, 0)]
    static public void CopyName()
    {
        UnityEngine.Object obj = Selection.activeObject;
        if (obj == null) return;
        string path = obj.name;
        Debug.Log(path);
        TextEditor te = new TextEditor();
        te.text = path;
        te.OnFocus();
        te.Copy();
    }

    [MenuItem("GameObject/复制路径", false, 0)]
    static public void GetHierarchy()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj == null) return;
        string path = obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = obj.name + "/" + path;
        }
        Debug.Log(path);
        TextEditor te = new TextEditor();
        te.text = path;
        te.OnFocus();
        te.Copy();
    }

    [SerializeField]
    private static string childNoStr = 0.ToString();

    private static string tag = "";
    private static string str = "";

    [MenuItem("Assets/复制路径")]
    public static void GetAssetsPath()
    {
        UnityEngine.Object obj = Selection.activeObject;
        if (obj && AssetDatabase.IsMainAsset(obj))
        {
            string path = AssetDatabase.GetAssetPath(obj);
            path = path.Remove(0, 7);
            Debug.Log(path);
            TextEditor te = new TextEditor();
            te.text = path;
            te.OnFocus();
            te.Copy();
            Debug.Log(Application.dataPath);
        }
    }

    [MenuItem("Assets/创建实例")]
    public static void CreatScriptInstance()
    {
        UnityEngine.Object obj = Selection.activeObject;
        if (obj is MonoScript)
        {
            MonoScript ms = (MonoScript)Selection.activeObject;
            Type type = ms.GetClass();
            GameObject go = new GameObject("@" + obj.name);
            go.AddComponent(type);
        }
    }

    private static string groupName = "";
    private static Vector3 VectorDestance;
    private static int menuType = 0;
    public string[] toolbarStrings = new string[] { "第一页", "第二页", "第三页" };

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal("GameViewBackground");
        menuType = GUILayout.Toolbar(menuType, toolbarStrings);
        EditorGUILayout.EndHorizontal();
        if (menuType == 0)
        {
            Page0();
        }
    }

    private void Page0()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("开头：");
        firstName = GUILayout.TextField(firstName);
        GUILayout.Label("编号：");
        childNoStr = GUILayout.TextField(childNoStr);
        ChildNo = childNoStr == string.Empty ? 0 : int.Parse(childNoStr);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("后缀（编号）：");
        endName = GUILayout.TextField(endName);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal("GameViewBackground");
        if (GUILayout.Button("改名", "ButtonMid"))
        {
            ChangeChidrensName();
        }
        if (GUILayout.Button("归零", "ButtonMid"))
        {
            NameTo_0();
        }
        if (GUILayout.Button("加后缀", "ButtonMid"))
        {
            ChangeChidrensEndName();
        }
        if (GUILayout.Button("除后缀", "ButtonMid"))
        {
            RemoveChidrensEndName();
        }
        EditorGUILayout.EndHorizontal();

        ///////////////////////////////////
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("查找特征:");
        tag = GUILayout.TextField(tag);
        //tag = GUILayout.TextField(tag);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal("GameViewBackground");
        if (GUILayout.Button("标签查", "ButtonMid"))
        {
            SelectByTag();
        }

        if (GUILayout.Button("名称查", "ButtonMid"))
        {
            SelectChilds(p => p.name.StartsWith(tag));
        }

        if (GUILayout.Button("Layer查", "ButtonMid"))
        {
            SelectChilds(p => p.gameObject.layer == int.Parse(tag));
        }

        if (GUILayout.Button("组件查", "ButtonMid"))
        {
            SelectChilds(p => p.GetComponent(tag) != null);
        }
        if (GUILayout.Button("自定义", "ButtonMid"))
        {
            Debug.Log("点击我在这里输入代码");
            // SelectChilds(p => p.gameObject.layer == int.Parse(tag));//修改这里
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal("GameViewBackground");
        GUILayout.Label("组名:");
        groupName = EditorGUILayout.TextField(groupName);
        if (GUILayout.Button("打组", "ButtonMid"))
        {
            Group();
        }
        if (GUILayout.Button("解散", "ButtonMid"))
        {
            foreach (UnityEngine.Object item in Selection.objects)
            {
                Transform tf = (item as GameObject).transform;
                if (tf.parent != null)
                {
                    tf.SetParent(tf.parent.parent);
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        VectorDestance = EditorGUILayout.Vector3Field("间距:", VectorDestance);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal("GameViewBackground");
        if (GUILayout.Button("等距排列", "ButtonMid"))
        {
            if (Selection.activeGameObject == null)
            {
                Debug.Log("请选择一个父物体");
                return;
            }
            Transform select = Selection.activeGameObject.transform;
            for (int i = 0; i < select.childCount; i++)
            {
                select.GetChild(i).position = select.position + VectorDestance * i;
            }
        }

        if (GUILayout.Button("重置", "ButtonMid"))
        {
            VectorDestance = Vector3.zero;
            if (Selection.activeGameObject == null)
            {
                Debug.Log("请选择一个父物体");
                return;
            }
            Transform select = Selection.activeGameObject.transform;
            for (int i = 0; i < select.childCount; i++)
            {
                select.GetChild(i).position = select.position + VectorDestance * i;
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("标记:", "ButtonMid"))
        {
            flagGameObjcet = Selection.activeGameObject;
        }

        flagGameObjcet = EditorGUILayout.ObjectField(flagGameObjcet, typeof(GameObject));
        if (GUILayout.Button("位置", "ButtonMid"))
        {
            GameObject selecet = Selection.activeGameObject;

            if (selecet && flagGameObjcet)
            {
                selecet.transform.localPosition = (flagGameObjcet as GameObject).transform.position;
            }
        }
        if (GUILayout.Button("旋转", "ButtonMid"))
        {
            GameObject selecet = Selection.activeGameObject;

            if (selecet && flagGameObjcet)
            {
                selecet.transform.rotation = (flagGameObjcet as GameObject).transform.rotation;
            }
        }

        EditorGUILayout.EndHorizontal();

        /////////////////////////////////////
        //EditorGUILayout.Space();
        //EditorGUILayout.LabelField("<-----查找当前物体所有以str开头的子物体------>");
        //EditorGUILayout.BeginHorizontal();
        //str = EditorGUILayout.TextField("Str", str);
        //EditorGUILayout.EndHorizontal();
    }

    private List<Transform> TransformAllChild(Transform trans)
    {
        List<Transform> tempList = new List<Transform>();
        recurrence(trans, tempList);
        return tempList;
    }

    private static void recurrence(Transform trans, List<Transform> tempList)
    {
        foreach (Transform item in trans)
        {
            tempList.Add(item);
            recurrence(item, tempList);
        }
    }

    private delegate bool findHandle(Transform tf);

    private void SelectChilds(findHandle handle)
    {
        List<GameObject> list = new List<GameObject>();
        Transform[] alltf;
        GameObject go = Selection.activeGameObject;
        if (go == null)
        {
            Debug.Log("请选择一个物体作为父物体才能查找~");
            alltf = GameObject.FindObjectsOfType<Transform>();
        }
        else
        {
            alltf = TransformAllChild(go.transform).ToArray();
        }

        foreach (var item in alltf)
        {
            if (handle(item))
            {
                list.Add(item.gameObject);
            }
        }
        Debug.Log("查找到：" + list.Count + "个");
        Selection.objects = list.ToArray();
    }

    #region 各平台StreamAsset路径

    private string wwwPath()
    {
        string filepath;
#if UNITY_EDITOR
        filepath = "file://" + Application.dataPath + "/StreamingAssets";
#elif UNITY_IPHONE
        filepath = "file://" + Application.dataPath + "/Raw" ;
#elif UNITY_ANDROID
      filepath = "jar:file://" + Application.dataPath + "!/assets" ;
     // filepath=   Application .streamingAssetsPath;
#endif
        return filepath;
    }

    #endregion 各平台StreamAsset路径

    [MenuItem("Quick/其他功能/移动平台www读取StreamingAssets路径")]
    public static void PrintStreamingAssetsPath() { Debug.Log("双击我从代码中行复制 这个功能在5.X版本中可以直接使用 Application .streamingAssetsPath;"); }

    [MenuItem("Quick/其他功能/特殊文件夹名称")]
    public static void SpecialFolder()
    {
        Debug.Log(@"
Editor
Editor Default Resources
Gizmos
Plugins
Resources
StreamingAssets
用途请查阅:http://www.xuanyusong.com/archives/3229");
    }

    [MenuItem("Quick/其他功能/预编译指令")]
    public static void PlatformDependentCompilation()
    {
        Debug.Log(@"
UNITY_EDITOR
UNITY_EDITOR_WIN ||UNITY_STANDALONE_WIN
UNITY_IOS ||UNITY_ANDROID
UNITY_IPHONE
UNITY_EDITOR_OSX
UNITY_STANDALONE_OSX
UNITY_STANDALONE_WIN
UNITY_STANDALONE_LINUX
UNITY_STANDALONE
UNITY_WEBPLAYER
UNITY_WII
UNITY_PS3
UNITY_PS4
UNITY_XBOX360
UNITY_XBOXONE
UNITY_TIZEN
UNITY_WP8
UNITY_WP8_1
UNITY_WSA
UNITY_WSA_8_0
UNITY_WSA_8_1
UNITY_WSA_10_0
UNITY_WINRT
UNITY_WINRT_8_0
UNITY_WINRT_8_1
UNITY_WINRT_10_0
UNITY_WEBGL
UNITY_ANALYTICS");
    }

    [MenuItem("Quick/其他功能/文件对话框获取文件路径")]
    public static void WindowGetPathFile()
    {
        string path = EditorUtility.OpenFilePanel("选择文件", "", "");
        Debug.Log(path);
        TextEditor te = new TextEditor();
        te.text = path;
        te.OnFocus();
        te.Copy();
    }

    [MenuItem("Quick/其他功能/文件对话框获取文件 夹 路径")]
    public static void WindowGetPathFloder()
    {
        string path = EditorUtility.OpenFolderPanel("获取文件夹", "", "");
        Debug.Log(path);
        TextEditor te = new TextEditor();
        te.text = path;
        te.OnFocus();
        te.Copy();
    }

    [MenuItem("Quick/示例功能/打开对话框")]
    public static void DisplayDialog()
    {
        if (EditorUtility.DisplayDialog("标题?", "根据你的选择来决定做什么事！", "是", "否"))
        {
            //BuildResources ("Datas");
            Debug.Log("OK");
        }
        else
        {
            Debug.Log("No");
        }
    }

    public static void Group()
    {
        if (Selection.objects.Length <= 1)
        {
            return;
        }
        GameObject group = new GameObject(groupName);
        Transform grateFater = (Selection.objects[0] as GameObject).transform.parent;
        if (grateFater != null)
        {
            group.transform.SetParent(grateFater);
        }
        group.transform.localPosition = Vector3.zero;
        foreach (UnityEngine.Object item in Selection.objects)
        {
            (item as GameObject).transform.SetParent(group.transform);
        }
    }
}