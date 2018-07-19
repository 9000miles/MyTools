using Common;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MyEditorTools /*: ScriptableWizard*/
{
    [MenuItem("MyTools/Change Selection Raycast Target", true, 1)]
    [MenuItem("MyTools/Change Selection Raycast Target On", true, 2)]
    [MenuItem("MyTools/Change Selection Raycast Target Off", true, 3)]
    private static bool IsSelection()
    {
        return Selection.gameObjects.Length > 0;
    }

    /// <summary>
    /// 反转所有的RaycastTarget
    /// </summary>
    [MenuItem("MyTools/Change Selection Raycast Target", false, 1)]
    private static void ChangeRaycastTargetValue()
    {
        GameObject[] gos = Selection.gameObjects;
        for (int i = 0; i < gos.Length; i++)
            gos[i].GetComponent<Graphic>().raycastTarget = !gos[i].GetComponent<Graphic>().raycastTarget;
        Debug.Log("Reverse RaycastTarget Value Done");
    }

    /// <summary>
    /// 打开所有的RaycastTarget
    /// </summary>
    [MenuItem("MyTools/Change Selection Raycast Target On", false, 2)]
    private static void ChangeRaycastTargetValueOn()
    {
        GameObject[] gos = Selection.gameObjects;
        for (int i = 0; i < gos.Length; i++)
            ChangeChildRaycastTargetValue(gos[i], true);
        Debug.Log("On GameObjects RaycastTarget Value Done");
    }

    /// <summary>
    /// 关闭所有的RaycastTarget
    /// </summary>
    [MenuItem("MyTools/Change Selection Raycast Target Off", false, 3)]
    private static void ChangeRaycastTargetValueOff()
    {
        GameObject[] gos = Selection.gameObjects;
        for (int i = 0; i < gos.Length; i++)
            ChangeChildRaycastTargetValue(gos[i], false);
        Debug.Log("Off GameObjects RaycastTarget Value Done");
    }

    /// <summary>
    /// 改变子级物体的RaycastTarget的值
    /// </summary>
    /// <param name="go">物体</param>
    /// <param name="isOn">是否打开</param>
    private static void ChangeChildRaycastTargetValue(GameObject go, bool isOn)
    {
        if (go.GetComponent<Graphic>() != null)
        {
            go.GetComponent<Graphic>().raycastTarget = isOn;//设置当前物体

            for (int i = 0; i < go.transform.childCount; i++)//递归子级物体
                ChangeChildRaycastTargetValue(go.transform.GetChild(i).gameObject, isOn);
        }
    }

    /// <summary>
    /// 手动递增改名
    /// </summary>
    [MenuItem("MyTools/Change Name[W]")]
    private static void ChangeNameOneByOne()
    {
        ChangeNameWindow window = new ChangeNameWindow();
        window.Show();
    }

    /// <summary>
    /// 排列物体
    /// </summary>
    [MenuItem("MyTools/Placing GameObject")]
    private static void PlacingObject()
    {
        PlacingObjectWindow window = new PlacingObjectWindow();
        window.Show();
    }

    /// <summary>
    /// 改变物体激活状态
    /// </summary>
    [MenuItem("MyTools/Change GameObject Active State %w")]
    private static void ToggleActiveState()
    {
        GameObject[] select = Selection.gameObjects;
        foreach (GameObject item in select)
        {
            item.SetActive(!item.activeSelf);
        }
    }

    /// <summary>
    /// 获取选中的物体数量
    /// </summary>
    [MenuItem("MyTools/Get Object Count")]
    private static void GetObjectCount()
    {
        GameObject[] select = Selection.gameObjects;
        int count = 0;
        foreach (var item in select)
        {
            count += item.GetComponentsInChildren<Transform>().Length;
            //count += GetObjcetCount(item);
        }
        Debug.Log(string.Format("-------选中{0}个物体，共有{1}个物体-------", select.Length, count));
    }

    private static int GetObjcetCount(GameObject go)
    {
        int count = 0;
        if (go.transform.childCount > 0)
        {
            Transform[] tfs = go.transform.GetComponentsInChildren<Transform>();
            foreach (var item in tfs)
            {
                if (item != go.transform)
                    count += GetObjcetCount(item.gameObject);
            }
        }
        else
            count = 1;
        return count;
    }

    /// <summary>
    /// 选中Player
    /// </summary>
    [MenuItem("MyTools/Selected Player &Q")]
    private static void SelectedPlayer()
    {
        GameObject player;
        player = GameObject.Find("Player");
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player != null)
        {
            Selection.activeGameObject = player;
        }
    }

    /// <summary>
    /// Alt+Up向上移动GameObject的层级
    /// </summary>
    [MenuItem("MyTools/Move Up GameObject In Hierarchy &UP")]
    private static void MoveUpGameObjectInHierarchy()
    {
        GameObject[] gos = Selection.gameObjects;
        gos.OrderAscending((t) => t.transform.GetSiblingIndex());
        foreach (var item in gos)
        {
            item.transform.SetSiblingIndex(item.transform.GetSiblingIndex() - 1);
        }
    }

    /// <summary>
    /// Alt+Down向下移动GameObject的层级
    /// </summary>
    [MenuItem("MyTools/Move Down GameObject In Hierarchy &DOWN")]
    private static void MoveDownGameObjectInTheHierarchy()
    {
        GameObject[] gos = Selection.gameObjects;
        gos.OrderDescending((t) => t.transform.GetSiblingIndex());
        foreach (var item in gos)
        {
            item.transform.SetSiblingIndex(item.transform.GetSiblingIndex() + 1);
        }
    }

    /// <summary>
    /// 打开选择物体窗口
    /// </summary>
    [MenuItem("MyTools/Open Seleted Window")]
    private static void OpenSelectionWindow()
    {
        SelectedGameObjectWindow window = new SelectedGameObjectWindow();
        window.Show();
    }

    //功能：查找指定物体 是否在选中的物体中，如果在则选中此物体（如果有多个物体，则全部展开），否则提示没有。
}