using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class TradeRequestItem : MonoBehaviour
{
    [SerializeField] private Text nameTF;
    [SerializeField] private TMP_Text firstLetterTF;
    [SerializeField] private Text oppo_nameTF;
    [SerializeField] private TMP_Text oppo_firstLetterTF;

    [SerializeField] private Dropdown itemDropDown;
    [SerializeField] private Dropdown repeatDropDown;
    [SerializeField] private Text amountTF;
    [SerializeField] private Text priceTF;

    private LUser me = null;
    private LUser you = null;
    private List<CResource> marketList = new List<CResource>();
    private List<LResource> resourceList = new List<LResource>();
    private EResources marketResource = EResources.Ale;
    private ETradeRepeat repeat = ETradeRepeat.Once;
    private Dictionary<string, ETradeRepeat> dic = new Dictionary<string, ETradeRepeat>();
    float amount1 = 0.0f;
    float amount2 = 0.0f;
    // Start is called before the first frame update

    private void Awake()
    {
        dic.Add("Once", ETradeRepeat.Once);
        dic.Add("Daily", ETradeRepeat.Daily);
        dic.Add("Every 2 Days", ETradeRepeat.Every2Days);
        dic.Add("Weekly", ETradeRepeat.Weekly);
    }

    void Start()
    {
    
        itemDropDown.onValueChanged.AddListener(delegate
        {
            SelectResource(itemDropDown);

        });

        repeatDropDown.onValueChanged.AddListener(delegate
        {
            SelectRepeat(itemDropDown);

        });
    }

    private void UpdateUI()
    {
        nameTF.text = me.Village_Name;
        firstLetterTF.text = me.Village_Name.Substring(0, 1).ToUpper();

        oppo_nameTF.text = you.Village_Name;
        oppo_firstLetterTF.text = you.Village_Name.Substring(0, 1).ToUpper();

        marketList = ResourceViewController.Instance.GetMarketResource(EMarketType.SellToCoalition);
        resourceList = ResourceViewController.Instance.GetUserResource();
        itemDropDown.options.Clear();

        foreach (CResource item in marketList)
        {
            itemDropDown.options.Add(new Dropdown.OptionData(item.name));
        }

        if (itemDropDown.options.Count > 0)
        {
            SelectResource(itemDropDown);
        }

        foreach (string key in dic.Keys)
        {
            repeatDropDown.options.Add(new Dropdown.OptionData(key));
        }

        SelectRepeat(repeatDropDown);
        
    }

    private void SelectResource(Dropdown dropdown)
    {
        SelectResource(dropdown.value);
    }

    private void SelectResource(int index)
    {
        marketResource = marketList[index].type;
        CResource fResource = marketList[index];
        LResource lResource = DataManager.Instance.GetCurrentResources().Find(lRes => lRes.id == fResource.id);
        float marketPrice = fResource.GetMarketPrice(EMarketType.SellToCoalition);
        if (lResource.current_amount > 0 && lResource.current_amount - fResource.market_amount >= 0)
        {
            amountTF.text = string.Format("{0}", fResource.market_amount);
            priceTF.text = string.Format("{0}", marketPrice);
            amount1 = fResource.market_amount;
            amount2 = marketPrice;
            //item1Amount = fResource.market_amount;
        }
        else
        {
            amountTF.text = "";
            priceTF.text = "";
            amount1 = 0f;
            amount2 = 0f;
            //item1Amount = 0;
        }
    }

    private void SelectRepeat(Dropdown dropdown)
    {
        Debug.LogError(dropdown.value + ":" + dropdown.options.Count);
        if (dic.Keys.Count > 0 && dic.Keys.Count >= dropdown.options.Count) {
            this.repeat = dic[dic.Keys.ElementAt(dropdown.value)];
        }
    }

    public void SetData(LUser curUser, LUser oppoUser)
    {
        me = curUser;
        you = oppoUser;
        UpdateUI();
    }

    public void OnRequest()
    {
        if (amount2 == 0.0f)
        {
            UIManager.Instance.ShowErrorDlg("You have not enough resources!");
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        
        InvitationViewController.Instance.SendTradeInvitation(you.id, ETradeInviteType.Request, EResources.Gold, amount2, marketResource, amount1, repeat, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowErrorDlg("Sent the request successfully!");
            }
        });
    }

    public void OnOffer()
    {
        if (amount1 == 0.0f)
        {
            UIManager.Instance.ShowErrorDlg("You have not enough resources!");
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        InvitationViewController.Instance.SendTradeInvitation(you.id, ETradeInviteType.Offer, marketResource, amount1, EResources.Gold, amount2, repeat, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowErrorDlg("Sent the offer successfully!");
            }
        });
    }
}
