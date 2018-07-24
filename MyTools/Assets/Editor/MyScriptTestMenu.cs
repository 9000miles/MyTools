using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using Common;
using System.Linq;

public class MyScriptTestMenu
{
    [MenuItem("My Script Test Menu/Execute Test", false, 1)]
    private static void ExecuteTest()
    {
        string[] paths = Selection.gameObjects.GetSelfPaths();

        //GameObject[] gos = Selection.gameObjects;
        //IEnumerable<Transform> hfiwhei = gos.Cast<Transform>().Where(t => t != null);
        //foreach (var item in hfiwhei)
        //{
        //    Debug.Log(item.name);
        //}
        //Transform[] fi = hfiwhei.ToArray();
        //Rigidbody[] tfs = gos.Select((t) => t.GetComponent<Rigidbody>());
        //gos.Select((t) => t.GetComponent<Rigidbody>());
        //tfs.GetOtherComponets((t) => t.GetComponent<Rigidbody>());

        //int[] arr = { 50, 6, 6, 3 };
        //int sum = arr.Sum();
        //int sj = arr.Aggregate((a, b) => a * b);
        //float liw = 4;
        ////public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func);
        //float sojf = arr.Aggregate(liw, (l, t) => l + t);
        ////int tt = arr.Single(t => t % 6 == 0);
        //bool isfe = arr.Any();
        //IEnumerable<int> list = arr.Distinct();
        //foreach (var item in list)
        //{
        //    int a = item;
        //}
        //int[] att = list.ToArray();

        string[] arr = { "oaofjw", "herh", "afwio", "afowe" };
        string[] newArr = arr.OrderBy((t) => t[0]).ToArray();

        int[] aojoe = { 432, 34, 345, 4 };
        int[] ojfwe = { 454, 345, 34, 4 };
        int[] newint = aojoe.Intersect(ojfwe).ToArray();
        int[] nofw = aojoe.Union(ojfwe).ToArray();
    }
}