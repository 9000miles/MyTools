using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public class MyScriptTestMenu
{
    [MenuItem("My Script Test Menu/Execute Test", false, 1)]
    private static void ExecuteTest()
    {
        EnumTest test = new EnumTest();
        test.Start();
    }
}