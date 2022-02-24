using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemGamesResult : MonoBehaviour
{
    [SerializeField] private TMP_Text txtGameNo, txtPl1, txtPl2, txtPl3, txtPl4;
    private bool isPlayer;
    private string logId;


    public void UpdateItem(int gameNo, List<int> data, string _logId, bool isPlayer)
    {
        gameObject.Show();
        var listScore = new List<TMP_Text> {txtPl1, txtPl2, txtPl3, txtPl4};
        for (var i = 0; i < data.Count; i++)
        {
            listScore[i].text = data[i].ToString();
        }

        txtGameNo.text = "Ván " + gameNo;
        logId = _logId;
    }

    public void ShowReplay()
    {
        ScreenManager.Instance.OpenReplayScreen(logId);
    }
}