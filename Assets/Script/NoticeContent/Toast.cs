using System;
using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using UnityEngine;

public class Toast : MonoBehaviour
{
    // public enum Position
    // {
    //     Center=0,
    //     Right=1,
    //     Top=2,
    // }
    private static Toast _instance;
    public static Toast Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = Instantiate(Resources.LoadAll("", typeof(Toast)).FirstOrDefault() as Toast);
            DontDestroyOnLoad(_instance);
            return _instance;
        }
    }
    // public List<LeanGameObjectPool> listToastContainer;
    public LeanGameObjectPool centerToastOne, centerToastFloat;
    
    public static void ShowFloat(string text, float time=2f, Action onToastClick=null)
    {
        var toastItem = Instance.centerToastFloat
            .Spawn(Vector3.zero, Quaternion.identity, Instance.centerToastFloat.transform)
            .GetComponent<ToastItem>();
        toastItem.Set(text, time, onToastClick);
        lastMess = text;
    }
    
    public static void Show(string text, float time=2f)
    {
        var toastItem = Instance.centerToastOne
            .Spawn(Vector3.zero, Quaternion.identity, Instance.centerToastOne.transform)
            .GetComponent<ToastItem>();
        
        toastItem.Set(text, time);
        lastMess = text;
    }

    public static void HideAllToast()
    {
        Instance.centerToastFloat.DespawnAll();
        Instance.centerToastOne.DespawnAll();
    }

    public static string lastMess = "";
}
