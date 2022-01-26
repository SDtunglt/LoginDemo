using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using UnityEngine;

public class ViewCreator
{
    //Đường dẫn này sẽ được sửa lại sau khi chuyển folder
    private static string BasePath = "Assets/Chan/Bundles/Panels/";
    private static string SoundPath = "Assets/Chan/Bundles/Sounds/";
    private static string PrefabExtension = ".prefab";

    private static Dictionary<string, UIBaseView> Prefabs = new Dictionary<string, UIBaseView>();
    private static List<UIBaseView> Screens = new List<UIBaseView>();
    private static List<UIBaseView> Popups = new List<UIBaseView>();
    private static Transform _PopupParent;

    private static Transform PopupParent
    {
        get
        {
            if (!_PopupParent)
            {
                _PopupParent = ObjectFinder.GetObject(TagId.PopupHolder);
            }

            return _PopupParent;
        }
    }

    private static Transform _ScreenParent;

    private static Transform ScreenParent
    {
        get
        {
            if (!_ScreenParent)
            {
                _ScreenParent = ObjectFinder.GetObject(TagId.ScreenHolder);
            }

            return _ScreenParent;
        }
    }

    private static Coroutine popupCt;
    private static Coroutine screenCt;


    public static void OpenPopup(string popupName, Action<UIBaseView> onLoadComplete = null,
        bool isWithoutParent = false, Transform parent = null)
    {
        if (popupCt != null)
        {
            Executors.StopCoroutine(popupCt);
        }

        if(SwitchUI.CurrentType == ComponentType.Chan2)
        {
            popupName += "_2";
        }

        popupCt = Executors.RunOnCoroutineReturn(IOpenPopup(popupName, onLoadComplete, isWithoutParent, parent));
    }

    public static void OpenScreen(string screenName, Action<UIBaseView> onLoadComplete = null,
        bool isWithoutParent = false, Transform parent = null)
    {
        if (screenCt != null)
        {
            Executors.StopCoroutine(screenCt);
        }

        screenCt = Executors.RunOnCoroutineReturn(IOpenScreen(screenName, onLoadComplete, isWithoutParent, parent));
    }

    public static IEnumerator IOpenPopup(string popupName, Action<UIBaseView> onLoadComplete = null,
        bool isWithoutParent = false, Transform parent = null)
    {
        var priority = ViewPriority.Ins.GetPriority(popupName);
        if (!Prefabs.ContainsKey(popupName))
        {
            foreach (var p in Popups)
            {
                if (p.GetGameObject().activeInHierarchy)
                {
                    if (p.Priority <= priority)
                    {
                        p.Close();
                    }
                }
            }

            yield return LoadView(popupName, (o) =>
            {
                var v = o.GetComponent<UIPopup>();
                Prefabs.Add(popupName, v);
                var m = LeanPool.Spawn(Prefabs[popupName], isWithoutParent ? null : !parent ? PopupParent : parent);
                m.ViewId = popupName;
                m.Priority = priority;
                Popups.Add(m);
                m.OpenView();
                onLoadComplete?.Invoke(m);
            });
        }
        else
        {
            UIBaseView cache = null;
            var isLoadYet = false;

            foreach (var view in Popups)
            {
                if (view.ViewId == popupName)
                {
                    isLoadYet = true;
                }

                if (view.GetGameObject().activeSelf)
                {
                    if (view.ViewId == popupName)
                    {
                        cache = view;
                    }
                    else
                    {
                        if (view.Priority <= priority)
                        {
                            view.Close();
                        }
                    }
                }
            }

            if (cache)
            {
                onLoadComplete?.Invoke(cache);
                yield break;
            }

            var m = LeanPool.Spawn(Prefabs[popupName], isWithoutParent ? null : (!parent ? PopupParent : parent));
            m.ViewId = popupName;
            m.OpenView();
            if (!isLoadYet)
            {
                Popups.Add(m);
            }

            onLoadComplete?.Invoke(m);
        }
    }

    public static IEnumerator IOpenScreen(string screenName, Action<UIBaseView> onLoadComplete = null,
        bool isWithoutParent = false, Transform parent = null)
    {
        if (!Prefabs.ContainsKey(screenName))
        {
            yield return LoadView(screenName, view => { Prefabs.Add(screenName, view); });
        }

        ShowFlashWithCallBack(() =>
        {
            UIBaseView cache = null;
            var isLoadYet = false;
            foreach (var view in Screens)
            {
                if (view.ViewId == screenName)
                {
                    isLoadYet = true;
                }

                if (view.GetGameObject().activeSelf)
                {
                    if (view.ViewId == screenName)
                    {
                        cache = view;
                    }
                    else
                    {
                        view.Close();
                    }
                }
            }

            if (cache)
            {
                onLoadComplete?.Invoke(cache);
                return;
            }

            var m = LeanPool.Spawn(Prefabs[screenName], isWithoutParent ? null : (!parent ? ScreenParent : parent));
            m.ViewId = screenName;
            m.OpenView();
            if (!isLoadYet)
            {
                Screens.Add(m);
            }

            onLoadComplete?.Invoke(m);
        });
    }

    public static void ClosePopup(string popupName)
    {
        var p = Popups.Find(s => s.ViewId == popupName);
        if (p)
        {
            if (p.GetGameObject().activeInHierarchy)
            {
                LeanPool.Despawn(p);
            }
        }
    }

    public static void CloseAllPopup()
    {
        foreach (var popup in Popups)
        {
            if (popup.GetGameObject().activeInHierarchy)
            {
                LeanPool.Despawn(popup);
            }
        }
    }

    public static UIBaseView GetPopup(string popupName)
    {
        return Popups.Find(s => s.ViewId == popupName);
    }

    public static bool IsOpenPopup(string popupName)
    {
        var popup = GetPopup(popupName);
        if (popup != null)
        {
            return popup.gameObject.activeInHierarchy;
        }
        else
        {
            return false;
        }
    }

    private static IEnumerator LoadView(string name, Action<UIBaseView> onComplete)
    {
        GameObject go = null;

        var request = AssetsLoader.LoadAsset($"{BasePath}{name}{PrefabExtension}", typeof(GameObject));
        yield return request;
        if (!string.IsNullOrEmpty(request.error))
        {
            request.Release();
            yield break;
        }

        go = request.asset as GameObject;
        onComplete.Invoke(go.GetComponent<UIBaseView>());
    }

    public static void LoadObject(string path, Action<GameObject> onComplete)
    {
        Executors.RunOnCoroutineNoReturn(ILoadObject(path, onComplete));
    }

    private static IEnumerator ILoadObject(string path, Action<GameObject> onComplete)
    {
        GameObject go = null;

        var request = AssetsLoader.LoadAsset(path, typeof(GameObject));
        yield return request;
        if (!string.IsNullOrEmpty(request.error))
        {
            request.Release();
            yield break;
        }

        go = request.asset as GameObject;
        onComplete.Invoke(go);
    }

    public static void LoadSound(string soundName, Action<AudioClip> onComplete)
    {
        Executors.RunOnCoroutineNoReturn(ILoadSound(soundName, onComplete));
    }

    private static IEnumerator ILoadSound(string soundName, Action<AudioClip> onComplete)
    {
        AudioClip go = null;

        var request = AssetsLoader.LoadAsset($"{SoundPath}{soundName}.mp3", typeof(AudioClip));
        yield return request;
        if (!string.IsNullOrEmpty(request.error))
        {
            request.Release();
            yield break;
        }

        go = request.asset as AudioClip;
        onComplete.Invoke(go);
    }

    public static void LoadListSound(List<string> soundNames, Action<List<AudioClip>> onComplete)
    {
        Executors.RunOnCoroutineNoReturn(ILoadListSound(soundNames, onComplete));
    }

    private static IEnumerator ILoadListSound(List<string> soundNames, Action<List<AudioClip>> onComplete)
    {
        var requests = new List<AssetRequest>();
        foreach (var soundName in soundNames)
        {
            if (string.IsNullOrEmpty(soundName))
            {
                continue;
            }

            var request = AssetsLoader.LoadAsset($"{SoundPath}{soundName}.mp3", typeof(AudioClip));
            requests.Add(request);
        }

        yield return new WaitUntil(() => requests.TrueForAll(o => o.isDone));
        var list = requests.Select(request => request.asset as AudioClip).ToList();
        onComplete.Invoke(list);
    }

    public static void ShowFlashWithCallBack(Action a)
    {
        FlashPanel.Open().ShowFlashWithCallBack(a);
    }
}