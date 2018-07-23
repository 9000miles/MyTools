using Common;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public static string[] GetSelfAndChilderPaths(this GameObject currentGO)
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
    public static string[] GetSelfAndChilderPaths(this GameObject[] gos)
    {
        List<string> pathList = new List<string>();
        foreach (var item in gos)
        {
            string[] itemPaths = item.GetSelfAndChilderPaths();
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

    /// <summary>
    /// 获取其它的Componet组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Q">需要获取的组件类型</typeparam>
    /// <param name="array">源数组</param>
    /// <param name="func">GetComponet<<typeparamref name="Q"/>>()</param>
    /// <param name="isMustGetAll">是否必须全部获得</param>
    /// <returns>需要获取的组件数组</returns>
    public static Q[] GetOtherComponets<T, Q>(this T[] array, Func<T, Q> func, bool isMustGetAll = true) where T : Component
    {
        List<Q> list = new List<Q>();
        foreach (var item in array)
        {
            Q temp = func(item);
            if (temp != null)
                list.Add(temp);
        }
        if (isMustGetAll == true)
        {
            if (array.Length != list.Count)
            {
                Debug.LogError("Some components are not retrieved");
                return null;
            }
        }
        return list.ToArray();
    }
}