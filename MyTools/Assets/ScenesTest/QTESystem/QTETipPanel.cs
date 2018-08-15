using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MarsPC;

namespace MarsPC
{
    public class QTETipPanel : SingletonBehaviour<QTETipPanel>
    {
        #region Test

        private bool isPlaying;
        private int clickCount = 1;
        public Image succedTip;
        public Image failureTip;
        public Transform enterQTE;
        public Transform singleKeyContinue;
        public Transform singleKeyRhythm;
        public Transform doubleKeyRepeat;
        public Transform linearClick;
        public Transform linearDirection;
        public Transform scrollBar;
        public Transform powerGauge;
        public Transform mouseGestures;
        public Transform focusPoint;
        private Tweener tweener = null;
        private Tweener scrollBarTweener = null;

        #endregion Test

        private void Start()
        {
            succedTip.gameObject.SetActive(false);
            failureTip.gameObject.SetActive(false);
            QTEManager.Singleton.succedCall += SuccedTip;
            QTEManager.Singleton.failureCall += FailureTip;
            QTEManager.Singleton.subOptimalCall += Pring;
            enterQTE.GetComponent<Button>().onClick.AddListener(EnterQTEOnCall);
        }

        private void EnterQTEOnCall()
        {
            QTEManager.Singleton.ActiveQTE();
        }

        private void Pring()
        {
            Debug.Log("SubOptimalCall");
        }

        private void Update()
        {
        }

        public void SuccedTip()
        {
            succedTip.gameObject.SetActive(true);
            HideTip();
        }

        public void FailureTip()
        {
            failureTip.gameObject.SetActive(true);
            HideTip();
        }

        public void ShowEnterQTEButton(bool isShow)
        {
            enterQTE.gameObject.SetActive(isShow);
        }

        public void ShowSingleKeyContinue(bool isShow)
        {
            if (isShow)
            {
                if (singleKeyContinue.gameObject.activeInHierarchy == false)
                    singleKeyContinue.gameObject.SetActive(true);
                singleKeyContinue.Find("Image").transform.DOPunchScale(new Vector3(-0.3f, -0.3f, -0.3f), 0.1f);
                singleKeyContinue.transform.Find("Text").GetComponent<Text>().text = clickCount++.ToString();
            }
            else
            {
                singleKeyContinue.gameObject.SetActive(false);
                clickCount = 1;
            }
        }

        public void ShowSingleKeyRhythm(bool isShow, params object[] parameter)
        {
            if (isShow)
            {
                if ((bool)parameter[0])
                {
                    tweener.Restart();
                }
                else
                {
                    singleKeyRhythm.gameObject.SetActive(true);
                    tweener = singleKeyRhythm.Find("OuterRing").DOScale(new Vector3(0, 0, 0), (float)parameter[1]);
                    tweener.SetEase(Ease.Linear);
                }
            }
            else
            {
                singleKeyRhythm.gameObject.SetActive(false);
                tweener.Pause();
                singleKeyRhythm.Find("OuterRing").localScale = Vector3.one;
            }
        }

        public void ShowDoubleKeyRepeat(bool isShow, params object[] parameter)
        {
            if (isShow)
            {
                doubleKeyRepeat.gameObject.SetActive(true);
                doubleKeyRepeat.Find("Left").gameObject.SetActive(true);
                doubleKeyRepeat.Find("Right").gameObject.SetActive(true);
                if ((bool)parameter[0])
                {
                    doubleKeyRepeat.Find("Left/Text").GetComponent<Text>().text = parameter[1].ToString();
                    doubleKeyRepeat.Find("Left").DOPunchScale(new Vector3(-0.3f, -0.3f, -0.3f), 0.1f);
                }
                else
                {
                    doubleKeyRepeat.Find("Right/Text").GetComponent<Text>().text = parameter[1].ToString();
                    doubleKeyRepeat.Find("Right").DOPunchScale(new Vector3(-0.3f, -0.3f, -0.3f), 0.1f);
                }
            }
            else
            {
                doubleKeyRepeat.gameObject.SetActive(false);
                doubleKeyRepeat.Find("Right").gameObject.SetActive(false);
                doubleKeyRepeat.Find("Right").gameObject.SetActive(false);
            }
        }

        public async void ShowLinearClick(bool isShow, params object[] parameter)
        {
            if (isShow)
            {
                linearClick.gameObject.SetActive(true);
                if ((bool)parameter[0])
                {
                    linearClick.Find("Click").DOPunchScale(new Vector3(-0.3f, -0.3f, -0.3f), 0.1f);
                    await new WaitForSeconds(1);
                    linearClick.transform.gameObject.SetActive(false);
                }
            }
            else
            {
                linearClick.gameObject.SetActive(false);
            }
        }

        public async void ShowLinearDirection(bool isShow, params object[] parameter)
        {
            if (isShow)
            {
                linearDirection.transform.gameObject.SetActive(true);
                for (int i = 0; i < linearDirection.childCount; i++)
                {
                    linearDirection.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                }
                Transform dirctionImage = linearDirection.Find(parameter[1].ToString());
                dirctionImage.GetComponent<Image>().color = new Color(1, 0, 0, 1);
                if ((bool)parameter[0])
                {
                    dirctionImage.DOPunchScale(new Vector3(-0.3f, -0.3f, -0.3f), 0.1f);
                    await new WaitForSeconds(1);
                    linearDirection.transform.gameObject.SetActive(false);
                }
            }
            else
            {
                linearDirection.transform.gameObject.SetActive(false);
            }
        }

        public EQTEResult ShowScrollBar(bool isShow, params object[] parameter)
        {
            RectTransform scrollBarRect = scrollBar.Find("Image").GetComponent<RectTransform>();
            RectTransform bar = scrollBar.Find("Bar").GetComponent<RectTransform>();
            RectTransform center = scrollBar.Find("Center").GetComponent<RectTransform>();
            RectTransform right = scrollBar.Find("Right").GetComponent<RectTransform>();
            RectTransform left = scrollBar.Find("Left").GetComponent<RectTransform>();
            if (isShow)
            {
                if (isPlaying) return EQTEResult.None;
                scrollBar.gameObject.SetActive(true);
                bar.localPosition = new Vector3(-(scrollBarRect.rect.width / 2f - bar.rect.width / 2f), bar.localPosition.y, bar.localPosition.z);
                scrollBarTweener = bar.DOLocalMoveX((scrollBarRect.rect.width - bar.rect.width) / 2f, Time.deltaTime * (float)parameter[0]);
                scrollBarTweener.SetLoops(-1, LoopType.Yoyo);
                scrollBarTweener.SetEase(Ease.Linear);
                isPlaying = true;
            }
            else
            {
                isPlaying = false;
                scrollBarTweener?.Pause();
                scrollBar.gameObject.SetActive(false);
                float ratio = (scrollBarRect.rect.width / 2f + bar.localPosition.x) / scrollBarRect.rect.width;
                bool isInLeft = bar.localPosition.x >= left.localPosition.x - left.rect.width / 2f && bar.localPosition.x <= left.localPosition.x + left.rect.width / 2f;
                bool isInRight = bar.localPosition.x >= right.localPosition.x - right.rect.width / 2f && bar.localPosition.x <= right.localPosition.x + right.rect.width / 2f;
                bool isCenter = bar.localPosition.x >= center.localPosition.x - center.rect.width / 2f && bar.localPosition.x <= center.localPosition.x + center.rect.width / 2f;
                if (isInLeft || isInRight)
                    return EQTEResult.Failure;
                else if (isCenter)
                    return EQTEResult.Succed;
                else
                    return EQTEResult.SubOptimal;
            }
            return EQTEResult.None;
        }

        public async void ShowPowerGauge(bool isShow, params object[] parameter)
        {
            Transform cursor = powerGauge.Find("Cursor");
            Transform articleCharged = powerGauge.Find("ArticleCharged");
            if (isShow)
            {
                powerGauge.gameObject.SetActive(true);
                if (isPlaying == false)
                {
                    float duration = (float)parameter[0];
                    while (duration > 0)
                    {
                        isPlaying = true;
                        articleCharged.GetComponent<Image>().fillAmount += 1f / (float)parameter[0] * Time.deltaTime;
                        await new WaitForUpdate();
                        duration -= Time.deltaTime;
                    }
                    powerGauge.gameObject.SetActive(false);
                }
                else
                {
                    if ((bool)parameter[1])
                    {
                        if (parameter.Length >= 4)
                            powerGauge.Find((string)parameter[3]).GetComponent<Selectable>().Select();
                        cursor.localRotation *= Quaternion.Euler(0, 0, (float)parameter[2]);
                    }
                }
            }
            else
            {
                isPlaying = false;
                cursor.localRotation = Quaternion.identity;
                articleCharged.GetComponent<Image>().fillAmount = 0;
                powerGauge.gameObject.SetActive(false);
            }
        }

        public async void ShowMouseGestures(bool isShow, params object[] parameter)
        {
            if (isShow)
            {
                mouseGestures.gameObject.SetActive(true);
                Transform instruction = mouseGestures.Find("Instruction");
                switch ((EMouseGesturesType)parameter[2])
                {
                    case EMouseGesturesType.LeftSlide:
                        instruction.localRotation = Quaternion.Euler(0, 0, 0);
                        break;

                    case EMouseGesturesType.LeftUpSlide:
                        instruction.localRotation = Quaternion.Euler(0, 0, -45);
                        break;

                    case EMouseGesturesType.LeftDownSlide:
                        instruction.localRotation = Quaternion.Euler(0, 0, 45);
                        break;

                    case EMouseGesturesType.RightSlide:
                        instruction.localRotation = Quaternion.Euler(0, 0, 180);
                        break;

                    case EMouseGesturesType.RightUpSilde:
                        instruction.localRotation = Quaternion.Euler(0, 0, -135);
                        break;

                    case EMouseGesturesType.RightDownSlide:
                        instruction.localRotation = Quaternion.Euler(0, 0, 135);
                        break;

                    case EMouseGesturesType.UpSlide:
                        instruction.localRotation = Quaternion.Euler(0, 0, -90);
                        break;

                    case EMouseGesturesType.DownSlide:
                        instruction.localRotation = Quaternion.Euler(0, 0, 90);
                        break;
                }
                if ((bool)parameter[1])
                {
                    isPlaying = true;
                    Text time = mouseGestures.Find("Time").GetComponent<Text>();
                    float duration = (float)parameter[0];
                    while (duration >= 0)
                    {
                        if (isPlaying == false) return;
                        time.text = duration.ToString();
                        await new WaitForSeconds(1);
                        duration--;
                    }
                }
            }
            else
            {
                isPlaying = false;
                mouseGestures.gameObject.SetActive(false);
            }
        }

        public void ShowFocusPoint(bool isShow, params object[] parameter)
        {
            if (isShow)
            {
                focusPoint.gameObject.SetActive(true);
                for (int i = 0; i < focusPoint.childCount; i++)
                {
                    focusPoint.GetChild(i).GetComponent<UIFocusPoint>().StartTimer((float)parameter[0]);
                }
            }
            else
            {
                isPlaying = false;
                focusPoint.gameObject.SetActive(false);
            }
        }

        private async void HideTip()
        {
            await new WaitForSeconds(2);
            succedTip.gameObject.SetActive(false);
            failureTip.gameObject.SetActive(false);
        }
    }
}