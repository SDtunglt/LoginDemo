using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolers : MonoBehaviour
{
    private static Poolers _ins;
    private static bool IsDestroyYet;
    public static Poolers ins
    {
        get
        {
            if (_ins != null)
            {
                return _ins;
            }
            else
            {
                if (!IsDestroyYet)
                {
                    var go = new GameObject("Poolers");
                    _ins = go.AddComponent<Poolers>();
                    _ins.pool = new Dictionary<GameObject, List<PoolItem>>();
                    return _ins;
                }
                else
                {
                    return null;
                }
            }
        }
    }
    private Dictionary<GameObject, List<PoolItem>> pool;

    private void OnDestroy()
    {
        IsDestroyYet = true;
    }

    public PoolItem GetObject(GameObject obj)
    {
        if (pool.ContainsKey(obj))
        {
            foreach (var item in pool[obj])
            {
                if (!item.m_Obj.activeInHierarchy)
                {
                    item.m_Obj.SetActive(true);
                    return item;
                }
            }

            var i = Instantiate(obj);
            var o = i.GetComponent<PoolItem>();
            pool[obj].Add(o);
            return o;

        }
        else
        {
            pool.Add(obj, new List<PoolItem>());
            var item = Instantiate(obj);
            var o = item.GetComponent<PoolItem>();
            pool[obj].Add(o);
            return o;
        }
    }

    public PoolItem GetObject(GameObject obj, Transform parent, Vector3 localScale)
    {
        if (pool.ContainsKey(obj))
        {
            foreach (var item in pool[obj])
            {
                if (!item.m_Obj.activeInHierarchy)
                {
                    item.m_transform.SetParent(parent);
                    item.m_transform.SetAsLastSibling();
                    item.m_transform.localScale = localScale;
                    item.m_Obj.SetActive(true);
                    return item;
                }
            }
            var i = Instantiate(obj, parent);
            var o = i.GetComponent<PoolItem>();
            pool[obj].Add(o);
            o.m_transform.SetAsLastSibling();
            o.m_transform.localScale = localScale;
            return o;

        }
        else
        {
            pool.Add(obj, new List<PoolItem>());
            var item = Instantiate(obj, parent);
            var o = item.GetComponent<PoolItem>();
            pool[obj].Add(o);
            o.m_transform.SetAsLastSibling();
            o.m_transform.localScale = localScale;
            return o;
        }

    }

    public PoolItem GetObject(GameObject obj, Vector3 pos, Quaternion rot)
    {
        if (pool.ContainsKey(obj))
        {
            foreach (var item in pool[obj])
            {
                if (!item.m_Obj.activeInHierarchy)
                {
                    item.m_transform.position = pos;
                    item.m_transform.rotation = rot;
                    item.m_Obj.SetActive(true);
                    return item;
                }
            }

            var i = Instantiate(obj, pos, rot);
            var o = i.GetComponent<PoolItem>();
            pool[obj].Add(o);
            return o;

        }
        else
        {
            pool.Add(obj, new List<PoolItem>());
            var item = Instantiate(obj, pos, rot);
            var o = item.GetComponent<PoolItem>();
            pool[obj].Add(o);
            return o;
        }
    }

    public PoolItem GetObject(GameObject obj, Vector3 pos, Quaternion rot, Transform parent, Vector3 localScale)
    {
        if (pool.ContainsKey(obj))
        {
            foreach (var item in pool[obj])
            {
                if (!item.m_Obj.activeInHierarchy)
                {
                    item.m_transform.position = pos;
                    item.m_transform.rotation = rot;
                    item.m_Obj.SetActive(true);
                    item.m_transform.SetParent(parent);
                    item.m_transform.SetAsLastSibling();
                    item.m_transform.localScale = localScale;
                    return item;
                }
            }

            var i = Instantiate(obj, pos, rot);
            var o = i.GetComponent<PoolItem>();
            pool[obj].Add(o);
            o.m_transform.SetParent(parent);
            o.m_transform.SetAsLastSibling();
            o.m_transform.localScale = localScale;
            return o;

        }
        else
        {
            pool.Add(obj, new List<PoolItem>());
            var item = Instantiate(obj, pos, rot);
            var o = item.GetComponent<PoolItem>();
            pool[obj].Add(o);
            o.m_transform.SetParent(parent);
            o.m_transform.SetAsLastSibling();
            o.m_transform.localScale = localScale;
            return o;
        }
    }

    public void ClearItem(GameObject obj)
    {
        if (pool.ContainsKey(obj))
        {
            foreach (var item in pool[obj])
            {
                item.m_Obj.SetActive(false);
            }
        }
    }

    public List<PoolItem> GetAllObject(GameObject obj)
    {
        if (pool.ContainsKey(obj))
        {
            return pool[obj];
        }

        return null;
    }

    public void ReInit()
    {
        if (pool != null)
        {
            foreach (var poolKey in pool.Keys)
            {
                foreach (var item in pool[poolKey])
                {
                    if (item != null)
                    {
                        item.m_Obj.SetActive(false);
                    }
                }
            }
        }
        pool = new Dictionary<GameObject, List<PoolItem>>();
    }

    public void ClearPools()
    {
        foreach (var poolKey in pool.Keys)
        {
            foreach (var item in pool[poolKey])
            {
                item.m_Obj.SetActive(false);
            }
        }
    }

}

public static class Extension
{
    public static void Hide(this GameObject obj)
    {
        obj.SetActive(false);
    }

    public static void Hide(this Component component)
    {
        component.gameObject.SetActive(false);
    }

    public static void Show(this GameObject obj)
    {
        obj.SetActive(true);
    }

    public static void Show(this Component o)
    {
        o.gameObject.SetActive(true);
    }

    public static T Cast<T>(this MonoBehaviour mono) where T : class
    {
        var t = mono as T;
        return t;
    }

    public static void ClearAllCard(this MonoBehaviour obj)
    {
    }


    public static void ShowButtonGroup(this MonoBehaviour obj, List<ButtonGroupType> types)
    {
        MenuController.Ins.HideButton(types);
    }


}


