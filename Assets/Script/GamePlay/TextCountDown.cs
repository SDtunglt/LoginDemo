using System;
using System.Collections;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TextCountDown : MonoBehaviour
{
    [SerializeField] private TMP_Text cdTxt;
    private float deltaTime = 1;
    private Coroutine cd;
    public bool isOnCountDown;

    public void ShowCountDown(float timeCd)
    {
        this.Show();
        if (cd != null)
        {
            StopCoroutine(cd);
        }

        cd = StartCoroutine(ICountDown(timeCd));
    }

    public void ShowCountDownWithCallback(float timeCd, Action callBack)
    {
        this.Show();
        if (cd != null)
        {
            StopCoroutine(cd);
        }

        cd = StartCoroutine(ICountDownWithCallBack(timeCd, callBack));
    }

    IEnumerator ICountDownWithCallBack(float  timeCd, Action callBack)
    {
        isOnCountDown = true;
        var wt = new WaitForSeconds(deltaTime);
        var t = timeCd;
        cdTxt.text = (int) t + "";
        while (t > 0)
        {
            yield return wt;
            t -= deltaTime;
            cdTxt.text = (int) t + "";
        }

        callBack.Invoke();
        this.Hide();
    }
    
    IEnumerator ICountDown(float timeCd)
    {
        isOnCountDown = true;
        var wt = new WaitForSeconds(deltaTime);
        var t = timeCd;
        cdTxt.text = (int) t + "";
        while (t > 0)
        {
            yield return wt;
            t -= deltaTime;
            cdTxt.text = (int) t + "";
        }

        this.Hide();
    }

    private void OnDisable()
    {
        this.Hide();
        isOnCountDown = false;
        if (cd != null)
        {
            StopCoroutine(cd);
        }
    }
}
