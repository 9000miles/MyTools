using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TestType
{
   A,
   B
}
public class Displayer : MonoBehaviour
{
    public TestType type;

    public A instanceA;

    public B instanceB;
	 
}

public class A
{
    public string a;
}

public class B
{
    public string b;
}