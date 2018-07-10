using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectHelper
{
    /// <summary>
    /// 获取该物体的路径
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static string GetGameObjectPath(GameObject go)
    {
        if (go == null) return null;
        string path = go.name;
        while (go.transform.parent != null)
        {
            go = go.transform.parent.gameObject;
            path = go.name + "/" + path;
        }
        return path;
    }

    /// <summary>
    /// 获取选中的多个物体路径
    /// </summary>
    /// <param name="gos"></param>
    /// <returns></returns>
    public static string[] GetPaths(GameObject[] gos)
    {
        List<string> pathsList = new List<string>();
        foreach (var item in gos)
        {
            if (item.transform.childCount > 0)
            {
                Transform[] tfs = item.GetComponentsInChildren<Transform>();
                for (int i = 0; i < tfs.Length; i++)
                {
                    pathsList.Add(GetGameObjectPath(tfs[i].gameObject));
                }
            }
            else
                pathsList.Add(GetGameObjectPath(item));
        }
        return pathsList.ToArray();
    }
}