using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TradeListItem : MonoBehaviour
{
    [SerializeField] private GameObject tradeItem;
    [SerializeField] private Transform parent_transform;

    [SerializeField] private TMP_Text titleTF;
    // Start is called before the first frame update

    private List<FTrade> tradeList = new List<FTrade>();
    private string requester_villageName = "";

    void Start()
    {

    }

    public void SetList(List<FTrade> list)
    {
        this.tradeList = list;
        var trade = list[0];
        if (trade.type == ETradeInviteType.Offer.ToString())
        {
            if (trade.sender == UserViewController.Instance.GetCurrentUser().id)
            {
                this.requester_villageName = string.Format("To: {0}", trade.receiver_village);
            }
            else
            {
                this.requester_villageName = string.Format("From: {0}", trade.sender_village);
            }
        }
        else
        {
            if (trade.sender == UserViewController.Instance.GetCurrentUser().id)
            {
                this.requester_villageName = string.Format("From: {0}", trade.receiver_village);
            }
            else
            {
                this.requester_villageName = string.Format("To: {0}", trade.sender_village);
            }
        }
        
        LoadUI();
    }

    private void LoadUI()
    {
        titleTF.text = this.requester_villageName;
        foreach (Transform child in parent_transform.transform)
        {
            if (child.GetComponent<TradeItem>() != null)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        foreach (FTrade trade in tradeList)
        {
            GameObject obj = Instantiate(tradeItem, parent_transform);
            obj.GetComponent<TradeItem>().SetData(trade);
        }
    }
}
