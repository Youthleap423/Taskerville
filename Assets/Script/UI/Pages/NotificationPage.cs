using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPage : Page
{
    [Space]
    [SerializeField] private GameObject requestItemPrefab;
    [SerializeField] private GameObject artTradeRequestItemPrefab;
    [SerializeField] private Transform responseTransform;
    [SerializeField] private Transform artExchangeTranform;

    private List<FInvitation> invitationList = new List<FInvitation>();
    private List<FArtTrade> tradeList = new List<FArtTrade>();
    #region Private Members
    private void OnEnable()
    {
        Initialize();
    }

    private void LoadUI()
    {
        foreach(Transform child in responseTransform)
        {
            if (!child.gameObject.name.Equals("title"))
            {
                Destroy(child.gameObject);
            }
        }



        foreach(FInvitation invitation in invitationList)
        {
            if (invitation.isOutOfDate())
            {
                InvitationViewController.Instance.RemoveInvitation(invitation);
                return;
            }

            if (invitation.state == EState.Created.ToString())
            {
                if (invitation.type == EInviteType.Invite_Coalition.ToString())
                {
                    //CreateObject(invitation, inviteTransform);
                    CreateObject(invitation, responseTransform);
                }
                else
                {
                    //CreateObject(invitation, requestTranform);
                    CreateObject(invitation, responseTransform);
                }
            }
            else
            {
                CreateObject(invitation, responseTransform);
            }
        }


        LayoutRebuilder.ForceRebuildLayoutImmediate(responseTransform.GetComponent<RectTransform>());
    }

    private void LoadTradeUI()
    {
        foreach (Transform child in artExchangeTranform)
        {
            if (!child.gameObject.name.Equals("title"))
            {
                Destroy(child.gameObject);
            }
        }
        foreach (FArtTrade trade in tradeList)
        {
            if (trade.state == EState.Created.ToString())
            {
                if (trade.Pid == UserViewController.Instance.GetCurrentUser().id)
                {
                    CreateObject(trade, artExchangeTranform);
                }
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(artExchangeTranform.GetComponent<RectTransform>());
    }

    private void CreateObject(FInvitation invitation, Transform trans)
    {
        GameObject obj = Instantiate(requestItemPrefab, trans);
        obj.GetComponent<RequestItem>().SetData(invitation, this);
    }

    private void CreateObject(FArtTrade trade, Transform trans)
    {
        
        GameObject obj = Instantiate(artTradeRequestItemPrefab, trans);
        obj.GetComponent<ArtTradeRequestItem>().SetData(trade, this);
    }

    #endregion

    #region Public Members
    public void Back()
    {
        transform.parent.GetComponent<NavPage>().Back();
    }

    public override void Initialize()
    {
        base.Initialize();
        this.invitationList.Clear();
        this.tradeList.Clear();

        UIManager.Instance.ShowLoadingBar(true);
        InvitationViewController.Instance.LoadInvitation((isSuccess, errMsg, invitationList) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                this.invitationList = invitationList;
                
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            LoadUI();
        });

        UIManager.Instance.ShowLoadingBar(true);
        ArtworkSystem.Instance.LoadArtTrades((isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                this.tradeList = ArtworkSystem.Instance.GetAllArtTrades();
                
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            LoadTradeUI();
        });
    }


    #endregion
}
