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

}
