using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KhungAvatar", menuName = "Data/KhungAvatar", order = 0)]
public class KhungAvatarData : ScriptableObject
{
    public List<KhungAvatarInfo> infos;
    
}

[Serializable]
public struct KhungAvatarInfo
{
    public int id;
    public string khungName;
    public Sprite khungAvt;
    public Sprite khungNhanVat;
}
 