using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CResourceItem
{
    public string itemName = "";
    public float amount = 0.0f;
    public float buyPriceFromRepublic = 0.0f;
    public float sellPriceToRepublic = 0.0f;
    public float buyPriceFromMerchant = 0.0f;
    public float sellPriceToMerchant = 0.0f;
    public float buyPriceFromCoalition = 0.0f;
    public float sellPriceToCoalition = 0.0f;
    public float buyPriceFromSpecialist = 0.0f;

    private List<float> prices = new List<float>();
    public CResourceItem()
    {
        prices.Add(buyPriceFromRepublic);
        prices.Add(sellPriceToRepublic);
        prices.Add(buyPriceFromMerchant);
        prices.Add(sellPriceToMerchant);
        prices.Add(buyPriceFromCoalition);
        prices.Add(sellPriceToCoalition);
        prices.Add(buyPriceFromSpecialist);
    }

    public CResourceItem(string name, float amount, float p1, float p2, float p3, float p4, float p5, float p6, float p7)
    {
        this.itemName = name;
        this.amount = amount;
        this.buyPriceFromRepublic = p1;
        this.sellPriceToRepublic = p2;
        this.buyPriceFromMerchant = p3;
        this.sellPriceToMerchant = p4;
        this.buyPriceFromCoalition = p5;
        this.sellPriceToCoalition = p6;
        this.buyPriceFromSpecialist = p7;

        prices.Clear();
        prices.Add(buyPriceFromRepublic);
        prices.Add(sellPriceToRepublic);
        prices.Add(buyPriceFromMerchant);
        prices.Add(sellPriceToMerchant);
        prices.Add(buyPriceFromCoalition);
        prices.Add(sellPriceToCoalition);
        prices.Add(buyPriceFromSpecialist);
    }

    public float GetValue(EMarketType type)
    {
        return prices[(int)type];
    }

    public string GetName()
    {
        return itemName;
    }

    public float GetAmount()
    {
        return amount;
    }
    
}

