using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserContainerVO
{
    public string uid;//uid in the container
    public string name;
    public string gender;//nam | nữ
    public UserContainerVO(string uid, string name, string gender){
        this.uid = uid;
        this.name = name;
        this.gender = gender;
    }

    public string GetGender() {
        switch (gender) {
            case "M": return "Nam";
            case "F": return "Nữ";
            default : return "Không";
        }
    }
}
