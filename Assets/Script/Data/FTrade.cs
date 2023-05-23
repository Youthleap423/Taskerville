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

    public void Sell(string sellerId, string buyerId)
    {
        var startDate = update_at == "" ? Convert.FDateToDateTime(created_at) : Convert.FDateToDateTime(update_at).AddDays(1);
        List<FTradeItem> itemList = new List<FTradeItem>();
        var allResourceDic = new Dictionary<EResources, float>();
        int nCount = 0;
        for (System.DateTime day = startDate.Date; day.Date <= System.DateTime.Now.Date; day = day.AddDays(1))
        {
            if (repeat == ETradeRepeat.Once.ToString() && nCount > 0)
            {
                break;
            }
            if (IsAvailable(day) == true)
            {
                EResources eResource = (EResources)Enum.Parse(typeof(EResources), resource);
                CResource cResource = ResourceViewController.Instance.GetCResource(eResource);
                var currentResValue = ResourceViewController.Instance.GetCurrentResourceValue(eResource);
                if (currentResValue >= cResource.market_amount)
                {
                    var resourceDic = new Dictionary<EResources, float>();
                    resourceDic.Add(eResource, -cResource.market_amount);
                    allResourceDic.Add(eResource, cResource.market_amount);
                    FTradeItem item = new FTradeItem(this, sellerId, buyerId, day);
                    itemList.Add(item);
                    ResourceViewController.Instance.UpdateResource(resourceDic);
                    UserViewController.Instance.GetCurrentUser().updateSales(System.DateTime.Now, cResource.sell_price_to_coalition);
                    nCount++;
                }                
            }
        }

        
        DataManager.Instance.CreateTradeItem(itemList, (isSuccess, errMsg) =>
        {
            if (repeat == ETradeRepeat.Once.ToString())
            {
                DataManager.Instance.RemoveData(this, null);
            }
            if (!isSuccess)
            {
                ResourceViewController.Instance.UpdateResource(allResourceDic);
            }
        });
        
    }

    public bool IsAvailable(System.DateTime dateTime)
    {
        System.DateTime begin_date_DT = Convert.FDateToDateTime(created_at);
        int repeatDays = (int)((ETradeRepeat)Enum.Parse(typeof(ETradeRepeat), repeat));
        if (repeatDays == 0)
        {
            return update_at == "";
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
