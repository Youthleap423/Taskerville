using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LResource : LData
{
    public float current_amount = 0f;
    public List<string> purchasedAt = new List<string>();
    public void OnPurchased()
    {
        string dateStr = Utilities.GetFormattedDate(0);
        if (!purchasedAt.Contains(dateStr))
        {
            purchasedAt.Add(dateStr);
        }
    }

    public int Effect_Out()
    {
        List<string> removeStrList = new List<string>();
        CResource cResource = ResourceViewController.Instance.GetCResource(id);
        foreach (string dateStr in purchasedAt)
        {
            if (dateStr.CompareTo(Utilities.GetFormattedDate(-(int)cResource.effect_duration)) <= 0)
            {
                removeStrList.Add(dateStr);
                current_amount -= cResource.market_amount;
            }
        }

        foreach (string str in removeStrList)
        {
            purchasedAt.Remove(str);
        }

        return removeStrList.Count;
    }

}
