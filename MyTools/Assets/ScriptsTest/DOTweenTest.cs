using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YouLe
{
    ///  <summary>
    ///
    ///  </summary>
    public class DOTweenTest : MonoBehaviour
    {
        public Vector3 myValue = new Vector3(1500, 0, 0);
        public Vector3 myVector = new Vector3(0, 0, 0);
        public Transform myTransform;
        public RectTransform rect;
        private bool isIn;
        private Tweener tweener;
        private void Start()

        {
            //Vector3 myVector = myTransform.position;
            //对变量做一个动画，通过插值的方式去修改一个值的变化

            //DOTween.To(() => myValue, x => myValue = x, new Vector3(0, 0, 0), 5);//用两秒的时间从0,0,0变化到10,10,10
            //()=> myValue,表示返回值为myValue,x=>myValue=x,表示将系统计算好的x值(当前值到目标值的插值)赋给myValue,new Vector3(10,0,0),表示达到的目标值，2表示所需时间
            DOTween.To(() => myVector, x => myVector = x, new Vector3(10, 0, 0), 2);//用两秒的时间从0,0,0变化到10,10,10
            tweener = rect.DOLocalMove(new Vector3(100, 0, 0), 1f);
            tweener.SetAutoKill(false);
            tweener.Pause();
        }
        private void Update()
        {
            //transform.localPosition = myVector;
        }
        public void Move()
        {
            if (isIn == false)
            {
                tweener.PlayForward();
                isIn = true;
            }
            else
            {
                tweener.PlayBackwards();
                isIn = false;
            }
        }
    }
}