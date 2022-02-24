using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDealCardMediator : MonoBehaviour
{
    [SerializeField] private Toggle continueMode, dropMode;
    [SerializeField] private Button btnCam, btnCamBoard;

    private Action<bool> _action;
    
    private void OnEnable()
    {
        btnCam.Hide();
        btnCamBoard.Show();
    }

    public void AddCallback(Action<bool> callback)
    {
        _action = callback;
    }
    public void OnConfirm() {
        var startVO = new ChiaBaiVO();
        if (continueMode.isOn){
            startVO.calcScoreMode = 0;
        }else if (dropMode.isOn){
            startVO.calcScoreMode = 1;
        }else{
            startVO.calcScoreMode = 2;
        }
        SmartFoxConnection.Instance.SendExt(ExtCmd.Start, startVO);
        _action?.Invoke(true);
        _action = null;
        this.Hide();
    }

    public void HidePopup()
    {
        _action?.Invoke(false);
        _action = null;
        this.Hide();
    }
}