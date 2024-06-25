using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TradeItem : MonoBehaviour
{
    [SerializeField] private TMP_Text resNameTF;
    [SerializeField] private TMP_Text repeatTF;
    private FTrade trade = null;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void LoadUI()
    {
        if (this.trade != null)
        {
            resNameTF.text = trade.sender_res.ToString();
            repeatTF.text = trade.repeat.ToString();
        }
    }

    public void SetData(FTrade invitation)
    {
        this.trade = invitation;
        LoadUI();
    }

    public void OnCancel()
    {
        if (this.trade == null)
        {
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);

        TradeViewController.Instance.CancelTrade(this.trade, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            GetComponentInParent<TradePage>().Initialize();
        });
    }
}
