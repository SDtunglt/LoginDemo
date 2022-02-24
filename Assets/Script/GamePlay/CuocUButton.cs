using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CuocUButton : MonoBehaviour
{
    [SerializeField] private GameObject img_checkmark;
    [SerializeField] private TextMeshProUGUI txt_White;
    [SerializeField] private TextMeshProUGUI txt_Black;
    [SerializeField] private Button btn;
    private Action<int> onButtonClick;
    private int num;
    [HideInInspector] public int Num
    {
        get => num;
        set => num = value;
    }

    public int indexOfCuoc;

    private void OnValidate()
    {
        txt_White = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        txt_Black = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        btn = GetComponent<Button>();
        
    }

    private void Start()
    {
        btn.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        onButtonClick?.Invoke(indexOfCuoc);   
    }

    private bool isSelect;
    public bool IsSellected()
    {
        return isSelect;
    }

    public void SetSellected(bool sellect)
    {
        img_checkmark.SetActive(sellect);
        txt_White.gameObject.SetActive(!sellect);
        txt_Black.gameObject.SetActive(sellect);
        isSelect = sellect;
        if (!sellect)
        {
            Num = 0;
        }
    }

    public void SetTextCuoc(string cuoc)
    {
        txt_White.text = cuoc;
        txt_Black.text = cuoc;
    }

    public void OnShow(int id, Action<int> onClick)
    {
        onButtonClick = onClick;
        indexOfCuoc = id;
        SetSellected(indexOfCuoc == 0);
        if (indexOfCuoc == 0)
        {
            Num = 1;
        }
    }
    
}
