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
            foreach(FTrade trade in this.currentReceiveTrades)
            {
                if (trade.type != ETradeInviteType.Offer.ToString())
                {
                    trade.Sell(trade.receiver_Id, trade.Pid);
                    trade.update_at = Convert.DateTimeToFDate(System.DateTime.Now);
                }
                
            }

            LoadSentTrades((isSuccess, errMsg, tradeList) =>
            {
                this.currentSentTrades = tradeList;
                foreach (FTrade trade in this.currentSentTrades)
                {
                    if (trade.type == ETradeInviteType.Offer.ToString())
                    {
                        trade.Sell(trade.Pid, trade.receiver_Id);
                        trade.update_at = Convert.DateTimeToFDate(System.DateTime.Now);
                    }
                }

                DataManager.Instance.GetReceiveTradeItems(UserViewController.Instance.GetCurrentUser().id, (isSuccess, errMsg, list) =>
                {
                    if (isSuccess)
                    {
                        foreach(FTradeItem item in list)
                        {
                            item.Buy();
                        }
                    }

                    DataManager.Instance.UpdateTrades(currentReceiveTrades, currentSentTrades, list, (isSuccess, errMsg) =>
                    {
                        if (isSuccess)
                        {
                            PlayerPrefs.SetString("TradeUpdate", Convert.DateTimeToFDate(System.DateTime.Now));
                        }
                    });
                });
            });

        });
    }

    public void LoadSentTrades(System.Action<bool, string, List<FTrade>> callback)
    {
        LUser me = UserViewController.Instance.GetCurrentUser();
        DataManager.Instance.GetSentTrades(me.id, callback);
    }

    public void LoadReceiveTrades(System.Action<bool, string, List<FTrade>> callback)
    {
        LUser me = UserViewController.Instance.GetCurrentUser();
        DataManager.Instance.GetReceiveTrades(me.id, callback);
    }

    public void CancelTrade(FTrade trade, System.Action<bool, string> callback)
    {
        DataManager.Instance.RemoveData(trade, callback);
    }

    public void DoTrade(FTrade trade)
    {

    }
}
