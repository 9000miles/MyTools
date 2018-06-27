using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListTest : MonoBehaviour
{
    public void Start()
    {
        //List<int> list = new List<int>(5);
        //for (int i = 0; i < list.Count; i++)
        //{
        //    Debug.Log(44);
        //}
        //Person[] people = new Person[] { new Person(), new Person(), new Person() };

        Person person = new Person();
        person[2] = "jof";
        person["ofo"] = "oojowej";
    }
}

public class Person
{
    private string[] personName = new string[5];
    private Dictionary<string, string> dic = new Dictionary<string, string>();

    public Person()
    {
        Init();
    }

    private void Init()
    {
        dic.Add("ofo", ")");
    }

    public string this[int index] { get { return personName[index]; } set { personName[index] = value; } }
    public string this[string index] { get { return dic[index]; } set { dic[index] = value; } }
}