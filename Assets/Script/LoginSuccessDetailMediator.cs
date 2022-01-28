
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
    }
