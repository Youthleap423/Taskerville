using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TradeOfferList: MonoBehaviour
{
    [SerializeField] private GameObject tradeOfferItem;
    [SerializeField] private Transform offer_parent_transform;

    [SerializeField] private TMP_Text titleTF;
    // Start is called before the first frame update

    private List<FTradeInvitation> invitationList = new List<FTradeInvitation>();
    private string requester_villageName = "";

    void Start()
    {
        
    }

    public void SetList(List<FTradeInvitation> list)
    {
        this.invitationList = list;
        this.requester_villageName = list[0].sender_village;
        LoadUI();
    }

    private void LoadUI()
    {
        foreach (Transform child in offer_parent_transform.transform)
        {
            if (child.GetComponent<TradeOfferItem>() != null)
            {
                DestroyImmediate(child.gameObject);                
            }
        }

        foreach (FTradeInvitation invitation in invitationList)
        {
            GameObject obj = Instantiate(tradeOfferItem, offer_parent_transform);
            obj.GetComponent<TradeOfferItem>().SetData(invitation);
        }
        if (invitationList[0].type == ETradeInviteType.Offer.ToString())
        {
            titleTF.text = string.Format("{0} Offers", this.requester_villageName);
        }
        else
        {
            titleTF.text = string.Format("{0} Requests", this.requester_villageName);
        }
        
    }
}
