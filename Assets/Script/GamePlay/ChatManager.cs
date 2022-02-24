using System;
using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using DG.Tweening;
using Kyub.EmojiSearch.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChatManager : MonoBehaviour, IEnhancedScrollerDelegate
{
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;

    private List<Message> _chatData = new List<Message>();

    // [SerializeField] private ChatInput inpChat;
    [SerializeField] private TMP_InputField tmp_input;
    [SerializeField] private Button btnSend;
    [SerializeField] private RectTransform chatRect;
    [SerializeField] private float minHeight = 150, maxHeight = 400, transitionTime = 0.5f;
    [SerializeField] private Button expandBtn;
    [SerializeField] private Sprite[] expandSprites;
    [SerializeField] private GameObject emotes;
    [SerializeField] private GameObject bgChat;
    // [SerializeField] private List<TMP_EmojiTextUGUI> 
    private bool isExpanded = true;
    private bool _permissionsDenied;
    private bool isOnSelectInput;
    private bool isEmoteExpanded;
    private ReceiveChatMsgSignal receiveChatMsgSignal = Signals.Get<ReceiveChatMsgSignal>();
    public Action<string> sendMsgFunc;

    private void Awake()
    {
        scroller.Delegate = this;
        receiveChatMsgSignal.AddListener(OnChatReceive);
    }

    private void OnDestroy()
    {
        sendMsgFunc = null;
        receiveChatMsgSignal.RemoveListener(OnChatReceive);
    }

    private void Start()
    {
        expandBtn.onClick.AddListener(Expand);
        tmp_input.Show();
        tmp_input.onSubmit.AddListener(s => { SendInputField(); });
    }

    private void OnEnable()
    {
        if (emotes)
        {
            emotes.gameObject.SetActive(false);
        }
        isExpanded = true;
    }

    public void OnChatReceive(string msg, bool isMod, bool isSumUp, bool isSystem)
    {
        var name = "";
        var i = msg.IndexOf(":");
        if (i >= 0) name = msg.Substring(0, i);
        msg = msg.Substring(i + 1);
        AddMessage(name, msg, isMod, isSumUp, isSystem);
    }

    public void EnableChat(bool enable)
    {
        tmp_input.enabled = tmp_input.interactable = enable;
        btnSend.enabled = btnSend.interactable = enable;

        tmp_input.text = enable ? "" : "Bạn không thể thao tác";
    }

    private Tween tw;
    private void Expand()
    {
        tw?.Kill();
        isExpanded = !isExpanded;
        expandBtn.image.sprite = expandSprites[isExpanded ? 1 : 0];
        tw = chatRect.DOSizeDelta(new Vector2(chatRect.sizeDelta.x, isExpanded? maxHeight:minHeight), transitionTime);
        //if (isExpanded)
        //{
        //    //tw = chatRect.DOAnchorPosY(isExpanded ? maxHeight : minHeight, transitionTime);
        //}
        //else
        //{
        //    tw = chatRect.DOSizeDelta(new Vector2(chatRect.sizeDelta.x, maxHeight), transitionTime).OnComplete(() =>
        //    {
        //        bgChat.SetActive(isExpanded);
        //    });
        //}
        scroller.ReloadData(1);
    }

    public void OnEmoClick()
    {
        bool isEmoteShow = emotes && emotes.gameObject.activeInHierarchy;
        if (!isEmoteShow && !isExpanded)
        {
            Expand();
            isEmoteExpanded = true;
        }
        else if (isEmoteShow && isExpanded && isEmoteExpanded)
        {
            Expand();
            isEmoteExpanded = false;
        }
        
        emotes?.gameObject.SetActive(!isEmoteShow);
    }
    private void AddMessage(string sender, string msg, bool isMod, bool isSumUp, bool isSystem)
    {
        Message message = new Message() {sender = sender, msg = msg, isMod = isMod, isSumUp = isSumUp, isSystem = isSystem};
        // Debug.Log("AddMessage: name: " + sender + ", msg: " + msg + ", isMod: " + isMod + ", isSumUp: " + isSumUp + ", isSystem: " + isSystem);
        AddElement(message);
    }

    private void AddElement(Message msg)
    {
        _chatData.Add(msg);
        scroller.ReloadData(1f);
    }

    public void ClearChat()
    {
        _chatData.Clear();
        scroller.ReloadData(1f);
    }

    public void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Return)) return;
        if (string.IsNullOrEmpty(tmp_input.text))
        {
            tmp_input.Select();
            tmp_input.text = "";
            tmp_input.ForceLabelUpdate();
            if (GameUtils.IsWeb()) tmp_input.ActivateInputField();
        }
        else
        {
            SendInputField();
        }
    }

    public void SendInputField()
    {
        // FirebaseAnalyticsExtension.Instance.SendMessage();
        //FirebaseAnalyticsExtension.Instance.LogEvent(FirebaseEvent.SendMessage);
        var str = tmp_input.text;
        if (str.Length > 120) str = str.Substring(0, 120);

        if (!string.IsNullOrEmpty(str))
        {
            str = UserModel.Instance.name + ": " + str;
            sendMsgFunc?.Invoke(str);
        }

        // tmp_input.Select();
        tmp_input.text = "";
        tmp_input.ForceLabelUpdate();
        if (GameUtils.IsWeb()) tmp_input.ActivateInputField();
        if (emotes)
        {
            emotes.Hide();
        }
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _chatData.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        cellViewPrefab.GetComponent<TMP_Text>().text = _chatData[dataIndex].ToMessage();
        float messSize = LayoutUtility.GetPreferredHeight(cellViewPrefab.GetComponent<RectTransform>());
        Debug.Log("messSize: " + messSize + "  " + _chatData[dataIndex].ToMessage());
        return messSize + 10;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cell = scroller.GetCellView(cellViewPrefab);
        cell.GetComponent<TMP_Text>().SetText(_chatData[dataIndex].ToMessage());
        return cell;
    }
}

public class Message
{
    public bool isMod;
    public bool isSumUp;
    public bool isSystem;
    public string sender;
    public string msg;
    public string ToMessage()
    {
        if (isSumUp)
        {
            return $"<b><color=#68EDA4>{msg}</color></b>";
        }
        if (isSystem || String.IsNullOrEmpty(sender))
        {
            return $"<color=#FFBC00>{msg}</color>";
        }
        return isMod
            ? $"<color=#ED6868>{sender}</color>:{msg}"
            : $"<color=#00C4FF>{sender}</color>:{msg}";
    }

    public int GetMsgLength()
    {
        return sender.Length + msg.Length + 2;
    }
}