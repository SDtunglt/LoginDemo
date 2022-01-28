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
    private UserModel userModel = UserModel.Instance;

    void OnEnable()
    {
        LobbyScreen.Instance.noticePopUp.gameObject.SetActive(true);
        LobbyScreen.Instance.noticePopUp.ShowPopup();
    }

}
