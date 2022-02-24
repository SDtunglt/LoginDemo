using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMediator : MonoBehaviour
{
    private static int STATUS_NORMAL = 1;
    private static int STATUS_BAO = 2;
    private static int STATUS_DIS = 3;

    [SerializeField] private TMP_Text txtName, txtCoin, txtScore;
    [SerializeField] private Image imgAvatar, mark;
    [SerializeField] private Image imgNenAvt;
    [SerializeField] private Image specialKhungAvt;
    [SerializeField] private Image normalKhungAvt;
    [SerializeField] private GameObject imgDisconnect, imgBao;
    [SerializeField] private GameObject score;
    

    private GamePlayModel gamePlayModel = GamePlayModel.Instance;

    private PlayerStatusChangedSignal playerStatusChangedSignal = Signals.Get<PlayerStatusChangedSignal>();
    private ScoreChangeSignal scoreChangeSignal = Signals.Get<ScoreChangeSignal>();
    private StopGameSignal stopGameSignal = Signals.Get<StopGameSignal>();

    public SDPlayer sdPlayer;
    public RectTransform tfCountdown;
    private Sequence sequence;
    private float dt = .5f;

    private void Awake()
    {
        playerStatusChangedSignal.AddListener(OnPlayerStatusChanged);
        scoreChangeSignal.AddListener(OnScoreChange);
        stopGameSignal.AddListener(OnStopGame);
        Signals.Get<AvatarChangeSignal>().AddListener(OnAvatarChange);
        Signals.Get<GamePlayBackgroundChangeSignal>().AddListener(OnChangeNenAvt);
        Signals.Get<OnChangeKhungAvatar>().AddListener(OnChangeKhungAvatar);
        
        OnScoreChange(gamePlayModel.GetScoreVO(), gamePlayModel.tinhDiem);
        //OnChangeNenAvt(GameUtils.GameBG);
    }

    private void OnChangeKhungAvatar(int i)
    {
        var id = GlobalDataManager.Ins.khungAvatarData.infos.Any(s => s.id == i) ? i : 0;
        if (id == 0)
        {
            //imgNenAvt.sprite = GlobalDataManager.Ins.nenAvatars[GameUtils.GameBG];
            normalKhungAvt.Show();
            specialKhungAvt.Hide();
        }
        else
        {
            normalKhungAvt.Hide();
            specialKhungAvt.Show();
            specialKhungAvt.sprite = GlobalDataManager.Ins.khungAvatarData.infos.Find(s => s.id == id).khungAvt;
            imgNenAvt.sprite = GlobalDataManager.Ins.khungAvatarData.infos.Find(s => s.id == id).khungNhanVat;
        }
    }

    private void OnChangeNenAvt(int obj)
    {
        if (sdPlayer != null)
        {
            SDBorderLoader.LoadBorder(sdPlayer.uid, OnChangeKhungAvatar, s => OnChangeKhungAvatar(obj));
        }
    }
    
    private void OnDestroy()
    { 
        playerStatusChangedSignal.RemoveListener(OnPlayerStatusChanged);
        scoreChangeSignal.RemoveListener(OnScoreChange);
        stopGameSignal.RemoveListener(OnStopGame);
        Signals.Get<AvatarChangeSignal>().RemoveListener(OnAvatarChange);
        Signals.Get<GamePlayBackgroundChangeSignal>().RemoveListener(OnChangeNenAvt);
        Signals.Get<OnChangeKhungAvatar>().RemoveListener(OnChangeKhungAvatar);
    }

    private void OnAvatarChange(int date)
    {
        if (sdPlayer.uid != UserModel.Instance.uid) return;
        SDImageLoader.Get().Load(GameUtils.GetAvatarUrl(sdPlayer.uid, "m", date)).Into(imgAvatar).StartLoading(false);
    }

    public void UpdateCoin(double coin)
    {
        txtCoin.text = StringUtils.FormatMoneyK(coin);
    }

    public void UpdateUser(SDPlayer pl)
    {
        sdPlayer = pl;

        txtName.text = pl.name;
        if (pl.name.Length > 12) txtName.text = pl.name.Substring(0, 10) + "...";

            txtCoin.text = StringUtils.FormatMoneyK(pl.coin);

        if (gamePlayModel.GetScoreVO() != null)
        {
            OnScoreChange(gamePlayModel.GetScoreVO(), gamePlayModel.tinhDiem);
        }

        SDImageLoader.Get().Load(GameUtils.GetAvatarUrl(sdPlayer.uid, "m")).Into(imgAvatar).StartLoading();

        var status = pl.dis ? STATUS_DIS : pl.bao ? STATUS_BAO : STATUS_NORMAL;
        UpdateStatus(status);
        SDBorderLoader.LoadBorder(pl.uid, OnChangeKhungAvatar, s => OnChangeKhungAvatar(0));
    }

    public void ResetUser()
    {
        sdPlayer = null;
        txtName.text = GameConfig.NameDefault;
        txtCoin.text = "0";
    }

    private void OnPlayerStatusChanged(SDPlayer p)
    {
        if (p != sdPlayer)
            return;
        if (p.bao)
            UpdateStatus(STATUS_BAO);
        else if (p.dis)
            UpdateStatus(STATUS_DIS);
        else
            UpdateStatus(STATUS_NORMAL);
    }

    private void UpdateStatus(int type)
    {
        if (type == STATUS_NORMAL)
        {
            imgBao.Hide();
            imgDisconnect.Hide();
            return;
        }

        if (type == STATUS_BAO)
        {
            imgBao.Show();
            imgDisconnect.Hide();
        }
        else
        {
            imgBao.Hide();
            imgDisconnect.Show();
        }
    }

    private void OnScoreChange(ScoreVO vo, bool isTinhDiem)
    {
        if (isTinhDiem && vo != null)
        {
            var idx = sdPlayer != null ? vo.uids.IndexOf(int.Parse(sdPlayer.uid)) : -1;
            UpdateScore(idx >= 0 ? vo.scores[idx] : 0, true);
        }
        else
        {
            UpdateScore(0, false);
        }
    }

    private void UpdateScore(int scoreNumber, bool isVisible)
    {
        txtScore.text = scoreNumber.ToString();
        score.SetActive(isVisible);
    }

    public void OnShowEffectU()
    {
        mark.Show();
        sequence?.Kill();
        sequence = DOTween.Sequence()
            .Append(mark.DOFade(0, dt).OnComplete(FadeOn).SetEase(Ease.Linear));
    }

    private void FadeOn()
    {
        mark.DOFade(1, dt).OnComplete(FadeOff).SetEase(Ease.Linear);
    }

    private void FadeOff()
    {
        mark.DOFade(0, dt).OnComplete(FadeOn).SetEase(Ease.Linear);
    }

    private void OnStopGame()
    {
        mark.Hide();
        sequence?.Kill();
    }

    public void OnClick()
    {
        UserDetailMediator.Open(sdPlayer.uid, sdPlayer.ip);
    }
}