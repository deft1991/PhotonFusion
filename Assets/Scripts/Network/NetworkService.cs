using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkService : MonoBehaviour
{
    /*
     * URL for request weather
     */
    private const string xmlApi =
        "https://api.openweathermap.org/data/2.5/weather?q=Los%20Angeles,us&mode=xml&APPID=47f8a39ab2adaaee0af9515f2cc7b351";

    private const string jsonApi =
        "https://api.openweathermap.org/data/2.5/weather?q=Los%20Angeles,us&APPID=47f8a39ab2adaaee0af9515f2cc7b351";

    private string webImage = "https://pbs.twimg.com/media/FTQnAbqUUAABGVW?format=jpg&name=large";

    private const string localApi = "http://localhost:8080/weather/log";

    public IEnumerator GetWeatherXML(Action<string> callback)
    {
        return CallAPI(xmlApi, null, callback);
    }

    public IEnumerator GetWeatherJSON(Action<string> callback)
    {
        return CallAPI(jsonApi, null, callback);
    }

    /**
     * Uses Texture2D ass callback parameter
     */
    public IEnumerator DownloadImage(Action<Texture2D> callback)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(webImage);
        yield return request.SendWebRequest();

        /*
         * Get downloaded image we service program DownloadHandler
         */
        callback(DownloadHandlerTexture.GetContent(request));
    }
    
    /**
     * Uses Texture2D ass callback parameter
     */
    public IEnumerator DownloadImage(Action<Texture2D> callback, string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        /*
         * Get downloaded image we service program DownloadHandler
         */
        callback(DownloadHandlerTexture.GetContent(request));
    }


    private IEnumerator CallAPI(string url, Action<string> callback)
    {
        /*
         * Call services MUST be coroutine. 
         */
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            /*
             * Pause in downloading
             */
            yield return request.SendWebRequest();

            /*
             * Check errors in response
             */
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("network problem: " + request.error);
            }
            else if (request.responseCode != (long)System.Net.HttpStatusCode.OK)
            {
                Debug.Log("response error: " + request.responseCode);
            }
            else
            {
                /*
                 * Call delegate
                 */
                callback(request.downloadHandler.text);
            }
        }
    }

    private IEnumerator CallAPI(string url, WWWForm form, Action<string> callback)
    {
        /*
         * POST with form
         * or
         * GET with URL
         */
        using UnityWebRequest request = (form == null) ? UnityWebRequest.Get(url) : UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("network problem: " + request.error);
        } else if (request.responseCode != (long)System.Net.HttpStatusCode.OK)
        {
            Debug.LogError("response error: " + request.responseCode);
        }
        else
        {
            callback(request.downloadHandler.text);
        }
    }

    public IEnumerator LogWeather(string name, float cloudValue, Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("message", name);
        form.AddField("cloud_value", cloudValue.ToString(CultureInfo.CurrentCulture));
        form.AddField("timestamp", DateTime.UtcNow.Ticks.ToString());

        return CallAPI(localApi, form, callback);
    }
}