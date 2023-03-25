using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectItem : EntryItem
{
    [SerializeField] private GameObject linkitemPrefab;
    [SerializeField] private Transform linkTaskGroup;
    [SerializeField] private Transform linkHabitGroup;
    private LProjectEntry projectEntry = null;
    private LAutoGoal autoGoalEntry = null;
    private ProjectGoalsPage parentPage = null;
    private List<LTaskEntry> linkedFTasks = new List<LTaskEntry>();
    private List<LHabitEntry> linkedHabits = new List<LHabitEntry>();
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        sublist_Toggle.onValueChanged.AddListener(delegate
        {
            ShowSubTaskList(sublist_Toggle);
        });

        complete_Toggle.onValueChanged.AddListener(delegate
        {
            OnComplete(complete_Toggle);
        });

        linkTaskGroup.gameObject.SetActive(false);
        linkHabitGroup.gameObject.SetActive(false);
    }

    #region Public Members
    public override void ShowSubTaskList(bool bShow)
    {
        if (UserViewController.Instance.GetCurrentSetting().current_mode == (int)Game_Mode.Game_Only)
        {
            return;
        }

        if (bShow)
        {
            if (projectEntry != null)
            {
                taskNameIF.text = this.projectEntry.taskName + string.Format("\n(Due date: {0})", this.projectEntry.endDate);
                this.subTaskList = TaskViewController.Instance.GetSubTasks(projectEntry);
                CreateSubTasks();
                this.linkedFTasks = TaskViewController.Instance.GetDailyTaskWithLink(projectEntry.id);
                LoadLinkedTask();
                this.linkedHabits = TaskViewController.Instance.GetHabitWithLink(projectEntry.id);
                LoadLinkedHabit();
            }
            else
            {
                CreateSubTasks();
                LoadLinkedTask();
                LoadLinkedHabit();
            }

        }
        else
        {
            taskNameIF.text = this.projectEntry.taskName;
            DeleteSubTasks();
            DeleteLinkedTasks();
            DeleteLinkedHabits();
        }
    }

    public void SetProject(LProjectEntry project, ProjectGoalsPage parentPage)
    {
        this.projectEntry = project;
        this.parentPage = parentPage;
        taskNameIF.text = project.taskName;
        float ratio = UnityEngine.Screen.height / (float)UnityEngine.Screen.width;
        taskNameIF.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1330.0f / ratio, 0f);
        complete_Toggle.isOn = this.projectEntry.isCompleted();
        UpdateCanvasGroup(this.projectEntry.isEnabled());
    }

    public void SetProject(LAutoGoal project, ProjectGoalsPage parentPage)
    {
        this.autoGoalEntry = project;
        this.parentPage = parentPage;
        taskNameIF.text = project.taskName;
        float ratio = UnityEngine.Screen.height / (float)UnityEngine.Screen.width;
        taskNameIF.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1330.0f / ratio, 0f);
        complete_Toggle.isOn = this.autoGoalEntry.isCompleted();
        UpdateCanvasGroup(this.autoGoalEntry.IsAvailable(System.DateTime.Now));
    }

    public void EditProject()
    {
        if (UserViewController.Instance.GetCurrentSetting().current_mode == (int)Game_Mode.Game_Only)
        {
            return;
        }
        transform.GetComponentInParent<NavigationPage>().Show<ProjectEntryPage>("project_entry", this.projectEntry);
    }

    public LProjectEntry GetProject()
    {
        return this.projectEntry;
    }
    #endregion
    #region Private Members

    private void ShowSubTaskList(Toggle toggle)
    {
        ShowSubTaskList(toggle.isOn);
    }

    private void LoadLinkedTask()
    {
        if (this.linkedFTasks.Count > 0)
        {
            this.linkTaskGroup.gameObject.SetActive(true);
            foreach (LTaskEntry fTask in this.linkedFTasks)
            {
                CreateLink(fTask);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent.gameObject.GetComponent<RectTransform>());
        }
    }

    private void LoadLinkedHabit()
    {
        if (this.linkedHabits.Count > 0)
        {
            this.linkHabitGroup.gameObject.SetActive(true);
            foreach (LHabitEntry fTask in this.linkedHabits)
            {
                CreateLink(fTask);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent.gameObject.GetComponent<RectTransform>());
        }
    }

    public void CreateLink(LTaskEntry entry)
    {
        GameObject newObj = GameObject.Instantiate(linkitemPrefab, linkTaskGroup);
        LinkItem item = newObj.GetComponent<LinkItem>();
        item.SetTaskEntry(entry, this.projectEntry);

        
    }

    public void CreateLink(LHabitEntry entry)
    {
        GameObject newObj = GameObject.Instantiate(linkitemPrefab, linkHabitGroup);
        LinkItem item = newObj.GetComponent<LinkItem>();
        item.SetHabitEntry(entry, this.projectEntry);


    }

    protected void DeleteLinkedTasks()
    {
        LinkItem[] items = linkTaskGroup.GetComponentsInChildren<LinkItem>();
        foreach (LinkItem item in items)
        {
            Destroy(item.gameObject);
        }

        RectTransform trans = gameObject.GetComponent<RectTransform>();
        trans.sizeDelta = new Vector2(trans.sizeDelta.x, item_height);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent.gameObject.GetComponent<RectTransform>());
        this.linkTaskGroup.gameObject.SetActive(false);
    }

    protected void DeleteLinkedHabits()
    {
        LinkItem[] items = linkHabitGroup.GetComponentsInChildren<LinkItem>();
        foreach (LinkItem item in items)
        {
            Destroy(item.gameObject);
        }

        RectTransform trans = gameObject.GetComponent<RectTransform>();
        trans.sizeDelta = new Vector2(trans.sizeDelta.x, item_height);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent.gameObject.GetComponent<RectTransform>());
        this.linkHabitGroup.gameObject.SetActive(false);
    }

    private void OnComplete(Toggle toggle)
    {
        if (UserViewController.Instance.GetCurrentSetting().current_mode == (int)Game_Mode.Game_Only)
        {
            return;
        }
        if (toggle.isOn && projectEntry.isEnabled() == true)
        {
            TaskViewController.Instance.OnComplete(projectEntry);
            if (this.parentPage != null)
            {
                parentPage.Reload();
            }
        }
    }
    #endregion


}
