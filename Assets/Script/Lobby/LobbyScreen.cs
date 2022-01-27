﻿using UnityEngine;

public class LobbyScreen : MonoBehaviour
{
    public static LobbyScreen Instance
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType<LobbyScreen>();
            }
            return instance;
        }
    }
    private static LobbyScreen instance;
    public LobbyMediator lobbyMediator;
    public NoticePopUp noticePopUp;
    public MenuMediator menuMediator;
    public UserDetailMediator userDetailMediator;
}
