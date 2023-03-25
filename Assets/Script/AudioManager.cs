using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgAudioSource;
    [SerializeField] private GameObject fxAudioObj;

    [Space]
    [Header("BG Sounds")]
    public AudioClip logoClip;
    public AudioClip firstClip;
    public AudioClip villagerClip;
    public AudioClip creditClip;
    public AudioClip cyclopaediaClip;

    [Header("FX Sounds")]
    public AudioClip bellClip;
    public AudioClip angryClip;
    public AudioClip stormClip;
    public AudioClip constructionClip;
    public AudioClip gemClip;
    public AudioClip specialConstructionClip;
    public AudioClip celebrationClip;
    private static AudioManager _instance = null;


    private float bgAudioVolumn = 0.5f;
    private float fxAudioVolumn = 0.5f;
    private bool isFading = false;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var managerObj = GameObject.Find("AudioManager");
                if (managerObj != null)
                {
                    _instance = managerObj.GetComponent<AudioManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        SetBGAudioVolumn(GetBGAudioVolumn());
        SetFXAudioVolumn(GetFXAudioVolumn());

        foreach(AudioSource asrc in fxAudioObj.GetComponentsInChildren<AudioSource>())
        {
            if (asrc.isPlaying == true)
            {
                asrc.Stop();
            }
            
            DestroyImmediate(asrc);
        }
    }

    public float GetBGAudioVolumn()
    {
        if (PlayerPrefs.HasKey("bgAudioVolumn"))
        {
            return PlayerPrefs.GetFloat("bgAudioVolumn");
        }
        else
        {
            return 0.5f;
        }
    }

    public void SetBGAudioVolumn(float f)
    {
        PlayerPrefs.SetFloat("bgAudioVolumn", f);
        bgAudioSource.volume = f;
    }

    public float GetFXAudioVolumn()
    {
        if (PlayerPrefs.HasKey("fxAudioVolumn"))
        {
            return PlayerPrefs.GetFloat("fxAudioVolumn");
        }
        else
        {
            return 0.5f;
        }
    }

    public void SetFXAudioVolumn(float f)
    {
        PlayerPrefs.SetFloat("fxAudioVolumn", f);

        foreach (AudioSource asrc in fxAudioObj.GetComponentsInChildren<AudioSource>())
        {
            asrc.volume = f;
        }
    }

    public void PlayBackgroundSound(AudioClip clip, ulong delay = 0)
    {
        bgAudioSource.volume = GetBGAudioVolumn();
        if (bgAudioSource.clip != null && clip.name == bgAudioSource.clip.name)
        {
            return;
        }

        bgAudioSource.Stop();
        bgAudioSource.clip = clip;
        bgAudioSource.Play(delay);
    }

    public void StopBackgroundSound()
    {
        bgAudioSource.Stop();
        bgAudioSource.clip = null;
    }

    public void PauseBackgroundSound()
    {
        bgAudioSource.Pause();
    }

    public void ResumeBackgroundSound()
    {
        if (bgAudioSource.clip != null && bgAudioSource.isPlaying == false)
        {
            bgAudioSource.Play();
        }
    }

    public void PlayFXSound(AudioClip clip, bool withBgSound = true, float delay = 0f)
    {
        StartCoroutine(EPlayFXSound(clip, withBgSound, delay));
    }

    public void PlayFXSound(List<AudioClip> clips, bool withBgSound = true, float delay = 0f)
    {
        StartCoroutine(EPlayFXSound(clips, withBgSound, delay));
    }

    public void FadeOut()
    {
        if (isFading)
        {
            return;

        }
        
        StartCoroutine(EFadeOut(bgAudioSource, 0f, 2f));
    }

    public void FadeIn()
    {

    }
    IEnumerator EPlayFXSound(AudioClip clip, bool withBgSound, float delay)
    {
        if (!withBgSound)
        {
            PauseBackgroundSound();
        }
        yield return new WaitForSeconds(delay);
        var fxAudioSource = fxAudioObj.AddComponent<AudioSource>();
        fxAudioSource.volume = GetFXAudioVolumn();
        fxAudioSource.clip = clip;
        fxAudioSource.Play();

        yield return new WaitForSeconds(clip.length + 0.1f);

        fxAudioSource.Stop();
        DestroyImmediate(fxAudioSource);

        ResumeBackgroundSound();
    }

    IEnumerator EPlayFXSound(List<AudioClip> clips, bool withBgSound, float delay)
    {
        if (!withBgSound)
        {
            PauseBackgroundSound();
        }
        yield return new WaitForSeconds(delay);

        var fxAudioSource = fxAudioObj.AddComponent<AudioSource>();
        fxAudioSource.volume = GetFXAudioVolumn();

        foreach (AudioClip clip in clips)
        {
            if (fxAudioSource.isPlaying)
            {
                fxAudioSource.Stop();
            }

            fxAudioSource.clip = clip;
            fxAudioSource.Play();

            yield return new WaitForSeconds(clip.length + 0.1f);
        }
        
        fxAudioSource.Stop();
        DestroyImmediate(fxAudioSource);

        ResumeBackgroundSound();
    }

    IEnumerator EFadeOut(AudioSource audioSource, float targetVolume, float duration)
    {
        isFading = true;
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        isFading = false;
        StopBackgroundSound();
        yield break;
    }
}
