using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class HttpUtils : MonoBehaviour
{
     private static HttpUtils _Instance;

    private static HttpUtils Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("HttpUtils").AddComponent(typeof(HttpUtils)) as HttpUtils;
                // DontDestroyOnLoad(_Instance);
            }

            return _Instance;
        }
    }

    public static void SendPost(string url,
        Action<string> onSuccess = null, Action<string> onFailure = null, string jsonData = "")
    {
        SendRequest("POST", url, onSuccess, onFailure, jsonData);
    }

    public static void SendGet(string url,
        Action<string> onSuccess = null, Action<string> onFailure = null, string jsonData = "")
    {
        SendRequest("GET", url, onSuccess, onFailure, jsonData);
    }

    public static void SendRequest(string method, string url,
        Action<string> onSuccess, Action<string> onFailure = null, string jsonData = "")
    {
        Instance.StartCoroutine(Request(method, url, onSuccess, onFailure, jsonData));
    }

    private static IEnumerator Request(string method, string url,
        Action<string> onSuccess, Action<string> onFailure, string jsonData)
    {
        // SDLogger.Log(method + ": " + url + "\n" + jsonData);
        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            var bodyRaw = string.IsNullOrEmpty(jsonData) ? null : Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("content-type", "application/json");
            var session = GameModel.Instance.loginSession;
            request.SetRequestHeader("SdAuthorization", session);
            request.SetRequestHeader("SdType", "2");
            // if (!GameUtils.IsWeb())
            // {
            //     request.SetRequestHeader("cookie", "xf_session=" + GameData.loginSession);
            // }
            Debug.Log(url + "   " + method + "   " + jsonData);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                //SDLogger.Log("Error: " + webRequest.error);
                onFailure?.Invoke(request.error);
            }
            else
            {
                //SDLogger.Log("Received: " + webRequest.downloadHandler.text);
                var data = request.downloadHandler.text;
                onSuccess(data);
            }
        }
    }

    public static void UploadFileToServer(string url, string filename, string paramName, string contentType,
        byte[] dataBytes,
        Action<JObject> onSuccess, Action onFailure)
    {
        var session = GameModel.Instance.loginSession;
        var form = new WWWForm();
        form.AddBinaryData(paramName, dataBytes, filename, contentType);
        var request = new UnityWebRequest(url) {method = "POST"};
        var uploader = new UploadHandlerRaw(form.data);
        var downloader = new DownloadHandlerBuffer();
        uploader.contentType = form.headers["Content-Type"];
        request.SetRequestHeader("content-type", form.headers["Content-Type"]);
        if (GameUtils.IsWeb())
        {
            request.SetRequestHeader("SdAuthorization", session);
            request.SetRequestHeader("SdType", "2");
        }

        // request.SetRequestHeader("Cookie", "xf_session=" + session);
        request.uploadHandler = uploader;
        request.downloadHandler = downloader;
        Instance.StartCoroutine(IRunRequest(request, onSuccess, onFailure));
    }

// todo Add IFix.interpret
    public static IEnumerator IRunRequest(UnityWebRequest request, Action<JObject> onComplete, Action onFailure)
    {
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError(request.error + "   " + request.downloadHandler.text);
            onFailure.Invoke();
        }
        else
        {
            var response = request.downloadHandler.text;
            Debug.Log(response);
            onComplete.Invoke(JObject.Parse(response));
        }
    }

    public static void UploadPostAttachment(Texture2D texture, List<string> formNames, List<string> formDatas,
        string fileName, string url, Action<JObject> onSuccess,
        Action onFailure)
    {
        byte[] dataBytes = texture.EncodeToJPG();
        var form = new WWWForm();
        form.AddBinaryData("image_file", dataBytes, $"{fileName}.jpg", "image/jpeg");

        var formHeaders = MergeFormDataHeader(form, formNames, formDatas);
        var request = new UnityWebRequest(url, "POST");
        var downloader = new DownloadHandlerBuffer();
        var uploader = new UploadHandlerRaw(formHeaders.data);
        uploader.contentType = formHeaders.headers["Content-Type"];
        request.SetRequestHeader("content-type", formHeaders.headers["Content-Type"]);
        if (GameUtils.IsWeb())
        {
            request.SetRequestHeader("SdAuthorization", GameModel.Instance.loginSession);
            request.SetRequestHeader("SdType", "2");
        }
        
        request.downloadHandler = downloader;
        request.uploadHandler = uploader;
        Instance.StartCoroutine(IRunRequest(request, onSuccess, onFailure));
    }
    
    private static WWWForm MergeFormDataHeader(WWWForm form, List<string> names, List<string> data)
    {
        for (int i = 0; i < names.Count; i++)
        {
            form.AddField(names[i], data[i]);
        }

        return form;
    }

    public static void RequestMultipleTextures(List<string> urls, Action<Dictionary<string, Texture2D>> onComplete)
    {
        Instance.StartCoroutine(GetTexture(urls, onComplete));
    }

    static IEnumerator GetTexture(List<string> requestUrls, Action<Dictionary<string, Texture2D>> onComplete) 
    {
        var requests = new List<UnityWebRequestAsyncOperation>(requestUrls.Count);

        // Start all requests
        for (var i = 0; i < requestUrls.Count; i++)
        {
            var www = UnityWebRequestTexture.GetTexture(requestUrls[i]);
            Debug.Log(requestUrls[i]);
            // starts the request but doesn't wait for it for now
            requests.Add(www.SendWebRequest());
        }

        // Now wait for all requests parallel
        yield return new WaitUntil(() => AllRequestsDone(requests));

        // Now evaluate all results
        onComplete?.Invoke(HandleAllRequestsWhenFinished(requests));

        foreach(var request in requests)
        {
            request.webRequest.Dispose();
        }
    }

    private static Dictionary<string, Texture2D> HandleAllRequestsWhenFinished(List<UnityWebRequestAsyncOperation> requests)
    {
        var ls = new Dictionary<string, Texture2D>();
        for(var i = 0; i < requests.Count; i++)
        {
            var www = requests[i].webRequest;
            if(www.isNetworkError || www.isHttpError) 
            {
                Debug.LogError(www.error);
            }
            else 
            {
                var myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                ls.Add(www.url, myTexture);
            }
        }

        return ls;
    }

    private static bool AllRequestsDone(List<UnityWebRequestAsyncOperation> requests)
    {
        return requests.All(r => r.isDone);
    }
}
