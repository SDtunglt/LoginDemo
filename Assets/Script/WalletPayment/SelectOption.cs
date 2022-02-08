using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectOption : MonoBehaviour
{
    public Toggle toggle;
    public TMP_Text labelField;
    // Use this for initialization
    public Toggle Init(SelectOptionData data)
    {
        labelField.text = data.label;
        return toggle;
    }
}
