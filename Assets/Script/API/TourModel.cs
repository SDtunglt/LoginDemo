using System.Collections.Generic;
using System.Linq;
using Sfs2X.Entities.Data;
using UnityEngine;

public class TourModel : Singleton<TourModel>
{
    private TourVOChangedSignal tourVOChangedSignal = Signals.Get<TourVOChangedSignal>();

    private MultiActiveToursVOChangedSignal multiActiveToursVOChangedSignal =
        Signals.Get<MultiActiveToursVOChangedSignal>();
    private TourVO _tourVO = new TourVO();
    private List<TourVO>_multiActiveTours;
    public static bool isOutDateConfig = false;
    public TourVO GetNextTour()
    {
        TourVO nextTour = null;
        if (multiActiveTours != null && multiActiveTours.Count != 0)
        {
            var min = 0;
            foreach (var activeTour in _multiActiveTours)
            {
                if (activeTour.openTime > _tourVO.openTime)
                {
                    if (min == 0 || activeTour.openTime - _tourVO.openTime < min)
                    {
                        nextTour = activeTour;
                        min = activeTour.openTime - _tourVO.openTime;
                    }
                }
            }
        }

        return nextTour;
    }

    public List<TourVO> multiActiveTours => _multiActiveTours;

    public TourVO tourVO => _tourVO;

    public string Name => GameConfig.ZoneCfg[Zone].name;

    public bool Joinable
    {
        get => _tourVO.joinable;
        set => _tourVO.joinable = value;
    }

    public int Zone => _tourVO.zone;

    public int Round
    {
        get => _tourVO.lastRound;
        set => _tourVO.lastRound = value;
    }

    public bool Playing
    {
        get => _tourVO.playing;
        set => _tourVO.playing = value;
    }

    public bool IsPlayed
    {
        get => _tourVO.isPlayed;
        set => _tourVO.isPlayed = value;
    }

    public bool IsThiHuong => _tourVO.zone == GameConfig.IdRoomThiHuong;

    public bool IsThiHoi => _tourVO.zone == GameConfig.IdRoomThiHoi;

    public bool IsThiDinh => _tourVO.zone == GameConfig.IdRoomThiDinh;

    public int TourFee => _tourVO.tourFee;

    public List<UserThiDinhVO> arrUsers = new List<UserThiDinhVO>();
    public int thidinhBaoDanhCount = 0;
    // Dùng để check xem Client đã nhận được danh sách người chơi từ server hay chưa
    public bool isReceiveThidinhUsers;

    public bool IsEndOfRound(GamesResultVO vo)
    {
        return vo.IsDinhFinal || (vo.IsHuongHoiTour && Round + 1 >= tourVO.maxRound);
    }

    public void InitData(TourVO vo)
    {
        _tourVO = vo;
        Dispatch();
    }

    public void InitMultiActiveTourData(MultiTourActiveVO vo)
    {
        if (vo?.activeTours == null) return;
        _multiActiveTours = vo.activeTours;
        multiActiveToursVOChangedSignal.Dispatch(_multiActiveTours);
    }

    /** dispatch only when game or round changed */
    public void Dispatch()
    {
        tourVOChangedSignal.Dispatch(_tourVO);
    }

    public void OnReceiveThiDinhListUser(SFSObject data)
    {
        var vo = new WaitingUsersVO();
        vo.fromSFSObject(data);
        isReceiveThidinhUsers = false;
        // SDLogger.LogError(data.ToJson());
        if (vo.users.Any(s => s.id == int.Parse(UserModel.Instance.uid)))
        {
            arrUsers = new List<UserThiDinhVO>();
            GetDsThiDinhAnUserJoinTour(vo);
        }
        else
        {
            GetDsThiDinhAnUserJoinTour(vo);
        }
    }

    private void GetDsThiDinhAnUserJoinTour(WaitingUsersVO vo)
    {
        if (UserModel.Instance.dsBaoDanh.Count <= 0)
        {
            API.GetThiDinhRank(d =>
            {
                // SDLogger.LogError(d.ToString());
                if (d["status"].ToString().ToLower() != "ok") return;
                if (UserModel.Instance.dsBaoDanh.Count > 0)
                    UserModel.Instance.dsBaoDanh = new List<UserThiDinhVO>();
                if (d["result"] == null) return;

                var result = d["result"].ToList();

                if (result.Count > 0)
                {
                    // view.tourView.mvWarning.visible = true;
                    foreach (var item in result)
                    {
                        var isBaoDanh = int.Parse(item["uid"].ToString()) == int.Parse(UserModel.Instance.uid);
                        var user = new UserThiDinhVO(int.Parse(item["uid"].ToString()),
                            int.Parse(item["rank"].ToString()),
                            item["name"].ToString(),
                            int.Parse(item["point"].ToString()),
                            double.Parse(item["coin"].ToString()),
                            isBaoDanh);
                        UserModel.Instance.dsBaoDanh.Add(user);
                    }
                }

                SetUpUserJoinThiDinhCount(vo);
            }, err =>
            {
                // SDLogger.LogError(err);
                SetUpUserJoinThiDinhCount(vo);
            });
        }
        else
        {
            SetUpUserJoinThiDinhCount(vo);
        }
    }

    private void SetUpUserJoinThiDinhCount(WaitingUsersVO vo)
    {
        foreach (var u in UserModel.Instance.dsBaoDanh)
        {
            if (arrUsers.All(s => s.uid != u.uid))
            {
                arrUsers.Add(new UserThiDinhVO(u.uid, u.rank, u.name, u.point, u.coin, u.isBaoDanh));
            }
        }

        foreach (var u in vo.users)
        {
            if (tourVO.tourLevel == -1)
            {
                if (arrUsers.All(s => s.uid != u.id))
                {
                    var user = new UserThiDinhVO(u.id, -1, u.name, 0, u.coin, true);
                    arrUsers.Add(user);
                }
            }
            else
            {
                for (var i = 0; i < arrUsers.Count; i++)
                {
                    if (arrUsers[i].uid == u.id) arrUsers[i].isBaoDanh = true;
                }
            }
        }

        foreach (var uid in vo.leaveUids)
        {
            for (var i = arrUsers.Count - 1; i >= 0; i--)
            {
                if (tourVO.tourLevel == -1)
                {
                    if (arrUsers[i].uid == uid) arrUsers.RemoveAt(i);
                }
                else
                {
                    if (arrUsers[i].uid == uid) arrUsers[i].isBaoDanh = false;
                }
            }
        }

        thidinhBaoDanhCount = arrUsers.Count(s => s.isBaoDanh);
        Signals.Get<OnReceiveThiDinhJoinData>().Dispatch();
        isReceiveThidinhUsers = true;
    }
}
