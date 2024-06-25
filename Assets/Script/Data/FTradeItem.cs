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
        Id = trade.id;
        collectionId = "TradeItem";
        Pid = pid;
        type = trade.type;
        resource = trade.sender_res.ToString();
        receiver_Id = receiverId;
        state = EState.Created.ToString();
        created_at = Convert.DateTimeToFDate(dateTime);
    }
}
