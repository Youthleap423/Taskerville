using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeViewController : SingletonComponent<TradeViewController>
{
    List<FTrade> currentReceiveTrades = new List<FTrade>();
    List<FTrade> currentSentTrades = new List<FTrade>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CheckTrades()
    {
        if (PlayerPrefs.GetString("TradeUpdate") == Convert.DateTimeToFDate(System.DateTime.Now))
        {
            return;
        }
        this.currentReceiveTrades.Clear();
        LoadReceiveTrades((isSuccess, errMsg, tradeList) =>
        {
            this.currentReceiveTrades = tradeList;
            foreach(FTrade trade in tradeList)
            {
                if (trade.type == ETradeInviteType.Offer.ToString())
                {
                    trade.Buy();
                }
                else
                {
                    trade.Sell();
                }
                
            }

            LoadSentTrades((isSuccess, errMsg, tradeList) =>
            {
                this.currentSentTrades = tradeList;
                foreach (FTrade trade in tradeList)
                {
                    if (trade.type == ETradeInviteType.Offer.ToString())
                    {
                        trade.Sell();
                    }
                    else
                    {
                        trade.Buy();
                    }
                }
                DataManager.Instance.UpdateTrades(currentReceiveTrades, currentSentTrades, (isSuccess, errMsg) =>
                {
                    if (isSuccess)
                    {
                        PlayerPrefs.SetString("TradeUpdate", Convert.DateTimeToFDate(System.DateTime.Now));
                    }
                });
            });

        });
    }

    public void LoadSentTrades(System.Action<bool, string, List<FTrade>> callback)
    {
        if (currentSentTrades.Count == 0)
        {
            LUser me = UserViewController.Instance.GetCurrentUser();
            DataManager.Instance.GetSentTrades(me.id, callback);
        }
        else
        {
            callback(true, "", currentSentTrades);
        }
    }

    public void LoadReceiveTrades(System.Action<bool, string, List<FTrade>> callback)
    {
        if (currentReceiveTrades.Count == 0)
        {
            LUser me = UserViewController.Instance.GetCurrentUser();
            DataManager.Instance.GetReceiveTrades(me.id, callback);
        }
        else
        {
            callback(true, "", currentReceiveTrades);
        }
    }

    public void CancelTrade(FTrade trade, System.Action<bool, string> callback)
    {
        DataManager.Instance.RemoveData(trade, callback);
    }

    public void DoTrade(FTrade trade)
    {

    }
}
