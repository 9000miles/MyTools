using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using MarsPC;

public class FindChildTest : TestBase
{
    public override void Pringt()
    {
        base.Pringt();
        Debug.Log(age);
    }

    //private void Start()
    //{
    //    print("查找到第一个子物体   " + transform.FindChildByName("GunTF").parent.parent.name);
    //    print("根据父物体查找子物体   " + transform.FindChildByName("Right", "GunTF").parent.parent.name);
    //}
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Pringt();
        }
    }
}

public class TestBase : MonoBehaviour
{
    public int age = 3;

    public virtual void Pringt()
    {
        if (age > 5) return;
    }
}