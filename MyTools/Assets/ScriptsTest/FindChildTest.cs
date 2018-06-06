﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyCommonTools;
public class FindChildTest : MonoBehaviour
{
    void Start()
    {
        print("查找到第一个子物体   " + transform.FindChildByName("GunTF").parent.parent.name);
        print("根据父物体查找子物体   " + transform.FindChildByName("Right", "GunTF").parent.parent.name);
    }
}