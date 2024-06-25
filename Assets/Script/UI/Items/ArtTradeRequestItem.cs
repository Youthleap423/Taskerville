using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtTradeRequestItem : MonoBehaviour
{
    [SerializeField] private Text signText_TF;
    [SerializeField] private Text message_TF;
    [SerializeField] private GameObject yes_Button;
    [SerializeField] private GameObject no_Button;

    private FArtTrade artTrade = null;
    private NotificationPage parentPage = null;

    private void LoadUI()
    {
        if (artTrade.state == EState.Created.ToString())
        {
            yes_Button.SetActive(true);
            no_Button.SetActive(true);
            message_TF.text = string.Format("{0} offers: {1}, {2}. In changes for: {3}, {4}\n", artTrade.receiverName, artTrade.paint2, artTrade.artist2, artTrade.paint1, artTrade.artist1);
        }
        else
        {
            yes_Button.SetActive(false);
            no_Button.SetActive(false);
            message_TF.text = string.Format("{0} offers: {1}, {2}. In changes for: {3}, {4}\n", artTrade.senderName, artTrade.paint2, artTrade.artist2, artTrade.paint1, artTrade.artist1);
        }

        

    }

    public void SetData(FArtTrade trade, NotificationPage page)
    {
        this.parentPage = page;
        artTrade = trade;
        LoadUI();
    }

    public void Accept()
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.AcceptArtTrades(artTrade.id, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                ArtworkSystem.Instance.Trade(artTrade);
                parentPage.Back();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }


    public void Decline()
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.RejectArtTrades(artTrade.id, (isSuccess, errMsg, _) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }
}
