using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CuocViewMediator : MonoBehaviour
{
    public CanvasGroup Group;
    public SkeletonGraphic graphic;
    public Image bg;
    public SkeletonDataAsset[] cuocAssets;
    public TextMeshProUGUI specialCuocTxt;
    public TextMeshProUGUI normalCuocTxt;

    public GameObject normalCuocGo, specialCuocGo;
    public Vector2[] posForBanners;
    public Vector2[] posForAnim;
    public CanvasGroup normalGroup, specialGroup;
    public float timeShowNormalBanner = 3;

    [Button]
    public void ShowCuocU(string cuocsMsg)
    {
        if (cuocsMsg.Length <= 0)
            return;
        gameObject.Show();

        var special = ULogic.GetSpecialCuoc(cuocsMsg);
        if (special == SpecialCuoc.None)
        {
            ShowNormalCuoc(cuocsMsg);
        }else if (special == SpecialCuoc.CaLoiNguOng)
        {
            var newMsg = SplitCuocKep(cuocsMsg, ULogic.CA, ULogic.NGU);
            SetUpViewSpecial(newMsg, 0);
        }
        else
        {
            SetUpViewSpecial(cuocsMsg, (int) special);
        }
    }

    private void ShowSpecialCuoc(string msg)
    {
        specialCuocGo.Show();
        normalCuocGo.Hide();
        graphic.Show();
        bg.enabled = true;
        specialCuocTxt.text = msg;
    }

    private Sequence s;
    public float timeShowBanner = 0.5f;

    private void ShowAnimSpecial()
    {
        s?.Kill();
        specialCuocGo.transform.localScale = new Vector3(0, 1, 1);
        specialGroup.alpha = 0;

        s = DOTween.Sequence()
            .AppendInterval(timeShowBanner)
            .Append(specialGroup.DOFade(1, 0.2f))
            .Join(specialCuocGo.transform.DOScaleX(1.1f, 0.2f))
            .Append(specialCuocGo.transform.DOScaleX(1f, 0.02f));
    }

    private void ShowAnimNormal()
    {
        s?.Kill();
        normalGroup.alpha = 0;
        normalCuocGo.transform.localScale = new Vector3(0, 1, 1);
        s = DOTween.Sequence()
            .Append(normalGroup.DOFade(1, 0.2f))
            .Append(normalCuocGo.transform.DOScale(1.1f, 0.2f))
            .Append(normalCuocGo.transform.DOScale(1, 0.02f))
            .OnComplete(() => { DOVirtual.DelayedCall(timeShowNormalBanner, OnClose); });
    }

    private void ShowNormalCuoc(string msg)
    {
        Group.alpha = 1;
        specialCuocGo.Hide();
        normalCuocGo.Show();
        graphic.Hide();
        bg.enabled = false;
        normalCuocTxt.text = msg;
        ShowAnimNormal();
    }

    private void OnClose()
    {
        Group.DOFade(0, 0.2f).OnComplete(() => { gameObject.Hide(); });
    }

    private void SetUpViewSpecial(string msg, int index)
    {
        ShowSpecialCuoc(msg);
        Group.alpha = 1;
        graphic.skeletonDataAsset = cuocAssets[index];
        graphic.Initialize(true);
        graphic.Skeleton.SetToSetupPose();
        graphic.AnimationState.SetAnimation(0, "animation", false).Complete += entry => { OnClose();};
        specialCuocGo.GetComponent<RectTransform>().anchoredPosition = posForBanners[index];
        graphic.GetComponent<RectTransform>().anchoredPosition = posForAnim[index];
        ShowAnimSpecial();
    }

    private string SplitCuocKep(string cuocsMsg, int i1, int i2)
    {
        if (cuocsMsg.Count(s => s == '\n') > 0)
        {
            return cuocsMsg;
        }

        int index = cuocsMsg.IndexOf(ULogic.CUOC_NAMES[i1], StringComparison.Ordinal);
        int lastCharOfCuoc = index + ULogic.CUOC_NAMES[i1].Length;
        var s1 = cuocsMsg.Substring(0, lastCharOfCuoc);

        var s2 = cuocsMsg.Substring(lastCharOfCuoc + 1);

        return s1 + "\n          " + s2;
    }

    [Button]
    public void CheckCuocs()
    {
        var ls = new List<int>();
        for (int i = 0; i < 23; i++)
        {
            ls.Add(Random.Range(0, 2));
        }

        ShowCuocU(ULogic.GetCuocsAfterSplit(ls));
    }
}