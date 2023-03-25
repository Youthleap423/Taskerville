using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;

public class RegisterScreen : IScreen
{
    [Space]
    [SerializeField] public InputField username_IF;
    [SerializeField] public InputField password_IF;
    [SerializeField] public InputField email_IF;
    [SerializeField] public Toggle toggle;
    #region Unity_Memebers
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

    void OnDestroy()
    {
        
    }
    #endregion

    #region Public_Memebers
    public void OnRegister()
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

        if (email_IF.text == "")
        {
            UIManager.Instance.ShowErrorDlg("Please input username");
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        UserViewController.Instance.OnRegister(username_IF.text, email_IF.text, password_IF.text, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                ScreenManager.Instance.Show("chooseAvatar");
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }

        });
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
