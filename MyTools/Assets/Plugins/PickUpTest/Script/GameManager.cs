using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HighlightingSystem;
public class GameManager : MonoBehaviour
{

    public struct Preset
    {
        public string name;
        public int downsampleFactor;
        public int iterations;
        public float blurMinSpread;
        public float blurSpread;
        public float blurIntensity;
    }

    List<Preset> presets = new List<Preset>()
    {
        new Preset() { name = "Default",    downsampleFactor = 4,   iterations = 2, blurMinSpread = 0.65f,  blurSpread = 0.25f, blurIntensity = 0.3f },
        new Preset() { name = "Strong",     downsampleFactor = 4,   iterations = 2, blurMinSpread = 0.5f,   blurSpread = 0.15f, blurIntensity = 0.325f },
        new Preset() { name = "Wide",       downsampleFactor = 4,   iterations = 4, blurMinSpread = 0.5f,   blurSpread = 0.15f, blurIntensity = 0.325f },
        new Preset() { name = "Speed",      downsampleFactor = 4,   iterations = 1, blurMinSpread = 0.75f,  blurSpread = 0f,    blurIntensity = 0.35f },
        new Preset() { name = "Quality",    downsampleFactor = 2,   iterations = 3, blurMinSpread = 0.5f,   blurSpread = 0.5f,  blurIntensity = 0.28f },
        new Preset() { name = "Solid 1px",  downsampleFactor = 1,   iterations = 1, blurMinSpread = 1f,     blurSpread = 0f,    blurIntensity = 1f },
        new Preset() { name = "Solid 2px",  downsampleFactor = 1,   iterations = 2, blurMinSpread = 1f,     blurSpread = 0f,    blurIntensity = 1f }
    };
    public CastManager m_CastManager;
    public PickUIManager uIManager;
    public Action enoughAction;
    public InputManager inputManager;
    void Start()
    {
        uIManager = GameObject.FindObjectOfType<PickUIManager>();
        GameObject inputObj = new GameObject("InputManager");
        GameObject.DontDestroyOnLoad(inputObj);
        inputManager = inputObj.AddComponent<InputManager>();

        m_CastManager = CastManager.Instance;
        m_CastManager.actionA = new Action<GameObject>(outRance);
        m_CastManager.actionB = new Action<GameObject>(innerRance);
        m_CastManager.exitArea = new Action(OutAway);
    }
    private GameObject currentScanObj;
    //外环操作
    private void outRance(GameObject _scanObj)
    {
        //Debug.Log("外环");
        BrachOption(false, _scanObj);
        currentScanObj = _scanObj;
    }
    //内环操作
    private void innerRance(GameObject _scanObj)
    {
        //Debug.Log("内环");
        BrachOption(true, _scanObj);
        currentScanObj = _scanObj;
    }
    private void setScanObj(GameObject obj)
    {
        var item = obj.GetComponent<CheackItem>();
        if (item == null)
        {
            Debug.Log("检测物品无CheackItem组件：" + obj.name);
            return;
        }
    }
    bool isBreak;
    private void BrachOption(bool isNear, GameObject _scanObj)
    {
        var _item = _scanObj.GetComponent<CheackItem>();
        var pos = _item.transform;
        var entryType = _item.myEntryType;
        switch (entryType)
        {
            case EntryType.item:
                if (isNear)
                {
                    CloseHighLight(_scanObj);
                    uIManager.SetPosAndShow("Block", pos);
                }
                else
                {
                    var p = presets[0];
                    SetHighLight(_scanObj, new Color(1f, 0f, 0f, 0f), new Color(1f, 0f, 0f, 1f), p);
                }
                break;
            case EntryType.Event:
                if (isNear)
                {
                    if (!isBreak)
                        uIManager.SetPosAndShow("E", pos);
                    if (inputManager.isButtonKeyDown)
                    {
                        isBreak = true;
                        if (_item.isBreathed)
                            uIManager.SetPosAndShow("Break", pos);
                        else
                        {
                            Debug.Log("交互事件完成:");
                            _item.isFinish = true;
                        }
                    }
                }
                else
                    uIManager.SetPosAndShow("Point", pos);
                break;
        }
    }
    //出触发区域
    private void OutAway()
    {
        //Debug.Log("角色离开区域");
        uIManager.CloseAllUI();
        CloseHighLight(currentScanObj);
    }
    /// <summary>
    /// 设置高亮
    /// </summary>
    public void SetHighLight(GameObject _scanObj, Color _hightlightNew, Color _hightlightOld, Preset p)
    {
        HighlightingBase hb = FindObjectOfType<HighlightingBase>();
        if (hb == null) { return; }
        hb.downsampleFactor = p.downsampleFactor;
        hb.iterations = p.iterations;
        hb.blurMinSpread = p.blurMinSpread;
        hb.blurSpread = p.blurSpread;
        hb.blurIntensity = p.blurIntensity;
        MyHighlightingManager myHL = null;
        myHL = _scanObj.GetComponent<MyHighlightingManager>();
        if (myHL == null)
        {
            myHL = _scanObj.AddComponent<MyHighlightingManager>();
        }
        myHL.FlashOn(_hightlightNew, _hightlightOld);
    }
    /// <summary>
    /// 关闭高亮
    /// </summary>
    /// <param name="_ScanObj"></param>
    public void CloseHighLight(GameObject _ScanObj)
    {
       var HL= _ScanObj.GetComponent<MyHighlightingManager>();
        if (HL != null) HL.FlashOff();
    }
}
