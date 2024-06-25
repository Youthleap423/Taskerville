using System;

[Serializable]
public class FTradeInvitation : Data
{

    public string id = "";
    public string receiver = "";
    public string sender = "";
    public string state = "";
    public string type = "";
    public EResources sender_res = EResources.Ale;
    public EResources receiver_res = EResources.Ale;
    public float sendAmount = 0f;
    public float receiveAmount = 0f;
    public ETradeRepeat repeat = ETradeRepeat.Once;
    public string sender_village = "";
    public string receiver_village = "";
    public string reply_at = "";
    public string created_at = "";

    public FTradeInvitation()
    {

    }
}
