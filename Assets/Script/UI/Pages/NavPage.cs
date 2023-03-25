using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavPage : Page
{
    [Space]
    [SerializeField] private GameObject[] pages;
    [SerializeField] private string mainPage_ID;
    //[SerializeField] private Page[] tab_pages;

    private GameObject currentPageObj = null;
    private List<string> backStack;

    #region Unity Members
    // Start is called before the first frame update
    void Awake()
    {
        type = "tab";
    }

    private void OnEnable()
    {
        currentPageObj = null;
        backStack = new List<string>();
        Show(mainPage_ID);
    }
    #endregion

    #region Public Members
    override
    public void Initialize()
    {
        base.Initialize();
    }

    public void Show(string pageId, bool bBack = false)
    {
        GameObject pageObj = null;
        for(int index = 0; index < pages.Length; index++)
        {
            var page = pages[index];
            if (page.GetComponent<Page>().Id == pageId)
            {
                pageObj = page;
            }
            page.SetActive(false);
        }

        //show main page
        if (pageObj != null)
        {
            pageObj.SetActive(true);
            if (currentPageObj != null && bBack == false)
            {
                backStack.Add(currentPageObj.GetComponent<Page>().Id);
            }
            currentPageObj = pageObj;
            if (pageId == "setting_credit")
            {
                AudioManager.Instance.PlayBackgroundSound(AudioManager.Instance.creditClip);
            }
        }
    }

    public void Back()
    {
        if (backStack.Count <= 0)
        {
            Debug.LogWarning("There is no page on the back stack to go back to.");
            gameObject.SetActive(false);
            return;
        }

        // Get the page id for the page at the end of the stack (The last shown page)
        string pageId = backStack[backStack.Count - 1];

        // Remove the page from the back stack
        backStack.RemoveAt(backStack.Count - 1);
        // Show the page
        Show(pageId, true);
    }

    #endregion
}
