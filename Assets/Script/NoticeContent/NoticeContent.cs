using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoticeContent : MonoBehaviour
{
    const float transitionTime = 0.05f;
    RectTransform contentRect;
    string link;
    RawImage contentImage;
    RectTransform rect;
    public Vector2 startPosition;
    NoticePopUp noticePopUp;
    ScreenManager screenManager;

    void Awake()
    {
        contentImage = GetComponent<RawImage>();
        rect = GetComponent<RectTransform>();
        contentRect = GetComponentInParent<RectTransform>();
        ScrollRect scrollRect = GetComponent<ScrollRect>();
        startPosition = rect.anchoredPosition;
        contentRect = scrollRect.content;
        Scrollbar scrollbar = scrollRect.horizontalScrollbar;
        scrollbar.onValueChanged.AddListener(OnScrollbarMove);
    }

    public void OnScrollbarMove(float f)
    {
        rect.anchoredPosition = startPosition - (1 - f) * transitionTime * contentRect.rect.height * Vector2.left;
    }

    public void OpenLink()
    {
        Debug.Log($"Link: {link}");
        GameUtils.OpenUrl(link);
    }

    public void SetData(NoticeModel content, NoticePopUp noticePopUp)
    {
        link = content.linkHref;
        gameObject.SetActive(false);
        this.noticePopUp = noticePopUp;
        contentImage.texture = content.tex;
    }

    public void Slide(bool isOut, float move, int direction = 1)
    {
        if(isOut)
        {
            StartCoroutine(InactiveTimer());
        }
        else
        {
            OnScrollbarMove(move);
            StopAllCoroutines();
            gameObject.SetActive(true);
            noticePopUp.Ready(true);
        }
    }

    IEnumerator InactiveTimer()
    {
        yield return new WaitForSeconds(transitionTime);
        gameObject.SetActive(false);
    }

}
