using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactListPage : Page
{
    [SerializeField] private GameObject prebBtnObj;
    [SerializeField] private GameObject nextBtnObj;
    [SerializeField] private List<ArtifactItem> itemList;

    [SerializeField] private GameObject detailedPageObj;

    private int currentPageIndex = 0;
    private List<LArtifact> currentArtList = new List<LArtifact>();
    private int currentDetailIndex = 0;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        currentArtList = ArtifactSystem.Instance.GetAllArtifacts().FindAll(item => item.progress >= 1.0f && item.isExchanged == false);
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
        
        for(int i = 0; i < 6; i++)
        {
            int currentArtIndex = currentPageIndex * 6 + i;
            if (currentArtIndex < currentArtList.Count)
            {
                itemList[i].gameObject.SetActive(true);
                itemList[i].SetData(ArtifactSystem.Instance.GetCArtifact(currentArtList[currentArtIndex]), currentArtIndex);
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
        var nIndex = obj.GetComponent<ArtifactItem>().GetIndex();
        detailedPageObj.GetComponent<ArtifactDetailPage>().SetData(currentArtList, nIndex);
        detailedPageObj.SetActive(true);
    }
}
