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
using System;

public class HabitEntryPage : EntryPage
{
    [Space]
    [SerializeField] private TMP_InputField taskNameIF;
    [SerializeField] private RectTransform scrollView_RectTransform;

    [Header("Category")]
    [SerializeField] private Toggle positiveToggle;
    [SerializeField] private Toggle negativeToggle;
    [SerializeField] private Dropdown categoryDropdown;


    [Header("Unit Toggles")]
    [SerializeField] private IOSToggle unitToggle;
    [SerializeField] private IOSToggle durationToggle;
    [SerializeField] private IOSToggle timespanToggle;
    [SerializeField] private IOSToggle recurrenceToggle;

    [Header("Unit Items")]
    [SerializeField] private GameObject unitItem;
    [SerializeField] private Dropdown numberOfUnitDropdown;
    [SerializeField] private Dropdown unitTypeDropdown;
    [SerializeField] private InputField customUnit_IF;

    [Header("Duration Items")]
    [SerializeField] private GameObject durationItem;
    [SerializeField] private Dropdown durationHDropdown;
    [SerializeField] private Dropdown durationMDropdown;


    [Header("TimeSpan Items")]
    [SerializeField] private GameObject timespanItem;
    [SerializeField] private Dropdown timespanHDropdown;
    [SerializeField] private Dropdown timespanMDropdown;
    [SerializeField] private Text timespanStartTF;
    [SerializeField] private Slider timespanSlider;

    [Header("Recurrence Items")]
    [SerializeField] private GameObject recurrenceItem;

    [Header("Repeatition")]
    [SerializeField] private Text beginDate_TF;
    [SerializeField] private TMP_Dropdown repeatition_dropdown;
    [SerializeField] private List<Toggle> Week_Toggles;
    [SerializeField] private InputField repeat_every;
    [SerializeField] private Text repeat_unit_Text;
    [SerializeField] private CanvasGroup repeat_CanvasGroup;

    [Header("Alarm")]
    [SerializeField] private Text alarmTimeTF;
    [SerializeField] private TMP_Dropdown alarmRepeat_dropdown;

    [Header("Streak")]
    [SerializeField] private Text streakTF;
    [SerializeField] private GameObject justForTodayObj;
    [SerializeField] private List<GameObject> categoryInfoObjs;
    //[Header("Alarm")]
    //[SerializeField] private Text alarmTimeTF;



    private LHabitEntry newHabitEntry = null;
    private List<string> unitList = new List<string> { "Day", "Week", "Month", "Year" };

    private NavigationPage navPage = null;

    #region Unity Members
    // Start is called before the first frame update
    void Awake()
    {
        newHabitEntry = new LHabitEntry();
        newHabitEntry.id = string.Format("{0}", Utilities.SystemTimeInMillisecondsString);
        
        negativeToggle.onValueChanged.AddListener(delegate
        {
            negativeToggle_valueChanged(negativeToggle);
        });

        repeat_every.onValueChanged.AddListener(delegate
        {
            repeat_every_valueChanged(repeat_every);
        });

        repeatition_dropdown.onValueChanged.AddListener(delegate
        {
            repeatition_dropdown_valueChanged(repeatition_dropdown);
        });

        unitTypeDropdown.onValueChanged.AddListener(delegate
        {
            unitType_dropdown_valueChanged(unitTypeDropdown);
        });

        unitToggle.OnValueChanged += UnitToggle_OnValueChanged;
        durationToggle.OnValueChanged += DurationToggle_OnValueChanged;
        timespanToggle.OnValueChanged += TimespanToggle_OnValueChanged;
        recurrenceToggle.OnValueChanged += RecurrenceToggle_OnValueChanged;
        timespanHDropdown.onValueChanged.AddListener(delegate
        {
            RefreshAlarm();
        });
        timespanMDropdown.onValueChanged.AddListener(delegate
        {
            RefreshAlarm();
        });
        unitTypeDropdown.value = 1;
        Initialize();
    }

    private void RecurrenceToggle_OnValueChanged(bool isOn)
    {
        recurrenceItem.SetActive(isOn);
        foreach (Toggle toggle in Week_Toggles)
        {
            toggle.isOn = false;
        }
        if (isOn)
        {
            beginDate_TF.text = Convert.FDateToEntryDate(this.newHabitEntry.begin_date);
            repeatition_dropdown.value = this.newHabitEntry.repeatition;
            foreach (int id in this.newHabitEntry.repeatDays)
            {
                Week_Toggles[id].isOn = true;
            }
            alarmTimeTF.text = this.newHabitEntry.remindAlarm;
            alarmRepeat_dropdown.value = this.newHabitEntry.repeat_alarm;
            repeat_every.text = this.newHabitEntry.repeat_every.ToString();
            updateUnit(repeat_every.text, repeatition_dropdown.value);
            streakTF.text = string.Format("{0}", newHabitEntry.streak);
        }
        justForTodayObj.SetActive(!isOn);
    }

    private void TimespanToggle_OnValueChanged(bool isOn)
    {
        timespanItem.SetActive(isOn);
        if (isOn)
        {
            durationToggle.SetState(false);
            timespanHDropdown.value = GetIndex(timespanHDropdown.options, newHabitEntry.span_h);
            timespanMDropdown.value = GetIndex(timespanMDropdown.options, newHabitEntry.span_m);
            if (newHabitEntry.span_startTime == "0")
            {
                timespanStartTF.text = System.DateTime.Now.ToString("hh:mm tt");
            }
            else
            {
                timespanStartTF.text = newHabitEntry.span_startTime;
            }

            timespanSlider.value = newHabitEntry.progress;
        }
    }

    private void DurationToggle_OnValueChanged(bool isOn)
    {
        durationItem.SetActive(isOn);
        if (isOn)
        {
            timespanToggle.SetState(false);
            durationHDropdown.value = GetIndex(durationHDropdown.options, newHabitEntry.dur_h);
            durationMDropdown.value = GetIndex(durationMDropdown.options, newHabitEntry.dur_m);
        }
    }

    private void UnitToggle_OnValueChanged(bool isOn)
    {
        unitItem.SetActive(isOn);
        if (isOn)
        {
            var index = unitTypeDropdown.options.FindLastIndex(item => item.text == newHabitEntry.unitType);
            if (index == -1 && newHabitEntry.unitType != "")
            {
                unitTypeDropdown.value = 31;
                customUnit_IF.text = newHabitEntry.unitType;
            }
            else
            {
                unitTypeDropdown.value = index;
            }
            
            numberOfUnitDropdown.value = newHabitEntry.numberOfUnit;
        }
    }

    private int GetIndex(List<Dropdown.OptionData> optionDatas, string content)
    {
        var result = 0;

        if (optionDatas.Count > 0)
        {
            for (int i = 0; i < optionDatas.Count; i++)
            {
                if (optionDatas[i].text == content)
                {
                    result = i;
                    break;
                }
            }
        }

        return result;
    }
    #endregion


    #region Public Memebers
    public override void Initialize()
    {
        base.Initialize();

        taskNameIF.text = "";
        beginDate_TF.text = Convert.DateTimeToEntryDate(System.DateTime.Now);
        negativeToggle.isOn = !newHabitEntry.isPositive;
        unitToggle.SetState(newHabitEntry.unit);
        durationToggle.SetState(newHabitEntry.duration);
        timespanToggle.SetState(newHabitEntry.timespan);
        recurrenceToggle.SetState(newHabitEntry.recurrence);
        alarmTimeTF.text = "";
        justForTodayObj.SetActive(true);
    }


    public override void OnLoadPage(NavigationPage page)
    {
        base.OnLoadPage(page);

        this.navPage = page;

        if (this.entry == null)
        {
            return;
        }

        var habitEntry = TaskViewController.Instance.GetHabitEntry(this.entry.id);

        if (habitEntry != null)
        {
            this.newHabitEntry = habitEntry;
            LoadTaskEntry();
        }
        
        //var taskEntry = TaskViewController.Instance.GetDailyTask(this.entry.id);
        //if (taskEntry != null)
        //{
        //    this.newTaskEntry = taskEntry;
        //    LoadTaskEntry();

        //    this.subFTasks = TaskViewController.Instance.GetSubTasks(taskEntry);
        //    LoadSubTasks();

        //    if (taskEntry.linkedGoalId != "")
        //    {
        //        var linkedProjectEntry = TaskViewController.Instance.GetProjectEntryWithLink(taskEntry.linkedGoalId);
        //        if (linkedProjectEntry.id != "")
        //        {
        //            this.linked_GoalText.text = linkedProjectEntry.taskName;
        //        }
        //    }
        //}
    }

    public override void SetEntry(LEntry entry)
    {
        this.entry = entry;
    }

    public void Back()
    {
        if (this.navPage != null)
        {
            this.navPage.Back();
        }
        //transform.parent.GetComponent<NavigationPage>().Back();
    }

    public void DeleteEntry()
    {
        CreateEntry(true);

    }

    
    public void CreateEntry(bool isRemove = false)
    {
        if (!isRemove)
        {
            newHabitEntry.taskName = taskNameIF.text;
            newHabitEntry.isPositive = !negativeToggle.isOn;
            newHabitEntry.unit = unitToggle.isOn;
            newHabitEntry.duration = durationToggle.isOn;
            newHabitEntry.timespan = timespanToggle.isOn;
            newHabitEntry.recurrence = recurrenceToggle.isOn;

            newHabitEntry.unitType = unitTypeDropdown.value == 31 ? customUnit_IF.text : unitTypeDropdown.options[unitTypeDropdown.value].text;
            newHabitEntry.numberOfUnit = numberOfUnitDropdown.value;

            newHabitEntry.dur_h = durationHDropdown.options[durationHDropdown.value].text;
            newHabitEntry.dur_m = durationMDropdown.options[durationMDropdown.value].text;

            newHabitEntry.span_h = timespanHDropdown.options[timespanHDropdown.value].text;
            newHabitEntry.span_m = timespanMDropdown.options[timespanMDropdown.value].text;
            newHabitEntry.span_start = timespanStartTF.text;
            //newHabitEntry.progress = timespanSlider.value;
            newHabitEntry.goldCount = 5;//changed 2023/05/23 by pooh

            newHabitEntry.streak = int.Parse(streakTF.text);

            if (timespanToggle.isOn)
            {
                if (unitToggle.isOn)
                {
                    if (newHabitEntry.progress == 0f)//if unit with timespan is in progress, it should save their span_startTime
                    {
                        newHabitEntry.span_startTime = "";
                    }
                }
                else
                {
                    newHabitEntry.span_startTime = newHabitEntry.span_start;
                }
            }
            newHabitEntry.repeatDays.Clear();
            for (int indexer = 0; indexer < Week_Toggles.Count; indexer++)
            {
                Toggle toggle = Week_Toggles[indexer];
                if (toggle.isOn)
                {
                    newHabitEntry.repeatDays.Add(indexer);
                }
            }
            
            
            if (newHabitEntry.orderId < 0)
            {
                newHabitEntry.orderId = Utilities.SystemTicks;
            }

            if (negativeToggle.isOn)
            {
                newHabitEntry.repeatition = 0;
                newHabitEntry.repeat_every = 1;
                newHabitEntry.bJustToday = false;
            }
            else
            {
                newHabitEntry.repeatition = repeatition_dropdown.value;
                if (repeat_every.text.Trim() == "")
                {
                    newHabitEntry.repeat_every = 1;
                }
                else
                {
                    newHabitEntry.repeat_every = int.Parse(repeat_every.text.Trim());
                }
                newHabitEntry.bJustToday = !recurrenceToggle.isOn;
            }

            newHabitEntry.begin_date = Convert.EntryDateToFDate(beginDate_TF.text);
            if (newHabitEntry.begin_date != Convert.EntryDateToFDate(beginDate_TF.text) || newHabitEntry.span_startDate == "")
            {
                newHabitEntry.span_startDate = newHabitEntry.begin_date;
            }
            
            if (recurrenceToggle.isOn)
            {
                newHabitEntry.repeat_alarm = alarmRepeat_dropdown.value;
                newHabitEntry.remindAlarm = alarmTimeTF.text;
            }
            else
            {
                newHabitEntry.repeat_alarm = 0;
                newHabitEntry.remindAlarm = "";
            }
            TaskViewController.Instance.CreateHabit(newHabitEntry, (isSuccess) =>
            {
                if (isSuccess)
                {
                    ShowHabitList();
                }
            });
        }
        else
        {
            TaskViewController.Instance.RemoveHabit(newHabitEntry, (isSuccess) =>
            {
                if (isSuccess)
                {
                    ShowHabitList();
                }
            });
        }
    }

    public void ShowHabitList()
    {
        Back();
    }

    public void ShowCategoryInfo()
    {
        if (categoryDropdown.value < categoryInfoObjs.Count)
        {
            categoryInfoObjs[categoryDropdown.value].SetActive(true);
        }
    }

    public void ShowTimespanDlg()
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
            timespanStartTF.text = dateTime.ToString("hh:mm tt");
            RefreshAlarm();
            //alarmTimeTF.text = dateTime.ToString("hh:mm tt");
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
    private void RefreshAlarm()
    {
        try
        {
            if (!unitToggle.isOn && timespanToggle.isOn)
            {
                var stStr = Convert.DateTimeToFDate(System.DateTime.Now) + "_" + timespanStartTF.text;
                var startTime = Convert.DetailedStringToDateTime(stStr);
                alarmTimeTF.text = startTime.AddHours(double.Parse(timespanHDropdown.options[timespanHDropdown.value].text)).AddMinutes(double.Parse(timespanMDropdown.options[timespanMDropdown.value].text)).ToString("hh:mm tt");
            }
        }
        catch { }
    }

    private void negativeToggle_valueChanged(Toggle toggle)
    {
        if (toggle.isOn == true)
        {
            unitToggle.SetState(false);
            timespanToggle.SetState(false);
            durationToggle.SetState(false);
            recurrenceToggle.SetState(true);
        }
        scrollView_RectTransform.gameObject.SetActive(!toggle.isOn);
    }
    
    private void repeat_every_valueChanged(InputField inputField)
    {
        updateUnit(inputField.text.Trim(), repeatition_dropdown.value);
    }

    private void repeatition_dropdown_valueChanged(TMP_Dropdown dropdown)
    {
        updateUnit(repeat_every.text.Trim(), dropdown.value);
    }

    private void unitType_dropdown_valueChanged(Dropdown dropdown)
    {
        if (dropdown.value == 31)//custom unit type
        {
            customUnit_IF.gameObject.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            customUnit_IF.gameObject.transform.parent.gameObject.SetActive(false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(unitItem.transform.GetComponent<RectTransform>());
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
        taskNameIF.text = this.newHabitEntry.taskName;
        repeatition_dropdown.value = this. newHabitEntry.repeatition;
        beginDate_TF.text = Convert.FDateToEntryDate(this.newHabitEntry.begin_date);
        timespanStartTF.text = this.newHabitEntry.span_start;
        alarmTimeTF.text = this.newHabitEntry.remindAlarm;
        alarmRepeat_dropdown.value = this.newHabitEntry.repeat_alarm;
        repeat_every.text = this.newHabitEntry.repeat_every.ToString();
        updateUnit(repeat_every.text, repeatition_dropdown.value);

        if (newHabitEntry.isPositive)
        {
            positiveToggle.isOn = true;
        }
        else
        {
            negativeToggle.isOn = true;
        }
        
        unitToggle.SetState(newHabitEntry.unit);
        durationToggle.SetState(newHabitEntry.duration);
        timespanToggle.SetState(newHabitEntry.timespan);
        recurrenceToggle.SetState(newHabitEntry.recurrence);
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
