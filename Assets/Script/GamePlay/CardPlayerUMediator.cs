using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CardPlayerUMediator : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefabs;
    [SerializeField] private RectTransform curveLayout;
    
    [SerializeField] private float radius = 140f;
    [SerializeField] private float arc = 12.5f;
    [SerializeField] private Vector2 cardSize;

    public void ShowCards(List<SDCard> cards)
    {
        SDCard.Sort(cards);
        for (var i = curveLayout.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(curveLayout.GetChild(i).gameObject);
        }
        
        foreach (var card in cards)
        {
            var cardObject = Instantiate(cardPrefabs, curveLayout);
            cardObject.GetComponent<Image>().sprite = card.GetSprite();
        }
        
        for (var i = 0; i < curveLayout.childCount; i++)
        {
            var childRTrans = (RectTransform) curveLayout.GetChild(i);
            var targetEulerAngles = Vector3.forward * (arc * (curveLayout.childCount - 1) / 2 - i * arc);
            childRTrans.DOKill();
            childRTrans.DOSizeDelta(cardSize, 0.3f);
            childRTrans.DOLocalRotate(targetEulerAngles, 0.3f).OnUpdate(() =>
            {
                childRTrans.anchoredPosition = Rotate(Vector2.up, childRTrans.localEulerAngles.z) *
                                               Mathf.Lerp(
                                                   (childRTrans.anchoredPosition - Vector2.down * radius).magnitude,
                                                   radius, 0.3f) +
                                               Vector2.down * radius;
            }).SetEase(Ease.OutQuad);
        }
    }
    
    private static Vector2 Rotate(Vector2 v, float delta)
    {
        delta *= Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }
}