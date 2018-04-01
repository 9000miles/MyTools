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
}