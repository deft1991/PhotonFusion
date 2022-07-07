using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioSource music1Source;
    [SerializeField] private AudioSource music2Source;

    [Tooltip("Music clip name")] [SerializeField]
    private string introBGMusic;

    [Tooltip("Music clip name")] [SerializeField]
    private string levelBGMusic;

    private float _musicVolume;

    /*
     * Check active source
     */
    private AudioSource _activeMusic;
    private AudioSource _inactiveMusic;

    private float crossFadeRate = 1.5f;

    /*
     * Switcher, protect us from errs in transition process
     */
    private bool _crossFading;

    public float MusicVolume
    {
        get { return _musicVolume; }
        set
        {
            _musicVolume = value;

            /*
             * Change volume
             */
            if (music1Source != null && !_crossFading)
            {
                music1Source.volume = _musicVolume;
                music2Source.volume = _musicVolume;
            }
        }
    }

    /*
     * Property for volume
     */
    public float SoundVolume
    {
        /*
         * Read and write use AudioListener
         */
        get { return AudioListener.volume; }
        set { AudioListener.volume = value; }
    }

    /*
     * Property to turn off sound
     */
    public bool SoundMute
    {
        get { return AudioListener.pause; }
        set { AudioListener.pause = value; }
    }

    public bool MusicMute
    {
        get
        {
            if (music1Source != null)
            {
                return music1Source.mute;
            }

            /*
             * Default value if we do not have audio source
             */
            return false;
        }
        set
        {
            if (music1Source != null)
            {
                music1Source.mute = value;
                music2Source.mute = value;
            }
        }
    }

    private NetworkService _networkService;

    public void Startup(NetworkService networkService)
    {
        Debug.Log("Audio manager starting...");
        _networkService = networkService;

        /*
         * Ignore Audio listener properties
         */
        music1Source.ignoreListenerVolume = true;
        music1Source.ignoreListenerPause = true;

        music2Source.ignoreListenerVolume = true;
        music2Source.ignoreListenerPause = true;

        /*
         * Initialize sound value (range from 0 to 1)
         */
        SoundVolume = 1f;
        MusicVolume = 1f;

        _activeMusic = music1Source;
        _inactiveMusic = music2Source;

        Status = ManagerStatus.Started;
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayMusic(AudioClip audioClip)
    {
        if (_crossFading)
        {
            return;
        }

        StartCoroutine(CrossFadeMusic(audioClip));
    }

    private IEnumerator CrossFadeMusic(AudioClip audioClip)
    {
        _crossFading = true;

        _inactiveMusic.clip = audioClip;
        _inactiveMusic.volume = 0;
        _inactiveMusic.Play();

        float scaledRate = crossFadeRate * _musicVolume;

        while (_activeMusic.volume > 0)
        {
            _activeMusic.volume -= scaledRate * Time.deltaTime;
            _inactiveMusic.volume += scaledRate * Time.deltaTime;

            yield return null;
        }

        AudioSource temp = _activeMusic;

        _activeMusic = _inactiveMusic;
        _activeMusic.volume = _musicVolume;
        _inactiveMusic = temp;
        _inactiveMusic.Stop();
        _crossFading = false;
    }

    public void StopMusic()
    {
        _activeMusic.Stop();
        _inactiveMusic.Stop();
    }

    /*
     * Load intro music from Resources folder
     */
    public void PlayIntroMusic()
    {
        PlayMusic(Resources.Load("Music/" + introBGMusic) as AudioClip);
    }

    /*
     * Load level music from resource folder
     */
    public void PlayLevelMusic()
    {
        PlayMusic(Resources.Load("Music/" + levelBGMusic) as AudioClip);
    }
}