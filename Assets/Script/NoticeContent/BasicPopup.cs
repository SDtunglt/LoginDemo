using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicPopup : UIPopup
{
    // [SerializeField] private TMP_Text txtHeader;
    [SerializeField] private TMP_Text txtMessage;
    [SerializeField] private Button[] buttons;
    [SerializeField] private Button[] buttonCloses;
    // [SerializeField] private Button btnCam, btnCamBoard;
    public static void Open(string header, string message,string labelBtn1="",Action btn1Callback=null,string labelBtn0="",Action btn0Callback=null,Action btnCloseCallback=null)
    {
        ViewCreator.OpenPopup(PopupId.BasicPopup, view =>
        {
            var p = view.Cast<BasicPopup>();
            p.UpdateView(header, message, labelBtn0, btn0Callback, labelBtn1, btn1Callback, btnCloseCallback);
            // p.btnCam.gameObject.SetActive(!ScreenManager.Instance.inBoard);
            // p.btnCamBoard.gameObject.SetActive(ScreenManager.Instance.inBoard);
        });
    }

    public void UpdateView(string header, string message,string labelBtn0="",Action btn0Callback=null,string labelBtn1="",Action btn1Callback=null,Action btnCloseCallback=null)
    {
        // txtHeader.SetText(header);
        txtMessage.SetText(message);
        if (!string.IsNullOrWhiteSpace(labelBtn0)) 
        {
            buttons[0].gameObject.SetActive(true);
            buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = labelBtn0;
            buttons[0].onClick.RemoveAllListeners();
            buttons[0].onClick.AddListener(() => { btn0Callback?.Invoke(); });
            buttons[0].onClick.AddListener(Close);
        }
        else
        {
            buttons[0].gameObject.SetActive(false);
        }
        if (!string.IsNullOrWhiteSpace(labelBtn1))
        {
            buttons[1].gameObject.SetActive(true);
            buttons[1].GetComponentInChildren<TextMeshProUGUI>().text = labelBtn1;
            buttons[1].onClick.RemoveAllListeners();
            buttons[1].onClick.AddListener(() => { btn1Callback?.Invoke(); });
            buttons[1].onClick.AddListener(Close);
        }
        else
        {
            buttons[1].gameObject.SetActive(false);
        }
        foreach (var btnClose in buttonCloses)
        {
            btnClose.onClick.RemoveAllListeners();
            btnClose.onClick.AddListener(() => { btnCloseCallback?.Invoke(); });
        }
    }

    public static void UpdateMessage(string message)
    {
        // var p = LastPanelOpened;
        // p.txtMessage.text = message;
        
        ViewCreator.OpenPopup(PopupId.BasicPopup, p =>
        {
            p.Cast<BasicPopup>().txtMessage.text = message;
        });
    }
    
    public static void Update(string header, string message,string labelBtn0="",Action btn0Callback=null,string labelBtn1="",Action btn1Callback=null,Action btnCloseCallback=null)
    {
        ViewCreator.OpenPopup(PopupId.BasicPopup, p =>
        {
            p.Cast<BasicPopup>().UpdateView(header, message, labelBtn0, btn0Callback, labelBtn1, btn1Callback,
                btnCloseCallback);
        });
    }

    public static void CloseLast()
    {
        ViewCreator.ClosePopup(PopupId.BasicPopup);
    }
}
