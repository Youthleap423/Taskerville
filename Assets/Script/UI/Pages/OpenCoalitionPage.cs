using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCoalitionPage : Page
{
    [Space]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform group_Transform;

    private List<FCoalition> fCoalitions = new List<FCoalition>();

    private void OnEnable()
    {
        Initialize();
    }

    private void LoadUI()
    {
        foreach(Transform child in group_Transform)
        {
            Destroy(child.gameObject);
        }

        Dictionary<string, List<FCoalition>> coalitionGroupDic = new Dictionary<string, List<FCoalition>>();
        foreach(FCoalition coalition in fCoalitions)
        {
            if (!coalition.isOpen)
            {
                continue;
            }

            if (coalitionGroupDic.ContainsKey(coalition.timeZone))
            {
                coalitionGroupDic[coalition.timeZone].Add(coalition);
            }
            else
            {
                coalitionGroupDic[coalition.timeZone] = new List<FCoalition> { coalition };
            }
        }

        foreach(string key in coalitionGroupDic.Keys)
        {
            GameObject groupObj = Instantiate(itemPrefab, group_Transform);
            groupObj.GetComponent<OpenCoalitionItem>().SetData(key);

            foreach(FCoalition coalition in coalitionGroupDic[key])
            {
                GameObject obj = Instantiate(itemPrefab, group_Transform);
                obj.GetComponent<OpenCoalitionItem>().SetData(coalition);
            }
        }
    }

    #region Public Members
    public void Back()
    {
        transform.parent.GetComponent<NavPage>().Back();
    }

    public override void Initialize()
    {
        base.Initialize();

        fCoalitions = CommunicationViewController.Instance.GetCurrentCoalitionList();
        LoadUI();
    }
    #endregion
}
