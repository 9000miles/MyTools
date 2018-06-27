using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumTest : MonoBehaviour
{
    [EnumFlags]
    public EEHHH eHHH;
    [Header("First")]
    public int age;
    public int owljo;
    [SerializeField]
    private int oaoejojw;
    public void Start()
    {
        int value = (int)eHHH;
        Debug.Log("枚举选中的值  " + value);
    }
}

public enum EEHHH
{
    None = 0,
    jofw = 1,
    jofwoe = 2,
    owej = 4,
    owoe = 8
}