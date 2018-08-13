using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

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
    private Tweener tweener = null;
    private Tweener scrollBarTweener = null;

    #endregion Test

    private void Start()
    {
        succedTip.gameObject.SetActive(false);
        failureTip.gameObject.SetActive(false);
        QTEManager.Singleton.succedCall += SuccedTip;
        QTEManager.Singleton.failureCall += FailureTip;
        enterQTE.GetComponent<Button>().onClick.AddListener(QTEManager.Singleton.ActiveQTE);
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

    public QTEResult ShowScrollBar(bool isShow, params object[] parameter)
    {
        RectTransform scrollBarRect = scrollBar.Find("Image").GetComponent<RectTransform>();
        RectTransform bar = scrollBar.Find("Bar").GetComponent<RectTransform>();
        RectTransform right = scrollBar.Find("Right").GetComponent<RectTransform>();
        RectTransform left = scrollBar.Find("Left").GetComponent<RectTransform>();
        if (isShow)
        {
            if (isPlaying) return QTEResult.None;
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
            float leftRatio = (scrollBarRect.localPosition.x - left.localPosition.x + left.rect.width / 2f) / scrollBarRect.rect.width;
            float rightRatio = (scrollBarRect.localPosition.x + right.localPosition.x - right.rect.width / 2f) / scrollBarRect.rect.width;
            if (isInLeft || isInRight)
                return QTEResult.Failure;
            else
                return QTEResult.Succed;
        }
        return QTEResult.None;
    }

    private async void HideTip()
    {
        await new WaitForSeconds(2);
        succedTip.gameObject.SetActive(false);
        failureTip.gameObject.SetActive(false);
    }
}