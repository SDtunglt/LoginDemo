using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuGroupController : MonoBehaviour
{
    public List<GameObject> buttons;
    public List<ButtonType> specialButtons;
    public List<ScreenButtonType> screens;
    public string currentScreen;
    private void Awake()
    {
        ScreenManager.Instance.onScreenChangeDefine += OnScreenChange;
        OnScreenChange(ScreenManager.LOGIN);
    }

    public void OnScreenChange(string id)
    {
        currentScreen = id;
        var screen = screens.Find(s => s.screenId == id);
        ControlButtons(screen.hiddenTypes);
    }

    public static MenuGroupController Ins
    {
        get
        {
            if (!_ins)
            {
                _ins = ObjectFinder.GetObject(TagId.MenuGroup).GetComponent<MenuGroupController>();
            }

            return _ins;
        }    
    }

    private static MenuGroupController _ins;

    private void HiddenButtons(List<ButtonType> types)
    {
        foreach (var btn in buttons)
        {
            btn.Show();
        }

        foreach (var type in types)
        {
            buttons[(int) type].Hide();
        }

        foreach (var type in specialButtons)
        {
            if (!GameUtils.IsWeb())
            {
                buttons[(int) type].Hide();
            }
        }
        
    }

    public static void ControlButtons(List<ButtonType> types)
    {
        Ins.HiddenButtons(types);
    }

    public static void ShowImmediate(string screenType)
    {
        var screen = Ins.screens.Find(s => s.screenId == screenType);
        ControlButtons(screen.hiddenTypes);
    }

    public static void HideMenu()
    {
        Ins.gameObject.Hide();
    }
    
    public static void ShowMenu()
    {
        Ins.gameObject.Show();
    }
    
}

[Serializable]
public class ScreenButtonType
{
    public string screenId;
    public List<ButtonType> hiddenTypes;
}

public enum ButtonType
{
    GamePlayMenu,
    GamePlayCapture,
    GamePlayExpand,
    LobbyCapture
}
