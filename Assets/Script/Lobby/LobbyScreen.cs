using UnityEngine;

public class LobbyScreen : MonoBehaviour
{
    public static LobbyScreen Instance
    {
        get
        {
            if(instance != null) return instance;
            instance = FindObjectOfType<LobbyScreen>();
            return instance;
        }
    }
    private static LobbyScreen instance;
    public LobbyMediator lobbyMediator;
    public NoticePopUp noticePopUp;
    public MenuMediator menuMediator;
    public UserDetailMediator userDetail;
    public BottomBarMediator bottomBarMediator;
}
