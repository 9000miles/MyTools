using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using Common;
using System.Linq;

public class MyScriptTestMenu
{
    private static TxtHelper txtHelper;

    [MenuItem("My Script Test Menu/Execute Test", false, 1)]
    private static void ExecuteTest()
    {
        txtHelper = new TxtHelper(@"E:\360Downloads\偶偶奇偶奇偶额外", "奇偶奇偶反文旁.txt");
        txtHelper.Write("654456449849494");
        txtHelper.Write("joifowjeojfojweoijf");
        txtHelper.Write("jofjwoejofjeownononohtht");
        txtHelper.Write("pohperpjoijioew");
        txtHelper.Write("eojoiogoreooi");
    }
}