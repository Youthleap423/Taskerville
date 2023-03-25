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
        //Invoke("CheckUserLoggedIn", 1);
        if (AppManager.Instance.isRestart)
        {
            UserViewController.Instance.OnAuthLogin(AppManager.Instance.userID, (isSuccess, errMsg) =>
            {
                AppManager.Instance.isRestart = false;
                AppManager.Instance.userID = "";
                if (isSuccess)
                {
                    Invoke("ShowMainScreen", 2);
                }
                else
                {
                    Invoke("ShowLoginScreen", 2);
                    //UIManager.Instance.ShowErrorDlg(errMsg);
                }

            });
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
        UserViewController.Instance.OnAuthLogin(userId, (isSuccess, errMsg) =>
        {
            AppManager.Instance.isRestart = false;
            AppManager.Instance.userID = "";
            if (isSuccess)
            {
                Invoke("ShowMainScreen", 2);
            }
            else
            {
                ShowLoginScreen();
                //UIManager.Instance.ShowErrorDlg(errMsg);
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
