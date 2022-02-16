using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindBoardMediator : UIPopup
{
    [SerializeField] private CustomSlider sldMinStake, sldMaxStake;
    [SerializeField] private Toggle tgRandomGa, tgKhongGa, tgNuoiGa;
    [SerializeField] private Toggle tgRandomScore, tgSuong, tg3Diem, tg4Diem;

    private UserModel userModel = UserModel.Instance;

    private int zone;
    private long minStake;
    private long maxStake;

    public static void OpenPopup(int z)
    {
        ViewCreator.OpenPopup(PopupId.FindBoardPopup, view =>
        {
            var p = view.Cast<FindBoardMediator>();
            p.zone = z;
            p.Init();
            // FirebaseAnalyticsExtension.Instance.FindBoardConfig();
            //FirebaseAnalyticsExtension.Instance.LogEvent(FirebaseEvent.FindBoardConfig);
        });
    }

    private void Start()
    {
        sldMaxStake.onChangeToStep += l =>
        {
            maxStake = l;
        };

        sldMinStake.onChangeToStep += l =>
        {
            minStake = l;
        };
    }

    private void Init()
    {
        minStake = GameConfig.ZoneCfg[zone].minStake;
        maxStake = GameConfig.ZoneCfg[zone].maxStake;

        var maxStakeOfUser = userModel.gVO.coin / (GameConfig.COIN_RATIO * 1000);
        maxStake = maxStake > maxStakeOfUser ? maxStakeOfUser : maxStake;

        UpdateStake();
    }

    private void UpdateStake()
    {
        var zoneStake = GameConfig.ZoneStake[zone];
        var uMaxStake = userModel.gVO.coin / GameConfig.COIN_RATIO;
        var max = uMaxStake < zoneStake[zoneStake.Length - 1] ? uMaxStake : zoneStake[zoneStake.Length - 1];

        sldMinStake.SetUpSlider( zoneStake[0], zoneStake[0], max, CheckMaxState(zoneStake , max));
        sldMaxStake.SetUpSlider( zoneStake[0], zoneStake[0], max, CheckMaxState(zoneStake, max));
        sldMaxStake.ChangeValue(1);
    }

    private long[] CheckMaxState(int[] stakes, long max)
    {
        var arrStake = new List<long>();
        for (var i = 0; i < stakes.Length; i++)
        {
            if (stakes[i] <= max) arrStake.Add(stakes[i]);
        }

        return arrStake.ToArray();
    }
    

    public void OnJoinClick()
    {
        var vo = new QuickJoinVO(zone, (int)minStake, (int)maxStake);
        // SDLogger.Log("Min: " + vo.min + " - Max: " + vo.max);
        if (tgKhongGa.isOn) vo.hasGa = 0;
        else if (tgNuoiGa.isOn) vo.hasGa = 1;
        else vo.hasGa = -1;
        
        if (tgSuong.isOn) vo.minU = 2;
        else if (tg3Diem.isOn) vo.minU = 3;
        else if (tg4Diem.isOn) vo.minU = 4;
        else vo.minU = -1;
        ClosePopup();
        ScreenManager.Instance.QuickJoin(vo);
    }

    public void ClosePopup()
    {
        tgRandomGa.isOn = true;
        tgRandomScore.isOn = true;
        
        Close();
    }
}