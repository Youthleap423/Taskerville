using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CResource: CData
{
    public EResources type = EResources.Ale;
    public float init_amount = 0.0f;
    public EEffect_Type effect_type = EEffect_Type.Happiness;
    public float effect_amount_per_day = 0.0f;
    public float effect_duration = 0.0f;
    public float marketable_count = 0.0f;
    public float market_duration = 0.0f;
    public float purchased_item_keep_hours = 0.0f;
    public float market_amount = 0.0f;
    public float buy_price_from_republic = 0.0f;
    public float sell_price_to_republic = 0.0f;
    public float buy_price_from_merchant = 0.0f;
    public float sell_price_to_merchant = 0.0f;
    public float buy_price_from_coalition = 0.0f;
    public float sell_price_to_coalition = 0.0f;
    public float buy_price_from_specialist = 0.0f;
    public string unit_single = "Bucket";
    public string unit_plural = "Buckets";
    public bool bCleanMode = true;

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

        LResource lResource = DataManager.Instance.GetCurrentResources().Find(lRes => lRes.id == id);
        var purchasedAt = lResource.purchasedAt;
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
}
