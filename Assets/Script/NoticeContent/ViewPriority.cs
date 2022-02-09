using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewPriority : MonoBehaviour
{
    private static ViewPriority _ins;
    public ViewPriorityData data;

    public static ViewPriority Ins
    {
        get
        {
            if (!_ins)
            {
                _ins = ObjectFinder.GetObject(TagId.ViewPriority).GetComponent<ViewPriority>();
            }

            return _ins;
        }
    }
    // public List<PriorityLayer> specialLayer;
    // public int defaultValue = 0;

    public void SetPriorityForView(UIBaseView view)
    {
        foreach (var layer in data.specialLayer.Where(layer => layer.viewIds.Contains(view.ViewId)))
        {
            view.Priority = layer.priority;
            return;
        }

        view.Priority = data.defaultValue;

    }

    public int GetPriority(string viewId)
    {
        foreach (var layer in data.specialLayer.Where(layer => layer.viewIds.Contains(viewId) || layer.viewIds.Contains(viewId.Replace("_2", ""))))
        {
            // Debug.Log(viewId + "   "+ layer.priority);
            return layer.priority;
        }
        // Debug.Log(viewId + "   "+ defaultValue);

        return data.defaultValue;
    }
    
}

[System.Serializable]
public class PriorityLayer
{
    public List<string> viewIds;
    public int priority;
}