using Firebase.Firestore;
using System.Collections.Generic;
using System;

[FirestoreData]
public class FTrade : FTradeInvitation
{
    [FirestoreProperty]
    public string update_at { get; set; }

    public FTrade()
    {
        update_at = "";
    }

    public FTrade(FTradeInvitation invitation)
    {
        collectionId = "Trade";
        Pid = invitation.Pid;
        receiver_Id = invitation.receiver_Id;
        this.type = invitation.type;
        state = EState.Created.ToString();
        resource = invitation.resource;
        repeat = invitation.repeat;
        sender_village = invitation.sender_village;
        receiver_village = invitation.receiver_village;
        created_at = Convert.DateTimeToFDate(System.DateTime.Now);
        update_at = "";
    }

    public void Sell()
    {
        var startDate = update_at == "" ? Convert.FDateToDateTime(created_at) : Convert.FDateToDateTime(update_at).AddDays(1);
        UIManager.LogError("FTrade Sell");
        for (System.DateTime day = startDate.Date; day.Date <= System.DateTime.Now.Date; day = day.AddDays(1))
        {
            if (IsAvailable(day) == true)
            {
                EResources eResource = (EResources)Enum.Parse(typeof(EResources), resource);
                CResource cResource = ResourceViewController.Instance.GetCResource(eResource);
                var currentResValue = ResourceViewController.Instance.GetCurrentResourceValue(eResource);
                if (currentResValue >= cResource.market_amount)
                {
                    UIManager.LogError("FTrade Sell Done:" + cResource.sell_price_to_coalition);
                    var resourceDic = new Dictionary<EResources, float>();
                    resourceDic.Add(eResource, -cResource.market_amount);
                    resourceDic.Add(EResources.Gold, cResource.sell_price_to_coalition);
                    ResourceViewController.Instance.UpdateResource(resourceDic, (isSuccuess, errMsg) =>
                    {
                        if (isSuccuess)
                        {
                            UserViewController.Instance.GetCurrentUser().updateBuy(System.DateTime.Now, cResource.sell_price_to_coalition);
                            update_at = Convert.DateTimeToFDate(day);
                        }
                    });
                }                
            }
        }
        
    }

    public void Buy()
    {
        var startDate = reply_at == "" ? Convert.FDateToDateTime(created_at) : Convert.FDateToDateTime(reply_at).AddDays(1);
        UIManager.LogError("FTrade Buy");
        for (System.DateTime day = startDate.Date; day.Date <= System.DateTime.Now.Date; day = day.AddDays(1))
        {
            if (IsAvailable(day) == true)
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
                            UserViewController.Instance.GetCurrentUser().updateSales(System.DateTime.Now, -cResource.buy_price_from_coalition);
                            RewardSystem.Instance.GivesTradeReward(eResource);
                            reply_at = Convert.DateTimeToFDate(day);
                        }
                    });
                }
            }
        }
    }

    public bool IsAvailable(System.DateTime dateTime)
    {
        System.DateTime begin_date_DT = Convert.FDateToDateTime(created_at);
        int repeatDays = (int)((ETradeRepeat)Enum.Parse(typeof(ETradeRepeat), repeat));
        if (repeatDays == 0)
        {
            return false;
        }

        for (System.DateTime day = begin_date_DT.Date; day.Date <= dateTime.Date; day = day.AddDays(repeatDays))
        {
            if (day.Date == dateTime.Date)
            {
                return true;
            }
        }
        return false;
    }
}
