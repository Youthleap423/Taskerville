using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BuyUIItem : MonoBehaviour
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
        index--;
        item1CaptionText.text = dropdown.captionText.text;

        float curGoldValue = ResourceViewController.Instance.GetResourceValue(EResources.Gold, resourceList);
        CResource fResource = marketList[index];

        float marketPrice = fResource.GetMarketPrice(type);

        if (curGoldValue - marketPrice >= 0 && fResource.isMarketable())
        {
            item1AmountText.text = string.Format("{0}", fResource.market_amount);
            item1PriceText.text = string.Format("{0}", marketPrice);
            item1Amount = marketPrice;
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
            dic.Add(key, float.Parse(item1AmountText.text));

            dic.Add(EResources.Gold, -item1Amount);

            UIManager.Instance.ShowLoadingBar(true);
            ResourceViewController.Instance.BuyResource(dic, key, (isSuccess, errMsg) =>
            {
                UIManager.Instance.ShowLoadingBar(false);
                this.Reload();
                if (!isSuccess)
                {
                    UIManager.Instance.ShowErrorDlg(errMsg);
                }
            });
        }

        
    }
}
