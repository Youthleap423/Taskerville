using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//2023_05_24 created by pooh
public class MarketUIItem : MonoBehaviour
{
    [SerializeField] protected EMarketType type;

    [SerializeField] protected Dropdown itemDropDown;

    [SerializeField] protected Text itemCaptionText;
    [SerializeField] protected Dropdown itemAmountDropDown;
    [SerializeField] protected Text itemPriceText;

    protected List<CResource> marketList = new List<CResource>();
    protected List<LResource> resourceList = new List<LResource>();

    protected List<float> amountList = new List<float>();
    protected List<float> priceList = new List<float>();

    private bool bInitialized = false;

    private void Start()
    {
        OnInitialize();
    }

    private void OnInitialize()
    {
        itemDropDown.onValueChanged.AddListener(delegate
        {
            SelectResource(itemDropDown);
        });

        itemAmountDropDown.onValueChanged.AddListener(delegate
        {
            SelectAmount(itemAmountDropDown);
        });
        bInitialized = true;
    }

    private void OnEnable()
    {
        if (!bInitialized)
        {
            OnInitialize();
        }
        Reload();
    }

    private void SelectResource(Dropdown dropdown)
    {
        int index = dropdown.value;

        if (index == 0)
        {
            ResetItem();
            return;
        }
        index--;
        itemCaptionText.text = dropdown.captionText.text;

        itemAmountDropDown.options.Clear();
        amountList.Clear();
        priceList.Clear();
        for (int i = 1; i <= 5; i++)
        {
            float amount = marketList[index].market_amount * i;
            float price = marketList[index].GetMarketPrice(type) * i;
            amountList.Add(amount);
            priceList.Add(price);
            itemAmountDropDown.options.Add(new Dropdown.OptionData(amount.ToString()));
        }
        itemAmountDropDown.value = 0;
        itemAmountDropDown.RefreshShownValue();
        SelectAmount(itemAmountDropDown);
    }

    private void SelectAmount(Dropdown dropdown)
    {
        itemPriceText.text = priceList[dropdown.value].ToString();
    }

    private void ResetItem()
    {
        itemCaptionText.text = "";
        itemPriceText.text = "";

        itemAmountDropDown.options.Clear();
        amountList.Clear();
        priceList.Clear();
        itemDropDown.value = 0;
        itemDropDown.RefreshShownValue();
        itemAmountDropDown.captionText.text = "";
    }

    protected void Reload()
    {
        marketList = ResourceViewController.Instance.GetMarketResource(type);
        resourceList = ResourceViewController.Instance.GetUserResource();
        itemDropDown.options.Clear();

        itemDropDown.options.Add(new Dropdown.OptionData("None"));
        foreach (CResource item in marketList)
        {
            if (item.isMarketable())
            {
                itemDropDown.options.Add(new Dropdown.OptionData(item.name));
            }
        }

        ResetItem();
    }

    public virtual void DoMarket()
    {

    }
}
