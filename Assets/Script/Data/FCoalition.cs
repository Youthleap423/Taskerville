
using System.Collections.Generic;

[System.Serializable]
public class FCoalition: CData
{

    public bool isOpen = true;
    public string timeZone = "";
    public List<string> members = new List<string>();
    public string creator = "";

    public FCoalition()
    {
        name = "";
        isOpen = false;
        timeZone = "GMT";
        members = new List<string>();
    }

    public FCoalition(LUser user, string name)
    {
        id = user.id;
        this.name = name;
        creator = user.id; //createor Id
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
