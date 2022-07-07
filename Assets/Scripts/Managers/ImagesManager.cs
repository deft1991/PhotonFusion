using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagesManager : MonoBehaviour, IGameManager
{
    
    public ManagerStatus Status { get; private set; }

    private NetworkService _network;

    /*
     * Save downloaded image
     */
    private Texture2D _webImage;

    public void Startup(NetworkService networkService)
    {
        Debug.Log("Images manager starting...");

        _network = networkService;

        Status = ManagerStatus.Started;
        Debug.Log("ImagesManager: started");
    }

    public void GetWebImage(Action<Texture2D> callback)
    {
        /*
         * Check that we don't have saved image
         */
        if (_webImage == null)
        {
            Action<Texture2D> action = image =>
            {
                _webImage = image;
                callback(_webImage);
            };
            StartCoroutine(_network.DownloadImage(action));
        }
        else
        {
            /*
             * If have image, immediately executes callback
             */
            callback(_webImage);
        }
    }
    
    public void GetWebImage(Action<Texture2D> callback, string imageUrl)
    {
        /*
         * Check that we don't have saved image
         */
        if (_webImage == null)
        {
            Action<Texture2D> action = image =>
            {
                _webImage = image;
                callback(_webImage);
            };
            StartCoroutine(_network.DownloadImage(action, imageUrl));
        }
        else
        {
            /*
             * If have image, immediately executes callback
             */
            callback(_webImage);
        }
    }
}
