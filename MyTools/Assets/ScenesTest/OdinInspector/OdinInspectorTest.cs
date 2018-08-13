using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class OdinInspectorTest : MonoBehaviour
{
    public string EnemyName;
    public List<Enemy> enemies = new List<Enemy>();
    public bool isShow;

    [ShowIf("isShow")]
    public string nameMy;
    [MinValue(30)]
    public int age = 30;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}

public class Enemy
{
    private string name;
}