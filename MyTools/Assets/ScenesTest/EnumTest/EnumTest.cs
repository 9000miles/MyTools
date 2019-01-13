using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumTest : MonoBehaviour
{
    //[EnumFlags]
    public EEHHH eHHH;
    [Header("First")]
    public int age;
    public int owljo;
    [SerializeField]
    private int oaoejojw;

    [ContextMenuItem("add testName", "Start")]
    public Transform point;
    private bool isDraw;

    [ContextMenu("Test Function")]
    public void Start()
    {
        int value = (int)eHHH;
        Debug.Log("枚举选中的值  " + value);
    }

    private void Update()
    {
        if (!isDraw)
        {
            isDraw = !isDraw;
            Debug.DrawLine(Vector3.zero, point.position, Color.red);
        }
    }
}

public enum EEHHH
{
    None = 0,
    oweio = 6,
    jofoiwejfo = 1,
    jofw = 2,
    jofwoe = 3,
    owej = 4,
    owoe = 5
}