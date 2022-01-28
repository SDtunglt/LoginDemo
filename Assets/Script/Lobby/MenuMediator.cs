using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuMediator : MonoBehaviour
{
    [SerializeField] private GameObject btnSetting,pullDownMenu,btnLogOut,backBtn;
    [SerializeField] private Image imgTaskNoti,imgMenuNoti;
    [SerializeField] private Button btnTask;
    [SerializeField] private GameObject avatar;
    [SerializeField] private Sprite[] frameIcon;

    private ScreenManager screenManager;
    private Tween delay;

    private void OnEnable()
    {
        screenManager = ScreenManager.Instance;
    }
}
