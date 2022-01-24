using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NoticePopUp : MonoBehaviour
{
    public GameObject ContentPrefab;
    public Transform PageContentContainer;
    public Transform PageBtnContainer;
    public float contentMove;
    private float move = 50f;

    private List<NoticeContent> contentList;
    private List<NoticePageBtn> pageBtns;

    public NoticePageBtn prefabPageBtn = null;
    public Button prevBtn;
    public Button nextBtn;


    public Toggle AutoNotifyToggle;
    int index; 
    bool ready = true;
    
    void Start()
    {
        prevBtn.onClick.AddListener(Prev);
        nextBtn.onClick.AddListener(Next);
        AutoNotifyToggle.onValueChanged.AddListener(ToggleAutoNotify);
    }


    public void Open()
    {
        if (((GameModel.Instance.IsNormalPlayer() && GameModel.Instance.payEnable != 2) || GameUtils.IsWeb()))
        {
            API.GetNotice(data =>
            {
                var token = data.GetValue("notice");
                var noticeModels = token.ToObject<List<NoticeModel>>();
                Debug.Log(data);
                foreach (var model in noticeModels)
                {
                    model.Init();
                }

                noticeModels = noticeModels.Where(s => !s.isPayNotice).ToList();
                if (!noticeModels.Any()) return;
                HttpUtils.RequestMultipleTextures(noticeModels.Select(s => s.imgURL).ToList(), list =>
                {
                    try
                    {
                        if (list == null || list.Count == 0)
                            return;
                        foreach (var t in noticeModels)
                        {
                            t.tex = list[t.imgURL];
                        }

                        ShowView(noticeModels);
                        /*ViewCreator.OpenPopup(PopupId.NoticePopup,
                        view => { view.Cast<NoticePopup>().ShowView(noticeModels); });*/
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                });

            }, s =>
            {

                SDLogger.Log(s);
            });
        }
    }

    private void ShowView(List<NoticeModel> noticeModels)
    {
        contentList = new List<NoticeContent>();
        pageBtns = new List<NoticePageBtn>();
        PageBtnContainer.DetachChildren();
        PageContentContainer.DetachChildren();
        noticeModels.ForEach((model) =>
        {
            // add image page
            GameObject content = Instantiate(ContentPrefab, PageContentContainer);
            NoticeContent noticeContent = content.GetComponentInChildren<NoticeContent>();
            noticeContent.SetData(model, this);
            contentList.Add(noticeContent);
            // add page btn
            pageBtns.Add(Instantiate(prefabPageBtn, PageBtnContainer).Init(this, pageBtns.Count));
        });
        index = 0;
        if (contentList[0] != null)
        {
            contentList[0].gameObject.SetActive(true);
            pageBtns[0].Check(true);
        }
        AutoNotifyToggle.isOn = false;
        Ready(true);
    }


    public void Scroll(int nextIndex)
    #region
    {

        {
            if(!ready || nextIndex == index)
            {
                return;
            }

            Ready(false);
            int direction = 1;
            if(nextIndex > index)
            {
                contentMove = -move;
                direction = -1;
            }

            if(nextIndex < index)
            {
                contentMove = move;
                direction = 1;
            }

            if(nextIndex > contentList.Count - 1)
            {
                contentMove = move;
                nextIndex = 0;
            }

            else if (nextIndex < 0)
            {
                contentMove = -move;
                nextIndex = contentList.Count - 1;
            }

            contentList[index].Slide(true,contentMove,direction);
            contentList[nextIndex].Slide(false,contentMove ,direction);
            pageBtns[index].Check(false);
            pageBtns[nextIndex].Check(true);
            index = nextIndex;
        }
    }
    #endregion

    private void Clear()
    {
        foreach(Transform child in PageContentContainer)
        {
            Destroy(child.gameObject);
        }

        foreach(Transform child in PageContentContainer)
        {
            Destroy(child.gameObject);
        }

        contentList.Clear();
        pageBtns.Clear();
    }

    public void Ready(bool status)
    {
        nextBtn.enabled = prevBtn.enabled = ready = status;
    }

    public void Prev()
    {
        Scroll(index + 1);
    }
    
    public void Next()
    {
        Scroll(index - 1);
    }

    public void ToggleAutoNotify(bool isOn)
    {
        if(isOn)
        {
            LocalStorageUtils.OffAutoNotify = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        else
        {
            LocalStorageUtils.OffAutoNotify = -1;
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable() 
    {
        Clear();
    }

    public void ShowPopup()
    {
        gameObject.SetActive(true);
        Open();
    }

    public void OpenLink()
    {
        contentList[index].OpenLink();
    }
}
public class NoticeModel
    {
    public string content;
    public string title;
    public string imgURL;
    public string linkHref;
    public Texture2D tex;
    public bool isPayNotice = false;
    const string imgURLPattern = "<img src='";
    const string linkHrefPattern = "href='";

    public NoticeModel Init()
    {
        imgURL = GetStringByPattern(imgURLPattern);
        linkHref = GetStringByPattern(linkHrefPattern);
        var temp = (content + title).ToLower();

        return this;
    }
        string GetStringByPattern(string pattern)
    {
        int SP = content.IndexOf(pattern);
        int EP = content.IndexOf("'", SP + pattern.Length);
        return content.Substring(SP + pattern.Length, EP - SP - pattern.Length);
    }
}
