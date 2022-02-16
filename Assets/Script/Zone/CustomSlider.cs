using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    public Image fillIm;
    public RectTransform handle;
    public CustomTriggerEvent trigger;
    public TextMeshProUGUI minValueTxt, maxValueTxt;
    public TextMeshProUGUI currentValueTxt;

    public Image handleIm;
    public Sprite[] selectIcons;
    // public GameObject lockGo;
    public float minPos = 0, maxPos = 400;

    public bool isCoinSlider = true;
    private Vector2 handlePos = new Vector2();
    public float speedSlider1 = 0.95f;
    private long currentValue;
    public long[] steps;
    public float[] showSteps;
    private bool isLock;

    public delegate void OnSliderChange(float x);
    public delegate void OnChangeToStep(long x);
    
    public OnSliderChange onSliderChange;
    public OnChangeToStep onChangeToStep;

    private void Start()
    {
        trigger.onPointDown += OnHandlePointDown;
        trigger.onPointerUp += OnHandlePointUp;
        trigger.onDrag += OnHandleDrag;
    }

    public void SetUpSlider(long current, long min, long max, long[] steps)
    {
        this.steps = steps;
        minValueTxt.text = SetString(min);
        maxValueTxt.text = SetString(max);
        showSteps = new float[steps.Length];
        var x = (float)(max - min);
        current = (long)Mathf.Clamp(current, min, max);
        for (int i = 0; i < showSteps.Length; i++)
        {
            showSteps[i] = (steps[i] - min) / x;
        }
        ChangeValue((current - steps[0]) / x);
    }

    private string SetString(long v)
    {
        return isCoinSlider ? StringUtils.FormatMoney(v) : v + "s";
    }

    public void ChangeValue(float x)
    {
        fillIm.fillAmount = x;
        handlePos.x = x * (maxPos - minPos);
        handle.anchoredPosition = handlePos;
        onSliderChange?.Invoke(x);
        OnMoveToStep(x);
    }

    public void OnMoveToStep(float x)
    {
        var i = showSteps.Nearest(x);
        currentValue = steps[i];
        currentValueTxt.text = SetString(currentValue);
        onChangeToStep?.Invoke(currentValue);
    }
    

    private bool isSelect = false;
    private float curX;

    public void OnHandlePointDown(PointerEventData eventData)
    {
        if(isLock) return;
        isSelect = true;
        curX = eventData.position.x;
    }

    public void OnHandleDrag(PointerEventData eventData)
    {       
        if(isLock) return;
        if (!isSelect)
            return;
        var nextX = eventData.position.x;
        var pos = Mathf.Lerp(handlePos.x, Mathf.Clamp((nextX - curX) + handlePos.x, minPos, maxPos), speedSlider1);
        var x = pos / maxPos;
        ChangeValue(x);
        curX = nextX;
    }

    public void OnHandlePointUp(PointerEventData eventData)
    {
        if(isLock) return;

        isSelect = false;
        var nextX = eventData.position.x;
        var pos = Mathf.Lerp(handlePos.x, Mathf.Clamp((nextX - curX) + handlePos.x, minPos, maxPos), speedSlider1);
        var x = pos / maxPos;
        curX = nextX;

        var i = showSteps.Nearest(x);
        currentValue = steps[i];
        currentValueTxt.text = SetString(currentValue);
        onChangeToStep?.Invoke(currentValue);

        x = showSteps[i];
        fillIm.fillAmount = x;
        handlePos.x = x * (maxPos - minPos);
        handle.anchoredPosition = handlePos;
        onSliderChange?.Invoke(x);
    }

    public void Lock(bool isLock)
    {
        // Group.alpha = isLock ? 0.7f : 1;
        handleIm.sprite = isLock ? selectIcons[1] : selectIcons[0];
        var handleAnchoredPosition = handle.anchoredPosition;
        handleAnchoredPosition.y = isLock ? 5 : 0;
        handle.anchoredPosition = handleAnchoredPosition;
        handleIm.SetNativeSize();
        // lockGo.SetActive(isLock);
        this.isLock = isLock;
    }

    public bool IsLock()
    {
        return isLock;
    }
    
}