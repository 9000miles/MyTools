﻿using System;
using System.Collections.Generic;

/*
c# 扩展方法
    定义：能够向现有类添加方法，而无需修改原始类或创建新的派生类。
    要素：
    1.扩展方法必须在非泛型的静态类中。
    2.第一个参数使用this关键字指定被扩展类型。
    3.建议在扩展方法所在类上，添加命名空间。
*/

namespace MyCommonTools
{
    ///<summary>
    /// 数组助手
    ///</summary>
    public static class ArrayHelper
    {
        /// <summary>
        /// 对象数组的升序排列
        /// </summary>
        /// <typeparam name="T">对象的类型 例如：Enemy</typeparam>
        /// <typeparam name="Q">对象的属性 例如：HP</typeparam>
        /// <param name="array">对象数组</param>
        /// <param name="handler">排序依据</param>
        public static void OrderBy<T, Q>(this T[] array, Func<T, Q> handler) where Q : IComparable
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    //if (array[i].ATK > array[j].ATK)
                    //if(Fun1(array[i]) > Fun1(array[j]) )
                    //if(handler(array[i])>handler(array[j]))
                    if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                    {
                        T temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// 对象数组的降序排列
        /// </summary>
        /// <typeparam name="T">对象的类型 例如：Enemy</typeparam>
        /// <typeparam name="Q">对象的属性 例如：HP</typeparam>
        /// <param name="array">对象数组</param>
        /// <param name="handler">排序依据</param>
        public static void OrderByDescending<T, Q>(this T[] array, Func<T, Q> handler) where Q : IComparable
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (handler(array[i]).CompareTo(handler(array[j])) < 0)
                    {
                        T temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// 查找所有满足条件的对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="array">对象数组</param>
        /// <param name="condition">查找条件</param>
        /// <returns></returns>
        public static T[] FindAll<T>(this T[] array, Func<T, bool> condition)
        {
            List<T> list = new List<T>(array.Length);
            for (int i = 0; i < array.Length; i++)
                if (condition(array[i]))
                    list.Add(array[i]);
            return list.ToArray();
        }

        /// <summary>
        /// 筛选对象数组
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <typeparam name="Q">筛选结果类型</typeparam>
        /// <param name="array">需要筛选的对象数组</param>
        /// <param name="handler">筛选逻辑</param>
        /// <returns></returns>
        public static Q[] Select<T, Q>(this T[] array, Func<T, Q> handler)
        {
            Q[] newArray = new Q[array.Length];
            for (int i = 0; i < array.Length; i++)
                newArray[i] = handler(array[i]);
            return newArray;
        }

        /// <summary>
        /// 获取指定条件的最大元素
        /// </summary>
        /// <typeparam name="T">需要查找的对象类型</typeparam>
        /// <typeparam name="Q">需要查找的对象属性</typeparam>
        /// <param name="array">需要查找的对象数组</param>
        /// <param name="condition">查找条件</param>
        /// <returns></returns>
        public static T GetMax<T, Q>(this T[] array, Func<T, Q> condition) where Q : IComparable
        {
            T max = array[0];
            for (int i = 1; i < array.Length; i++)
                if (condition(max).CompareTo(condition(array[i])) < 0)
                    max = array[i];
            return max;
        }

        /// <summary>
        /// 数组快速排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T[] QuickSort<T>(T[] arr) where T : IComparable
        {
            if (arr.Length < 2)//只有一个元素或者没有元素，直接返回
                return arr;
            T baseNum = arr[arr.Length / 2];//取第一个元素为基数
            List<T> lessBaseArr = new List<T>();//小于基数的集合
            List<T> greaterBaseArr = new List<T>();//大于基数的集合
            for (int i = 0; i < arr.Length; i++)//与第一个元素比较，进行分区
            {
                if (i == arr.Length / 2)
                    continue;
                if (arr[i].CompareTo(baseNum) < 0)//小于基数
                    lessBaseArr.Add(arr[i]);
                else//大于基数
                    greaterBaseArr.Add(arr[i]);
            }
            T[] lesSortArr = lessBaseArr.Count == 0 ? lessBaseArr.ToArray() : QuickSort(lessBaseArr.ToArray());//对小于基数的集合递归调用
            T[] greaterSortArr = greaterBaseArr.Count == 0 ? greaterBaseArr.ToArray() : QuickSort(greaterBaseArr.ToArray());//对大于基数的集合递归调用
            List<T> Arr = new List<T>();//用于拼接小于，基数，大于集合
            Arr.AddRange(lesSortArr);
            Arr.Add(baseNum);
            Arr.AddRange(greaterSortArr);
            return Arr.ToArray();
        }

        /// <summary>
        /// 利用二分查找法，在数组中查找元素是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool FindItemByHalfSerach<T>(T[] arr, T item) where T : IComparable
        {
            if (arr.Length == 1)//只有一个元素判断是否相等
                return item.CompareTo(arr[0]) == 0;
            int middle = arr.Length / 2;
            T middleItem = arr[middle];
            bool isFinded = false;
            if (item.CompareTo(middleItem) < 0)//在比中间元素小的数组中查找
            {
                int lessArrayLength = arr.Length % 2 == 0 ? arr.Length - middle : arr.Length - middle - 1;
                T[] tempArr = new T[lessArrayLength];
                Array.Copy(arr, 0, tempArr, 0, lessArrayLength);
                isFinded = FindItemByHalfSerach(tempArr, item);
            }
            else if (item.CompareTo(middleItem) > 0)//在比中间元素大的数组中查找
            {
                T[] tempArr = new T[arr.Length - middle - 1];
                Array.Copy(arr, middle + 1, tempArr, 0, arr.Length - middle - 1);
                isFinded = FindItemByHalfSerach(tempArr, item);
            }
            else//中间元素就是查找的元素
                isFinded = true;
            return isFinded;
        }
    }
}