using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

[FirestoreData]
public class FResource: FData
{
    [FirestoreProperty]
    public string name { get; set; }

    [FirestoreProperty]
    public string type { get; set; }

    [FirestoreProperty]
    public float init_amount { get; set; }

    [FirestoreProperty]
    public float current_amount { get; set; }

    [FirestoreProperty]
    public string effect_type { get; set; }

    [FirestoreProperty]
    public float effect_amount_per_day { get; set; }

    [FirestoreProperty]
    public float effect_duration { get; set; }

    [FirestoreProperty]
    public float marketable_count { get; set; }

    [FirestoreProperty]
    public float market_duration { get; set; }

    [FirestoreProperty]
    public float purchased_item_keep_hours { get; set; }

    [FirestoreProperty]
    public List<string> purchasedAt { get; set; }

    [FirestoreProperty]
    public float market_amount { get; set; }

    [FirestoreProperty]
    public float buy_price_from_republic { get; set; }

    [FirestoreProperty]
    public float sell_price_to_republic { get; set; }

    [FirestoreProperty]
    public float buy_price_from_merchant { get; set; }

    [FirestoreProperty]
    public float sell_price_to_merchant { get; set; }

    [FirestoreProperty]
    public float buy_price_from_coalition { get; set; }

    [FirestoreProperty]
    public float sell_price_to_coalition { get; set; }

    [FirestoreProperty]
    public float buy_price_from_specialist { get; set; }

    public FResource()
    {
        type = "";
        init_amount = 0.0f;
        current_amount = 0.0f;
        effect_type = EResources.Happiness.ToString();
        effect_amount_per_day = 0.0f;
        effect_duration = 0;
        marketable_count = 0;
        market_duration = 0;
        purchased_item_keep_hours = 0;
        purchasedAt = new List<string>();
        market_amount = 0;
        buy_price_from_republic = 0;
        sell_price_to_republic = 0;
        buy_price_from_merchant = 0;
        sell_price_to_merchant = 0;
        buy_price_from_coalition = 0;
        sell_price_to_coalition = 0;
        buy_price_from_specialist = 0;
        name = "";
    }

    public float GetMarketPrice(EMarketType type)
    {
        float result = 0;
        switch (type)
        {
            case EMarketType.BuyFromRepublic:
                result = buy_price_from_republic;
                break;
            case EMarketType.BuyFromMerchant:
                result = buy_price_from_merchant;
                break;
            case EMarketType.BuytFromCoalition:
                result = buy_price_from_coalition;
                break;
            case EMarketType.BuyFromSpecialist:
                result = buy_price_from_specialist;
                break;
            case EMarketType.SellToRepublic:
                result = sell_price_to_republic;
                break;
            case EMarketType.SellToMerchant:
                result = sell_price_to_merchant;
                break;
            case EMarketType.SellToCoalition:
                result = sell_price_to_coalition;
                break;
            default:
                break;
        }

        return result;
    }

    public bool isMarketable()
    {
        if (marketable_count == 0)
        {
            return true;
        }

        int purchasedCountInDuration = 0;
        for (System.DateTime day = System.DateTime.Now.AddDays(-market_duration + 1).Date; day.Date <= System.DateTime.Now; day = day.AddDays(1))
        {
            if (purchasedAt.Contains(Convert.DateTimeToFDate(day)))
            {
                purchasedCountInDuration++;
            }
        }

        return purchasedCountInDuration < marketable_count;
    }

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
        foreach(string dateStr in purchasedAt)
        {
            if (dateStr.CompareTo(Utilities.GetFormattedDate(-(int)effect_duration)) <= 0)
            {
                removeStrList.Add(dateStr);
                current_amount -= market_amount;
            }
        }

        foreach(string str in removeStrList)
        {
            purchasedAt.Remove(str);
        }

        return removeStrList.Count;
    }
}
