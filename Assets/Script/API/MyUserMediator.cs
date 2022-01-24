using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class MyUserMediator : MonoBehaviour
{
    [SerializeField] private TMP_Text txtName,txtCoin;
    private UserModel userModel = UserModel.Instance;

    public void OnClickButton()
    {
        ScreenManager.Ins.userDetail.GetUserInfo(userModel.uid,userModel.ip);

        //UserDetailMediator.Open(userModel.uid, userModel.ip);
        // if (GameModel.Instance.IsNormalPlayer())
        // {
        //     UserDetailMediator.Open(userModel.uid, userModel.ip);   
        // }
        //FirebaseAnalyticsExtension.Instance.LogEventWithParam(FirebaseEvent.ViewUserDetail, "uid", userModel.uid);
    }
}

