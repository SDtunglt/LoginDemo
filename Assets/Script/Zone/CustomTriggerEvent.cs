using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomTriggerEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Action<PointerEventData> onPointDown, onPointerUp, onDrag;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        onPointDown?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData);
    }
}