using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestItem : MonoBehaviour
{
    [SerializeField] private Text signText_TF;
    [SerializeField] private Text message_TF;
    [SerializeField] private GameObject yes_Button;
    [SerializeField] private GameObject no_Button;

    private FInvitation fInvitation = null;
    private NotificationPage parentPage = null;

    private void LoadUI()
    {
        if (fInvitation.state == EState.Created.ToString())
        {
            yes_Button.SetActive(true);
            no_Button.SetActive(true);
        }
        else
        {
            yes_Button.SetActive(false);
            no_Button.SetActive(false);
        }

        InvitationViewController.Instance.GetMessage(fInvitation, (isSuccess, msg) =>
        {
            if (isSuccess)
            {
                message_TF.text = msg;
            }
        });

    }

    public void SetData(FInvitation invitation, NotificationPage page)
    {
        this.parentPage = page;
        fInvitation = invitation;
        LoadUI();
    }

    public void Accept()
    {
        UIManager.Instance.ShowLoadingBar(true);
        InvitationViewController.Instance.AcceptInvitation(fInvitation, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                if (parentPage != null)
                {
                    parentPage.Initialize();
                }
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }
    
        
    public void Decline()
    {
        UIManager.Instance.ShowLoadingBar(true);
        InvitationViewController.Instance.DeclineInvitation(fInvitation, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                if (parentPage != null)
                {
                    parentPage.Initialize();
                }
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }
}
