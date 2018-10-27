using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignUI : MonoBehaviour
{
    public string btn1;
    public string btn2;
    public string btn3;
    private List<string> btnStr = new List<string>();
    // Use this for initialization
    void Start()
    {
        btnStr.Add(btn1);
        btnStr.Add(btn2);
        btnStr.Add(btn3);
    }
    private int h = 30;
    private void OnGUI()
    {
        float ox = Screen.width - 140f;
        float oy = 10f;
        for (int i = 0; i < 3; i++)
        {
            if (GUI.Button(new Rect(ox, oy + 20f + i * h, 120, 20), btnStr[i]))
            {

            }
        }
    }
}
