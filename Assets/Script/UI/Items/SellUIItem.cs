using System.Collections.Generic;

//2023_05_24 change by pooh

public class SellUIItem : MarketUIItem
{
    public override void DoMarket()
    {
        var index = itemDropDown.value - 1;
        if (index < 0)
        {
            return;
        }

        CResource fResource = marketList[index];
        EResources key = fResource.type;
        float curValue = ResourceViewController.Instance.GetResourceValue(key, resourceList);

        if (curValue - amountList[itemAmountDropDown.value] < 0)
        {
            UIManager.Instance.ShowErrorDlg("More supplies needed, Mayor");
            return;
        }

        Dictionary<EResources, float> dic = new Dictionary<EResources, float>();

        dic.Add(key, -amountList[itemAmountDropDown.value]);
        dic.Add(EResources.Gold, priceList[itemAmountDropDown.value]);

        UIManager.Instance.ShowLoadingBar(true);
        ResourceViewController.Instance.BuyResource(dic, key, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                if (type == EMarketType.SellToRepublic)
                {
                    if (DataManager.Instance.exportResources.Contains(key))
                    {
                        UserViewController.Instance.AddExport(key, amountList[itemAmountDropDown.value]);
                    }
                }
            }
            Reload();
        });
    }
}
