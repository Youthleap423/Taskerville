using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectGoalsPage : Page
{
    [SerializeField] private GameObject projectItemPrefab;
    [SerializeField] private GameObject autogoalItemPrefab;
    [SerializeField] private Transform projectListGroup;
    [SerializeField] private GameObject newButton;
    [SerializeField] private GameObject infoGlowEffectObj;

    [Space(10)]
    [SerializeField] private Slot slotComponent;

    #region Unity_Members
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        slotComponent.OnDragDropFinished += Rearrange;
        Initialize();
    }
    private void OnDisable()
    {
        slotComponent.OnDragDropFinished -= Rearrange;
    }
    #endregion

    #region Public Members
    public override void Initialize()
    {
        base.Initialize();

        //PlayerPrefs.SetString("NotifyPage", "");

        infoGlowEffectObj.SetActive(UserViewController.Instance.GetCurrentUser().GetPlayerAgesAsDays() < 7);

        if (UserViewController.Instance.GetCurrentSetting().current_mode == (int)Game_Mode.Game_Only)
        {
            newButton.SetActive(false);
            var projectList = TaskViewController.Instance.GetAutoGoals();
            CreateProjectList(projectList);
        }
        else
        {
            newButton.SetActive(true);
            var projectList = TaskViewController.Instance.GetProjects();
            CreateProjectList(projectList);
        }
        
    }

    public void Reload()
    {
        Initialize();
    }

    public void ShowEntryPage()
    {
        transform.parent.GetComponent<NavigationPage>().Show("project_entry");
    }

    public void Rearrange(bool bFlag)
    {
        List<LProjectEntry> entryList = new List<LProjectEntry>();
        foreach (Transform child in projectListGroup)
        {
            ProjectItem item = child.gameObject.GetComponent<ProjectItem>();
            if (item == null)
            {
                continue;
            }

            LProjectEntry entry = item.GetProject();
            int itemIndex = child.gameObject.GetComponentInChildren<DragHandler>().getIndex();
            entry.orderId = itemIndex;
            entryList.Add(entry);
        }

        TaskViewController.Instance.UpdateEntries(entryList);
    }

    public void ShowInfoPage()
    {
        TutorialManager.Instance.ShowInfo(3);
    }
    #endregion

    #region Private Members
    private void CreateProjectList(List<LProjectEntry> projectList)
    {
        DeleteProjectList();
        projectListGroup.gameObject.SetActive(true);
        for (int index = 0; index < projectList.Count; index++)
        {
            if (projectList[index].isCompleted())
            {
                continue;
            }
            GameObject subItemObj = GameObject.Instantiate(projectItemPrefab, projectListGroup);
            subItemObj.GetComponent<ProjectItem>().SetProject(projectList[index], this);
            subItemObj.GetComponentInChildren<DragHandler>().setIndex(index);
            slotComponent.items.Add(subItemObj);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(projectListGroup.transform.GetComponent<RectTransform>());
    }

    private void CreateProjectList(List<LAutoGoal> projectList)
    {
        DeleteProjectList();
        projectListGroup.gameObject.SetActive(true);
        for (int index = 0; index < projectList.Count; index++)
        {
            //if (projectList[index].IsAvailable(System.DateTime.Now) != true)
            //{
            //    continue;
            //}
            if (projectList[index].isEnabled(System.DateTime.Now) != true)
            {
                continue;
            }
            GameObject subItemObj = GameObject.Instantiate(autogoalItemPrefab, projectListGroup);
            subItemObj.GetComponent<AutoGoalItem>().SetProject(projectList[index], this);
            slotComponent.items.Add(subItemObj);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(projectListGroup.transform.GetComponent<RectTransform>());
    }

    private void DeleteProjectList()
    {
        foreach (Transform child in projectListGroup.transform)
        {
            Destroy(child.gameObject);
        }

        slotComponent.items.Clear();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Initialize();
        }
    }
    #endregion
}
