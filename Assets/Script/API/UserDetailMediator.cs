using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

public class UserDetailMediator : MonoBehaviour
{
    private UserDetailVO userDetailVO;
    private UserModel userModel = UserModel.Instance;
    public string uid;
    [SerializeField] private TMP_Text txtName,
        txtCoin,
        txtGender,
        txtId,
        txtPlay,
        //txtBigU,
        txtCuocBigU;
        //txtIp;

    public void GetUserInfo(string uid, double ip)
    {
        gameObject.SetActive(true);
        API.GetUserDetail(data =>
        {
            OnGetUserDetailSuccess(uid, ip, data);
        },
         OnGetUserDetailError, uid, GameConfig.APP_ID.ToString());
        
    }

    private void OnGetUserDetailError(string error)
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
        txtGender.text = "Giới tính: " + vo.cVO.GetGender();
        txtPlay.text = $"Số ván chơi: {vo.gVO.total}";
        //txtCuocBigU.text = $"Số ván Ù: {vo.gVO.strBigU}";
        //txtCoin.text = $"Coin: {vo.gVO.coin}";


        //txtU.text = StringUtils.FormatMoney(vo.gVO.win);
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
}
