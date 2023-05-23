using Firebase.Firestore;
using System.Collections.Generic;
using System;

[FirestoreData]
public class FTradeItem : FData
{
    [FirestoreProperty]
    public string receiver_Id { get; set; }
    [FirestoreProperty]
    public string resource { get; set; }
    [FirestoreProperty]
    public string state { get; set; }
    [FirestoreProperty]
    public string type { get; set; }
    
    public FTradeItem()
    {
        Id = "";
        collectionId = "TradeItem";
        Pid = "";
        created_at = Convert.DateTimeToFDate(System.DateTime.Now);
        state = EState.Created.ToString();
    }

    public FTradeItem(FTrade trade, string pid, string receiverId, System.DateTime dateTime)
    {
        Id = trade.Id;
        collectionId = "TradeItem";
        Pid = pid;
        type = trade.type;
        resource = trade.resource;
        receiver_Id = receiverId;
        state = EState.Created.ToString();
        created_at = Convert.DateTimeToFDate(dateTime);
    }


    public void Buy()
    {
        EResources eResource = (EResources)Enum.Parse(typeof(EResources), resource);
        CResource cResource = ResourceViewController.Instance.GetCResource(eResource);
        var currentResValue = ResourceViewController.Instance.GetCurrentResourceValue(EResources.Gold);
        if (currentResValue >= cResource.buy_price_from_coalition)
        {
            var resourceDic = new Dictionary<EResources, float>();
            UIManager.LogError("FTrade Buy Done:" + cResource.buy_price_from_coalition);
            resourceDic.Add(eResource, cResource.market_amount);
            resourceDic.Add(EResources.Gold, -cResource.buy_price_from_coalition);
            ResourceViewController.Instance.UpdateResource(resourceDic, (isSuccuess, errMsg) =>
            {
                if (isSuccuess)
                {
                    UserViewController.Instance.GetCurrentUser().updateBuy(System.DateTime.Now, -cResource.buy_price_from_coalition);
                    RewardSystem.Instance.GivesTradeReward(eResource);
                }
            });
        }
    }
}
