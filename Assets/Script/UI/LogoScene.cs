using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EasyMobile;

public class LogoScene : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject errObject;
    [SerializeField] private GameObject loadingObject;
    // Start is called before the first frame update

    private void Awake()
    {
        errObject.SetActive(false);
        loadingObject.SetActive(false);
    }
    void Start()
    {
        Notifications.LocalNotificationOpened += Notifications_LocalNotificationOpened;
        AudioManager.Instance.PlayBackgroundSound(AudioManager.Instance.logoClip);
        StartCoroutine(FadeIn(0f, 0.7f));
        Invoke(nameof(LoadInitResources), 2f);
    }

    void LoadInitResources()
    {
        DataManager.Instance.LoadInitCResources((isSucces, errMsg) =>
        {
            if (isSucces)
            {
                DataManager.Instance.LoadInitCBuildings((isSucces, errMsg) =>
                {
                    if (isSucces)
                    {
                        DataManager.Instance.LoadInitCVillagers((isSucces, errMsg) =>
                        {
                            if (isSucces)
                            {
                                loadingObject.SetActive(false);
                                PrepareNextScene();
                            }
                            else
                            {
                                Debug.LogError(errMsg);
                                errObject.SetActive(true);
                            }
                        });
                    }
                    else
                    {
                        Debug.LogError(errMsg);
                        errObject.SetActive(true);
                    }
                });
            }
            else
            {
                Debug.LogError(errMsg);
                errObject.SetActive(true);
            }
        });
    }

    void PrepareNextScene()
    {
        StartCoroutine(FadeOut(0f, 0.7f));
        Invoke(nameof(NextScene), 0.7f);
    }

    void NextScene()
    {
        SceneManager.LoadScene("AuthScene");
    }


    private void OnDestroy()
    {
        Notifications.LocalNotificationOpened -= Notifications_LocalNotificationOpened;
    }

    private void Notifications_LocalNotificationOpened(EasyMobile.LocalNotification notify)
    {
        string notifPageId = "";
        if (notify.content.userInfo.TryGetValue("type", out object typeObj))
        {
            notifPageId = System.Convert.ToString(typeObj);
        };
        PlayerPrefs.SetString("NotifyPage", notifPageId);
    }

    IEnumerator FadeIn(float delayTime, float fadeTime)
    {
        yield return new WaitForSeconds(delayTime);

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    IEnumerator FadeOut(float delayTime, float fadeTime)
    {
        yield return new WaitForSeconds(delayTime);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;
            yield return null;
        }
    }
}
