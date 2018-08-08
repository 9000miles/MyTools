using System;
using System.Collections.Generic;

namespace Common
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
        public static void OrderAscending<T, Q>(this T[] array, Func<T, Q> handler) where Q : IComparable
        {
            if (array == null || array.Length <= 0) return;
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
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
        public static void OrderDescending<T, Q>(this T[] array, Func<T, Q> handler) where Q : IComparable
        {
            if (array == null || array.Length <= 0) return;
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
            if (array == null || array.Length <= 0) return null;
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
            if (array == null || array.Length <= 0) return null;
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
            if (array == null || array.Length <= 0) return default(T);
            T max = array[0];
            for (int i = 1; i < array.Length; i++)
                if (condition(max).CompareTo(condition(array[i])) < 0)
                    max = array[i];
            return max;
        }

        /// <summary>
        /// 获取指定条件的最小元素
        /// </summary>
        /// <typeparam name="T">需要查找的对象类型</typeparam>
        /// <typeparam name="Q">需要查找的对象属性</typeparam>
        /// <param name="array">需要查找的对象数组</param>
        /// <param name="condition">查找条件</param>
        /// <returns></returns>
        public static T GetMin<T, Q>(this T[] array, Func<T, Q> condition) where Q : IComparable
        {
            if (array == null || array.Length <= 0) return default(T);
            T min = array[0];
            for (int i = 1; i < array.Length; i++)
                if (condition(min).CompareTo(condition(array[i])) > 0)
                    min = array[i];
            return min;
        }

        /// <summary>
        /// 数组快速排序，升序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T[] QuickSortAscending<T, Q>(this T[] arr, Func<T, Q> handler) where Q : IComparable
        {
            if (arr.Length < 2)//只有一个元素或者没有元素，直接返回
                return arr;
            T baseNum = arr[arr.Length / 2];//取第一个元素为基数
            List<T> lessBaseNumArr = new List<T>();//小于基数的集合
            List<T> greaterBaseNumArr = new List<T>();//大于基数的集合
            for (int i = 0; i < arr.Length; i++)//与第一个元素比较，进行分区
            {
                if (i == arr.Length / 2)
                    continue;
                if (handler(arr[i]).CompareTo(handler(baseNum)) < 0)//小于基数
                    lessBaseNumArr.Add(arr[i]);
                else//大于基数
                    greaterBaseNumArr.Add(arr[i]);
            }
            T[] lesSortArr = lessBaseNumArr.Count == 0 ? lessBaseNumArr.ToArray() : QuickSortAscending(lessBaseNumArr.ToArray(), handler);//对小于基数的集合递归调用
            T[] greaterSortArr = greaterBaseNumArr.Count == 0 ? greaterBaseNumArr.ToArray() : QuickSortAscending(greaterBaseNumArr.ToArray(), handler);//对大于基数的集合递归调用
            List<T> Arr = new List<T>();//用于拼接小于，基数，大于集合
            Arr.AddRange(lesSortArr);
            Arr.Add(baseNum);
            Arr.AddRange(greaterSortArr);
            return Arr.ToArray();
        }

        /// <summary>
        /// 数组快速排序，升序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T[] QuickSortAscending<T>(this T[] arr) where T : IComparable
        {
            if (arr.Length < 2)//只有一个元素或者没有元素，直接返回
                return arr;
            T baseNum = arr[arr.Length / 2];//取第一个元素为基数
            List<T> lessBaseNumArr = new List<T>();//小于基数的集合
            List<T> greaterBaseNumArr = new List<T>();//大于基数的集合
            for (int i = 0; i < arr.Length; i++)//与第一个元素比较，进行分区
            {
                if (i == arr.Length / 2)
                    continue;
                if (arr[i].CompareTo(baseNum) < 0)//小于基数
                    lessBaseNumArr.Add(arr[i]);
                else//大于基数
                    greaterBaseNumArr.Add(arr[i]);
            }
            T[] lesSortArr = lessBaseNumArr.Count == 0 ? lessBaseNumArr.ToArray() : QuickSortAscending(lessBaseNumArr.ToArray());//对小于基数的集合递归调用
            T[] greaterSortArr = greaterBaseNumArr.Count == 0 ? greaterBaseNumArr.ToArray() : QuickSortAscending(greaterBaseNumArr.ToArray());//对大于基数的集合递归调用
            List<T> Arr = new List<T>();//用于拼接小于，基数，大于集合
            Arr.AddRange(lesSortArr);
            Arr.Add(baseNum);
            Arr.AddRange(greaterSortArr);
            return Arr.ToArray();
        }

        /// <summary>
        /// 数组快速排序，降序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T[] QuickSortDescending<T, Q>(this T[] arr, Func<T, Q> handler) where Q : IComparable
        {
            if (arr.Length < 2)//只有一个元素或者没有元素，直接返回
                return arr;
            T baseNum = arr[arr.Length / 2];//取第一个元素为基数
            List<T> lessBaseNumArr = new List<T>();//小于基数的集合
            List<T> greaterBaseNumArr = new List<T>();//大于基数的集合
            for (int i = 0; i < arr.Length; i++)//与第一个元素比较，进行分区
            {
                if (i == arr.Length / 2)
                    continue;
                if (handler(arr[i]).CompareTo(handler(baseNum)) > 0)//大于基数
                    greaterBaseNumArr.Add(arr[i]);
                else//小于基数
                    lessBaseNumArr.Add(arr[i]);
            }
            T[] lesSortArr = lessBaseNumArr.Count == 0 ? lessBaseNumArr.ToArray() : QuickSortDescending(lessBaseNumArr.ToArray(), handler);//对小于基数的集合递归调用
            T[] greaterSortArr = greaterBaseNumArr.Count == 0 ? greaterBaseNumArr.ToArray() : QuickSortDescending(greaterBaseNumArr.ToArray(), handler);//对大于基数的集合递归调用
            List<T> Arr = new List<T>();//用于拼接小于，基数，大于集合
            Arr.AddRange(greaterSortArr);
            Arr.Add(baseNum);
            Arr.AddRange(lesSortArr);
            return Arr.ToArray();
        }

        /// <summary>
        /// 数组快速排序，降序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T[] QuickSortDescending<T>(this T[] arr) where T : IComparable
        {
            if (arr.Length < 2)//只有一个元素或者没有元素，直接返回
                return arr;
            T baseNum = arr[arr.Length / 2];//取第一个元素为基数
            List<T> lessBaseNumArr = new List<T>();//小于基数的集合
            List<T> greaterBaseNumArr = new List<T>();//大于基数的集合
            for (int i = 0; i < arr.Length; i++)//与第一个元素比较，进行分区
            {
                if (i == arr.Length / 2)
                    continue;
                if (arr[i].CompareTo(baseNum) > 0)//大于基数
                    greaterBaseNumArr.Add(arr[i]);
                else//小于基数
                    lessBaseNumArr.Add(arr[i]);
            }
            T[] lesSortArr = lessBaseNumArr.Count == 0 ? lessBaseNumArr.ToArray() : QuickSortDescending(lessBaseNumArr.ToArray());//对小于基数的集合递归调用
            T[] greaterSortArr = greaterBaseNumArr.Count == 0 ? greaterBaseNumArr.ToArray() : QuickSortDescending(greaterBaseNumArr.ToArray());//对大于基数的集合递归调用
            List<T> Arr = new List<T>();//用于拼接小于，基数，大于集合
            Arr.AddRange(greaterSortArr);
            Arr.Add(baseNum);
            Arr.AddRange(lesSortArr);
            return Arr.ToArray();
        }

        /// <summary>
        /// 利用二分查找法，在升序数组中查找元素是否存在
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