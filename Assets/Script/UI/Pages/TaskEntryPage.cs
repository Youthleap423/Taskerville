using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SA.iOS.UIKit;
using SA.Android.App;
using FantomLib;
using TMPro;

public class TaskEntryPage : EntryPage
{
    [Space]
    [SerializeField] private TMP_InputField taskNameIF;
    [SerializeField] private GameObject advancedGroupObj;
    [SerializeField] private RectTransform scrollView_RectTransform;

    [Header("Difficulty")]
    [SerializeField] private List<Toggle> Difficulty_Toggles;

    [Header("Start Date")]
    [SerializeField] private Text beginDate_TF;

    [Header("Repeatition")]
    [SerializeField] private TMP_Dropdown repeatition_dropdown;
    [SerializeField] private List<Toggle> Week_Toggles;
    [SerializeField] private TMP_InputField repeat_every;
    [SerializeField] private Text repeat_unit_Text;
    [SerializeField] private CanvasGroup repeat_CanvasGroup;

    [Header("Alarm")]
    [SerializeField] private Text alarmTimeTF;
    [SerializeField] private TMP_Dropdown alarmRepeat_dropdown;

    [Header("SubTask")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform subTaskGroup;

    [Header("Link")]
    [SerializeField] private Text linked_GoalText;



    private LTaskEntry newTaskEntry = null;
    private List<LSubTask> subFTasks = new List<LSubTask>();
    private Dictionary<string, LSubTask> taskDic = new Dictionary<string, LSubTask>();
    private List<string>  unitList = new List<string>{"Day", "Week", "Month", "Year"};

    private NavigationPage navPage = null;

    #region Unity Members
    // Start is called before the first frame update
    void Awake()
    {
        newTaskEntry = new LTaskEntry();
        newTaskEntry.id = string.Format("{0}", Utilities.SystemTimeInMillisecondsString);

        repeat_every.onValueChanged.AddListener(delegate
        {
            repeat_every_valueChanged(repeat_every);
        });

        repeatition_dropdown.onValueChanged.AddListener(delegate
        {
            repeatition_dropdown_valueChanged(repeatition_dropdown);
        });

        Initialize();
    }

    #endregion

    
    #region Public Memebers
    public override void Initialize()
    {
        base.Initialize();

        taskNameIF.text = "";
        beginDate_TF.text = Convert.DateTimeToEntryDate(System.DateTime.Now);
        alarmTimeTF.text = "";

        foreach (Transform child in subTaskGroup.transform)
        {
            Destroy(child.gameObject);
        }

        subFTasks.Clear();

        if (Difficulty_Toggles.Count > 0)
        {
            Difficulty_Toggles[3].isOn = true;
        }

        foreach(Toggle toggle in Week_Toggles)
        {
            toggle.isOn = false;
        }

        if (advancedGroupObj != null)
        {
            advancedGroupObj.SetActive(false);
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

        var taskEntry = TaskViewController.Instance.GetDailyTask(this.entry.id);
        if (taskEntry != null)
        {
            this.newTaskEntry = taskEntry;
            LoadTaskEntry();

            this.subFTasks = TaskViewController.Instance.GetSubTasks(taskEntry);
            LoadSubTasks();

            if (taskEntry.linkedGoalId != "")
            {
                var linkedProjectEntry = TaskViewController.Instance.GetProjectEntryWithLink(taskEntry.linkedGoalId);
                if (linkedProjectEntry.id != "")
                {
                    this.linked_GoalText.text = linkedProjectEntry.taskName;
                }
            }
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

    public void Back()
    {
        if (this.navPage != null)
        {
            this.navPage.Back();
        }
        //transform.parent.GetComponent<NavigationPage>().Back();
    }

    public void DeleteTaskEntry()
    {
        CreateTaskEntry(true);

    }

    public void CreateSubTask()
    {
        LSubTask newTask = new LSubTask();
        newTask.id = string.Format("{0}", Utilities.SystemTimeInMillisecondsString);
        newTask.type = ESubEntryType.SubDailyTask.ToString();
        newTask.goldCount = 1;
        CreateSubTask(newTask);
    }

    public void CreateTaskEntry(bool isRemove = false)
    {
        if (!isRemove)
        {
            newTaskEntry.taskName = taskNameIF.text;
            for (int indexer = 0; indexer < Difficulty_Toggles.Count; indexer++)
            {
                Toggle toggle = Difficulty_Toggles[indexer];
                if (toggle.isOn)
                {
                    newTaskEntry.diffculty = indexer;
                }
            }

            newTaskEntry.goldCount = newTaskEntry.diffculty + 1;
            newTaskEntry.repeatDays.Clear();
            for (int indexer = 0; indexer < Week_Toggles.Count; indexer++)
            {
                Toggle toggle = Week_Toggles[indexer];
                if (toggle.isOn)
                {
                    newTaskEntry.repeatDays.Add(indexer);
                }
            }
            newTaskEntry.remindAlarm = alarmTimeTF.text;
            newTaskEntry.repeatition = repeatition_dropdown.value;
            if (newTaskEntry.orderId == 0)
            {
                newTaskEntry.orderId = Utilities.SystemTicks;
            }
            
            if (repeat_every.text.Trim() == "")
            {
                newTaskEntry.repeat_every = 1;
            }
            else
            {
                newTaskEntry.repeat_every = int.Parse(repeat_every.text.Trim());
            }
            newTaskEntry.repeat_alarm = alarmRepeat_dropdown.value;
            this.subFTasks = taskDic.Values.ToList();
            newTaskEntry.subTasks.Clear();
            newTaskEntry.begin_date = Convert.EntryDateToFDate(beginDate_TF.text);

            foreach (LTask task in this.subFTasks)
            {
                if (task.IsRemoved() == false)
                {
                    newTaskEntry.subTasks.Add(task.id);
                }
            }
        }
        else
        {
            newTaskEntry.SetRemoved(true);

            this.subFTasks = taskDic.Values.ToList();
            newTaskEntry.subTasks.Clear();

            foreach (LTask task in this.subFTasks)
            {
                task.SetRemoved(true);
                task.begin_date = Convert.DateTimeToFDate(System.DateTime.Now);
                newTaskEntry.subTasks.Add(task.id);
            }
        }

        newTaskEntry.Update(this.subFTasks);
        ShowDailyTask();
    }

    public void ShowAdvanced()
    {
        if (advancedGroupObj != null)
        {
            advancedGroupObj.SetActive(true);
        }
        AdjustScrollViewRect();
    }

    public void ShowDailyTask()
    {
        Back();
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

    public void ShowDateDlg()
    {
#if UNITY_ANDROID
        System.DateTime dateTime = System.DateTime.Now;
        AN_DatePickerDialog picker = new AN_DatePickerDialog(dateTime.Year, dateTime.Month - 1, dateTime.Day);
        picker.Show((result) =>
        {
            if (result.IsSucceeded)
            {
                beginDate_TF.text = Convert.DateTimeToEntryDate(result.Year, result.Month, result.Day);
            }
            else
            {
                beginDate_TF.text = "";
            }
        });
#else
        ISN_UIDateTimePicker datePicker = new ISN_UIDateTimePicker();
        datePicker.DatePickerMode = ISN_UIDateTimePickerMode.Date;

        datePicker.Show((dateTime) =>
        {
            beginDate_TF.text = Convert.DateTimeToEntryDate(dateTime);
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

    private void repeat_every_valueChanged(TMP_InputField inputField)
    {
        updateUnit(inputField.text.Trim(), repeatition_dropdown.value);
    }

    private void repeatition_dropdown_valueChanged(TMP_Dropdown dropdown)
    {
        updateUnit(repeat_every.text.Trim(), dropdown.value);
    }


    private void updateUnit(string text, int value)
    {
        if ((Repeatition)value == Repeatition.Weekly)
        {
            UpdateCanvasGroup(true);
        }
        else
        {
            UpdateCanvasGroup(false);
        }

        repeat_unit_Text.text = unitList[value];

        if (text == "")
        {            
            return;
        }
        string suffix = int.Parse(text) > 1 ? "s" : "";
        repeat_unit_Text.text = unitList[value] + suffix;

    }

    private void LoadTaskEntry()
    {
        taskNameIF.text = this.newTaskEntry.taskName;
        Difficulty_Toggles[this.newTaskEntry.diffculty].isOn = true;
        repeatition_dropdown.value = this.newTaskEntry.repeatition;
        foreach(int id in this.newTaskEntry.repeatDays)
        {
            Week_Toggles[id].isOn = true;
        }
        beginDate_TF.text = Convert.FDateToEntryDate(this.newTaskEntry.begin_date);
        alarmTimeTF.text = this.newTaskEntry.remindAlarm;
        alarmRepeat_dropdown.value = this.newTaskEntry.repeat_alarm;
        repeat_every.text = this.newTaskEntry.repeat_every.ToString();
        updateUnit(repeat_every.text, repeatition_dropdown.value);
    }

    private void LoadSubTasks()
    {
        ShowAdvanced();
        foreach (LSubTask fTask in this.subFTasks)
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
        int count = Convert.Min(subTaskGroup.childCount, 6);
        scrollView_RectTransform.sizeDelta = new Vector2(sizeDelta.x, (float)(count * 75.0f));
        StartCoroutine(AdjustScrollView());
    }

    private IEnumerator AdjustScrollView()
    {
        yield return new WaitForEndOfFrame();
        
        scrollView_RectTransform.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 0.0f;
        scrollView_RectTransform.gameObject.GetComponent<ScrollRect>().verticalScrollbar.value = 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }

    private void UpdateCanvasGroup(bool isEnable)
    {
        if (isEnable)
        {
            repeat_CanvasGroup.alpha = 1.0f;
            repeat_CanvasGroup.blocksRaycasts = true;
            repeat_CanvasGroup.interactable = true;
        }
        else
        {
            repeat_CanvasGroup.alpha = 0.6f;
            repeat_CanvasGroup.blocksRaycasts = false;
            repeat_CanvasGroup.interactable = false;
        }
    }

#endregion

}
