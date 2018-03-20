using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YouLe
{
    ///  <summary>
    ///  ScrollView点选位置向2边扩张，指定范围内显示，其余范围不显示
    ///  </summary>
    public class ScrollViewTest : MonoBehaviour
    {
        public GameObject go;
        public int num = 0;
        private GameObject[] objArray;
        public GameObject gogo;
        public Scrollbar scrollbar;
        private void Awake()
        {
            objArray = new GameObject[num];
            CreatImage(num);
        }
        private void Start()
        {
            //scrollbar.onValueChanged.AddListener(ShowItem);//会产生报错，不明原因
            ShowItem(scrollbar.value);
            //MyCommonTools.CommonTools.Sort(array, (a, b) => a > b);
            //MyCommonTools.CommonTools.Sort(mytransform, (a, b) => a.childCount > b.childCount);
            //MyCommonTools.CommonTools.Sort(stringArr, (a, b) => a.Length > b.Length);
            //MyCommonTools.CommonTools.Sort(test, a => a.age);
            //MyCommonTools.CommonTools.Sort(test, a => a.age, false);
        }

        /// <summary>
        /// 获取鼠标点击的对象
        /// </summary>
        /// <returns>点击对象</returns>
        public GameObject OnePointColliderObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0 ? results[0].gameObject : null;
        }
        public int index;
        public int offsetNum;
        private void Update()
        {
            ShowItem(scrollbar.value);//不会报错，运行正常
        }
        /// <summary>
        /// 判断鼠标是否目标物上，如果在，则取当前UI的名字，调用显示函数
        /// </summary>
        //private void ShowHideController(float arg0)
        //{
        //    ShowItem((int)(50 - scrollbar.value * 100 * 0.5));
        //    //            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        //    //            {
        //    //#if UNITY_ANDROID || UNITY_IPHONE
        //    //            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        //    //#else
        //    //                if (EventSystem.current.IsPointerOverGameObject())
        //    //#endif
        //    //                {
        //    //gogo = OnePointColliderObject();
        //    //if (gogo != null && gogo.tag.Equals("UI"))
        //    //{
        //    //    index = int.Parse(gogo.name);
        //    //    //ShowItem(index);
        //    //    ShowItem((int)(50 - scrollbar.value * 100 * 0.5));
        //    //}
        //    //    }
        //    //}
        //}
        /// <summary>
        /// 指定显示哪些UI，隐藏其余UI
        /// </summary>
        /// <param name="index"></param>
        private void ShowItem(float value)
        {
            int index = (int)(num - value * num);
            int startIndex = index - offsetNum > 0 ? index - offsetNum : 0;
            int endIndex = index + offsetNum < objArray.Length ? index + offsetNum : objArray.Length;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (i >= startIndex && i <= endIndex)
                    SetItemState(i, true);
                else
                    SetItemState(i, false);
            }
        }

        private void SetItemState(int index, bool isEnable)
        {
            transform.GetChild(index).GetComponent<Image>().enabled = isEnable;
            transform.GetChild(index).GetComponentInChildren<Text>().enabled = isEnable;
        }

        ///            Trying to remove 6 (UnityEngine.UI.Text) from rebuild list while
        ///             we are already inside a rebuild loop. This is not supported.
        /// <summary>
        /// 生成UI
        /// </summary>
        /// <param name="num"></param>
        private void CreatImage(int num)
        {
            for (int i = 0; i < num; i++)
            {
                objArray[i] = Instantiate(go);
                objArray[i].name = i.ToString();
                objArray[i].GetComponentInChildren<Text>().text = i.ToString();
                objArray[i].transform.GetChild(0).name = i.ToString();
                objArray[i].transform.SetParent(transform);
            }
        }
    }
}