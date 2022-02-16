using System;
using UnityEngine;

public class ShowItemReviewController : MonoBehaviour
{
    [SerializeField] private GameObject[] itemReview;

    private GameModel gameModel = GameModel.Instance;

    private void OnEnable()
    {
        Signals.Get<UpdateShowCoinSignal>().AddListener(UpdateCoinState);
        UpdateCoinState(gameModel.IsNormalPlayer());
    }

    private void OnDisable()
    {
        Signals.Get<UpdateShowCoinSignal>().RemoveListener(UpdateCoinState);
    }

    private void UpdateCoinState(bool isShow)
    {
        foreach(var item in itemReview)
        {
            item.SetActive(true);
        }
    }
}
