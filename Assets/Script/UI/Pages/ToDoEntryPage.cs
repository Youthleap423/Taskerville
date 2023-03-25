using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SA.iOS.UIKit;
using SA.Android.App;
using FantomLib;
using TMPro;

public class ToDoEntryPage : EntryPage
{
    [Space]
    [SerializeField] private TMP_InputField taskNameIF;
    [SerializeField] private RectTransform scrollView_RectTransform;

    [Header("Difficulty")]
    [SerializeField] private List<Toggle> Difficulty_Toggles;

    [Header("Alarm")]
    [SerializeField] private Text dueDateTF;
    [SerializeField] private Text alarmTimeTF;
    [SerializeField] private TMP_Dropdown alarmRepeat_dropdown;

    [Header("SubChecklist")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform subTaskGroup;

    private LToDoEntry newToDoEntry = null;
    private List<LSubTask> subLTasks = new List<LSubTask>();
    private Dictionary<string, LSubTask> taskDic = new Dictionary<string, LSubTask>();

    private NavigationPage navPage = null;

    #region Unity Members
    // Start is called before the first frame update
    void Awake()
    {
        newToDoEntry = new LToDoEntry();
        newToDoEntry.id = string.Format("{0}", Utilities.SystemTimeInMillisecondsString);

        Initialize();
    }

    #endregion


    #region Public Memebers
    override public void Initialize()
    {
        base.Initialize();

        taskNameIF.text = "";
        dueDateTF.text = Convert.DateTimeToEntryDate(System.DateTime.Now);
        alarmTimeTF.text = "";

        foreach (Transform child in subTaskGroup.transform)
        {
            Destroy(child.gameObject);
        }
        
        subLTasks.Clear();

        if (Difficulty_Toggles.Count > 0)
        {
            Difficulty_Toggles[3].isOn = true;
        }

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

        var todoEntry = TaskViewController.Instance.GetToDoEntry(this.entry.id);
        if (todoEntry != null)
        {
            this.newToDoEntry = todoEntry;
            LoadToDoEntry();
            this.subLTasks = TaskViewController.Instance.GetSubTasks(todoEntry);
            LoadSubTasks();
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

    public override void UpdateSubTask(LSubTask task)
    {
        base.UpdateSubTask(task);
        if (taskDic.ContainsKey(task.id))
        {
            taskDic[task.id] = task;
        }
    }

    public void DeleteToDoEntry()
    {
        CreateToDoEntry(true);
    }

    public void CreateSubTask()
    {
        LSubTask newTask = new LSubTask();
        newTask.id = string.Format("{0}", Utilities.SystemTimeInMillisecondsString);
        newTask.type = ESubEntryType.SubToDo.ToString();
        newTask.goldCount = 0.5f;
        CreateSubTask(newTask);
    }

    public void CreateToDoEntry(bool isRemove = false)
    {
        if (!isRemove)
        {
            newToDoEntry.taskName = taskNameIF.text;
            for (int indexer = 0; indexer < Difficulty_Toggles.Count; indexer++)
            {
                Toggle toggle = Difficulty_Toggles[indexer];
                if (toggle.isOn)
                {
                    newToDoEntry.diffculty = indexer;
                }
            }
            newToDoEntry.goldCount = newToDoEntry.diffculty + 1;
            newToDoEntry.remindAlarm = alarmTimeTF.text;
            newToDoEntry.dueDate = dueDateTF.text.Equals("") ? "" : Convert.EntryDateToFDate(dueDateTF.text);
            newToDoEntry.orderId = Utilities.SystemTicks;

            this.subLTasks = taskDic.Values.ToList();
            newToDoEntry.checkList.Clear();
            newToDoEntry.repeat_alarm = alarmRepeat_dropdown.value;
            foreach (LTask task in this.subLTasks)
            {
                if (task.IsRemoved() == false)
                {
                    newToDoEntry.checkList.Add(task.id);
                }
            }
        }
        else
        {
            newToDoEntry.SetRemoved(true);

            this.subLTasks = taskDic.Values.ToList();
            newToDoEntry.checkList.Clear();

            foreach (LTask task in this.subLTasks)
            {
                task.SetRemoved(true);
                newToDoEntry.checkList.Add(task.id);
            }
        }

        newToDoEntry.Update(subLTasks);
        ShowToDoList();
    }

    public void ShowToDoList()
    {
        if (this.navPage != null)
        {
            this.navPage.Back();
        }
        //transform.parent.GetComponent<NavigationPage>().Back();
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

    private void ReceiveResult(string result)
    {
        alarmTimeTF.text = result;
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
                dueDateTF.text = Convert.DateTimeToEntryDate(result.Year, result.Month, result.Day);
            }
            else
            {
                dueDateTF.text = "";
            }
        });
#else
        ISN_UIDateTimePicker datePicker = new ISN_UIDateTimePicker();
        datePicker.DatePickerMode = ISN_UIDateTimePickerMode.Date;

        datePicker.Show((dateTime) =>
        {
            dueDateTF.text = Convert.DateTimeToEntryDate(dateTime);
        });
#endif

    }

    public void RemoveAlarm()
    {
        alarmTimeTF.text = "";
        alarmRepeat_dropdown.value = 0;
        alarmRepeat_dropdown.RefreshShownValue();
    }

    public void RemoveEndDate()
    {
        dueDateTF.text = "";
    }
    #endregion


    #region Private Members

    private void LoadToDoEntry()
    {
        taskNameIF.text = this.newToDoEntry.taskName;
        Difficulty_Toggles[this.newToDoEntry.diffculty].isOn = true;
        dueDateTF.text = Convert.FDateToEntryDate(this.newToDoEntry.dueDate);
        alarmTimeTF.text = this.newToDoEntry.remindAlarm;
        alarmRepeat_dropdown.value = this.newToDoEntry.repeat_alarm;
    }

    private void LoadSubTasks()
    {
        foreach (LSubTask fTask in this.subLTasks)
        {
            CreateSubTask(fTask);
        }
    }

    private void CreateSubTask(LSubTask fTask)
    {
        GameObject newObj = GameObject.Instantiate(itemPrefab, subTaskGroup);
        SubTaskEntryItem item = newObj.GetComponent<SubTaskEntryItem>();
        item.Task = fTask;
        taskDic.Add(fTask.id, fTask);
        AdjustScrollViewRect();
    }

    private void AdjustScrollViewRect()
    {
        Vector2 sizeDelta = scrollView_RectTransform.sizeDelta;
        int count = Convert.Min(subTaskGroup.childCount, 13);
        scrollView_RectTransform.sizeDelta = new Vector2(sizeDelta.x, (float)(count * 75.0f));

        StartCoroutine(AdjustScrollView());
    }

    private IEnumerator AdjustScrollView()
    {
        yield return new WaitForEndOfFrame();

        scrollView_RectTransform.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 0.0f;
        scrollView_RectTransform.gameObject.GetComponent<ScrollRect>().verticalScrollbar.value = 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollView_RectTransform.parent.GetComponent<RectTransform>());
    }


    #endregion
}
