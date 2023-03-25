using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class SellUIItem : MonoBehaviour
{
    [SerializeField] private EMarketType type;

    [SerializeField] private Dropdown item1DropDown;
    
    [SerializeField] private Text item1CaptionText;
    [SerializeField] private Text item1AmountText;
    [SerializeField] private Text item1PriceText;

    private List<CResource> marketList = new List<CResource>();
    private List<LResource> resourceList = new List<LResource>();

    private float item1Amount = 0;

    private void Start()
    {
        item1DropDown.onValueChanged.AddListener(delegate
        {
            SelectResource1(item1DropDown);

        });
    }

    private void OnEnable()
    {
        Reload();
    }

    private void SelectResource1(Dropdown dropdown)
    {
        int index = dropdown.value;

        if (index == 0)
        {
            ResetItem1();
            return;
        }

        index = index - 1;
        item1CaptionText.text = dropdown.captionText.text;

        CResource fResource = marketList[index];
        LResource lResource = DataManager.Instance.GetCurrentResources().Find(lRes => lRes.id == fResource.id);
        float marketPrice = fResource.GetMarketPrice(type);
        if (lResource.current_amount > 0 && lResource.current_amount - fResource.market_amount >= 0)
        {
            item1AmountText.text = string.Format("{0}", fResource.market_amount);
            item1PriceText.text = string.Format("{0}", marketPrice);
            item1Amount = fResource.market_amount;
        }
        else
        {
            item1AmountText.text = "";
            item1PriceText.text = "";
            item1Amount = 0;
        }
    }



    private void ResetItem1()
    {
        item1Amount = 0;
        item1CaptionText.text = "";
        item1AmountText.text = "";
        item1PriceText.text = "";

        item1DropDown.value = 0;
        item1DropDown.RefreshShownValue();
    }

    
    private void Reload()
    {
        marketList = ResourceViewController.Instance.GetMarketResource(type);
        resourceList = ResourceViewController.Instance.GetUserResource();
        item1DropDown.options.Clear();

        item1DropDown.options.Add(new Dropdown.OptionData("None"));
        foreach (CResource item in marketList)
        {
            item1DropDown.options.Add(new Dropdown.OptionData(item.name));
        }
        
        ResetItem1();
    }

    public void DoMarket()
    {
        Dictionary<EResources, float> dic = new Dictionary<EResources, float>();

        if (item1Amount > 0)
        {
            EResources key = marketList[item1DropDown.value - 1].type;
            dic.Add(key, -float.Parse(item1AmountText.text));

            float price = 0;
            if (!item1PriceText.text.Equals(""))
            {
                price += float.Parse(item1PriceText.text);
            }

            dic.Add(EResources.Gold, price);

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
                            UserViewController.Instance.AddExport(key, float.Parse(item1AmountText.text));
                        }
                    }
                }
                this.Reload();
            });
        }
    }
}
