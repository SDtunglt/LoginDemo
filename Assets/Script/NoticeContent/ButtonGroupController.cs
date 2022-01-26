using System.Collections.Generic;
using UnityEngine;

public class ButtonGroupController : MonoBehaviour
{
    public GameObject[] buttons;
    public void HideButton(List<ButtonGroupType> types)
    {
        // SDLogger.Log("ShowButton: " + types.Count);
        for (int i = 1; i < buttons.Length; i++)
        {
            if (types.Contains((ButtonGroupType) i))
            {
                
                buttons[i].Hide();
            }
            else
            {
                buttons[i].Show();
            }
        }

        if (GameUtils.IsWeb())
        {
            buttons[0].SetActive(!types.Contains(ButtonGroupType.Expand));
        }
        else
        {
            buttons[0].SetActive(false);
        }
    }

}

public enum ButtonGroupType
{
    Expand,
    GopY,
    Capture,
    Tutorial,
    Setting
}