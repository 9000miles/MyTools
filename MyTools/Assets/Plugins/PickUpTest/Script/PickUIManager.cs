using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class PickUIManager : MonoBehaviour
{
    public Image[] images;
    public float offset = 1.5f;
    private Dictionary<string, GameObject> items = new Dictionary<string, GameObject>();
    // Use this for initialization
    void Start()
    {
        images = this.transform.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            var key = images[i].gameObject.name;
            items.Add(key, images[i].gameObject);
        }
        this.GetComponent<CanvasGroup>().alpha = 1;
        CloseAllUI();
        // ShowUI("E");
    }
    /// <summary>
    /// 显示
    /// </summary>
    /// <param _name="_name">通过CheackIteam Name</param>
    public void ShowUI(string _name)
    {
        GameObject value;
        CloseAllUI();
        if (items.TryGetValue(_name, out value))
        {
            if (!value.gameObject.activeSelf)
                value.gameObject.SetActive(true);
        }
    }
    public void CloseAllUI()
    {
        var itemEnum = items.GetEnumerator();
        while (itemEnum.MoveNext())
        {
            var item = itemEnum.Current.Value;
            if (item.activeSelf)
            {
                item.gameObject.SetActive(false);
                continue;
            }
        }
    }

    public void SetPosAndShow(string _uiName, Transform trans)
    {
        GameObject uiObj;
        if (items.TryGetValue(_uiName, out uiObj))
        {
            uiObj.transform.position = new Vector3(trans.position.x, trans.position.y + offset, trans.position.z);
        }
        ShowUI(_uiName);
    }
}
