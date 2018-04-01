using UnityEngine;
using System.Collections;

using UnityEngine.iOS;
using admob;

public class AdsMobile : MonoBehaviour
{
    public string adUnitId = "ca-app-pub-1528318812740485/7686981059";
    private Admob ad;
    //private BannerView bannerView;
    // Use this for initialization
    private void Start()
    {
        Admob.Instance().initAdmob("ca-app-pub-3940256099942544/2934735716", "ca-app-pub-3940256099942544/4411468910");// id is got from apps.admob.com
        Admob.Instance().loadInterstitial();
    }
    public void Func()
    {
        if (Admob.Instance().isInterstitialReady())
            Admob.Instance().showInterstitial();
        else
            Admob.Instance().loadInterstitial();

        //if (Admob.Instance().isInterstitialReady())
        //{
        //    Admob.Instance().showInterstitial();
        //}
        //else
        //{
        //    Admob.Instance().loadInterstitial();
        //}
    }
}