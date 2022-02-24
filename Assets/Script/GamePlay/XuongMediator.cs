using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class XuongMediator : MonoBehaviour
{
    [SerializeField] private TMP_Text txt_cuocs;
    [SerializeField] private List<CuocUButton> listCuocButtons;
    [SerializeField] private TextCountDown txtCountDown;

    [SerializeField] private GameObject gridCuocs;
    [SerializeField] private RectTransform panelRect;

    [SerializeField] private RectTransform cardInHand;
    [SerializeField]
    private float panelRectMaxWidth = 1920;
    [SerializeField]
    private float panelRectMinWidth = 1080;
    [SerializeField]
    private float panelRectMaxHeight = 860;
    [SerializeField]
    private float panelRectMinHeight = 300;

    // private int leo = 0, tom = 0, thienKhai = 0, chiu = 0, bon = 0;
    // private List<int> listCuocs;

    private GamePlayModel gamePlayModel = GamePlayModel.Instance;

    private ShowTimeOutMsgSignal showTimeOutMsgSignal = Signals.Get<ShowTimeOutMsgSignal>();
    private XuongSignal xuongSignal = Signals.Get<XuongSignal>();

    /*  Nếu click Suông:
     *      - Suông đang off => on, off tất cả những cước khác
     *  Nếu click lèo, tôm, thiên khai, chíu, bòn:
     *      - Thay đổi số lượng
     *      - Nếu số lượng > 0 => on, off cước Suông đi, thay text
     *      - Nếu = 0 => off, thay text
     *  Nếu click những cước còn lại:
     *      - Nếu đang on => off
     *      - Nếu đang off => on, off cước Suông
     *  Cuối cùng phải check, nếu đang ko có cước nào ngoài Suông được chọn thì chọn cước Suông
     */

    private void Awake()
    {
        Signals.Get<HideXuongSignal>().AddListener(OnHideXuong);
    }

    private void OnDestroy()
    {
        Signals.Get<HideXuongSignal>().RemoveListener(OnHideXuong);
    }

    private void OnHideXuong()
    {
        this.Hide();
    }

    private void OnEnable()
    {
        SetDefaults();
        txtCountDown.ShowCountDownWithCallback(SDTimeout.XUONG, OnXuongClick);
        OnGenerateButton();
    }

    private void SetDefaults()
    {
        // float width = currentState ? panelRectMinWidth : panelRectMaxWidth;
        var width = panelRectMaxWidth;
        var height = panelRectMaxHeight;
        gridCuocs.SetActive(true);
        panelRect.sizeDelta = new Vector2(width, height);
    }

    private void OnGenerateButton()
    {
        for (int i = 0; i < listCuocButtons.Count; i++)
        {
            listCuocButtons[i].OnShow(i, OnButtonClick);
        }
        
        txt_cuocs.text = ULogic.getCuocsStr(GetListCuocsId());
    }

    private List<int> GetListCuocsId()
    {
        var ls = new List<int>();
        var isNotSelectOne = true;
        foreach (var btn in listCuocButtons)
        {
            if (btn.IsSellected())
            {
                isNotSelectOne = false;
                ls.Add(btn.Num);
            }
            else
            {
                ls.Add(0);
            }
        }

        if (isNotSelectOne)
        {
            ls[0] = 1;
        }
        
        return ls;
    }

    private void OnButtonClick(int cuocIdx)
    {
        var clickedButton = listCuocButtons[cuocIdx];
        if (cuocIdx == 0) //click "Suong"
        {
            if (!clickedButton.IsSellected())
            {
                clickedButton.SetSellected(true);
                for (int i = 1; i < listCuocButtons.Count; i++)
                {
                    listCuocButtons[i].SetSellected(false);
                    listCuocButtons[i].SetTextCuoc(Utils.CUOC_NAMES[i]);
                }
            }
        }
        else if (cuocIdx >= 17 && cuocIdx <= 21) //click leo, tom, thien khai, chiu, bon
        {
            clickedButton.Num = ++clickedButton.Num % 5;
            if (clickedButton.Num > 0)
            {
                clickedButton.SetSellected(true);
                clickedButton.SetTextCuoc(clickedButton.Num + " " + Utils.CUOC_NAMES[cuocIdx]);
                listCuocButtons[0].SetSellected(false);
            }
            else
            {
                clickedButton.SetSellected(false);
                clickedButton.SetTextCuoc(Utils.CUOC_NAMES[cuocIdx]);
            }
        }
        else
        {
            if (clickedButton.IsSellected())
            {
                clickedButton.SetSellected(false);
            }
            else
            {
                clickedButton.Num++;
                clickedButton.SetSellected(true);
                listCuocButtons[0].SetSellected(false);
            }
        }

        UpdateSuongButton();
        // UpdateListCuocs();
        txt_cuocs.text = ULogic.getCuocsStr(GetListCuocsId());
    }
    //
    // public void OnCuocClick()
    // {
    //     var clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<CuocUButton>();
    //     int cuocIdx = listCuocButtons.IndexOf(clickedButton);
    //     if (cuocIdx == 0) //click "Suong"
    //     {
    //         if (!clickedButton.IsSellected())
    //         {
    //             clickedButton.SetSellected(true);
    //             for (int i = 1; i < listCuocButtons.Count; i++)
    //             {
    //                 listCuocButtons[i].SetSellected(false);
    //                 listCuocButtons[i].SetTextCuoc(Utils.CUOC_NAMES[i]);
    //             }
    //         }
    //     }
    //     else if (cuocIdx >= 17 && cuocIdx <= 21) //click leo, tom, thien khai, chiu, bon
    //     {
    //         clickedButton.Num = ++clickedButton.Num % 5;
    //         if (clickedButton.Num > 0)
    //         {
    //             clickedButton.SetSellected(true);
    //             clickedButton.SetTextCuoc(clickedButton.Num + " " + Utils.CUOC_NAMES[cuocIdx]);
    //             listCuocButtons[0].SetSellected(false);
    //         }
    //         else
    //         {
    //             clickedButton.SetSellected(false);
    //             clickedButton.SetTextCuoc(Utils.CUOC_NAMES[cuocIdx]);
    //         }
    //     }
    //     else
    //     {
    //         if (clickedButton.IsSellected())
    //         {
    //             clickedButton.SetSellected(false);
    //         }
    //         else
    //         {
    //             clickedButton.Num++;
    //             clickedButton.SetSellected(true);
    //             listCuocButtons[0].SetSellected(false);
    //         }
    //     }
    //
    //     UpdateSuongButton();
    //     UpdateListCuocs();
    //     txt_cuocs.text = ULogic.getCuocsStr(listCuocs);
    // }

    private void UpdateSuongButton()
    {
        var suong = true;
        for (var i = 1; i < listCuocButtons.Count; i++)
        {
            if (listCuocButtons[i].IsSellected())
            {
                suong = false;
                break;
            }
        }

        listCuocButtons[0].SetSellected(suong);
        listCuocButtons[0].Num = suong ? 1 : 0;
    }

    // private void UpdateListCuocs()
    // {
    //     for (var i = 0; i < listCuocButtons.Count; i++)
    //     {
    //         listCuocs[i] = listCuocButtons[i].Num;
    //     }
    // }

    public void OnXuongClick()
    {
        // SumUpVO vo = new SumUpVO {type = SumUpVO.XUONG_DUNG, idxU = GamePlayModel.Instance.myIdx, cuocHos = listCuocs};
        //
        // SmartFoxConnection.Instance.SendExt(ExtCmd.Sumup, vo);

        showTimeOutMsgSignal.Dispatch(SDMsg.Join(AppMsg.PLSWAIT));

        var vo = new SumUpVO
        {
            idxU = gamePlayModel.myIdx,
            cuocHos = GetListCuocsId()
        };
        xuongSignal.Dispatch(vo);
        txtCountDown.Hide();
        if (cardInHand)
        {
            cardInHand.Hide();
        }
    }

    private void OnDisable()
    {
        listCuocButtons[0].SetSellected(true);
        for (var i = 1; i < listCuocButtons.Count; i++)
        {
            listCuocButtons[i].SetSellected(false);
            listCuocButtons[i].SetTextCuoc(Utils.CUOC_NAMES[i]);
        }

        txtCountDown.Hide();
    }

    public void OnShowHide()
    {
        var currentState = gridCuocs.activeSelf;
        // float width = currentState ? panelRectMinWidth : panelRectMaxWidth;
        var width = panelRectMaxWidth;
        var height = currentState ? panelRectMinHeight : panelRectMaxHeight;
        var timeGridCuocs = currentState ? 0 : 0.3f;
        Invoke(nameof(ShowHideGridCuocs), timeGridCuocs);
        panelRect.DOSizeDelta(new Vector2(width, height), 0.3f);
    }
    
    private void ShowHideGridCuocs()
    {
        gridCuocs.SetActive(!gridCuocs.activeSelf);
    }
}