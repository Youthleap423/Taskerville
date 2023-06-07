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
            message_TF.text = string.Format("{0} offers: {1}, {2}. In changes for: {3}, {4}\n", artTrade.readers[1], artTrade.painting2, artTrade.artistName2, artTrade.painting1, artTrade.artistName1);
        }
        else
        {
            yes_Button.SetActive(false);
            no_Button.SetActive(false);
            message_TF.text = string.Format("{0} offers: {1}, {2}. In changes for: {3}, {4}\n", artTrade.readers[0], artTrade.painting2, artTrade.artistName2, artTrade.painting1, artTrade.artistName1);
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
        artTrade.Accept();
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.UpdateData(artTrade, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                ArtworkSystem.Instance.Trade(artTrade);
                DataManager.Instance.RemoveData(artTrade, (isSuccess, errMsg) =>
                {
                    parentPage.Back();
                });
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }


    public void Decline()
    {
        artTrade.Decline();
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.RemoveData(artTrade, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {

            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }
}
