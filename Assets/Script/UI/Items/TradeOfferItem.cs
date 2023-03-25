using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TradeOfferItem : MonoBehaviour
{
    [SerializeField] private TMP_Text resNameTF;
    [SerializeField] private TMP_Text repeatTF;
    private FTradeInvitation invitation = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LoadUI()
    {
        if (this.invitation != null)
        {
            resNameTF.text = invitation.resource;
            repeatTF.text = invitation.repeat;
        }
    }

    public void SetData(FTradeInvitation invitation)
    {
        this.invitation = invitation;
        LoadUI();
    }

    public void OnAccept()
    {
        if (this.invitation == null)
        {
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        InvitationViewController.Instance.AcceptInvitation(this.invitation, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                UIManager.Instance.ShowLoadingBar(false);
                GetComponentInParent<MemberOffer>().Initialize();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            
        });
    }

    public void OnDecline()
    {
        if (this.invitation == null)
        {
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        InvitationViewController.Instance.DeclineInvitation(this.invitation, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            GetComponentInParent<MemberOffer>().Initialize();
        });
    }
}
