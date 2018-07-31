using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class GameObjectHelper
{
    /// <summary>
    /// 获取该物体本身的路径
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static string GetSelfPath(this GameObject currentGO)
    {
        if (currentGO == null) return null;
        string path = currentGO.name;
        while (currentGO.transform.parent != null)
        {
            currentGO = currentGO.transform.parent.gameObject;
            path = currentGO.name + "/" + path;
        }
        return path;
    }

    /// <summary>
    /// 获取多个物体本身的路径
    /// </summary>
    /// <param name="gos"></param>
    /// <returns></returns>
    public static string[] GetSelfPaths(this GameObject[] gos)
    {
        List<string> pathList = new List<string>();
        foreach (var item in gos)
        {
            pathList.Add(item.GetSelfPath());
        }
        return pathList.ToArray();
    }

    /// <summary>
    /// 获取多个物体本身的路径
    /// </summary>
    /// <param name="gos"></param>
    /// <returns></returns>
    public static string[] GetSelfPaths(this List<GameObject> goList)
    {
        return goList.ToArray().GetSelfPaths();
    }

    /// <summary>
    /// 获取本身和孩子的路径
    /// </summary>
    /// <param name="currentGO"></param>
    /// <returns></returns>
    public static string[] GetSelfAndChildrenPaths(this GameObject currentGO)
    {
        List<string> pathList = new List<string>();
        Transform[] tfs = currentGO.GetComponentsInChildren<Transform>();
        foreach (var item in tfs)
        {
            pathList.Add(item.GetSelfPath());
        }
        return pathList.ToArray();
    }

    /// <summary>
    /// 获取本身和孩子的路径
    /// </summary>
    /// <param name="gos"></param>
    /// <returns></returns>
    public static string[] GetSelfAndChildrenPaths(this GameObject[] gos)
    {
        List<string> pathList = new List<string>();
        foreach (var item in gos)
        {
            string[] itemPaths = item.GetSelfAndChildrenPaths();
            pathList.AddRange(itemPaths);
        }
        return pathList.ToArray();
    }

    /// <summary>
    /// 获取对应的Transform组件
    /// </summary>
    /// <param name="currentGOs"></param>
    /// <returns></returns>
    public static Transform[] GetTransforms(this GameObject[] currentGOs)
    {
        List<Transform> list = new List<Transform>();
        foreach (var item in currentGOs)
        {
            list.Add(item.GetComponent<Transform>());
        }
        return list.ToArray();
    }

    /// <summary>
    /// 获取对应的Transform组件
    /// </summary>
    /// <param name="currentGOList"></param>
    /// <returns></returns>
    public static List<Transform> GetTransforms(this List<GameObject> currentGOList)
    {
        List<Transform> list = new List<Transform>();
        foreach (var item in currentGOList)
        {
            list.Add(item.GetComponent<Transform>());
        }
        return list;
    }
}