using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PoolItem : MonoBehaviour
{
    public Transform m_transform;
    public GameObject m_Obj;
    private Tween disableTween;

    void Reset()
    {
        m_Obj = gameObject;
        m_transform = transform;
    }

    private void OnValidate()
    {
        m_transform = transform;
        m_Obj = gameObject;
    }

    public void Disable()
    {
        disableTween?.Kill();
        disableTween = m_transform.DOScale(Vector3.zero, 0.4f).OnComplete(() =>
        {
            m_Obj.Hide();
        });
    }

    public void ClearTimeDisable()
    {
        disableTween?.Kill();
    }

}
