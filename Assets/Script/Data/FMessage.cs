using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class FMessage : FData
{
    [FirestoreProperty]
    public string receiver_Id { get; set; }

    [FirestoreProperty]
    public string type { get; set; }

    [FirestoreProperty]
    public string content { get; set; }

    [FirestoreProperty]
    public bool isRead { get; set; }

    [FirestoreProperty]
    public List<string> members { get; set; }
    public FMessage()
    {
        receiver_Id = "";
        type = EMessageType.Private.ToString();
        content = "";
        isRead = false;
    }

    public FMessage(LUser sender, LUser receiver, string msg, EMessageType type)
    {
        this.Pid = sender.id;
        this.receiver_Id = receiver.id;
        this.members = new List<string>();
        members.Add(sender.id);
        members.Add(receiver.id);
        this.content = msg;
        this.created_at = Utilities.SystemTicks.ToString();
        this.type = type.ToString();
        this.collectionId = "Messages";
    }

    public FMessage(LUser sender, List<LUser> receiverList, string msg, EMessageType type)
    {
        this.Pid = sender.id;
        this.receiver_Id = sender.joined_coalition;
        this.members = new List<string>();
        members.Add(sender.id);
        foreach(LUser receiver in receiverList)
        {
            if (!members.Contains(receiver.id))
            {
                members.Add(receiver.id);
            }
        }
        
        this.content = msg;
        this.created_at = Utilities.SystemTicks.ToString();
        this.type = type.ToString();
        this.collectionId = "Messages";
    }
}
