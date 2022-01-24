
using UnityEngine;
    
    public class ScreenManager : MonoBehaviour {
        
    
        public static ScreenManager Ins{
            get{
                if(!_ins){
                    _ins = FindObjectOfType<ScreenManager>();
                }
                return _ins;
            }
        }
        private static ScreenManager _ins;
        public LoginMediator login;
        public UserDetailMediator userDetail;
        public LoginSuccessDetailMediator success;
        public NoticePopUp noticePopUp;

        public static int x = 10;
    }
