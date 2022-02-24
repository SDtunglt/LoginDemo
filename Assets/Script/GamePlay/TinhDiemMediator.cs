using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TinhDiemMediator : MonoBehaviour
{
    [SerializeField] private TMP_Text txtGa, txtGameNo;
    [SerializeField] private Button btnXem;

    private GamePlayModel gamePlayModel = GamePlayModel.Instance;

    private SyncBoardConfigSignal syncBoardConfigSignal = Signals.Get<SyncBoardConfigSignal>();
    private GameNoChangedSignal gameNoChangedSignal = Signals.Get<GameNoChangedSignal>();
    private ScoreChangeSignal scoreChangeSignal = Signals.Get<ScoreChangeSignal>();
    private GaScoreChangeSignal gaScoreChangeSignal = Signals.Get<GaScoreChangeSignal>();

    private void Awake()
    {
        syncBoardConfigSignal.AddListener(UpdateView);
        gameNoChangedSignal.AddListener(GameNoChanged);
        scoreChangeSignal.AddListener(ScoreChanged);
        gaScoreChangeSignal.AddListener(GaScoreChanged);
        UpdateView();
    }

    private void OnDestroy()
    {
        syncBoardConfigSignal.RemoveListener(UpdateView);
        gameNoChangedSignal.RemoveListener(GameNoChanged);
        scoreChangeSignal.RemoveListener(ScoreChanged);
        gaScoreChangeSignal.RemoveListener(GaScoreChanged);
    }

    private void UpdateView()
    {
        if (!gamePlayModel.tinhDiem) return;
        txtGameNo.text = "VÁN: " + gamePlayModel.gameNo;
        txtGa.gameObject.SetActive(gamePlayModel.isNuoiGa && !ScreenManager.Instance.inTour() && ScreenManager.Instance.InChallenge());
    }

    private void GameNoChanged()
    {
        txtGameNo.text = "VÁN: " + gamePlayModel.gameNo;
    }

    private void ScoreChanged(ScoreVO vo, bool isTinhDiem)
    {
        if (!isTinhDiem) return;
        txtGa.text = "GÀ: " + (vo?.gaScore() ?? 0);
    }

    private void GaScoreChanged(int score, bool isShowGa)
    {
        txtGa.text = "GÀ: " + score;
        txtGa.gameObject.SetActive(isShowGa);
    }

    public void OnShowGameResults()
    {
        SmartFoxConnection.Instance.SendExt(ExtCmd.GamesResult);
        btnXem.interactable = false;
        DOVirtual.DelayedCall(2, () => btnXem.interactable = true);
    }
}