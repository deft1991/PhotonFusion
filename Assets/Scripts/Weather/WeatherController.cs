using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    [SerializeField] private Material sky;
    [SerializeField] private Light sun;

    private float _fullIntensity;
    private static readonly int Blend = Shader.PropertyToID("_Blend");

    private void Awake()
    {
        Messenger.AddListener(GameEvent.WEATHER_UPDATED, OnWeatherUpdated);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.WEATHER_UPDATED, OnWeatherUpdated);
    }

    private void Start()
    {
        /*
         * Start intensity == same as full 
         */
        _fullIntensity = sun.intensity;
    }

    private void OnWeatherUpdated()
    {
        SetOvercast(Managers.Weather.cloudValue);
    }
    
    private void SetOvercast(float value)
    {
        /*
         * Correcting value of material Blend and light intensity
         *
         * Increase blend, reduce light
         * Reduce blend, increase light
         */
        sky.SetFloat(Blend, value);
        sun.intensity = _fullIntensity - (_fullIntensity * value);
    }
}