using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class LoginScreen : IScreen
{
    [Space]
    [SerializeField] public InputField username_IF;
    [SerializeField] public InputField password_IF;
    [SerializeField] public Toggle toggle;
    #region Unity_Members
    // Start is called before the first frame update
    void Start()
    {
        toggle.onValueChanged.AddListener(delegate {
            onToggleValueChanged(toggle);
        });
    }

    private void OnEnable()
    {
        
    }
    #endregion

    #region Public_Members
    public void OnLogin()
    {
        if (username_IF.text == "")
        {
            UIManager.Instance.ShowErrorDlg("Please input username");
            return;
        }

        if (password_IF.text == "")
        {
            UIManager.Instance.ShowErrorDlg("Please input username");
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        UserViewController.Instance.OnSignIn(username_IF.text, password_IF.text, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                AppManager.Instance.LoadTaskScene();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public void OnRegister()
    {
        ScreenManager.Instance.Show("register");
    }

    public void OnForgotPassword()
    {
        ScreenManager.Instance.Show("recover_password");
    }

    private void onToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            password_IF.contentType = InputField.ContentType.Standard;
        }
        else
        {
            password_IF.contentType = InputField.ContentType.Password;
        }
        password_IF.ForceLabelUpdate();
    }
    #endregion
}
