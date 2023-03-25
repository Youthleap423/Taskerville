using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class FTradeInvitation : FInvitation
{

    [FirestoreProperty]
    public string resource { get; set; }

    [FirestoreProperty]
    public string repeat { get; set; }

    [FirestoreProperty]
    public string sender_village { get; set; }

    [FirestoreProperty]
    public string receiver_village { get; set; }
    public FTradeInvitation()
    {
        receiver_Id = "";
        type = "";
        state = "";
        resource = "";
        repeat = "";
    }

    public FTradeInvitation(LUser sender, LUser receiver, EResources res, ETradeRepeat repeat, ETradeInviteType type)
    {
        this.sender = sender;
        this.receiver = receiver;
        collectionId = "Trade_Invitation";
        Pid = sender.id;
        receiver_Id = receiver.id;
        this.type = type.ToString();
        state = EState.Created.ToString();
        resource = res.ToString();
        this.repeat = repeat.ToString();
        sender_village = sender.Village_Name;
        receiver_village = receiver.Village_Name;
    }
}
