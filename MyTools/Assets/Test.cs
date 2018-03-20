using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;
using Baidu.Aip.Ocr;

public class Test : MonoBehaviour
{
    private Baidu.Aip.Ocr.Ocr client;
    private void Start()
    {
        // 设置APPID/AK/SK
        var APP_ID = "10966743";
        var API_KEY = "12Q6kkw1S9vGDzWCOe8iL3D0";
        var SECRET_KEY = "hLpCDBVzfasayF1AHfDLhC28O7HgBueE";

        client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
        GeneralBasicDemo();
    }
    public void GeneralBasicDemo()
    {
        var client = new Ocr("Api Key", "Secret Key");
        var image = File.ReadAllBytes(@"C:\Users\Administrator\Desktop\新建文件夹\识别.png");

        // 通用文字识别
        var result = client.GeneralBasic(image);

        //var image = File.ReadAllBytes(@"C:\Users\Administrator\Desktop\新建文件夹\识别.png");
        //// 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
        //var result = client.GeneralBasic(image);
        //Console.WriteLine(result);
        // 如果有可选参数
        var options = new Dictionary<string, object>{
                {"language_type", "CHN_ENG"},
                {"detect_direction", "true"},
                {"detect_language", "true"},
                {"probability", "true"}};
        // 带参数调用通用文字识别, 图片参数为本地图片
        result = client.GeneralBasic(image, options);
        string str = result.ToString();
        //Console.WriteLine(result);
    }
    private Person[] students = new Person[4];
    private Person studens = new Student(151, 1616.46f, "oijfo");

    private void Start1()
    {
        students[0] = new Person(23, 345.2f, "jofwe", true);
        students[1] = new Person(56, 354.35f, "fer4", false);
        students[2] = new Person(378, 64533.34f, "gerg", false);
        students[3] = new Person(5474, 23.3f, "shrt", true);

        //MyCommonTools.CommonTools.Sort(students, a => a.arrayInt);
        MyCommonTools.CommonTools.Sort(students, a => a.str);
        MyCommonTools.CommonTools.Sort(students, a => a.age, false);

        MyCommonTools.CommonTools.Sort(students, a => a.atk);
        MyCommonTools.CommonTools.Sort(students, a => a.atk, false);

        MyCommonTools.CommonTools.Sort(students, a => a.str.Length);
        MyCommonTools.CommonTools.Sort(students, a => a.str.Length, false);
    }
}

public class Student : Person
{
    //int agee = 23;
    public Student(int age, float atk, string str) : base()
    {
    }
    //public Student(int age, float atk, string str)
    //{
    //    this.age = age;
    //    this.atk = atk;
    //    this.str = str;
    //}
}

//public class Teacher : Person
//{
//}

public class Person
{
    public int age;
    public float atk;
    public string str;
    public bool jsie;
    public int[] arrayInt;
    public Person()
    {
    }

    public Person(int age, float atk, string str, bool js)
    {
        this.age = age;
        this.atk = atk;
        this.str = str;
        this.jsie = js;
    }
    //public Person()
    //{ }
    //public List<Transform> arrayList;
    //public void Person
    //{
    //}
}

#region MyRegion

//// Use this for initialization
//public int age;
//private void Start()
//{
//    //AssetBundle assetBundle = new AssetBundle();
//    //assetBundle.Unload();
//    //SceneManager sceneManager = new SceneManager();
//    //SceneManager.LoadScene();
//    //this.gameObject.GetComponent<Text>().text = "<size=60><color=#0000ff>小明</color></size>送了<color=#ff00ff><size=60>小红</size></color>一辆游艇";
//    //this.gameObject.GetComponent<Text>().text = "<size=30>Some <color=yellow>RICH</color> text</size>";
//    this.gameObject.GetComponent<Text>().text = "<color=#ffffffaa><b><size=70>李</size></b><size=30><i>自</i></size></color><color=red>军</color>";
//}
//private Button button;
////private void Start()
////{
////    button = transform.GetComponent<Button>();
////    button.onClick.AddListener(DFunc);
////}
//private delegate void funcrt(int i);
//private funcrt fucrt;
////private void Start()
////{
////    //int count = transform.childCount;
////    //for (int i = 0; i < count; i++)
////    //{
////    //    DestroyImmediate(transform.GetChild(0).gameObject);
////    //}
////    for (int i = 0; i < 5; i++)
////    {
////        fucrt = new funcrt((t) => print(i));
////        //fucrt(() => { print(i); });
////    }
////}
//public GameObject gogo;
//private void DFunc()
//{
//    //gogo.;
//    Destroy(gogo.transform, 1);
//    //DestroyImmediate(gogo, false);
//    //DontDestroyOnLoad(gogo);
//}
#endregion MyRegion