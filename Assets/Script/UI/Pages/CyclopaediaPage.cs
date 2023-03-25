using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopaediaPage : Page
{
    [SerializeField] private GameObject cyclopaediaObj;
    [SerializeField] private GameObject screenObj;
    [SerializeField] private GameObject prevBtnObj;
    [SerializeField] private GameObject nextBtnObj;

    [Header("Data")]
    [SerializeField] private List<GameObject> infoObjects;
    [SerializeField] private List<GameObject> mapsObjects;
    private int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        LinkHandlerForTMPText.OnClickedOnLinkEvent += LinkHandlerForTMPText_OnClickedOnLinkEvent;
        Hide();
    }

    private void OnDestroy()
    {
        LinkHandlerForTMPText.OnClickedOnLinkEvent -= LinkHandlerForTMPText_OnClickedOnLinkEvent;
    }

    private void LinkHandlerForTMPText_OnClickedOnLinkEvent(string keyword)
    {
        ShowMap(keyword);
    }

    public void ShowInfo(int id)
    {
        currentIndex = id;
        Show();
    }

    public void HideInfo()
    {
        foreach (GameObject obj in infoObjects)
        {
            obj.SetActive(false);
        }
        screenObj.SetActive(false);
    }

    public void Back()
    {
        Hide();
    }

    public void Next()
    {
        currentIndex++;
        Show();
    }

    public void Prev()
    {
        currentIndex--;
        Show();
    }

    private void Show()
    {
        prevBtnObj.SetActive(true);
        nextBtnObj.SetActive(true);

        if (currentIndex == 0)
        {
            prevBtnObj.SetActive(false);
        }

        if (currentIndex == infoObjects.Count - 1)
        {
            nextBtnObj.SetActive(false);
        }
        HideInfo();
        screenObj.SetActive(true);
        infoObjects[currentIndex].SetActive(true);
        cyclopaediaObj.SetActive(true);
        AudioManager.Instance.PlayBackgroundSound(AudioManager.Instance.cyclopaediaClip);
    }

    public void ShowMap()
    {
        HideInfo();
        ShowMap("1");
        
    }

    public void ShowMap(string sID)
    {
        var nID = System.Convert.ToInt32(sID) - 1;
        cyclopaediaObj.SetActive(true);
        foreach(GameObject obj in mapsObjects)
        {
            obj.SetActive(false);
        }
        if (nID >= 0 && nID < mapsObjects.Count)
        {
            mapsObjects[nID].SetActive(true);
        }
        AudioManager.Instance.PlayBackgroundSound(AudioManager.Instance.cyclopaediaClip);
    }

    private void Hide()
    {

        cyclopaediaObj.SetActive(false);
    }

    private void OnDisable()
    {
        AudioManager.Instance.FadeOut();
    }

}
