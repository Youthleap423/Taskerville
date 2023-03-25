using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArtExchangeOfferItem : MonoBehaviour
{
    [SerializeField] private TMP_Text index_TF;
    [SerializeField] private TMP_Text painting1_TF;
    [SerializeField] private TMP_Text artist2_TF;
    
    private FArtTrade _fArtTrade = null;
    private int _index = 0;
    private void Start()
    {
        
    }

    private void Initialize()
    {
        if (_fArtTrade == null)
        {
            return;
        }

        LoadUI();
    }

    private void LoadUI()
    {
        index_TF.text = _index.ToString();
        painting1_TF.text = string.Format("{0},\n{1}", _fArtTrade.painting1, _fArtTrade.artistName1);
        artist2_TF.text = _fArtTrade.artistName2;
    }

    public void SetData(int index, FArtTrade data)
    {
        _fArtTrade = data;
        _index = index;
        Initialize();
    }

    public void Remove()
    {
        GetComponentInParent<ArtExchangeOfferListPage>().RemoveTrade(_fArtTrade);
    }
}
