using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZoneMediator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtInfo;
    [SerializeField] private int zoneId;
    void Start()
    {
        UpdateZoneInfo();
    }

    private void UpdateZoneInfo()
    {
        txtInfo.text = $"{StringUtils.FormatMoneyK(GameConfig.ZoneCfg[zoneId].minStake)} - {StringUtils.FormatMoneyK(GameConfig.ZoneCfg[zoneId].maxStake)}";
    }

    public void OnZoneViewClick()
    {
        ScreenManager.Instance.JoinRoom(zoneId, 0);
        return;
        if(GameModel.Instance.IsNormalPlayer())
        {
            if(ScreenManager.Instance.CheckJoinZone(zoneId))
            {
                ScreenManager.Instance.JoinRoom(zoneId, 0);
            }
        }
        else
        {
            ScreenManager.Instance.JoinRoom(zoneId, 0);
        }
    }
}
