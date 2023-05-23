using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradePage : Page
{

    [SerializeField] private GameObject tradeListItem;
    [SerializeField] private Transform offer_parent_transform;

    private Dictionary<string, List<FTrade>> filteredOfferDic = new Dictionary<string, List<FTrade>>();
    private Dictionary<string, List<FTrade>> filteredRequestDic = new Dictionary<string, List<FTrade>>();
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        Initialize();
    }

    private void FilterTrade(List<FTrade> list)
    {
        filteredOfferDic = new Dictionary<string, List<FTrade>>();
        filteredRequestDic = new Dictionary<string, List<FTrade>>();
        foreach (FTrade trade in list)
        {
            string key = trade.Pid + "_" + trade.receiver_Id;

            if (trade.type == ETradeInviteType.Offer.ToString())
            {
                if (!filteredOfferDic.ContainsKey(key))
                {
                    filteredOfferDic.Add(key, new List<FTrade>());
                }
                filteredOfferDic[key].Add(trade);
            }
            else
            {
                if (!filteredRequestDic.ContainsKey(key))
                {
                    filteredRequestDic.Add(key, new List<FTrade>());
                }
                filteredRequestDic[key].Add(trade);
            }
            
        }
        LoadUI();
    }

    private void LoadUI()
    {
        LoadTradeList();
    }

    public override void Initialize()
    {
        base.Initialize();

        UIManager.Instance.ShowLoadingBar(true);
        
        TradeViewController.Instance.LoadReceiveTrades((isSuccess, errMsg, tradeList) =>
        {
            var myTradeList = tradeList;
            TradeViewController.Instance.LoadSentTrades((isSuccess, errMsg, tradeList) =>
            {
                UIManager.Instance.ShowLoadingBar(false);
                foreach (FTrade trade in tradeList)
                {
                    myTradeList.Add(trade);
                }
                FilterTrade(myTradeList);
                if (!isSuccess)
                {
                    Debug.LogError("Sent");
                    UIManager.Instance.ShowErrorDlg(errMsg);
                }
            });
                
            if (!isSuccess)
            {
                UIManager.Instance.ShowLoadingBar(false);
                Debug.LogError("Receive");
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    private void LoadTradeList()
    {
        foreach (Transform child in offer_parent_transform.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (string userId in filteredOfferDic.Keys)
        {
            GameObject obj = Instantiate(tradeListItem, offer_parent_transform);
            obj.GetComponent<TradeListItem>().SetList(filteredOfferDic[userId]);
        }

        foreach (string userId in filteredRequestDic.Keys)
        {
            GameObject obj = Instantiate(tradeListItem, offer_parent_transform);
            obj.GetComponent<TradeListItem>().SetList(filteredRequestDic[userId]);
        }
    }

    #region Public Members
    public void ShowTradeOffer()
    {
        gameObject.GetComponentInParent<NavPage>().Show("trade_offer");
    }

    public void ShowAcquisitionOffer()
    {
        gameObject.GetComponentInParent<NavPage>().Show("acquisition_offer");
    }

    public void ShowMemberOffer()
    {
        gameObject.GetComponentInParent<NavPage>().Show("member_offer");
    }

    public void Back()
    {
        gameObject.GetComponentInParent<NavPage>().Back();
    }

    #endregion
}
