using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamesResultMediator : UIPopup
{
    [SerializeField] private TMP_Text txtInfo, txtPl1, txtPl2, txtPl3, txtPl4;
    [SerializeField] private TMP_Text txtPoint1, txtPoint2, txtPoint3, txtPoint4;
    [SerializeField] private GameObject scroll;
    [SerializeField] private Transform content;
    [SerializeField] private ScrollRect scrollRect;
    private UserModel userModel = UserModel.Instance;
    private TourModel tourModel = TourModel.Instance;
    private GamesResultVO resultVo;
    [SerializeField] private ItemGamesResult itemResult;
    [SerializeField] private RectTransform panelRect, txtInfoRect;
    [SerializeField] private float basePanelWidth, challengePanelWidth;
    [SerializeField] private float baseTxtInfoWidth, challengeTxtInfoWidth;
    [SerializeField] private float baseTxtInfoPosY, challengeTxtInfoPosY;
    public string color;
    public string loseColor;

    public static void OpenPopup(GamesResultVO vo, string name)
    {
        ViewCreator.OpenPopup(PopupId.GamesResultPanel, view =>
        {
            var p = view.Cast<GamesResultMediator>();
            p.resultVo = vo;
            p.UpdateView(vo, name);
        });
    }

    private void UpdateView(GamesResultVO vo, string name)
    {
        var isPlayer = false;
        var challengeModel = ChallengeModel.Instance;
        if (vo.gameInfos == null)
        {
            txtInfo.text = "Chưa chơi xong ván nào";
            ShowPanel(false);
            scroll.Hide();
        }
        else
        {
            scroll.Show();
            if (!vo.IsChallenge)
            {
                ShowPanel(false);
                if (vo.IsEndOfTour)
                {
                    if (vo.IsNoOneWin)
                    {
                        txtInfo.text = "Không có ai " + (vo.IsArenaFinal ? "qua vòng" : "đỗ trong kì thi này.");
                    }
                    else
                    {
                        var msgText = "Chúc mừng ";
                        foreach (var _name in vo.winNames)
                        {
                            msgText += "<b><color=#C00000>";
                            msgText += _name;
                            msgText += "</color></b>";
                            msgText += vo.winNames.IndexOf(_name) == vo.winNames.Count - 1 ? " " : ", ";
                        }

                        msgText += "đã " + (vo.IsArenaFinal ? "qua vòng" : "đỗ ");
                        txtInfo.text = msgText;
                    }
                }
                else
                {
                    txtInfo.text = "Số ván đã chơi: " + vo.gameInfos.Count;
                }
            }
            else
            {
                if (vo.IsEndOfTour)
                {
                    ShowPanel(true);
                    if (vo.IsNoOneWin)
                    {
                        var msgText = "";
                        msgText = "Không có ai chiến thắng lượt Thách đấu này";
                        msgText +=
                            $"\nThách đấu mức cược {SetColor(StringUtils.FormatMoneyK(challengeModel.gameInfo.stake))} - Tổng gà góp {SetColor(StringUtils.FormatMoneyK(challengeModel.gameInfo.totalGa))}";
                        msgText +=
                            $"\nNhận lại {SetColor(StringUtils.FormatMoneyK(challengeModel.gameInfo.gaStake))} Bảo từ góp gà Thách đấu";
                        txtInfo.text = msgText;
                    }
                    else
                    {
                        var msgText = "";
                        var idx = resultVo.winUids.IndexOf(int.Parse(userModel.uid));
                        if (idx == 0)
                        {
                            msgText = "Chúc mừng ";
                            msgText += SetColor($"{resultVo.winNames[idx]}");
                            msgText += " đã chiến thắng lượt Thách đấu này";
                            msgText +=
                                $"\nThách đấu mức cược {SetColor(StringUtils.FormatMoneyK(challengeModel.gameInfo.stake))} - Tổng gà góp {SetColor(StringUtils.FormatMoneyK(challengeModel.gameInfo.totalGa))}";
                            msgText +=
                                $"\nBạn thắng {SetColor(StringUtils.FormatMoneyK(challengeModel.gameInfo.totalGa * GameConfig.CHALLENGE_RATIO))} Bảo từ góp gà Thách đấu";
                        }
                        else
                        {
                            msgText += "Bạn đã thua trong lượt Thách đấu này";
                            msgText +=
                                $"\nThách đấu mức cược {SetColor(StringUtils.FormatMoneyK(challengeModel.gameInfo.stake))} - Tổng gà góp {SetColor(StringUtils.FormatMoneyK(challengeModel.gameInfo.totalGa))}";
                            msgText +=
                                $"\nBạn mất {SetColor2(StringUtils.FormatMoneyK(challengeModel.gameInfo.gaStake))} Bảo góp gà Thách đấu";
                        }

                        txtInfo.text = msgText;
                    }
                }
                else
                {
                    ShowPanel(false);
                    txtInfo.text = "Số ván đã chơi: " + vo.gameInfos.Count;
                }
            }


            var listScore = new List<TMP_Text> {txtPl1, txtPl2, txtPl3, txtPl4};
            for (var i = 0; i < listScore.Count; i++)
            {
                if (i < vo.names.Count)
                {
                    listScore[i].text = vo.names[i];
                    if (vo.names[i] == name) isPlayer = true;
                }
                else
                {
                    listScore[i].text = "";
                }
            }

            foreach (RectTransform child in content)
            {
                child.Hide();
            }

            var gameTotal = vo.gameInfos.Count;
            for (var i = 0; i < gameTotal; i++)
            {
                if (i >= content.childCount)
                {
                    Instantiate(itemResult, content);
                }

                var item = content.GetChild(i).GetComponent<ItemGamesResult>();
                item.UpdateItem(i + 1, vo.gameInfos[i], vo.logIds[i], isPlayer);
            }

            var listPoint = new List<TMP_Text> {txtPoint1, txtPoint2, txtPoint3, txtPoint4};
            for (var i = 0; i < listPoint.Count; i++)
            {
                listPoint[i].text = i < vo.finalPoint.Count ? vo.finalPoint[i].ToString() : "";
            }
        }

        if (vo.IsEndOfTour && !resultVo.IsChallenge)
        {
            if (!vo.IsNoOneWin)
            {
                UpdateMsg();
            }
        }

        scrollRect.verticalNormalizedPosition = 0;
        OnUpdateData();
    }

    private void UpdateMsg()
    {
        if (!resultVo.IsChallenge)
        {
            var path = "";
            var msgText = "";
            if (!isPlayer) return; // là player
            var nextTour = tourModel.GetNextTour();
            if (!tourModel.IsThiDinh)
            {
                //Hương hội.
                if (resultVo.winUids.IndexOf(int.Parse(userModel.uid)) != -1)
                {
                    //và là thằng thắng.
                    msgText += "Chúc mừng ";
                    msgText += "<b><color=#C00000>";
                    msgText += userModel.name;
                    msgText += "</color></b>";
                    msgText += " đã đỗ ";
                    if (tourModel.IsEndOfRound(resultVo))
                    {
                        // đạt được trophy nào đó.
                        msgText += resultVo.TourWinTrophy(int.Parse(userModel.uid));
                        path = "tour_win";
                    }
                    else
                    {
                        msgText += tourModel.IsThiHuong ? "Thi Hương " : "Thi Hội ";
                        msgText += " - Kỳ " + (tourModel.Round + 1);
                    }
                }
                else
                {
                    //thằng thua.
                    msgText += "Bạn đã trượt.";
                    path = "tour_lose";
                }
            }
            else if (resultVo.IsThiDinhVongLoai)
            {
                // thi đình vòng loại thua cũng có thể được chọn =>
                if (nextTour != null)
                {
                    msgText +=
                        "Bạn đã hoàn thành vòng loại 1.\nKết quả và thông tin vòng tiếp theo sẽ được cập nhật trong ít phút nữa.";
                }
                else
                {
                    msgText += "Bạn đã hoàn thành kỳ thi. Kết quả sẽ được cập nhật sau khi thi Đình kết thúc.";
                }
            }
            else
            {
                /* Thi đình không phải vòng chung kết => chỉ có 2 vị trí đầu là thông báo thắng
                      2 vị trí sau thì thông báo hoàn thành kì thi nhưng nếu là trận chung kết thì thằng thua
                      cũng đạt được chức danh gì đó. */
                var pos = resultVo.winUids.IndexOf(int.Parse(userModel.uid));
                if (pos != -1 && pos < 2 || tourModel.IsEndOfRound(resultVo))
                {
                    msgText += "Chúc mừng ";
                    msgText += "<b><color=#C00000>";
                    msgText += userModel.name;
                    msgText += "</color></b>";
                    msgText += " đã đỗ ";
                    msgText += resultVo.TourWinTrophy(int.Parse(userModel.uid));
                    path = "tour_win";
                }
                else
                {
                    //thằng thua.
                    msgText += "Bạn được chọn vào danh sách dự bị vòng " + resultVo.DinhNextTourName();
                    path = "tour_lose";
                }

                if (nextTour != null && msgText.Length > 0)
                {
                    msgText += "\nXếp hạng và thông tin vòng thi sẽ được cập nhật trong ít phút nữa.";
                }
            }

            txtInfo.text = msgText;

            ViewCreator.LoadSound(path, clip =>
            {
                //if (clip != null) StartCoroutine(SoundManager.Instance.PlaySoundTour(clip));
            });
        }
        else
        {
            var msgText = "";
            if (resultVo.winUids.IndexOf(int.Parse(userModel.uid)) == 0)
            {
                //và là thằng thắng.
                msgText += "Chúc mừng ";
                msgText += "<b><color=#C00000>";
                msgText += userModel.name;
                msgText += "</color></b>";
                msgText += " đã chiến thắng lượt Thách đấu này";
            }
            else
            {
                //thằng thua.
                msgText += "Bạn thua trong lượt Thách đấu này";
            }

            txtInfo.text = msgText;
        }
    }

    private void OnUpdateData()
    {
        if (!resultVo.IsEndOfTour) return;
        ScreenManager.Instance.GoEntrance();
        if (resultVo.winUids.IndexOf(int.Parse(userModel.uid)) != -1)
        {
            if (tourModel.IsEndOfRound(resultVo))
            {
                tourModel.Round = 0;
            }
            else
            {
                tourModel.Round += 1;
            }
        }
        else if (isPlayer)
        {
            tourModel.Round = 0;
        }

        tourModel.Playing = false;
        if (resultVo.IsDinhFinal && resultVo.allUids.IndexOf(int.Parse(userModel.uid)) != -1)
        {
            tourModel.Joinable = false;
        }

        if (tourModel.IsThiDinh)
        {
            // kết thúc thi đình thì không cho join, phải refresh để update thông tin tour
            tourModel.Joinable = false;
        }

        if (isPlayer)
        {
            tourModel.IsPlayed = true;
        }

        tourModel.Dispatch();
    }

    private bool isPlayer => resultVo.allUids != null && resultVo.allUids.IndexOf(int.Parse(userModel.uid)) != -1;

    public void OnHide()
    {
        for (var i = content.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(content.transform.GetChild(i).gameObject);
        }

        Close();
    }

    private void ShowPanel(bool isChallenge)
    {
        if (isChallenge)
        {
            panelRect.ChangeHeight(challengePanelWidth);
            txtInfoRect.ChangeHeight(challengeTxtInfoWidth);
            txtInfoRect.ChangeAnchorY(challengeTxtInfoPosY);
        }
        else
        {
            panelRect.ChangeHeight(basePanelWidth);
            txtInfoRect.ChangeHeight(baseTxtInfoWidth);
            txtInfoRect.ChangeAnchorY(baseTxtInfoPosY);
        }
    }

    private string SetColor(string s)
    {
        return $"<b><color=#{color}>{s}</b></color>";
    }

    private string SetColor2(string s)
    {
        return $"<b><color=#{loseColor}>{s}</b></color>";
    }
}