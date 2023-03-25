using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordRecoveryScreen : IScreen
{
    [Space]
    [SerializeField] public InputField email_IF;
    [SerializeField] public GameObject LoadingBar;

    #region Public_Members
    public void SendResetEmail()
    {
        if (email_IF.text == "")
        {
            UIManager.Instance.ShowErrorDlg("Please input Email Address.");
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        UserViewController.Instance.OnResetPassword(email_IF.text, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                ScreenManager.Instance.Show("login");
            }
        });
    }
    #endregion

}
