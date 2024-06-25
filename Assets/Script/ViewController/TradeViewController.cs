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

    public void LoadSentTrades(System.Action<bool, string, List<FTrade>> callback)
    {
        DataManager.Instance.GetSentTrades(callback);
    }

    public void LoadReceiveTrades(System.Action<bool, string, List<FTrade>> callback)
    {
        DataManager.Instance.GetReceiveTrades(callback);
    }

    public void CancelTrade(FTrade trade, System.Action<bool, string> callback)
    {
        DataManager.Instance.CancelTrades(trade.id, callback);
    }
}
