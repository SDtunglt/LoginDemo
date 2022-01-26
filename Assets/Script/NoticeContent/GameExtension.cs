using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class GameExtension
{
    public static void ShowFlashWithCallBack(this MonoBehaviour obj, Action callBack)
    {
        FlashPanel.Open().ShowFlashWithCallBack(callBack);
    }

    public static int GetUnixTimer()
    {
        return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
    
    public static string ToTitleCase(this string title)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower()); 
    }
    
    public static Coroutine SetTimeout(this MonoBehaviour obj, float dt, Action cb)
    {
        if (obj.gameObject.activeInHierarchy)
            return obj.StartCoroutine(ITimer(dt, cb));
        return null;
    }

    private static IEnumerator ITimer(float dt, Action cb)
    {
        yield return new WaitForSeconds(dt);
        cb?.Invoke();
    }

    public static Coroutine WaitNewFrame(this MonoBehaviour obj, Action cb)
    {
        if (obj.gameObject.activeInHierarchy)
            return obj.StartCoroutine(IWaitNewFrame(cb));
        return null;
    }
    
    public static Coroutine WaitTimeout(this MonoBehaviour obj, Action cb, float delay)
    {
        if (obj.gameObject.activeInHierarchy)
            return obj.StartCoroutine(IWaitTimeout(cb, delay));
        return null;
    }
    
    private static IEnumerator IWaitNewFrame(Action cb)
    {
        yield return null;
        cb.Invoke();
    }
    private static IEnumerator IWaitTimeout(Action cb,float delay)
    {
        yield return new WaitForSeconds(delay);
        cb.Invoke();
    }
    
    public static void ChangeHeight(this RectTransform rect, float s)
    {
        var size = rect.sizeDelta;
        size.y = s;
        rect.sizeDelta = size;
    }

    public static void ChangeWidth(this RectTransform rect, float s)
    {
        var size = rect.sizeDelta;
        size.x = s;
        rect.sizeDelta = size;
    }
    
    public static void ChangeSize(this RectTransform rect, Vector2 s)
    {
        rect.sizeDelta = s;
    }
    
    public static void ChangeAnchorY(this RectTransform rect, float y)
    {
        var l = rect.anchoredPosition;
        l.y = y;
        rect.anchoredPosition = l;
    }

    public static void ChangeAnchorX(this RectTransform rect, float x)
    {
        var l = rect.anchoredPosition;
        l.x = x;
        rect.anchoredPosition = l;
    }

    public static void ChangeAlpha(this Graphic s, float f)
    {
        var c = s.color;
        c.a = f;
        s.color = c;
    }
    
    public static List<T> Splice<T>(this List<T> list, int offset, int count)
    {
        var startIdx = offset < 0 ? list.Count + offset : offset;
        var result = list.Skip(startIdx).Take(count).ToList();
        list.RemoveRange(startIdx, count);
        return result;
    }
    
    public static T Pop<T>(this List<T> list)
    {
        var end = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        return end;
    }
    
    public static string Join<T>(this List<T> list, string insert)
    {
        return list.Aggregate("", (current, t) => current + (t + insert));
    }
    
    public static void SetCount<T>(this List<T> list, int count, T defaulValue)
    {
        for (var i = 0; i < count; i++)
        {
            list.Add(defaulValue);
        }
    }
    
    public static void SetAnchor(this RectTransform source, AnchorPresets allign, int offsetX=0, int offsetY=0)
     {
         source.anchoredPosition = new Vector3(offsetX, offsetY, 0);
 
         switch (allign)
         {
             case(AnchorPresets.TopLeft):
             {
                 source.anchorMin = new Vector2(0, 1);
                 source.anchorMax = new Vector2(0, 1);
                 break;
             }
             case (AnchorPresets.TopCenter):
             {
                 source.anchorMin = new Vector2(0.5f, 1);
                 source.anchorMax = new Vector2(0.5f, 1);
                 break;
             }
             case (AnchorPresets.TopRight):
             {
                 source.anchorMin = new Vector2(1, 1);
                 source.anchorMax = new Vector2(1, 1);
                 break;
             }
 
             case (AnchorPresets.MiddleLeft):
             {
                 source.anchorMin = new Vector2(0, 0.5f);
                 source.anchorMax = new Vector2(0, 0.5f);
                 break;
             }
             case (AnchorPresets.MiddleCenter):
             {
                 source.anchorMin = new Vector2(0.5f, 0.5f);
                 source.anchorMax = new Vector2(0.5f, 0.5f);
                 break;
             }
             case (AnchorPresets.MiddleRight):
             {
                 source.anchorMin = new Vector2(1, 0.5f);
                 source.anchorMax = new Vector2(1, 0.5f);
                 break;
             }
 
             case (AnchorPresets.BottomLeft):
             {
                 source.anchorMin = new Vector2(0, 0);
                 source.anchorMax = new Vector2(0, 0);
                 break;
             }
             case (AnchorPresets.BottonCenter):
             {
                 source.anchorMin = new Vector2(0.5f, 0);
                 source.anchorMax = new Vector2(0.5f,0);
                 break;
             }
             case (AnchorPresets.BottomRight):
             {
                 source.anchorMin = new Vector2(1, 0);
                 source.anchorMax = new Vector2(1, 0);
                 break;
             }
 
             case (AnchorPresets.HorStretchTop):
             {
                 source.anchorMin = new Vector2(0, 1);
                 source.anchorMax = new Vector2(1, 1);
                 break;
             }
             case (AnchorPresets.HorStretchMiddle):
             {
                 source.anchorMin = new Vector2(0, 0.5f);
                 source.anchorMax = new Vector2(1, 0.5f);
                 break;
             }
             case (AnchorPresets.HorStretchBottom):
             {
                 source.anchorMin = new Vector2(0, 0);
                 source.anchorMax = new Vector2(1, 0);
                 break;
             }
 
             case (AnchorPresets.VertStretchLeft):
             {
                 source.anchorMin = new Vector2(0, 0);
                 source.anchorMax = new Vector2(0, 1);
                 break;
             }
             case (AnchorPresets.VertStretchCenter):
             {
                 source.anchorMin = new Vector2(0.5f, 0);
                 source.anchorMax = new Vector2(0.5f, 1);
                 break;
             }
             case (AnchorPresets.VertStretchRight):
             {
                 source.anchorMin = new Vector2(1, 0);
                 source.anchorMax = new Vector2(1, 1);
                 break;
             }
 
             case (AnchorPresets.StretchAll):
             {
                 source.anchorMin = new Vector2(0, 0);
                 source.anchorMax = new Vector2(1, 1);
                 break;
             }
         }
     }

    public static int GetTimer(this MonoBehaviour mono)
    {
        return (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
    
    public static void SetAnchorPosition(this RectTransform source, RectTransform destination)
    {
        source.anchorMin = destination.anchorMin;
        source.anchorMax = destination.anchorMax;
        source.anchoredPosition = destination.anchoredPosition;
    }
 
    public static void SetPivot(this RectTransform source, PivotPresets preset)
     {
 
         switch (preset)
         {
             case (PivotPresets.TopLeft):
             {
                 source.pivot = new Vector2(0, 1);
                 break;
             }
             case (PivotPresets.TopCenter):
             {
                 source.pivot = new Vector2(0.5f, 1);
                 break;
             }
             case (PivotPresets.TopRight):
             {
                 source.pivot = new Vector2(1, 1);
                 break;
             }
 
             case (PivotPresets.MiddleLeft):
             {
                 source.pivot = new Vector2(0, 0.5f);
                 break;
             }
             case (PivotPresets.MiddleCenter):
             {
                 source.pivot = new Vector2(0.5f, 0.5f);
                 break;
             }
             case (PivotPresets.MiddleRight):
             {
                 source.pivot = new Vector2(1, 0.5f);
                 break;
             }
 
             case (PivotPresets.BottomLeft):
             {
                 source.pivot = new Vector2(0, 0);
                 break;
             }
             case (PivotPresets.BottomCenter):
             {
                 source.pivot = new Vector2(0.5f, 0);
                 break;
             }
             case (PivotPresets.BottomRight):
             {
                 source.pivot = new Vector2(1, 0);
                 break;
             }
         }
     }
    
    public enum AnchorPresets
    {
        TopLeft,
        TopCenter,
        TopRight,
 
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
 
        BottomLeft,
        BottonCenter,
        BottomRight,
        BottomStretch,
 
        VertStretchLeft,
        VertStretchRight,
        VertStretchCenter,
 
        HorStretchTop,
        HorStretchMiddle,
        HorStretchBottom,
 
        StretchAll
    }
 
    public enum PivotPresets
    {
        TopLeft,
        TopCenter,
        TopRight,
 
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
 
        BottomLeft,
        BottomCenter,
        BottomRight,
    }
    
    public static int ToBase10(string number, int start_base)
    {
        number = number.ToLower();

        if (start_base < 2 || start_base > 36) return 0; 
        if (start_base == 10) return Convert.ToInt32(number); 

        char[] chrs = number.ToCharArray(); 
        int m = chrs.Length - 1;
        int x; 
        int rtn = 0; 

        foreach(char c in chrs) { 

            if (char.IsNumber(c)) 
                x = int.Parse(c.ToString()); 
            else 
                x = Convert.ToInt32(c) - 87; 

            rtn += x * (Convert.ToInt32(Math.Pow(start_base, m))); 

            m--; 

        } 

        return rtn; 

    } 
    
    public static long ToBaseInt64(string number, int start_base)
    {
        number = number.ToLower();

        if (start_base < 2 || start_base > 36) return 0; 
        if (start_base == 10) return Convert.ToInt64(number); 

        char[] chrs = number.ToCharArray(); 
        int m = chrs.Length - 1;
        long x; 
        long rtn = 0; 

        foreach(char c in chrs) { 

            if (char.IsNumber(c)) 
                x = int.Parse(c.ToString()); 
            else 
                x = Convert.ToInt64(c) - 87; 

            rtn += x * (Convert.ToInt64(Math.Pow(start_base, m))); 

            m--; 

        } 

        return rtn; 

    } 
    
    public static int Nearest(this float[] arr, float value)
    {
        var index = 0;
        var v = Mathf.Abs(arr[0] - value);
        for (int i = 1; i < arr.Length; i++)
        {
            if (Mathf.Abs(arr[i] - value) < v)
            {
                index = i;
                v = Mathf.Abs(arr[i] - value);
            }
        }

        return index;
    }
    public static bool IsOK(this JObject obj)
    {
        return obj.GetValue("status").ToString().ToLower().Equals("ok");
    }
    public static string GetReason(this JObject obj)
    {
        return obj.GetValue("reason").ToString();
    }
}