using Firebase.Firestore;

[FirestoreData]
public class FInvitation : FData
{
    [FirestoreProperty]
    public string receiver_Id { get; set; }

    [FirestoreProperty]
    public string type { get; set; }

    [FirestoreProperty]
    public string reply_at { get; set; }

    [FirestoreProperty]
    public string state { get; set; }

    public LUser sender = null;
    public LUser receiver = null;

    public FInvitation()
    {
        receiver_Id = "";
        type = "";
        state = "";
        reply_at = "";
    }

    public FInvitation(LUser sender, LUser receiver, EInviteType type)
    {
        this.sender = sender;
        this.receiver = receiver;
        collectionId = "Invitation";
        Pid = sender.id;
        reply_at = "";
        receiver_Id = receiver.id;
        this.type = type.ToString();
        state = EState.Created.ToString();
    }

    public bool isOutOfDate()
    {
        var days = reply_at == "" ? Utilities.GetDays(Convert.FDateToDateTime(created_at), System.DateTime.Now) : Utilities.GetDays(Convert.FDateToDateTime(reply_at), System.DateTime.Now);
        return days > 2;
    }
}
