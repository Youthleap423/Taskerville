using System;

[Serializable]
public class FTrade : Data
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
    public string updated_at = "";
    public string created_at = "";

    public FTrade()
    {

    }

    public bool IsAvailable(System.DateTime dateTime)
    {
        System.DateTime begin_date_DT = Convert.FDateToDateTime(created_at);
        int repeatDays = (int)repeat;
        if (repeatDays == 0)
        {
            return updated_at == "";
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
