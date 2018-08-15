using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MarsPC
{
    public class UIFocusPoint : MonoBehaviour, IPointerDownHandler
    {
        private bool isDestroyed;

        public async void StartTimer(float time)
        {
            Transform timer = transform.Find("Timer");
            float totalTime = time;
            while (time >= 0)
            {
                if (isDestroyed) return;
                timer.GetComponent<Image>().fillAmount -= 1f / totalTime * Time.deltaTime;
                await new WaitForUpdate();
                time -= Time.deltaTime;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDestroyed = true;
            Destroy(this);
            Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }
}