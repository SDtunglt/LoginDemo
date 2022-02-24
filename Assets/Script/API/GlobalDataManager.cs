using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif
using UnityEngine;
using UnityEngine.Networking;

public class GlobalDataManager : MonoBehaviour
{
    public static GlobalDataManager Ins
    {
        get
        {
            if (!_ins)
            {
                _ins = ObjectFinder.GetObject(TagId.GlobalDataManager).GetComponent<GlobalDataManager>();
            }

            return _ins;
        }
    }

    private static GlobalDataManager _ins;

    [Header("Tắt bật API Dev mode hay Release Mode")]
    public bool isDev;
    [Header("Dev Chắn RET (không hoạt động nếu isDev = false)")]
    public bool isChanRET;
    [Header("Dev API")]
    public string dev_BASE_URL = "https://dev.sandinh.com/";
    [Header("Prod API")]
    public string prod_BASE_URL = "https://api.sandinhstudio.com/";
    // [Header("Release API")] public string release_BASE_URL() =  "https://chanphom.com/";
    public bool isWriteGranted = false;
    public bool isTurnOnLog = false;

    public string BASE_URL => isDev ? dev_BASE_URL: ReadURL();

    public string API_URL => BASE_URL + (isDev ? (isChanRET ? dev_api_url_ret : dev_api_url) : release_api_url);

    public string API_TOP => BASE_URL + (isDev ? (isChanRET ? dev_api_top_ret : dev_api_top) : release_api_top);
    
    [Header("Các Đuôi API")]
    [SerializeField]
    private string dev_api_url = "api/user/";
    [SerializeField]
    private string dev_api_top = "api/top/";
    
    [SerializeField]
    private string release_api_url = "api-5g/user/";
    [SerializeField]
    private string release_api_top = "api-5g/top/";
    
    [SerializeField]
    private string dev_api_url_ret = "api-c3/user/";
    [SerializeField]
    private string dev_api_top_ret = "api-c3/top/";
    [SerializeField]
    private int port_chan_ret = 9413;
    
    [Header("Avatar API")]
    public string dev_avt_api = "https://dev.chanphom.com/";
    public string product_avt_api = "https://chanphom.com/";
    public string AVT_API => isDev ? dev_avt_api : product_avt_api;

#if UNITY_WEBGL
    [DllImport("__Internal")] 
    private static extern string GetURLFromPage();

    [DllImport("__Internal")]
    private static extern void ShareGameFbWithApi(string feedLink);
#endif

    public int PortChanRet => port_chan_ret;
    private void Start()
    {
        if (GameUtils.IsEditor())
        {
            Debug.unityLogger.logEnabled = true;
        }
        else
        {
            Debug.unityLogger.logEnabled = isTurnOnLog;
        }
    }

    private string m_base_url = "";
    public string ReadURL()
    {
        if (!string.IsNullOrEmpty(m_base_url))
        {
            // Debug.Log("m_base_url: " + m_base_url);
            return m_base_url;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        if (!GameConfig.IsAppFacebook)
        {
            m_base_url = GetURLFromPage();
        }else{
            m_base_url = "https://chanphom.com/";
            return m_base_url;
        }
        Debug.Log("m_base_url: " + m_base_url);
#else
        // m_base_url = "https://dev.sandinh.com/game/chan5g";
        // m_base_url = "https://api.sandinhstudio.com/";
        m_base_url = prod_BASE_URL;
#endif
        m_base_url = GetDomainURL(m_base_url);
        if (m_base_url.Contains("localhost"))
        {
            m_base_url = dev_BASE_URL;
        }
        return m_base_url;
    }

    public string GetDomainURL(string url)
    {
        Uri myUri = new Uri(url);
        return myUri.GetLeftPart(UriPartial.Authority) + "/";
    }

    public bool isFullScreenMode = false;

    public Sprite[] backgrounds;
    public Sprite[] nenAvatars;
    public Sprite[] backCard;
    public Color[] backCardTxtColors;
    public KhungAvatarData khungAvatarData;

    
    #region Share

    public static Action<bool, string> OnShareComplete;

    
    public void HandleShareWeb(string response)
    {
        if (response == "KO" || response.Contains("canceled"))
        {
            OnShareComplete?.Invoke(false, null);
            // MinioUploadHelper.DeleteLastObject(BucketConfig.SHARE_FB);
            return;
        }
        Debug.Log("Share Complete");
        OnShareComplete?.Invoke(true, null);
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    #endregion
  }