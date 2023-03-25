using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : SingletonComponent<PageManager>
{
    [SerializeField] private string mainPageId = "main";
    [SerializeField] private List<Page> pages = null;

	[Space]
	[SerializeField] private GameObject oneline_DivideBar;
	[SerializeField] private GameObject doubleline_DivideBar;

	private Page currentPage;

	// Page id back stack
	private List<string> backStack;

	public string MainPageId { get { return mainPageId; } }
    public string CurrentPageId { get { return currentPage == null ? "" : currentPage.Id; } }

    #region Unity Members
    private void Start()
	{
		backStack = new List<string>();
		// Initialize and hide all the pages
		for (int i = 0; i < pages.Count; i++)
		{
			Page page = pages[i];
			page.Initialize();
			page.gameObject.SetActive(false);
		}

		Show(mainPageId);
	}
    #endregion

    #region Public Members
    public void Home()
	{
		if (CurrentPageId == mainPageId)
		{
			return;
		}

		Show(mainPageId);
	}

	public void Show(string pageId)
    {
		Page page = GetPageById(pageId);
		if (page == null)
		{
			UIManager.LogError("Could not find page with the given pageId: " + pageId);
			return;
		}
		if (currentPage != null)
        {
			if (currentPage.Id != pageId)
            {
				backStack.Add(currentPage.Id);
				currentPage.gameObject.SetActive(false);
				Show(page);
			}
        }
        else
        {
			Show(page);
		}
    }

	public void Show(Page page)
    {
		if (page.Type == "tab")
        {
			oneline_DivideBar.SetActive(false);
			doubleline_DivideBar.SetActive(true);
        }
        else
        {
			oneline_DivideBar.SetActive(true);
			doubleline_DivideBar.SetActive(false);
		}

		page.gameObject.SetActive(true);
		currentPage = page;
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

	public void BackTo(string pageId)
	{
		for (int i = backStack.Count - 1; i >= 0; i--)
		{
			if (pageId == backStack[i])
			{
				Back();
				return;
			}
			else
			{
				backStack.RemoveAt(i);
			}
		}

		// If we get here then the page was not found to just go to home
		Home();
	}

	#endregion

	#region Private Members
	private Page GetPageById(string id)
	{
		for (int i = 0; i < pages.Count; i++)
		{
			if (id == pages[i].Id)
			{
				return pages[i];
			}
		}

		UIManager.LogError("No Page exists with the id " + id);

		return null;
	}

	private void ClearBackStack()
	{
		backStack.Clear();
	}
	#endregion
}
