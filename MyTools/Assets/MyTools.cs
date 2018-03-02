using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YouLe_MyTools
{
    ///  <summary>
    ///
    ///  </summary>
    public class MyTools
    {
        /// <summary>
        /// 求2个字符串最长公共字符串
        /// </summary>
        /// <param name="longString">长字符串</param>
        /// <param name="shortString">短字符串</param>
        /// <returns>最长公共字符串</returns>
        public static string LongestCommonString(string longString, string shortString)
        {
            string resultString = ""; //结果字符串
            if (longString.Length < shortString.Length)//交换字符串
            {
                string strTemp = "";//转换字符串
                strTemp = shortString;
                shortString = longString;
                longString = strTemp;
            }
            for (int i = 0; i < longString.Length; i++)//循环最大长度，保证每个字符都进行比对
            {
                for (int j = 0; j < shortString.Length; j++)//循环较短的字符串
                {
                    if (longString[i] == shortString[j])//2个字符串的每个字符进行比较，如果相同
                    {
                        int n = 0;
                        while (longString[i + n] == shortString[j + n])//从相同的位置向后进行比较
                        {
                            n++;//如果相同则加1，记录相同的字符个数
                            if (i + n > longString.Length - 1 || j + n > shortString.Length - 1)//判断是否超出字符串长度范围
                                break;
                        }
                        if (n > resultString.Length)//如果新的相同字符串长度大于之前的
                            resultString = longString.Substring(i, n);//从相同的位置向后截取n个字符，即是最长公共字符串
                    }
                }
            }
            return resultString;//最长公共字符串：  fgfgfgfgfgfgf5h45h
        }
        /// <summary>
        /// 合并2个数组，根据源数组的排序方式排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayA">A数组</param>
        /// <param name="arrayB">B数组</param>
        /// <returns></returns>
        public static T[] MergeSortedArray<T>(T[] arrayA, T[] arrayB) where T : IComparable
        {
            T[] mergeArray = new T[arrayA.Length + arrayB.Length];
            Array.Copy(arrayA, mergeArray, arrayA.Length);
            Array.Copy(arrayB, 0, mergeArray, arrayA.Length, arrayB.Length);
            Array.Sort(mergeArray);
            if (arrayA[0].CompareTo(arrayA[1]) > 0)//判断源数组是什么排序方式，如果是从大到小就反转数组
                Array.Reverse(mergeArray);
            return mergeArray;
        }
        /// <summary>
        /// 数组万能排序，冒泡排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">数组</param>
        /// <param name="handler">排序规则</param>
        public static void Sort<T>(T[] array, Func<T, T, bool> handler) where T : IComparable
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (handler(array[i], array[j]))
                    {
                        T temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
        }
    }
}