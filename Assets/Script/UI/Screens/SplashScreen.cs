using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class SplashScreen : IScreen
{
    #region Unity_Members
    private void Awake()
    {
        FAuth.Instance.OnFAuthLoginFailed += OnFAuthLoginFailed;
        FAuth.Instance.OnFAuthLoginSucceeded += OnFAuthLoginSucceeded;
    }
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.ShowLoadingBar(true);
        if (AppManager.Instance.isRestart)
        {
            CheckLoginStatus();
            AppManager.Instance.isRestart = false;
        }
    }

    private void CheckLoginStatus()
    {
        if (DataManager.Instance.CurrentFUser.id != "")
        {
            UserViewController.Instance.OnGetFUser(DataManager.Instance.CurrentFUser.id, (isSuccess, errMsg) =>
            {
                AppManager.Instance.userID = "";
                if (isSuccess)
                {
                    ShowMainScreen();
                }
                else
                {
                    ShowLoginScreen();
                    //UIManager.Instance.ShowErrorDlg(errMsg);
                }
            });
        }
        else
        {
            ShowLoginScreen();
        }
    }
    #endregion

    #region Private_Members

    private void OnFAuthLoginFailed(string strMsg)
    {
        Debug.LogError("Failed:" + strMsg);
        FAuth.Instance.OnFAuthLoginFailed -= OnFAuthLoginFailed;
        FAuth.Instance.OnFAuthLoginSucceeded -= OnFAuthLoginSucceeded;
        Invoke("ShowLoginScreen", 2);
    }

    private void OnFAuthLoginSucceeded(string userId)
    {
        Debug.LogError("Success:" + userId);
        FAuth.Instance.OnFAuthLoginFailed -= OnFAuthLoginFailed;
        FAuth.Instance.OnFAuthLoginSucceeded -= OnFAuthLoginSucceeded;
        UserViewController.Instance.OnGetFUser(userId, (isSuccess, errMsg) =>
        {
            AppManager.Instance.userID = "";
            if (isSuccess)
            {
                Invoke(nameof(ShowMainScreen), 2);
            }
            else
            {
                ShowLoginScreen();
            }
            
        });
    }


    private void ShowLoginScreen()
    {
        UIManager.Instance.ShowLoadingBar(false);
        ScreenManager.Instance.Show("login");
    }

    private void ShowMainScreen()
    {
        UIManager.Instance.ShowLoadingBar(false);
        //ScreenManager.Instance.Show("main");
        AppManager.Instance.LoadTaskScene();
    }
    #endregion
}
