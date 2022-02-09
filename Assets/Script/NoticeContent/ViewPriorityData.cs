using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ViewPriorityData", menuName = "Data/ViewPriorityData", order = 0)]
public class ViewPriorityData : ScriptableObject
{
    public List<PriorityLayer> specialLayer;
    public int defaultValue = 5;
}
