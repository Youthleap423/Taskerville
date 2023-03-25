using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationPage : Page
{
    [Space]
    [SerializeField] private GameObject[] pagePrefabs;
    [SerializeField] private string mainPage_ID;
    [SerializeField] private Transform fullScreen_Transform;

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
        backStack = new List<string>();
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Show(mainPage_ID);
    }
    #endregion

    #region Public Members
    override
    public void Initialize()
    {
        base.Initialize();
    }

    public void Show(string pageId)
    {
        Show<EntryPage>(pageId, null);
    }

    public void Show<T>(string pageId, LEntry entry) where T: EntryPage
    {
        GameObject prefabObj = null;
        for (int index = 0; index < pagePrefabs.Length; index++)
        {
            var page = pagePrefabs[index];
            if (page.GetComponent<Page>().Id == pageId)
            {
                prefabObj = page;
                break;
            }
        }

        //show main page
        if (prefabObj != null)
        {
            GameObject pageObj = GameObject.Instantiate(prefabObj, transform);
            if (pageId.Contains("_entry"))
            {
                pageObj.transform.SetParent(fullScreen_Transform);
            }

            RectTransform trans = pageObj.GetComponent<RectTransform>();
            trans.localPosition = Vector3.zero;
            trans.localScale = Vector3.one;
            trans.offsetMax = Vector2.zero; //right-top
            trans.offsetMin = Vector2.zero; //left-bottom

            T component = pageObj.GetComponent<T>();
            if (component != null)
            {
                component.SetEntry(entry);
                component.OnLoadPage(this);
            }
            
            if (currentPageObj != null)
            {
                backStack.Add(currentPageObj.GetComponent<Page>().Id);
                Destroy(currentPageObj);
            }

            currentPageObj = pageObj;
        }
    }

    public void Back()
    {
        if (backStack.Count <= 0)
        {
            Debug.LogWarning("There is no page on the back stack to go back to.");

            return;
        }

        // Get the page id for the page at the end of the stack (The last shown page)
        string pageId = backStack[backStack.Count - 1];

        // Remove the page from the back stack
        backStack.RemoveAt(backStack.Count - 1);

        // Show the page
        Show(pageId);
    }

    #endregion
}
