using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemRoomSelect : MonoBehaviour
{
    [SerializeField] private TMP_Text txtNameRoom, txtUserCount;
    [SerializeField] private Toggle tgItemRoom;
    [SerializeField] private Image statusIm;
    [SerializeField] private GameObject border;
    [SerializeField] private Color selectedColor;
    public Sprite[] statusSprites;
    private int room;
    private Action<int> callback;
    private const float maxValue = 120;

    public void UpdateView(int r, string nameRoom, Action<int> cb, ToggleGroup group, bool isLast = false, int z = -1)
    {
        if(z == -1)
        {
            z = ScreenManager.Instance.zone;
        }
        room = r;
        callback = cb;
        var userCount = 0;
        userCount = int.Parse(UserCountsModel.Instance.uCounts[z][r]);

        tgItemRoom.group = group;
        tgItemRoom.isOn = ScreenManager.Instance.room == r;
        txtNameRoom.text = nameRoom;
        txtUserCount.text = userCount.ToString();

        var x = Mathf.Clamp01(userCount/ maxValue);
        statusIm.fillAmount = x;
        statusIm.sprite = x < 0.3f ? statusSprites[0] : x < 0.6f ? statusSprites[1] : statusSprites[2];
        border.SetActive(!isLast);
    }

    public void OnItemChange()
    {
        if(tgItemRoom.isOn)
        {
            callback?.Invoke(room);
            if(selectedColor != null)
            txtNameRoom.color = txtUserCount.color = selectedColor;
        }
        else
        {
            txtNameRoom.color = txtUserCount.color = Color.white;
        }
    }

    private void OnDestroy()
    {
        callback = null;
    }
}
