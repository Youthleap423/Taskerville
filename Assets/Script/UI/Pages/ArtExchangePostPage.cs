using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtExchangePostPage : Page
{
    [SerializeField] private TMP_Dropdown artwork_dropdown;
    [SerializeField] private TMP_Dropdown artist_dropdown;


    private List<LArtwork> _artworks = new List<LArtwork>();
    private List<string> _artists = new List<string>();
    private int _artIndex = -1;
    private int _artistIndex = -1;
    // Start is called before the first frame update
    void Start()
    {
        artwork_dropdown.onValueChanged.AddListener(delegate
        {
            SelectArtwork(artwork_dropdown);

        });

        artist_dropdown.onValueChanged.AddListener(delegate
        {
            SelectArtist(artist_dropdown);

        });
    }

    private void OnEnable()
    {
        Initialize();    
    }


    private void SelectArtwork(TMP_Dropdown dropdown)
    {
        _artIndex = dropdown.value;
    }

    private void SelectArtist(TMP_Dropdown dropdown)
    {
        _artistIndex = dropdown.value;
    }


    public override void Initialize()
    {
        base.Initialize();

        UIManager.Instance.ShowLoadingBar(true);
        ArtworkSystem.Instance.LoadArtTrades((isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });

        _artworks = ArtworkSystem.Instance.GetAllArtworks();
        artwork_dropdown.options.Clear();
        artwork_dropdown.options.Add(new TMP_Dropdown.OptionData(""));
        foreach (LArtwork art in _artworks)
        {
            var cArt = ArtworkSystem.Instance.GetCArtwork(art);
            var str = string.Format("{0},\n{1}", cArt.name, cArt.artist_name);
            artwork_dropdown.options.Add(new TMP_Dropdown.OptionData(str));
        }

        artist_dropdown.options.Clear();
        _artists = ArtworkSystem.Instance.GetAllArtists();
        artist_dropdown.options.Add(new TMP_Dropdown.OptionData(""));
        foreach (string artName in _artists)
        {
            artist_dropdown.options.Add(new TMP_Dropdown.OptionData(artName));
        }

        _artIndex = 0;
        _artistIndex = 0;
    }

    public void GoNexus()
    {
        transform.parent.GetComponent<NavPage>().Show("artist_select");
    }

    public void GoMuseum()
    {

    }

    public void GoOffer()
    {
        transform.parent.GetComponent<NavPage>().Show("art_exchange_offer_list");
    }

    public void PostNexus()
    {
        if (_artIndex == 0)
        {
            UIManager.Instance.ShowErrorDlg("You need to select artist's work");
            return;
        }

        
        if (_artistIndex == 0)
        {
            UIManager.Instance.ShowErrorDlg("You need to select artist");
            return;
        }
        var selectedArtwork = _artworks[_artIndex - 1];
        var selectedArtist = _artists[_artistIndex - 1];
        UIManager.Instance.ShowLoadingBar(true);
        ArtworkSystem.Instance.PostToNexus(selectedArtwork, selectedArtist, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                GoNexus();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }
}
