using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberOffer : Page
{

    [SerializeField] private GameObject tradeListItem;
    [SerializeField] private Transform offer_parent_transform;

    private Dictionary<string, List<FTradeInvitation>> filteredOfferDic = new Dictionary<string, List<FTradeInvitation>>();
    private Dictionary<string, List<FTradeInvitation>> filteredRequestDic = new Dictionary<string, List<FTradeInvitation>>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void FilterInvitaion(List<FTradeInvitation> list)
    {
        filteredOfferDic = new Dictionary<string, List<FTradeInvitation>>();
        filteredRequestDic = new Dictionary<string, List<FTradeInvitation>>();

        foreach (FTradeInvitation invitation in list)
        {
            if (invitation.type == ETradeInviteType.Offer.ToString())
            {
                if (!filteredOfferDic.ContainsKey(invitation.sender))
                {
                    filteredOfferDic.Add(invitation.sender, new List<FTradeInvitation>());
                }
                filteredOfferDic[invitation.sender].Add(invitation);
            }
            else
            {
                if (!filteredRequestDic.ContainsKey(invitation.sender))
                {
                    filteredRequestDic.Add(invitation.sender, new List<FTradeInvitation>());
                }
                filteredRequestDic[invitation.sender].Add(invitation);
            }
            
        }
        LoadUI();
    }

    private void LoadUI()
    {
        LoadInvitationList();
    }

    private void LoadInvitationList()
    {
        foreach (Transform child in offer_parent_transform.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (string userId in filteredOfferDic.Keys)
        {
            GameObject obj = Instantiate(tradeListItem, offer_parent_transform);
            obj.GetComponent<TradeOfferList>().SetList(filteredOfferDic[userId]);
        }

        foreach (string userId in filteredRequestDic.Keys)
        {
            GameObject obj = Instantiate(tradeListItem, offer_parent_transform);
            obj.GetComponent<TradeOfferList>().SetList(filteredRequestDic[userId]);
        }
    }


    public override void Initialize()
    {
        base.Initialize();

        UIManager.Instance.ShowLoadingBar(true);
        InvitationViewController.Instance.LoadTradeInvitation((isSuccess, errMsg, inviteList) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                FilterInvitaion(inviteList);                
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public void Back()
    {
        gameObject.GetComponentInParent<NavPage>().Back();
    }
}
