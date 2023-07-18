using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using EasyMobile;

using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    [HideInInspector] public bool isRestart = false;
    [HideInInspector] public string userID = "";
    [HideInInspector] public bool singedIn = false;
    [HideInInspector] public string currentScene = "";
    [HideInInspector] public int excavationDigIndex = -1;

    private static AppManager _instance = null;
    private string prevSceneStr = "";

    public event Action<string> OnNotificationOpened = delegate { };
    public static AppManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var appManagerObj = GameObject.Find("AppManager");
                if (appManagerObj != null)
                {
                    _instance = appManagerObj.GetComponent<AppManager>();
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
        Notifications.LocalNotificationOpened += Notifications_LocalNotificationOpened;
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

        NotificationManager.Instance.RescheduleLocalNotification(notify.content);

        if (notifPageId != "")
        {
            if (notify.isAppInForeground)
            {
                UIManager.Instance.ShowNotification(notify.content);
                PlayerPrefs.SetString("NotifyPage", string.Empty);
            }
            else
            {
                PlayerPrefs.SetString("NotifyPage", notifPageId);
                if (SceneManager.GetActiveScene().name != "TaskScene")
                {
                    LoadTaskScene();
                }
                else
                {
                    OnNotificationOpened(notifPageId);
                }
            }
        }
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForEndOfFrame();

        isRestart = true;
        userID = UserViewController.Instance.GetCurrentUser().id;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    [ContextMenu("Clear")]
    public void ClearPlayerPrefab()
    {
        PlayerPrefs.DeleteAll();
    }

    [ContextMenu("Test")]
    public void Test()
    {
        var notif = new NotificationContent();
        notif.title = "Taskerville";
        notif.subtitle = "Notification";
        notif.body = "test";
        UIManager.Instance.ShowNotification(notif);
    }

    public void ChangeMode(Game_Mode game_mode)
    {
        NotificationManager.Instance.CancelAllPendingLocalNotifications();
        UserViewController.Instance.UpdateSetting(game_mode);
        StartCoroutine(RestartGame());
    }

    public Game_Mode GetCurrentMode()
    {
        return (Game_Mode)(UserViewController.Instance.GetCurrentSetting().current_mode);
    }

    public void SignOut()
    {
        isRestart = true;
        NotificationManager.Instance.CancelAllPendingLocalNotifications();
        UserViewController.Instance.OnSignOut();
        userID = "";
        SceneManager.LoadSceneAsync("AuthScene");
    }

    public void HandleAIPlayer()
    {
        var aiplayer = GetComponent<AIGamePlay>();
        if (GetCurrentMode() == Game_Mode.Task_Only)
        {
            if (aiplayer == null)
            {
                gameObject.AddComponent<AIGamePlay>();
            }
        }
        else
        {
            RemoveAIPlayer();
        }
    }

    public void RemoveAIPlayer()
    {
        var aiplayer = GetComponent<AIGamePlay>();
        if (aiplayer != null)
        {
            Destroy(aiplayer);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
#if !UNITY_EDITOR

        if (focus)
        {
            if (SceneManager.GetActiveScene().name == "Background")
            {
                StartCoroutine(changeScene("GameScene"));
            }
        }
        else
        {

            prevSceneStr = SceneManager.GetActiveScene().name;
            if (SceneManager.GetActiveScene().name == "GameScene")
            {
                UIManager.Instance.ShowBackground(true);
                SceneManager.LoadScene("Background");
                //StartCoroutine(changeScene("Background"));
                //SceneManager.LoadSceneAsync("Background");
            }
        }
#endif
    }

    private void OnApplicationQuit()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SaveData();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (DataManager.Instance != null)
            {
                DataManager.Instance.SaveData();
            }
        }
    }

    IEnumerator changeScene(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
        
    }

    public void LoadGameScene()
    {
        //LoadScene("Background");
        LoadScene("GameScene");
    }

    public void LoadTaskScene()
    {
        LoadScene("TaskScene");
    }

    public void LoadScene(string sceneName)
    {
        Resources.UnloadUnusedAssets();
        SceneManager.LoadSceneAsync(sceneName);
        
        currentScene = sceneName;
    }

    [ContextMenu("Remove Cache")]
    public void Clearcache()
    {
        var allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true);
        foreach (GameObject obj in allObjects)
        {
            if (obj.GetComponent<Image>() != null)
            {
                obj.GetComponent<Image>().sprite = null;
            }

        }
        Resources.UnloadUnusedAssets();
    }


}
