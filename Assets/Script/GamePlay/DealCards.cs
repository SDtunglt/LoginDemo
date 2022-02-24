using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DealCards : MonoBehaviour
{
    [SerializeField] private Transform[] splitGroups;
    [FormerlySerializedAs("card")] [SerializeField] private Image[] backCardImages;
    [SerializeField] private Transform[] dealtGroups;
    [SerializeField] private Button[] dealtButtons;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject[] playerObjs;
    [SerializeField] private GameObject toastTutorial;
    private int _targetIdxGroup = 0;
    private static readonly int[] pairs = {3, 6, 4, 7, 5, 8, 9, 1, 10, 2};
    private int _cardCaiId;

    private int _seatCoCai;

    // private int _timeChonNoc;
    private string _usernameChonNoc; //Là chính mình thì sẽ là rỗng;
    private List<PlayerMediator> _listPlayerView;

    private Action _dealDone;
    // private long t1;

    private Transform Target(int idxSplitGroup)
    {
        return dealtGroups[1 + idxSplitGroup * dealtGroups.Length / 2 + _targetIdxGroup % (dealtGroups.Length / 2)];
    }

    private Sequence DealAnim => DOTween.Sequence()
        .Join(splitGroups[0].DOMove(
            Target(0).position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)),
            0.2f))
        .Join(splitGroups[0].DOLocalRotate(Vector3.forward * Random.Range(50f, 60f), 0.2f))
        .Join(splitGroups[1].DOMove(
            Target(1).position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)),
            0.2f))
        .Join(splitGroups[1].DOLocalRotate(Vector3.forward * Random.Range(50f, 60f), 0.2f))
        .OnComplete(() =>
        {
            splitGroups[0].GetChild(0).SetParent(Target(0));
            splitGroups[1].GetChild(0).SetParent(Target(1));
            _targetIdxGroup++;
            if (_targetIdxGroup < 20)
                DealAnim.Play();
            else
            {
                splitGroups[1].DOMove(Vector3.zero, 0.2f);

                var flyTween = DOTween.Sequence();
                for (var i = 0; i < pairs.Length / 2; i++)
                {
                    dealtGroups[pairs[i * 2]].SetParent(dealtGroups[pairs[i * 2 + 1]]);
                    flyTween.Join(dealtGroups[pairs[i * 2]].DOLocalMove(Vector3.zero, 0.5f))
                        .SetDelay(Random.Range(0f, 0.2f));
                }

                flyTween.OnComplete(() =>
                {
                    if (canvasGroup == null) return;

                    if (!string.IsNullOrEmpty(_usernameChonNoc))
                    {
                        Toast.Show($"Đợi <b>{_usernameChonNoc}</b> chọn nọc, bốc cái");
                    }
                    else
                    {
                        canvasGroup.blocksRaycasts = true;
                        dealtButtons[dealtButtons.Length - 1].interactable = true;
                        toastTutorial.SetActive(true);
                        toastTutorial.transform.GetChild(0).GetComponent<TMP_Text>().text = AppMsg.CLICKHERE;
                    }

                    CountTimeChonNoc();
                    Signals.Get<DealCardFlyDoneSignal>().Dispatch();
                });
                flyTween.Play();
            }
        });

    private void CountTimeChonNoc()
    {
        var idx = GamePlayModel.Instance.UserBocCaiIdx();
        var vo = idx == GamePlayModel.Instance.myIdx
            ? new ShowTimeCounterVO(idx, SDTimeout.CHON_NOC, OnTimeOutChonNoc)
            : new ShowTimeCounterVO(idx, SDTimeout.CHON_NOC, null);
        Signals.Get<ShowTimeCounterSignal>().Dispatch(vo);
    }

    private void Start()
    {
        canvasGroup.blocksRaycasts = false;
        mainCamera = Camera.main;
        Signals.Get<ChonNocSignal>().AddListener(OnChonNocReceive);
        Signals.Get<BocCaiSignal>().AddListener(OnBocCaiReceive);
        Signals.Get<GamePlayBackCardSignal>().AddListener(ChangeBackCard);
    }

    private void OnEnable()
    {
        ChangeBackCard(GameUtils.GameBC);
    }

    private void ChangeBackCard(int i)
    {
        Sprite backCard = GlobalDataManager.Ins.backCard[i];
        foreach (var card in backCardImages)
        {
            if (!card.IsDestroyed())
                card.sprite = backCard;
        }
    }

    private void OnDisable()
    {
        Signals.Get<ChonNocSignal>().RemoveListener(OnChonNocReceive);
        Signals.Get<BocCaiSignal>().RemoveListener(OnBocCaiReceive);
        Signals.Get<GamePlayBackCardSignal>().RemoveListener(ChangeBackCard);
    }

    public void SetData(int cardCaiId, int seatCoCai, string usernameChonNoc, List<PlayerMediator> listPlayerView,
        Action dealDone)
    {
        gameObject.SetActive(true);
        _cardCaiId = cardCaiId;
        _seatCoCai = seatCoCai;
        _usernameChonNoc = usernameChonNoc;
        _listPlayerView = listPlayerView;
        playerObjs = listPlayerView.Select(s => s.tfCountdown.gameObject).ToArray();
        _dealDone = dealDone;
        DealAnim.Play();
    }

    private bool dangChonNoc = false;
    private bool dangChonBaiCai = false;
    private int idxNoc;
    private Camera mainCamera;

    public void ClickBatDauChonNoc()
    {
        Toast.Show(AppMsg.CHONNOC);
        dealtButtons[0].interactable = false;
        for (int i = 0; i < dealtButtons.Length - 1; i++) dealtButtons[i].interactable = true;
        toastTutorial.SetActive(false);
        dangChonNoc = true;
    }

    public void ChonNocClick(int idx)
    {
        if (!dangChonNoc) return;
        SmartFoxConnection.Instance.SendExt(ExtCmd.ChonNoc, new ChonNocVO(idx));
        dangChonNoc = false;
        dangChonBaiCai = true;
    }

    private void OnChonNocReceive()
    {
        idxNoc = ChiaBaiModel.Instance.nocIdx;
        if (string.IsNullOrEmpty(_usernameChonNoc))
        {
            Toast.Show(AppMsg.CHONBAICAI);
            dealtButtons[idxNoc].gameObject.SetActive(false);
            for (int i = 0; i < 5; i++)
            {
                if (i == idxNoc) continue;
                int i1 = i;
                dealtButtons[i].onClick.AddListener(() => BocCaiClick(i1));
            }
        }
        else
        {
            dealtButtons[idxNoc].gameObject.SetActive(false);
            splitGroups[1].GetChild(0).gameObject.SetActive(false);
        }

        Signals.Get<ChonNocDoneSignal>().Dispatch();
    }

    public void BocCaiClick(int idx)
    {
        if (dangChonNoc || !dangChonBaiCai) return;
        SmartFoxConnection.Instance.SendExt(ExtCmd.BocCai, new BocCaiVO(idx));
        dangChonBaiCai = false;
    }

    private void OnBocCaiReceive()
    {
        SDLogger.LogError(_cardCaiId + "  ");
        Signals.Get<ShowTimeOutMsgSignal>().Dispatch("");
        Signals.Get<HideTimeCounterSignal>().Dispatch();
        int idx = ChiaBaiModel.Instance.baiCaiIdx;
        splitGroups[1].GetChild(0).gameObject.SetActive(true);
        SpriteAtlas atlas = Resources.Load<SpriteAtlas>("SpriteAtlas/CardsAtlas");
        if(atlas!= null)
            splitGroups[1].GetChild(0).GetComponent<Image>().sprite = atlas.GetSprite(_cardCaiId.ToString());
        splitGroups[1].GetChild(0).SetParent(dealtButtons[idx].transform);
        dealtButtons[idx].transform.GetChild(dealtButtons[idx].transform.childCount - 1).localPosition = Vector3.zero;
        if (!string.IsNullOrEmpty(_usernameChonNoc))
        {
            try
            {
                dealtButtons[idx].transform.GetChild(5).localPosition = Vector3.zero;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        Invoke(nameof(CardMoveToPlayer), 1f);
    }

    private void CardMoveToPlayer()
    {
        int idx = ChiaBaiModel.Instance.baiCaiIdx;
        Sequence cardMove = DOTween.Sequence();
        cardMove.Join(dealtButtons[idx].transform.DOMove(playerObjs[_seatCoCai].transform.position, 1f)
            .OnComplete(() => dealtButtons[idx].gameObject.SetActive(false)));

        //Xếp lại 4 tụ bài còn lại theo đúng thứ tự, tụ đầu tiên là cái
        List<Transform> listHeapCard = new List<Transform>();
        listHeapCard.Add(dealtButtons[idx].transform);
        for (int i = 0; i < 3; i++)
        {
            idx = GetNextHipCardIdx(idx);
            listHeapCard.Add(dealtButtons[idx].transform);
        }

        //Di chuyển 4 tụ bài về phía 4 người chơi, (i1 + _seatCoCai) % 4 để tụ đầu tiên bay về phía người chơi có cái
        for (int i = 0; i < 4; i++)
        {
            int i1 = i;
            cardMove.Join(listHeapCard[i1].transform.DOMove(playerObjs[(i1 + _seatCoCai) % 4].transform.position, 1f)
                .OnComplete(() => listHeapCard[i1].gameObject.SetActive(false)));
        }

        cardMove.OnComplete(() =>
        {
            _dealDone?.Invoke();
            Signals.Get<DealCardDoneSignal>().Dispatch();
        });
        cardMove.Play();
    }

    private int GetNextHipCardIdx(int idx)
    {
        int nextIdx = idx;
        do
        {
            nextIdx--;
            if (nextIdx < 0) nextIdx = 4;
        } while (!dealtButtons[nextIdx].gameObject.activeInHierarchy);

        return nextIdx;
    }

    private void LateUpdate()
    {
        if (dangChonNoc || dangChonBaiCai)
        {
            Vector3 pos = Input.mousePosition;
            try
            {
                pos.z = splitGroups[1].GetChild(0).position.z - mainCamera.transform.position.z;
                splitGroups[1].GetChild(0).position = mainCamera.ScreenToWorldPoint(pos);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    private void OnTimeOutChonNoc()
    {
        toastTutorial.SetActive(false);
        if (!dangChonNoc && !dangChonBaiCai) dangChonNoc = true;
        if (dangChonNoc && !dangChonBaiCai)
        {
            idxNoc = Random.Range(0, 4);
            ChonNocClick(idxNoc);
        }

        if (!dangChonNoc && dangChonBaiCai)
        {
            int idxCai = (idxNoc + Random.Range(1, 4)) % 5;
            // SDLogger.Log(idxNoc + " auto " + idxCai);
            BocCaiClick(idxCai);
        }
    }
}