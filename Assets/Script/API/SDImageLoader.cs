using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SDImageLoader : MonoBehaviour
{
    private Image target = null;
    private string url = null;

    private static Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    public static SDImageLoader Get()
    {
        return new GameObject("SDImageLoader").AddComponent<SDImageLoader>();
    }

    public SDImageLoader Load(string url)
    {
        this.url = url;
        return this;
    }

    public SDImageLoader Into(Image image)
    {
        target = image;
        return this;
    }

    public void StartLoading(bool useCache = true)
    {
        if (url == null)
        {
            SDLogger.LogError("Url has not been set. Use 'load' funtion to set image url.");
            return;
        }

        if (cache.ContainsKey(url) && useCache)
        {
            target.sprite = cache[url];
            Destroyer();
            return;
        }
        
        try
        {
            Uri uri = new Uri(url);
            this.url = uri.AbsoluteUri;
        }
        catch (Exception)
        {
            SDLogger.LogError("Url is not correct.");
            return;
        }
        
        StopAllCoroutines();
        StartCoroutine(Downloader());
    }

    private IEnumerator Downloader()
    {
        var www = new UnityWebRequest(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();
        while (!www.isDone)
        {
            if (www.error != null)
            {
                SDLogger.LogError("Error while downloading the image : " + www.error);
                yield break;
            }

            yield return null;
        }
        StartCoroutine(ImageLoader(www.downloadHandler.data));
        www.Dispose();
    }

    private IEnumerator ImageLoader(byte[] bytes)
    {
        TextureFormat format = TextureFormat.DXT5;
        Texture2D texture = new Texture2D(2, 2, format, false);
        //ImageConversion.LoadImage(texture, fileData);
        texture.LoadImage(bytes); //..this will auto-resize the texture dimensions.

        // if (texture.width > 1000 && texture.width < 2000)
        // {
        //     TextureScale.Bilinear(texture, texture.width / 2, texture.height / 2);
        // }
        // else if (texture.width > 2000)
        // {
        //     TextureScale.Bilinear(texture, texture.width / 3, texture.height / 3);
        // }

        texture.Compress(false);

        if (ReferenceEquals(target, null)) yield break;

        Sprite sprite = Sprite.Create(texture,
            new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        if(!cache.ContainsKey(url)) cache.Add(url, sprite);
        if (cache.Count > 20)
        {
            cache.Remove(cache.Keys.First());
        }
        if(target != null) target.sprite = sprite;
        Destroyer();
    }

    private void Destroyer()
    {
        Destroy(gameObject);
    }

    // public static void ClearCache(string url)
    // {
    //     foreach (var c in cache)
    //     {
    //         Destroy(c.Value);
    //         cache.Remove(c.Key);
    //     }
    // }
}