using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OdinInspectorTest : MonoBehaviour
{
    public string EnemyName;
    public List<Enemy> enemies = new List<Enemy>();

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