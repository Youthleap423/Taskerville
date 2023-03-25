using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SA.iOS.UIKit;
using SA.Android.App;
using TMPro;

public class ProjectEntryPage : EntryPage
{
    [Space]
    [SerializeField] private TMP_InputField taskNameIF;
    [SerializeField] private GameObject advancedGroupObj;
    [SerializeField] private GameObject checkListObj;
    [SerializeField] private GameObject taskListPanelObj;
    [SerializeField] private GameObject habitListPanelObj;
    [SerializeField] private RectTransform subtask_scrollView_RectTransform;
    [SerializeField] private RectTransform linktask_scrollView_RectTransform;
    [SerializeField] private RectTransform linkhabit_scrollView_RectTransform;


    [Header("Date")]
    [SerializeField] private Text beginDateTF;
    [SerializeField] private Text endDateTF;

    [Header("SubTask")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform subTaskGroup;

    [Header("Link")]
    [SerializeField] private GameObject linkitemPrefab;
    [SerializeField] private Transform linkTaskGroup;
    [SerializeField] private Transform linkHabitGroup;

    [Header("Alarm")]
    [SerializeField] private Text alarmTimeTF;
    [SerializeField] private TMP_Dropdown alarmRepeat_dropdown;

    private List<LinkItem> linkTaskList = new List<LinkItem>();
    private List<LinkItem> linkHabitList = new List<LinkItem>();
    private LProjectEntry newProjectEntry = null;
    private List<LSubTask> subFTasks = new List<LSubTask>();
    private List<LTaskEntry> linkedFTasks = new List<LTaskEntry>();
    private List<LHabitEntry> linkedHabits = new List<LHabitEntry>();
    private Dictionary<string, LSubTask> taskDic = new Dictionary<string, LSubTask>();
    private Dictionary<string, LTaskEntry> entryDic = new Dictionary<string, LTaskEntry>();
    private Dictionary<string, LHabitEntry> habitEntryDic = new Dictionary<string, LHabitEntry>();

    private NavigationPage navPage = null;

    #region Unity Members
    // Start is called before the first frame update
    void Awake()
    {
        newProjectEntry = new LProjectEntry();
        newProjectEntry.id = string.Format("{0}", Utilities.SystemTimeInMillisecondsString);

        Initialize(); 
    }
    #endregion


    #region Public Memebers
    override public void Initialize()
    {
        base.Initialize();

        checkListObj.SetActive(false);
        taskListPanelObj.SetActive(false);
        habitListPanelObj.SetActive(false);

        taskNameIF.text = "";
        beginDateTF.text = System.DateTime.Now.ToString("MMMM d, yyyy");
        endDateTF.text = System.DateTime.Now.AddDays(1.0).ToString("MMMM d, yyyy");
        alarmTimeTF.text = "";

        foreach (Transform child in subTaskGroup.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (LinkItem item in linkTaskList)
        {
            Destroy(item.gameObject);
        }

        linkTaskList.Clear();

        foreach (LinkItem item in linkHabitList)
        {
            Destroy(item.gameObject);
        }

        linkHabitList.Clear();


        if (advancedGroupObj != null)
        {
            advancedGroupObj.SetActive(false);
        }

        subFTasks.Clear();
        linkedFTasks.Clear();
        linkedHabits.Clear();
        taskDic.Clear();
    }

    public override void OnLoadPage(NavigationPage page)
    {
        base.OnLoadPage(page);

        this.navPage = page;

        if (this.entry == null)
        {
            return;
        }

        var projectEntry = TaskViewController.Instance.GetProjectEntry(this.entry.id);
        if (projectEntry != null)
        {
            this.newProjectEntry = projectEntry;
            LoadProjectEntry();
            this.subFTasks = TaskViewController.Instance.GetSubTasks(projectEntry);
            LoadSubTasks();
            this.linkedFTasks = TaskViewController.Instance.GetDailyTaskWithLink(projectEntry.id);
            LoadLinkedTask();
            this.linkedHabits = TaskViewController.Instance.GetHabitWithLink(projectEntry.id);
            LoadLinkedHabit();

        }
    }

    public override void SetEntry(LEntry entry)
    {
        this.entry = entry;
    }

    public override void DeleteSubTask(LSubTask task)
    {
        base.DeleteSubTask(task);

        if (taskDic.ContainsKey(task.id))
        {
            taskDic[task.id] = task;
        }
    }

    public override void DeleteLinkTask(LTaskEntry task)
    {
        base.DeleteLinkTask(task);

        if (entryDic.ContainsKey(task.id))
        {
            entryDic[task.id] = task;
        }

        task.linkedGoalId = "";
        DataManager.Instance.UpdateEntry(task, new List<LSubTask>());
    }

    public override void DeleteLinkHabit(LHabitEntry habit)
    {
        base.DeleteLinkHabit(habit);

        if (habitEntryDic.ContainsKey(habit.id))
        {
            habitEntryDic[habit.id] = habit;
        }

        habit.linkedGoalId = "";
        DataManager.Instance.UpdateEntry(habit);
    }

    public override void UpdateSubTask(LSubTask task)
    {
        base.UpdateSubTask(task);
        if (taskDic.ContainsKey(task.id))
        {
            taskDic[task.id] = task;
        }
    }

    public void DeleteProjectEntry()
    {
        CreateProjectEntry(true);
    }

    public void CreateSubTask()
    {
        if (taskDic.Count >= 8)
        {
            return;
        }

        LSubTask newTask = new LSubTask();
        newTask.id = string.Format("{0}", Utilities.SystemTimeInMillisecondsString);
        newTask.type = ESubEntryType.SubProject.ToString();
        newTask.goldCount = 10;
        CreateSubTask(newTask);
    }

    public void CreateProjectEntry(bool isRemove = false)
    {
        if (!isRemove)
        {
            newProjectEntry.taskName = taskNameIF.text;
            newProjectEntry.beginDate = beginDateTF.text;
            newProjectEntry.endDate = endDateTF.text;
            newProjectEntry.goldCount = 50;
            newProjectEntry.orderId = Utilities.SystemTicks;
            newProjectEntry.remindAlarm = alarmTimeTF.text;

            this.subFTasks = taskDic.Values.ToList();

            newProjectEntry.subProjects.Clear();

            foreach (LTask task in this.subFTasks)
            {
                if (task.IsRemoved() == false)
                {
                    newProjectEntry.subProjects.Add(task.id);
                }
            }
            newProjectEntry.repeat_alarm = alarmRepeat_dropdown.value;
            newProjectEntry.linkedTasks.Clear();
            this.linkedFTasks = entryDic.Values.ToList();
            foreach (LTaskEntry task in this.linkedFTasks)
            {
                if (task.IsRemoved() == false)
                {
                    newProjectEntry.linkedTasks.Add(task.id);
                }
            }

            newProjectEntry.linkedTasks.Clear();
            this.linkedHabits = habitEntryDic.Values.ToList();
            foreach (LHabitEntry task in this.linkedHabits)
            {
                if (task.IsRemoved() == false)
                {
                    newProjectEntry.linkedHabits.Add(task.id);
                }
            }
        }
        else
        {
            newProjectEntry.SetRemoved(true);

            this.subFTasks = taskDic.Values.ToList();
            newProjectEntry.subProjects.Clear();
            foreach (LTask task in this.subFTasks)
            {
                task.SetRemoved(true);
            }

            newProjectEntry.linkedTasks.Clear();
            this.linkedFTasks = entryDic.Values.ToList();
            foreach (LTaskEntry task in this.linkedFTasks)
            {
                task.linkedGoalId = "";
                DataManager.Instance.UpdateEntry(task, new List<LSubTask>());
            }

            this.linkedHabits = habitEntryDic.Values.ToList();
            foreach (LHabitEntry task in this.linkedHabits)
            {
                task.linkedGoalId = "";
                DataManager.Instance.UpdateEntry(task);
            }
        }

        newProjectEntry.Update(this.subFTasks, linkedFTasks);
        ShowProjectGoal();
    }

    public void ShowTaskListPanel()
    {
        taskListPanelObj.SetActive(true);
    }

    public void ShowHabitListPanel()
    {
        habitListPanelObj.SetActive(true);
    }

    public void SelectedTask(LTaskEntry entry)
    {
        taskListPanelObj.SetActive(false);

        entry.linkedGoalId = newProjectEntry.id;

        /*
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("linkedGoalId", entry.linkedGoalId);

        UIManager.Instance.ShowLoadingBar(true);
        FirestoreManager.Instance.UpdateData(EntryType.DailyTask.ToString(), entry.Id, dic, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
        */
        CreateLink(entry);
    }

    public void SelectedTask(LHabitEntry entry)
    {
        habitListPanelObj.SetActive(false);

        entry.linkedGoalId = newProjectEntry.id;

        CreateLink(entry);
    }


    public void ShowAdvanced()
    {
        if (advancedGroupObj != null)
        {
            advancedGroupObj.SetActive(true);
        }
        AdjustSubTaskScrollViewRect();
        AdjustLinkTaskScrollViewRect();
    }

    public void ShowProjectGoal()
    {
        if (this.navPage != null)
        {
            this.navPage.Back();
        }
        //transform.parent.GetComponent<NavigationPage>().Back();
    }

    public void ShowBeginDateDlg()
    {
#if UNITY_ANDROID
        System.DateTime dateTime = System.DateTime.Now;
        AN_DatePickerDialog picker = new AN_DatePickerDialog(dateTime.Year, dateTime.Month - 1, dateTime.Day);
        picker.Show((result) =>
        {
            if (result.IsSucceeded)
            {
                beginDateTF.text = Convert.DateTimeToEntryDate(result.Year, result.Month, result.Day);
                newProjectEntry.endDate = beginDateTF.text;
            }
            else
            {
                beginDateTF.text = "";
            }
        });
#else
        ISN_UIDateTimePicker datePicker = new ISN_UIDateTimePicker();
        datePicker.DatePickerMode = ISN_UIDateTimePickerMode.Date;

        datePicker.Show((dateTime) =>
        {
            beginDateTF.text = dateTime.ToString("MMMM d, yyyy");
            newProjectEntry.beginDate = beginDateTF.text;
        });
#endif
    }

    public void ShowEndDateDlg()
    {
#if UNITY_ANDROID
        System.DateTime dateTime = System.DateTime.Now;
        AN_DatePickerDialog picker = new AN_DatePickerDialog(dateTime.Year, dateTime.Month - 1, dateTime.Day);
        picker.Show((result) =>
        {
            if (result.IsSucceeded)
            {
                endDateTF.text = Convert.DateTimeToEntryDate(result.Year, result.Month, result.Day);
                newProjectEntry.endDate = endDateTF.text;
            }
            else
            {
                endDateTF.text = "";
            }
        });
#else
        ISN_UIDateTimePicker datePicker = new ISN_UIDateTimePicker();
        datePicker.DatePickerMode = ISN_UIDateTimePickerMode.Date;

        datePicker.Show((dateTime) =>
        {
            endDateTF.text = dateTime.ToString("MMMM d, yyyy");
            newProjectEntry.endDate = endDateTF.text;
        });
#endif
    }

    public void ShowAlarmDlg()
    {
#if UNITY_ANDROID
        string style = "android:Theme.DeviceDefault.Light.Dialog.Alert";
        AndroidPlugin.ShowTimePickerDialog(
                alarmTimeTF.text,
                "hh:mm:aa",
                gameObject.name,
                "ReceiveResult",
                style);
#else 
        ISN_UIDateTimePicker timePicker = new ISN_UIDateTimePicker();
        timePicker.DatePickerMode = ISN_UIDateTimePickerMode.Time;

        timePicker.Show((dateTime) =>
        {
            alarmTimeTF.text = dateTime.ToString("hh:mm tt");
        });
#endif
    }

    public void RemoveAlarm()
    {
        alarmTimeTF.text = "";
        alarmRepeat_dropdown.value = 0;
        alarmRepeat_dropdown.RefreshShownValue();
    }
    #endregion


    #region Private Members
    private void LoadProjectEntry()
    {
        taskNameIF.text = this.newProjectEntry.taskName;
        beginDateTF.text = this.newProjectEntry.beginDate;
        endDateTF.text = this.newProjectEntry.endDate;
        alarmTimeTF.text = this.newProjectEntry.remindAlarm;
        alarmRepeat_dropdown.value = this.newProjectEntry.repeat_alarm;
    }

    private void ReceiveResult(string result)
    {
        alarmTimeTF.text = result;
    }

    private void LoadSubTasks()
    {
        ShowAdvanced();
        foreach (LSubTask fTask in this.subFTasks)
        {
            CreateSubTask(fTask);
        }
    }

    private void LoadLinkedTask()
    {
        foreach (LTaskEntry fTask in this.linkedFTasks)
        {
            CreateLink(fTask);
        }
    }

    private void LoadLinkedHabit()
    {
        foreach (LHabitEntry fHabit in this.linkedHabits)
        {
            CreateLink(fHabit);
        }
    }

    private void CreateSubTask(LSubTask fTask)
    {
        checkListObj.SetActive(true);
        GameObject newObj = GameObject.Instantiate(itemPrefab, subTaskGroup);
        SubTaskEntryItem item = newObj.GetComponent<SubTaskEntryItem>();
        item.Task = fTask;
        taskDic.Add(fTask.id, fTask);
        AdjustSubTaskScrollViewRect();
    }

    private void AdjustSubTaskScrollViewRect()
    {
        Vector2 sizeDelta = subtask_scrollView_RectTransform.sizeDelta;
        subtask_scrollView_RectTransform.sizeDelta = new Vector2(sizeDelta.x, 0);

        StartCoroutine(AdjustSubTaskScrollView());
    }

    private void AdjustLinkTaskScrollViewRect()
    {
        Vector2 sizeDelta = linktask_scrollView_RectTransform.sizeDelta;
        int count = Convert.Min(linkTaskGroup.childCount, 4);
        linktask_scrollView_RectTransform.sizeDelta = new Vector2(sizeDelta.x, (float)(count * 90.0f));

        StartCoroutine(AdjustLinkTaskScrollView());
    }

    private void AdjustLinkHabitScrollViewRect()
    {
        Vector2 sizeDelta = linkhabit_scrollView_RectTransform.sizeDelta;
        int count = Convert.Min(linkHabitGroup.childCount, 4);
        linkhabit_scrollView_RectTransform.sizeDelta = new Vector2(sizeDelta.x, (float)(count * 90.0f));

        StartCoroutine(AdjustLinkHabitScrollView());
    }

    private IEnumerator AdjustLinkTaskScrollView()
    {
        yield return new WaitForEndOfFrame();

        linktask_scrollView_RectTransform.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 0.0f;
        linktask_scrollView_RectTransform.gameObject.GetComponent<ScrollRect>().verticalScrollbar.value = 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }

    private IEnumerator AdjustLinkHabitScrollView()
    {
        yield return new WaitForEndOfFrame();

        linkhabit_scrollView_RectTransform.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 0.0f;
        linkhabit_scrollView_RectTransform.gameObject.GetComponent<ScrollRect>().verticalScrollbar.value = 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }

    private IEnumerator AdjustSubTaskScrollView()
    {
        yield return new WaitForEndOfFrame();

        subtask_scrollView_RectTransform.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 0.0f;
        subtask_scrollView_RectTransform.gameObject.GetComponent<ScrollRect>().verticalScrollbar.value = 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }

    public void CreateLink(LTaskEntry entry)
    {
        GameObject newObj = GameObject.Instantiate(linkitemPrefab, linkTaskGroup);
        LinkItem item = newObj.GetComponent<LinkItem>();
        item.SetTaskEntry(entry, this.newProjectEntry);
        if (!entryDic.Keys.Contains(entry.id))
        {
            entryDic.Add(entry.id, entry);
        }
        else
        {
            entryDic[entry.id] = entry;
        }
        
        linkTaskList.Add(item);
        //AdjustLinkTaskScrollViewRect();
    }

    public void CreateLink(LHabitEntry entry)
    {
        GameObject newObj = GameObject.Instantiate(linkitemPrefab, linkHabitGroup);
        LinkItem item = newObj.GetComponent<LinkItem>();
        item.SetHabitEntry(entry, this.newProjectEntry);
        if (!habitEntryDic.Keys.Contains(entry.id))
        {
            habitEntryDic.Add(entry.id, entry);
        }
        else
        {
            habitEntryDic[entry.id] = entry;
        }

        linkHabitList.Add(item);
        //AdjustLinkTaskScrollViewRect();
    }

    #endregion
}
