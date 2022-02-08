using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SelectBox : MonoBehaviour
{
    [SerializeField]
    SelectOption toggleTemplate;
    [SerializeField]
    Transform container;
    [SerializeField]
    Transform scrollContainer;
    [SerializeField]
    float transitionTime = 0.2f;
    [SerializeField]
    TMP_Text label;
    [SerializeField]
    Image dropIcon;
    [SerializeField]
    Sprite expandingIcon;
    [SerializeField]
    Sprite collapsingIcon;
    [SerializeField]
    Transform EmptyRect;

    public UnityEvent<object> OnSelect = new ObjectEvent();
    [SerializeField]
    List<SelectOptionData> dataSet;

    ToggleGroup selfGroup;
    Button selfBtn;
    bool expading = false;

    Tweener tw;
    bool inited = false;

    private void Start()
    {
        selfGroup = gameObject.AddComponent<ToggleGroup>();
        selfBtn = gameObject.AddComponent<Button>();
        selfBtn.onClick.AddListener(ToggleExpand);
        Revalidate(true);
        scrollContainer.localScale = Vector3.right;
    }
    private void OnDisable()
    {
        Collapse();
    }

    public void Revalidate(bool autoActive)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
        foreach (SelectOptionData data in dataSet)
        {
            SelectOption option = Instantiate(toggleTemplate, container);
            option.gameObject.SetActive(true);
            Toggle tg = option.Init(data);
            tg.group = selfGroup;
            data.Toggle = tg;
            if (autoActive && label.text == data.label)
                tg.SetIsOnWithoutNotify(true);
            tg.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    OnSelect.Invoke(data.value);
                    label.text = data.label;
                }
            });
        }
        if (dataSet.Count == 0 && EmptyRect != null)
        {
            Transform empty = Instantiate(EmptyRect, container);
            empty.gameObject.SetActive(true);
        }
        inited = true;
    }

    public void Active(object value)
    {
        SelectOptionData activing = dataSet.Find(x => x.value == value);
        if (activing != null)
        {
            label.text = activing.label;
            if (inited)
                activing.Toggle.SetIsOnWithoutNotify(true);
        }
        else
        {
            Debug.LogWarning($"Cann't found option with value '{value}'");
        }

    }
    public void Active(int index)
    {
        if(index > dataSet.Count - 1)
        {
            return;
        }
        SelectOptionData activing = dataSet[index];
        if (activing != null)
        {
            label.text = activing.label;
            if (inited)
                activing.Toggle.SetIsOnWithoutNotify(true);
        }
        else
        {
            Debug.LogWarning($"Cann't found option with index '{index}'");
        }

    }
    public void ActiveNotify(int index)
    {
        if (index > dataSet.Count - 1)
        {
            return;
        }
        SelectOptionData activing = dataSet[index];
        if (activing != null)
        {
            label.text = activing.label;
            if (inited)
                activing.Toggle.isOn = true;
        }
        else
        {
            Debug.LogWarning($"Cann't found option with index '{index}'");
        }

    }

    public void SetData(List<SelectOptionData> data, bool autoActive = true)
    {
        dataSet = data;
        Revalidate(autoActive);
    }

    public void Expand()
    {
        tw?.Kill();
        tw = scrollContainer.DOScaleY(1, transitionTime);
        dropIcon.sprite = expandingIcon;
        expading = true;
    }
    public void Collapse()
    {
        tw?.Kill();
        tw = scrollContainer.DOScaleY(0, transitionTime);
        dropIcon.sprite = collapsingIcon;
        expading = false;
    }

    public void ToggleExpand()
    {
        if (expading)
        {
            Collapse();
        }
        else
        {
            Expand();
        }
    }
    public class ObjectEvent : UnityEvent<object>
    {
    }
}

[Serializable]
public class SelectOptionData
{
    public string label;
    public object value;
    public Toggle Toggle { get; set; }
}
