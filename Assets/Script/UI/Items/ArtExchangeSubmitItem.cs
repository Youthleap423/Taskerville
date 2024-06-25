using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ArtExchangeSubmitItem : MonoBehaviour
{
    [SerializeField] private TMP_Text poster_TF;
    [SerializeField] private TMP_Text painting1_TF;
    [SerializeField] private TMP_Text artist2_TF;
    [SerializeField] private TMP_Dropdown painting2_dropdown;

    private FArtTrade _fArtTrade = null;
    private List<CArtwork> _cArtworks = new List<CArtwork>();
    private int _artIndex = -1;
    private void Start()
    {
        painting2_dropdown.onValueChanged.AddListener(delegate
        {
            SelectArtwork(painting2_dropdown);

        });
    }

    private void Initialize()
    {
        if (_fArtTrade == null)
        {
            return;
        }

        _cArtworks = ArtworkSystem.Instance.GetSelectedCArtworks(_fArtTrade.artist2);

        LoadUI();
    }

    private void LoadUI()
    {
        painting1_TF.text = _fArtTrade.paint1;
        poster_TF.text = _fArtTrade.sender;
        artist2_TF.text = _fArtTrade.artist2;

        painting2_dropdown.options.Clear();
        foreach (CArtwork art in _cArtworks)
        {
            var str = string.Format("{0}, {1}", art.name, art.artist_name);
            painting2_dropdown.options.Add(new TMP_Dropdown.OptionData(str));
        }

        if (_cArtworks.Count > 0)
        {
            _artIndex = 0; //default selection of dropbox
        }
    }


    private void SelectArtwork(TMP_Dropdown dropdown)
    {
        _artIndex = dropdown.value;
    }

    public void SetData(FArtTrade data)
    {
        _fArtTrade = data;
        Initialize();
    }

    public void Submit()
    {
        
        transform.GetComponentInParent<ArtExchangeSubmitPage>().Submit(_fArtTrade, _cArtworks[_artIndex]);

    }
}
