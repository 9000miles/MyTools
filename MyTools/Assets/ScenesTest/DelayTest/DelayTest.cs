using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DelayTest : MonoBehaviour
{
    private Image image;
    public float time = 1;

    // Use this for initialization
    private void Start()
    {
        image = GetComponent<Image>();
        //StartCoroutine(Test());
        TestAsync();
    }

    private IEnumerator Test()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("jojofwoejo");
    }

    private async void TestAsync()
    {
        await new WaitForSeconds(2);
        Debug.Log("jojofwoejo");
    }

    private Tweener tweener;

    // Update is called once per frame
    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        //Fade(time);
    //        //StartCoroutine(FadeEnumerator(time));
    //        image.DOFade(0, time);
    //        //transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), time, 10, 0).Flip();
    //        //tweener = transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), time, 10, 0).Flip();
    //    }
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        image.color = new Color(1, 1, 1, 1);
    //        tweener.Pause();
    //    }
    //}

    private IEnumerator FadeEnumerator(float time)
    {
        Debug.Log("Start      " + Time.time);
        float a = image.color.a;
        while (image.color.a > 0)
        {
            yield return null;
            a -= Time.deltaTime / time;
            image.color = new Color(image.color.r, image.color.g, image.color.b, a);
        }
        Debug.Log("End                          " + Time.time);
    }

    private async void Fade(float time)
    {
        Debug.Log("Start      " + Time.time);
        float a = image.color.a;
        while (image.color.a > 0)
        {
            await new WaitForUpdate();
            a -= Time.deltaTime / time;
            image.color = new Color(image.color.r, image.color.g, image.color.b, a);
        }
        Debug.Log("End                          " + Time.time);
    }
}