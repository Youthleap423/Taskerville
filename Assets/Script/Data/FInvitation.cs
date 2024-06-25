using System;

[Serializable]
public class FInvitation : Data
{

    public string id = "";
    public string sender = "";
    public string receiver = "";
    public string reply_at = "";
    public string state = "";
    public string coalitionId = "";
    public string type = "";
    public string created_at = "";
    
    public bool isOutOfDate()
    {
        var days = reply_at == "" ? Utilities.GetDays(Convert.FDateToDateTime(created_at), System.DateTime.Now) : Utilities.GetDays(Convert.FDateToDateTime(reply_at), System.DateTime.Now);
        return days > 2;
    }
}
