using UnityEngine;

public class LoginScreen : MonoBehaviour
{
    public static LoginScreen Instance
    {
        get{
            if(!instance)
            {
                instance = FindObjectOfType<LoginScreen>();
            }
            return instance;
        }
    }
    private static LoginScreen instance;
    public LoginMediator login;
    public UserDetailMediator userDetail;
    public LoginSuccessDetailMediator success;
    public NoticePopUp noticePopUp;



}
