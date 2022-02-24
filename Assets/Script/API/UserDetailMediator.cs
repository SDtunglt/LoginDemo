using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserDetailMediator : MonoBehaviour
{
    private UserDetailVO userDetailVO;
    [SerializeField] private Image specialBorder;
    [SerializeField] private Image normalBorder;
    private UserModel userModel = UserModel.Instance;
    public string uid;
    [SerializeField] private Image imgAvatar, imgCapBac, expBar, iconVip;
    [SerializeField] private TMP_Text txtName,
        txtCoin,
        txtGender,
        txtId,
        txtPlay,
        txtU,
        //txtBigU,
        txtCuocBigU;
        //txtIp;

    private void OnEnable()
    {
        Signals.Get<AvatarChangeSignal>().AddListener(OnAvatarChange);
        Signals.Get<RefreshCoinSignal>().AddListener(OnRefreshCoin);
        Signals.Get<OnChangeKhungAvatar>().AddListener(OnChangeKhungAvatar);
    }

    private void OnAvatarChange(int date)
    {
        SDImageLoader.Get().Load(GameUtils.GetAvatarUrl(UserModel.Instance.uid, "m", date)).Into(imgAvatar)
            .StartLoading(false);
    }

    private void OnRefreshCoin()
    {
        if (uid == UserModel.Instance.uid) txtCoin.text = StringUtils.FormatMoney(UserModel.Instance.gVO.coin);
    }

    private void OnChangeKhungAvatar(int id)
    {
        id = GlobalDataManager.Ins.khungAvatarData.infos.Any(s => s.id == id) ? id : 0;
        if (id == 0)
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

    public void GetUserInfo(string uid, double ip)
    {
        gameObject.SetActive(true);
        API.GetUserDetail(data =>
        {
            OnGetUserDetailSuccess(uid, ip, data);
        },
         OnGetUserDetailError, uid, GameConfig.APP_ID.ToString());
        
    }

    public void ClosePopUpUserDetail()
    {
        gameObject.SetActive(false);
    }

    private static void OnGetUserDetailError(string error)
    {
        Debug.Log("Thông báo không thể kết nối được với hệ thống vui lòng thử lại");  
    }

    private void OnGetUserDetailSuccess(string uid, double ip, JObject obj)
    {
        this.uid = uid;
        UserDetailVO vo = new UserDetailVO(obj, uid);
        userDetailVO = vo;
        Debug.Log($"{vo.cVO.name} {vo.gVO.coin} {uid} {vo.cVO.GetGender()} {vo.gVO.win} {vo.gVO.total}");
        txtId.text = "ID: " + uid;
        txtName.text = $"UserName: {vo.cVO.name}";
        txtCoin.text = $"Coin: {vo.gVO.coin}";
        txtGender.text = "Giới tính: " + vo.cVO.GetGender();
        txtPlay.text = $"Số ván chơi: {vo.gVO.total}";
        //txtCuocBigU.text = $"Số ván Ù: {vo.gVO.strBigU}";



        txtU.text = StringUtils.FormatMoney(vo.gVO.win);
        //txtLvl.text = vo.gVO.level.ToString();
        string diem = "0";
        if (string.IsNullOrEmpty(vo.gVO.strBigU))
        {
            txtCuocBigU.text = "Chưa ù ván nào";
        }
        else
        {
            string[] arrBigU = vo.gVO.strBigU.Split('/');
            diem = arrBigU[0];
            txtCuocBigU.text = arrBigU[1].Split(',')[0];
        }

        /*if (string.IsNullOrEmpty(vo.gVO.strBigWin))
        {
            txtCuocBigWin.text = "Chưa ù ván nào";
        }
        else
        {
            string[] arrBigWin = vo.gVO.strBigWin.Split('/');
            bao = arrBigWin[0];
            txtCuocBigWin.text = arrBigWin[1].Split(',')[0];
        }*/

        //txtBigU.text = diem;
        //txtBigWin.text = bao;
    }

    public static void Open(string uid, double ip)
    {
        // LoadingEffect.Open();
        API.GetUserDetail(data =>
        {
            ViewCreator.OpenPopup(PopupId.UserDetailPopup, view =>
            {
                var p = view.Cast<UserDetailMediator>();
                p.OnGetUserDetailSuccess(uid, ip, data);
            });
        }, OnGetUserDetailError, uid, GameConfig.APP_ID.ToString());
    }
}
