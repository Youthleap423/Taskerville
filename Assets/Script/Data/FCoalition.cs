using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class FCoalition : FData
{
    [FirestoreProperty]
    public string name { get; set; }

    [FirestoreProperty]
    public bool isOpen { get; set; }

    [FirestoreProperty]
    public string timeZone { get; set; }

    [FirestoreProperty]
    public List<string> members { get; set; }

    public FCoalition()
    {
        name = "";
        isOpen = false;
        timeZone = "GMT";
        members = new List<string>();
    }

    public FCoalition(LUser user, string name)
    {
        Id = user.id;
        this.name = name;
        Pid = user.id; //createor Id
        collectionId = "Coalition";
        isOpen = true;
        members = new List<string>();
        members.Add(user.id);
        timeZone = Convert.GetSTZ(System.TimeZoneInfo.Local);
    }

    public void RemoveMember(string id, System.Action<bool, string> callback)
    {
        members.Remove(id);
        DataManager.Instance.UpdateCoalition(this, callback);
    }

    public void AddMember(string id, System.Action<bool, string> callback)
    {
        if (!members.Contains(id))
        {
            members.Add(id);
            DataManager.Instance.UpdateCoalition(this, callback);
        }
        else
        {
            callback(true, "");
        }
    }

    public bool isMemberOf(string id)
    {
        if (id == "")
        {
            return false;
        }

        return members.Contains(id);
    }
}
