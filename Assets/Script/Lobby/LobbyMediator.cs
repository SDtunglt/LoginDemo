using System;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMediator : MonoBehaviour
{
    [SerializeField] private GameObject chonkhuCanvas, thiCuCanvas, gameCanvas;
    [SerializeField] public GameObject btnBack;
    private UserModel userModel = UserModel.Instance;

    void OnEnable()
    {
        LobbyScreen.Instance.noticePopUp.gameObject.SetActive(true);
        LobbyScreen.Instance.noticePopUp.ShowPopup();
        chonkhuCanvas.Hide();
        var menuHasBack = MenuType.Lobby;
        if (ScreenManager.LastZone >= 0 && ScreenManager.LastZone <= 5 && ScreenManager.LastRoom != -1 &&
            ScreenManager.LastBoard == -1)
        {
            chonkhuCanvas.SetActive(true);
            menuHasBack = MenuType.SelectZone;
        }
    }

    public void OnChonKhuClick()
    {
        chonkhuCanvas.SetActive(true);
        btnBack.Show();
        Signals.Get<UpdateMenuMediatorSignal>().Dispatch(MenuType.SelectZone);
    }

    public void OnBackClick()
    {
        chonkhuCanvas.SetActive(false);
        LobbyScreen.Instance.bottomBarMediator.OnCloseShop();
        btnBack.Hide();
        Signals.Get<UpdateMenuMediatorSignal>().Dispatch(MenuType.Lobby);
    }

}
