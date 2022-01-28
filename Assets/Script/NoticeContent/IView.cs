
using System;
using UnityEngine;
public interface IView
{
    string ViewId { get; set; }
    int Priority { get; set; }
    void OpenView();
    void Close();
    GameObject GetGameObject();
}