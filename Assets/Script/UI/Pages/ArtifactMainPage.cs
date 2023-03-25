using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactMainPage : Page
{
    [SerializeField] private GameObject artExchangeObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        artExchangeObj.SetActive(false);
    }

    public void onMuseum()
    {
        if (ResourceViewController.Instance.IsBuildingBuilt("94"))
        {
            transform.parent.GetComponent<NavPage>().Show("artifact_list");
        }
        else
        {
            UIManager.Instance.ShowErrorDlg("You must construct a Museum first in order to see your Artifacts.");
        }
    }

    public void onGallery()
    {
        if (ResourceViewController.Instance.IsBuildingBuilt("95"))
        { 
            transform.parent.GetComponent<NavPage>().Show("artwork_list");
        }
        else
        {
            UIManager.Instance.ShowErrorDlg("You must construct a Gallery first in order to see your Artwork.");
        }
    }

    public void onArtExchange()
    {
        artExchangeObj.SetActive(true);
    }
}
