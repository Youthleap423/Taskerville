using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EasyMobile;

public class LogoScene : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        Notifications.LocalNotificationOpened += Notifications_LocalNotificationOpened;
        AudioManager.Instance.PlayBackgroundSound(AudioManager.Instance.logoClip);
        StartCoroutine(FadeIn(0f, 0.7f));
        StartCoroutine(FadeOut(3.5f, 0.7f));
        Invoke("NextScene", 4.3f);
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
