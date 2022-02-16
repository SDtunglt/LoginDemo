using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class RoomMediator : MonoBehaviour
{
    [SerializeField] private TMP_Text txtLocationVg, txtZoneName,txtRoomName;
    [SerializeField] private List<BoardMediator> listBoardViews;
    [SerializeField] private List<Sprite> btnSorts;
    [SerializeField] private Button btnSortStake, btnSortUser,btnSortStatus,btnVaoBan;
    [SerializeField] private GameObject popupNhapMatKhau;
    [SerializeField] TMP_InputField ipfPassword,ipfBoardNum;
    [SerializeField] GameObject roomNormal;
    //[SerializeField] private RoomVuongPhuMediator roomVuongPhu;
    [SerializeField] private Transform contentListRoom,contentListBoard, zoneHeader;
    [SerializeField] private ToggleGroup gGroup;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button hideFullBoardBtn;
    [SerializeField] private GameObject tickHideFullBoardGo;

    private UserModel userModel = UserModel.Instance;
    private ScreenManager screenManager;
    private BoardInfoModel boardInfoModel = BoardInfoModel.Instance;

    private UpdateBoardInfoSignal updateBoardInfoSignal = Signals.Get<UpdateBoardInfoSignal>();
    private ScreenChangedSignal screenChangedSignal = Signals.Get<ScreenChangedSignal>();

    private RefreshCoinSignal refreshCoinSignal = Signals.Get<RefreshCoinSignal>();

    private bool isSortStake = false;
    private bool isSortUser = false;
    private bool isSortStatus = false;
    [SerializeField] private ItemRoomSelect itemSelect;

    [ContextMenu("FindBoardView")]

    void FindBoardView()
    {
        listBoardViews = GetComponentsInChildren<BoardMediator>().ToList();
    }
    private void Awake()
    {
        screenManager = ScreenManager.Instance;
        if(tickHideFullBoardGo)
        {
            tickHideFullBoardGo.Hide();
        }
        isHideFullBoard = false;
    }

    private void Start()
    {
        if(hideFullBoardBtn)
        {
            hideFullBoardBtn.onClick.AddListener(HideFullBoard);
        }
    }

    private bool isHideFullBoard = false;
    private void HideFullBoard()
    {
        isHideFullBoard = !isHideFullBoard;
        tickHideFullBoardGo.SetActive(isHideFullBoard);
        UpdateWithHideFullBoard();
    }

    private void OnDisable()
    {
        screenChangedSignal.RemoveListener(UpdateNameRoom);
        updateBoardInfoSignal.RemoveListener(UpdateBoardInfo);
    }

    private void OnEnable()
    {
        Init();
        screenChangedSignal.AddListener(UpdateNameRoom);
        updateBoardInfoSignal.AddListener(UpdateBoardInfo);
        UpdateNameRoom();
        UpdateRoom();
        DOVirtual.DelayedCall(1, OnCheckResume);
    }

    private void Update()
    {
        if(Application.platform != RuntimePlatform.Android) return;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            DOVirtual.DelayedCall(.2f, () =>
            {
                Debug.Log("Thông báo ,Bạn có muốn thoát khỏi trò chơi?");
                Application.Quit();
            });
        }
    }

    private void OnCheckResume()
    {
        if(screenManager.roomResume == null) return;
        screenManager.JoinBoard(screenManager.roomResume.b,screenManager.roomResume.r);
        screenManager.roomResume = null;
    }

    public void OnJoinBoard()
    {
        if(string.IsNullOrEmpty(ipfBoardNum.text))
        {
            Debug.Log("Thông báo , vui lòng nhập sô bàn");
            return;
        }

        var boardId = int.Parse(ipfBoardNum.text) - 1;
        if(boardId < 0 || boardId >= GameConfig.MaxBoardInRoom)
        {
            Debug.Log("Thông báo, số bàn bạn nhập vượt quá số lượng");
        }
        else
        {
            if(boardInfoModel.GetInfo(boardId).isLocked && !GameUtils.IsMod(int.Parse(userModel.uid)))
            {
                popupNhapMatKhau.Show();
                btnVaoBan.onClick.RemoveAllListeners();
                btnVaoBan.onClick.AddListener(() =>
                {
                    popupNhapMatKhau.Hide();
                    if(string.IsNullOrEmpty(ipfPassword.text))
                    {
                        Debug.Log($"Lỗi {AppMsg.CANTJOINLOCKED}");
                    }
                    else
                    {
                        screenManager.JoinBoard(boardId,screenManager.room,ipfPassword.text);
                    }
                });
            }
            else
            {
                screenManager.JoinBoard(boardId, screenManager.room);
            }
        }
    }

    private bool isOnInit = false;

    private void UpdateRoom()
    {
        if(screenManager.zone == GameConfig.IdRoomVuongPhu)
        {
            roomNormal.SetActive(false);
        }
        else
        {
            roomNormal.SetActive(true);
        }

        for(var i = contentListRoom.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(contentListRoom.GetChild(i).gameObject);
        }

        var index = 0;
        var count = GameConfig.ZoneCfg[screenManager.zone].rooms.Length;
        foreach(var roomName in GameConfig.ZoneCfg[screenManager.zone].rooms)
        {
            index++;
            var idx = GameConfig.ZoneCfg[screenManager.zone].rooms.ToList().IndexOf(roomName);
            var item = Instantiate(itemSelect, contentListRoom);
            item.UpdateView(idx, roomName,OnRoomChangeCb,gGroup,index == count);
            DOVirtual.DelayedCall(.2f, () => {isOnInit = false; });
        }
    }

    private void Init()
    {
        isOnInit = true;
        GameConfig.MaxBoardInRoom = listBoardViews.Count;
        for( var i = 0;i < listBoardViews.Count;i++)
        {
            var boardId = i;
            listBoardViews[boardId].SetData(boardId);
        }
        if(!SyncBoardInfo())
        {
            boardInfoModel.InitBoardInfos();
        }
    }

    private void UpdateBoardInfo(int i)
    {
        if(i >= listBoardViews.Count) return;
        foreach(var t in listBoardViews)
        {
            if(t.GetBoardID != i) continue;
            var info = boardInfoModel.GetInfo(i);
            t.UpdateData(info);
        }

        UpdateWithHideFullBoard();
    }

    private void UpdateWithHideFullBoard()
    {
        foreach(var boardMediator in listBoardViews)
        {
            if(isHideFullBoard)
            {
                boardMediator.gameObject.SetActive(!boardMediator.IsFullBoard);
            }
            else
            {
                boardMediator.Show();
            }
        }
    }

    bool SyncBoardInfo()
    {
        if(boardInfoModel != null && boardInfoModel.IsInited())
        {
            for(int i = 0; i< listBoardViews.Count; i++)
            {
                listBoardViews[i].UpdateData(boardInfoModel.GetInfo(i));
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateNameRoom(NormalJoinVO vo = null)
    {
        if(txtZoneName != null && screenManager.zone >= 0 && screenManager.room >= 0)
        {
            txtZoneName.text = GameConfig.ZoneCfg[screenManager.zone].name + " -";
            txtRoomName.text = GameConfig.ZoneCfg[screenManager.zone].rooms[screenManager.room];
        }

        switch(screenManager.zone)
        {
            case -1:
                return;
            case GameConfig.IdRoomVuongPhu:
                txtLocationVg.text =    GameConfig.ZoneCfg[screenManager.zone].name + " -" +
                                        GameConfig.ZoneCfg[screenManager.zone].rooms[screenManager.room];
                break;
            default:
                {
                    break;
                }
        }
    }

    private void OnRoomChangeCb(int room)
    {
        if(!isOnInit)
        {
            scrollRect.DOVerticalNormalizedPos(1,.4f);
        }
        ScreenManager.Instance.JoinRoom(screenManager.zone,room,true);
    }

    public void OnSortStake()
    {
        btnSortStake.image.sprite = isSortStake ? btnSorts[1] : btnSorts[0];
        isSortStake = !isSortStake;
        OnSortStake(isSortStake);
    }

    public void OnSortUser()
    {
        btnSortUser.image.sprite = isSortUser ? btnSorts[1] : btnSorts[0];
        isSortUser = ! isSortUser;
        OnSortUser(isSortUser);
    }

    public void OnSortStatus()
    {
        btnSortStatus.image.sprite = isSortStatus ? btnSorts[1] : btnSorts[0];
        isSortStatus = !isSortStatus;
        OnSortStatus(isSortStatus);
    }

    private void OnSortStake(bool isAscending)
    {
        for (var i = 0; i < listBoardViews.Count; i++)
        {
            for (var j = i + 1; j < listBoardViews.Count; j++)
            {
                if ((isAscending || listBoardViews[i].GetBoardInfo.stake >= listBoardViews[j].GetBoardInfo.stake) &&
                    (!isAscending || listBoardViews[i].GetBoardInfo.stake <= listBoardViews[j].GetBoardInfo.stake))
                    continue;
                var cur = listBoardViews[i];
                listBoardViews[i] = listBoardViews[j];
                listBoardViews[j] = cur;

                contentListBoard.GetChild(j).SetSiblingIndex(i);
                contentListBoard.GetChild(i + 1).SetSiblingIndex(j);
            }
        }
    }

    private void OnSortUser(bool isAscending)
    {
        for (var i = 0; i < listBoardViews.Count; i++)
        {
            for (var j = i + 1; j < listBoardViews.Count; j++)
            {
                if ((isAscending ||
                     listBoardViews[i].GetBoardInfo.sitCount >= listBoardViews[j].GetBoardInfo.sitCount) &&
                    (!isAscending || listBoardViews[i].GetBoardInfo.sitCount <=
                        listBoardViews[j].GetBoardInfo.sitCount))
                    continue;
                var cur = listBoardViews[i];
                listBoardViews[i] = listBoardViews[j];
                listBoardViews[j] = cur;

                contentListBoard.GetChild(j).SetSiblingIndex(i);
                contentListBoard.GetChild(i + 1).SetSiblingIndex(j);
            }
        }
    }

    private void OnSortStatus(bool isAscending)
    {
        for(var i = 0; i < listBoardViews.Count;i++)
        {
            for(var j = i + 1; j < listBoardViews.Count;j++)
            {
                var statusA = listBoardViews[i].GetBoardInfo.isPlaying ? 1 : 0;
                var statusB = listBoardViews[j].GetBoardInfo.isPlaying ? 1 : 0;
                if((isAscending || statusA >= statusB) && (!isAscending || statusA <= statusB))
                    continue;
                var cur = listBoardViews[i];
                listBoardViews[i] = listBoardViews[j];
                listBoardViews[j] = cur;
                
                contentListBoard.GetChild(j).SetSiblingIndex(i);
                contentListBoard.GetChild(i + 1).SetSiblingIndex(j);
            }
        }
    }

    public ItemRoomSelect GetRoomItem(int index)
    {
        return contentListRoom.GetChild(index).GetComponent<ItemRoomSelect>();
    }

    public BoardMediator GetBoardItem(int index)
    {
        return listBoardViews[index];
    }
}
