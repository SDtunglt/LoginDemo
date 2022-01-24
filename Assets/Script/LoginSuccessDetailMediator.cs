
using UnityEngine;
using TMPro;
    
    public class LoginSuccessDetailMediator : MonoBehaviour {
            [SerializeField] public TextMeshProUGUI hostText,portText,uidText;
            void OnEnable()
            {
                uidText.text = $"Uid: {UserModel.Instance.uid}";
                hostText.text = $"Host: {GameConfig.HOST}";
                portText.text = $"Port: {GameConfig.PORT}";
            }

            public void ShowUserDetail(){
                gameObject.SetActive(false);
                Debug.Log("uid: " + UserModel.Instance.uid);
                ScreenManager.Ins.userDetail.GetUserInfo(UserModel.Instance.uid, UserModel.Instance.ip);
                Debug.Log("userInfo");
            }
    }
