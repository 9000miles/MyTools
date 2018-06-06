//By:熊峻玉
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExMonoBehaviour : MonoBehaviour
{
    /// <summary>
    /// 返回GameObject.name=name的 T 类型组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T ExmChildComponent<T>(string name)
    {
        Transform tf = FindChild(this.transform, name);
        if (tf == null)
        {
            return default(T);
        }
        return FindChild(this.transform, name).GetComponent<T>();
    }

    /// <summary>
    /// 返回制定路径物体上的T类型组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Path"></param>
    /// <returns></returns>
    public T ExmPathComponent<T>(string Path)
    {
        Transform tf = transform.Find(Path);
        if (tf == null)
        {
            Debug.LogError("没有这个子物体: " + Path);
            return default(T);
        }
        return tf.GetComponent<T>();
    }

    private static Transform fndChildResurt = null;

    private static void FindChildRrecurrence(Transform trans, string goName)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            //如果容器有东西了就不做了
            if (fndChildResurt != null)
            {
                return;
            }
            //符合条件就放到容器 并返回
            if (trans.GetChild(i).name.Equals(goName))
            {
                Console.WriteLine(trans.GetChild(i));
                fndChildResurt = trans.GetChild(i);
                return;
            }
            //否则继续向下（深度优先）
            FindChildRrecurrence(trans.GetChild(i), goName);
        }
    }

    public static Transform FindChild(Transform trans, string goName)
    {
        ////先在直接下级找
        //Transform child = trans.FindChild(goName);
        ////如果找到返回
        //if (child != null) return child;
        ////没找到，继续在每一个子物体的下级找
        //Transform go = null;
        //for (int i = 0; i < trans.childCount; i++)
        //{
        //    child = trans.GetChild(i);
        //    go = FindChild(child, goName);
        //    if (go != null) return go;
        //}
        //return null;

        //初始化容器
        fndChildResurt = null;
        //执行递归
        FindChildRrecurrence(trans, goName);
        //返回结果
        return fndChildResurt;
    }

    #region 查找复合条件的所有子物体

    //递归容器
    protected List<Transform> list_TransformAllChild;

    /// <summary>
    /// 查找复合条件的所有子物体
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public List<Transform> ExmFindAllChild(Func<Transform, bool> deligate)
    {   //初始化容器
        if (list_TransformAllChild == null) { list_TransformAllChild = new List<Transform>(); }
        list_TransformAllChild.Clear();
        //执行递归
        recurrence(transform, list_TransformAllChild, deligate);
        //返回结果

        return list_TransformAllChild;
    }

    //递归体
    private void recurrence(Transform trans, List<Transform> tempList, Func<Transform, bool> deligate)
    {
        //foreach (Transform item in trans)
        //{
        //    if (deligate(item))
        //    {
        //        tempList.Add(item);
        //    }
        //    recurrence(item, tempList, deligate);
        //}

        //   深度优先
        for (int i = 0; i < trans.childCount; i++)
        {
            if (deligate(trans.GetChild(i)))
            {
                tempList.Add(trans.GetChild(i));
            }
            recurrence(trans.GetChild(i), tempList, deligate);
        }

        //广度优先
        //for (int i = 0; i < trans.childCount; i++)
        //{
        //    if (deligate(trans.GetChild(i)))
        //    {
        //        tempList.Add(trans.GetChild(i));
        //    }
        //}
        //for (int i = 0; i < trans.childCount; i++)
        //{
        //    recurrence(trans.GetChild(i), tempList, deligate);
        //}
    }

    #endregion 查找复合条件的所有子物体
}