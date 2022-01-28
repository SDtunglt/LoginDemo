using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class MyUserMediator : MonoBehaviour
{
    [SerializeField] private TMP_Text txtName,txtCoin;
    [SerializeField] private Image specialBorder;
    [SerializeField] private Image imgAvatar;
    [SerializeField] private Image normalBorder;

    private UserModel userModel = UserModel.Instance;

    private void OnEnable()
    {
        SDImageLoader.Get().Load(GameUtils.GetAvatarUrl(userModel.uid,"m")).Into(imgAvatar).StartLoading();

        if(UserModel.Instance != null)
        {
            UpdatePlayerInfo();
        }
        Signals.Get<UpdatePlayerInfoSignal>().AddListener(UpdatePlayerInfo);
        Signals.Get<RefreshCoinSignal>().AddListener(UpdatePlayerInfo);
        Signals.Get<AvatarChangeSignal>().AddListener(OnAvatarChange);
        Signals.Get<OnChangeKhungAvatar>().AddListener(OnChangeKhungAvatar);
    }

    private void OnChangeKhungAvatar(int i)
    {
        var id = GlobalDataManager.Ins.khungAvatarData.infos.Any(s => s.id == i) ? i : 0;
        if(id == 0)
        {
            normalBorder.Show();
            specialBorder.Hide();
        } 
        else
        {
            normalBorder.Hide();
            specialBorder.Show();
            specialBorder.sprite = GlobalDataManager.Ins.khungAvatarData.infos.Find(s => s.id == id).khungAvt;
        }
    }

    private void OnDisable()
    {
        Signals.Get<UpdatePlayerInfoSignal>().RemoveListener(UpdatePlayerInfo);
        Signals.Get<RefreshCoinSignal>().RemoveListener(UpdatePlayerInfo);
        Signals.Get<AvatarChangeSignal>().RemoveListener(OnAvatarChange);
        Signals.Get<OnChangeKhungAvatar>().RemoveListener(OnChangeKhungAvatar);
    }

    private void OnAvatarChange(int date)
    {
        SDImageLoader.Get().Load(GameUtils.GetAvatarUrl(UserModel.Instance.uid,"m", date)).Into(imgAvatar).StartLoading(false);
    }

    public void OnClickButton()
    {
        LobbyScreen.Instance.userDetail.gameObject.SetActive(true);
        LobbyScreen.Instance.userDetail.GetUserInfo(userModel.uid,userModel.ip);

        //UserDetailMediator.Open(userModel.uid, userModel.ip);
        // if (GameModel.Instance.IsNormalPlayer())
        // {
        //     UserDetailMediator.Open(userModel.uid, userModel.ip);   
        // }
        //FirebaseAnalyticsExtension.Instance.LogEventWithParam(FirebaseEvent.ViewUserDetail, "uid", userModel.uid);
    }

    private void UpdatePlayerInfo()
    {
        if(txtName != null)
        {
            txtName.text = userModel.name;
        }

        if(txtCoin != null)
        {
            txtCoin.text = StringUtils.FormatMoney(userModel.gVO.coin);
        }

        if(UserModel.Instance.currentSelectBorder == 0)
        {
            normalBorder.Show();
            specialBorder.Hide();
        }
        else
        {
            normalBorder.Hide();
            specialBorder.Show();
            specialBorder.sprite = GlobalDataManager.Ins.khungAvatarData.infos.Find(s => s.id == UserModel.Instance.currentSelectBorder).khungAvt;
        }
    }
    
}

