using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
public class MyHighlightingManager : MonoBehaviour
{

    private Highlighter h;

    void Awake()
    {
        h = gameObject.GetComponent<Highlighter>();
        if (h == null) { h = gameObject.AddComponent<Highlighter>(); }
    }
    public void FlashOn(Color newColor, Color oldColor)
    {
        h.FlashingOn(newColor, oldColor);
    }
    public void FlashOff()
    {
        if (h.highlighted)
            h.FlashingOff();
        else return;
    }
}
