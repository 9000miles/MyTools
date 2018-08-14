using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Displayer))]
public class DisplayerEditor : Editor
{
    //private Displayer obj;

    //private void Awake()
    //{
    //    obj = target as Displayer;
    //    if (obj.instanceA == null)
    //        obj.instanceA = new A();
    //    if (obj.instanceB == null)
    //        obj.instanceB = new B();
    //}

    //public override void OnInspectorGUI()
    //{
    //    base.OnInspectorGUI();

    //    if (obj.type == TestType.A)
    //    {
    //        EditorGUILayout.LabelField("A对象");
    //        obj.instanceA.a = EditorGUILayout.TagField(obj.instanceA.a);
    //    }
    //    //else
    //    //{
    //    //    EditorGUILayout.LabelField("B对象");
    //    //    obj.instanceB.b = EditorGUILayout.TextField(obj.instanceB.b);
    //    //}
    //}
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        reorderableList = new ReorderableList(serializedObject,
            serializedObject.FindProperty("texts"));

        //      绘制元素
        var prop = serializedObject.FindProperty("texts");

        reorderableList = new ReorderableList(serializedObject, prop);

        //      绘制按钮
        reorderableList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) =>
        {
            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.miniButton.Draw(rect, false, isActive, isFocused, false);
            }
        };

        //      绘制文本框
        reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = prop.GetArrayElementAtIndex(index);
            rect.height -= 4;
            rect.y += 2;
            EditorGUI.PropertyField(rect, element);
        };

        reorderableList.onAddCallback += (list) =>
        {
            //添加元素
            prop.arraySize++;

            //选择状态设置为最后一个元素
            list.index = prop.arraySize - 1;

            //          新元素添加默认字符串
            var element = prop.GetArrayElementAtIndex(list.index);
            element.stringValue = "New String " + list.index;
        };

        //      AddMenue ();

        reorderableList.onReorderCallback = (list) =>
        {
            //元素更新
            Debug.Log("onReorderCallback");
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    private void AddMenue()
    {
        reorderableList.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) =>
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Example 1"), true, () =>
            {
            });
            menu.AddSeparator("");
            menu.AddDisabledItem(new GUIContent("Example 2"));
            menu.DropDown(buttonRect);
        };
    }
}

public class AssetBundleCreator : EditorWindow
{
    [MenuItem("Tools/Build Asset Bundle")]
    public static void BuildAssetBundle()
    {
        var win = GetWindow<AssetBundleCreator>("Build Asset Bundle");
        win.Show();
    }

    [SerializeField]//必须要加
    protected List<UnityEngine.Object> _assetLst = new List<UnityEngine.Object>();

    //序列化对象
    protected SerializedObject _serializedObject;

    //序列化属性
    protected SerializedProperty _assetLstProperty;

    protected void OnEnable()
    {
        //使用当前类初始化
        _serializedObject = new SerializedObject(this);
        //获取当前类中可序列话的属性
        _assetLstProperty = _serializedObject.FindProperty("_assetLst");
    }

    protected void OnGUI()
    {
        //更新
        _serializedObject.Update();

        //开始检查是否有修改
        EditorGUI.BeginChangeCheck();

        //显示属性
        //第二个参数必须为true，否则无法显示子节点即List内容
        EditorGUILayout.PropertyField(_assetLstProperty, true);

        //结束检查是否有修改
        if (EditorGUI.EndChangeCheck())
        {//提交修改
            _serializedObject.ApplyModifiedProperties();
        }
    }
}