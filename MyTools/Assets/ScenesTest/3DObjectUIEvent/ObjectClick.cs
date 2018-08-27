using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("pointerPress            " + eventData.pointerPress);
        //Debug.Log("pointerDrag            " + eventData.pointerDrag);
        //Debug.Log("rawPointerPress            " + eventData.rawPointerPress);
        //Debug.Log("lastPress            " + eventData.lastPress);
        //Debug.Log("pointerEnter            " + eventData.pointerEnter);
    }

    private void Start()
    {
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    GameObject go = OnePointColliderObject();
        //    if (go != null)
        //        Debug.Log("                  ---------------" + go.name);
        //}
    }

    /// <summary>
    /// 获取鼠标点击的UI对象
    /// </summary>
    /// <returns>点击的UI对象</returns>
    public GameObject OnePointColliderObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0 ? results[0].gameObject : null;
    }
}