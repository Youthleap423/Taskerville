using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtworkListPage : Page
{
    [SerializeField] private GameObject prebBtnObj;
    [SerializeField] private GameObject nextBtnObj;
    [SerializeField] private List<ArtworkItem> itemList;

    [SerializeField] private GameObject detailedPageObj;

    private int currentPageIndex = 0;
    private List<LArtwork> currentArtList = new List<LArtwork>();
    private int currentDetailIndex = 0;

    void Start()
    {

    }

    private void OnEnable()
    {
        var artList  = ArtworkSystem.Instance.GetAllArtworks();

        //sort by artist name
        var cArtList = new List<CArtwork>();
        foreach(LArtwork lartwork in artList)
        {
            var cArtwork = DataManager.Instance.GetCArtwork(lartwork);
            if (cArtwork != null)
            {
                cArtList.Add(cArtwork);
            }
        }
        cArtList = cArtList.OrderBy(item => item.artist_name).ToList();

        currentArtList.Clear();
        foreach(CArtwork artwork in cArtList)
        {
            var lArtwork = artList.Find(item => item.id == artwork.id);
            if (lArtwork != null)
            {
                currentArtList.Add(lArtwork);
            }
        }

        currentPageIndex = 0;
        ReloadUI();
    }

    private void ReloadUI()
    {
        if (currentPageIndex == 0)
        {
            prebBtnObj.SetActive(false);
        }
        else
        {
            prebBtnObj.SetActive(true);
        }

        if (currentPageIndex >= currentArtList.Count / 6)
        {
            nextBtnObj.SetActive(false);
        }
        else
        {
            nextBtnObj.SetActive(true);
        }

        for (int i = 0; i < 6; i++)
        {
            int currentArtIndex = currentPageIndex * 6 + i;
            if (currentArtIndex < currentArtList.Count)
            {
                itemList[i].gameObject.SetActive(true);
                itemList[i].SetData(ArtworkSystem.Instance.GetCArtwork(currentArtList[currentArtIndex]), currentArtIndex);
            }
            else
            {
                itemList[i].gameObject.SetActive(false);
            }
        }

    }
    public void onPrev()
    {
        currentPageIndex--;
        ReloadUI();
    }

    public void onNext()
    {
        currentPageIndex++;
        ReloadUI();
    }

    public void onBack()
    {
        transform.parent.GetComponent<NavPage>().Back();
    }

    public void OnSelect(GameObject obj)
    {
        var nIndex = obj.GetComponent<ArtworkItem>().GetIndex();
        detailedPageObj.SetActive(true);
        detailedPageObj.GetComponent<ArtworkDetailPage>().SetData(currentArtList, nIndex);
    }
}
