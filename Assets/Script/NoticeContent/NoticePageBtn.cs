using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticePageBtn : MonoBehaviour
{
    NoticePopUp noticePopUp;
    int pageIndex;

    private Toggle checkGo;


    void Awake()
    {
        checkGo = GetComponent<Toggle>();
    }
    
    public NoticePageBtn Init(NoticePopUp noticePopUp,int pageIndex)
    {
        this.noticePopUp = noticePopUp;
        this.pageIndex = pageIndex;
        gameObject.SetActive(true);
        return this;
    }

    public void Check(bool check)
    {
        checkGo.isOn = check;
    }

    public void Scroll()
    {
        noticePopUp.Scroll(pageIndex);
    }
}
