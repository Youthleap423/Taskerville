using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TabPage : Page
{
    [Space]
    [SerializeField] protected Toggle[] tab_toggles;
    //[SerializeField] private Page[] tab_pages;

    #region Unity Members
    // Start is called before the first frame update
    void Awake()
    {
        type = "tab";
    }

    protected virtual void OnEnable()
    {
        for(int index = 0; index < tab_toggles.Length; index++)
        {
            Toggle toggle = tab_toggles[index];
            toggle.isOn = index == 0;
            toggle.gameObject.GetComponent<TabItem>().PageObject.SetActive(index == 0);
        }
    }
    #endregion

    #region Public Members
    override
    public void Initialize()
    {
        base.Initialize();

    }

    virtual public void ShowPage(string pageId)
    {
        for (int index = 0; index < tab_toggles.Length; index++)
        {
            Toggle toggle = tab_toggles[index];
            string id = toggle.gameObject.GetComponent<TabItem>().Id;
            if (pageId == id)
            {
                toggle.isOn = true;
                toggle.GetComponentInParent<ToggleGroup>().NotifyToggleOn(toggle);
            }
        }
        Resources.UnloadUnusedAssets();
    }

    virtual public void ChangedTab()
    {

    }
    #endregion

}
