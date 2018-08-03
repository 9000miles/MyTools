using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTETipPanel : MonoBehaviour
{
    #region Test

    public Image succedTip;
    public Image failureTip;
    public GameObject testObject;

    #endregion Test

    private void Start()
    {
        succedTip.enabled = false;
        failureTip.enabled = false;
        QTEManager.Singleton.succedCall += SuccedTip;
        QTEManager.Singleton.failureCall += FailureTip;
    }

    private void Update()
    {
    }

    public void SuccedTip()
    {
        succedTip.enabled = true;
        HideTip();
    }

    public void FailureTip()
    {
        failureTip.enabled = true;
        HideTip();
    }

    private async void HideTip()
    {
        await new WaitForSeconds(3);
        succedTip.enabled = false;
        failureTip.enabled = false;
    }
}