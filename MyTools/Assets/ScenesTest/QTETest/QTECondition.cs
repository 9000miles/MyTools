using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QTECondition : MonoBehaviour
{
    public string description;
    [HideInInspector]
    public Transform ower;
    public bool isTrue;
    public QTEInfo info;

    private void Start()
    {
        ower = transform;
    }

    protected abstract bool Check();

    public void CheckIsTrue()
    {
        isTrue = Check();
    }
}