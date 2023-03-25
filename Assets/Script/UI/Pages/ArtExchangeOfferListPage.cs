using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtExchangeOfferListPage : Page
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform listTransform;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        LoadUI();
    }

    private void LoadUI()
    {
        foreach (Transform child in listTransform.transform)
        {
            Destroy(child.gameObject);
        }

        var artTrades = ArtworkSystem.Instance.GetAllArtTrades().FindAll(item => item.Pid == UserViewController.Instance.GetCurrentUser().id && item.state == EState.Posted.ToString());

        int index = 0;
        foreach (FArtTrade trade in artTrades)
        {
            index++;
            GameObject obj = GameObject.Instantiate(prefab, listTransform);
            obj.GetComponent<ArtExchangeOfferItem>().SetData(index, trade);
        }
    }

    public void RemoveTrade(FArtTrade trade)
    {
        UIManager.Instance.ShowLoadingBar(true);
        ArtworkSystem.Instance.RemoveArtTrade(trade, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                LoadUI();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg("Failed to remove the offer. Try again later.");
            }
        });
    }
}
