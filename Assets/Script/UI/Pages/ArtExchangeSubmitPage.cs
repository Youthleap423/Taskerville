using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArtExchangeSubmitPage : Page
{
    [SerializeField] private TMP_Text title_TF;
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
        title_TF.text = ArtworkSystem.Instance.selectedArtist;
        foreach (Transform child in listTransform.transform)
        {
            Destroy(child.gameObject);
        }

        var artTrades = ArtworkSystem.Instance.GetSelectedArtTrades().FindAll(item => item.readers[0] != UserViewController.Instance.GetCurrentUser().GetFullName() && item.state == EState.Posted.ToString());

        foreach (FArtTrade trade in artTrades)
        {
            GameObject obj = GameObject.Instantiate(prefab, listTransform);
            obj.GetComponent<ArtExchangeSubmitItem>().SetData(trade);
        }
    }

    public void Submit(FArtTrade trade, CArtwork artwork)
    {
        trade.Submit(artwork.name, artwork.artist_name);
        trade.readers.Add(UserViewController.Instance.GetCurrentUser().GetFullName());
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.UpdateData(trade, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                //TODO - completed exchange
                LoadUI();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }
}
