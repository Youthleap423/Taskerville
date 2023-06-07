using System.Collections.Generic;

//2023_05_24 change by pooh

public class BuyUIItem : MarketUIItem
{
    public override void DoMarket()
    {
        var index = itemDropDown.value - 1;
        if (index < 0)
        {
            return;
        }

        float curGoldValue = ResourceViewController.Instance.GetResourceValue(EResources.Gold, resourceList);

        CResource fResource = marketList[index];
        if (curGoldValue - priceList[itemAmountDropDown.value] < 0)
        {
            UIManager.Instance.ShowErrorDlg("More gold needed, Mayor");
            return;
        }

        Dictionary<EResources, float> dic = new Dictionary<EResources, float>();

        EResources key = marketList[itemDropDown.value - 1].type;
        dic.Add(key, amountList[itemAmountDropDown.value]);

        dic.Add(EResources.Gold, -priceList[itemAmountDropDown.value]);

        UIManager.Instance.ShowLoadingBar(true);
        ResourceViewController.Instance.BuyResource(dic, key, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            Reload();
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });

    }
}
