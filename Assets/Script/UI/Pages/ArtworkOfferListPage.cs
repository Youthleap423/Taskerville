using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtworkOfferListPage : Page
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform listTransform;

    private List<string> _nameList = new List<string>();
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();
        _nameList = ArtworkSystem.Instance.GetAllArtists();

        RefreshUI();
    }

    private void RefreshUI()
    {
        foreach (Transform child in listTransform.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (string artist in _nameList)
        {
            GameObject obj = GameObject.Instantiate(prefab, listTransform);
            obj.name = artist;
            obj.GetComponent<Text>().text = artist;
            var button = obj.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onSelectArtist(artist));
        }
    }

    private void onSelectArtist(string artist)
    {
        ArtworkSystem.Instance.selectedArtist = artist;
        var navPage = transform.parent.GetComponent<NavPage>();
        if (navPage != null)
        {
            transform.parent.GetComponent<NavPage>().Back();
        }
    }

    public void SetData(List<string> list)
    {
        _nameList = list;
        RefreshUI();
    }
}
