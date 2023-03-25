using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class LinkItem : MonoBehaviour
{
    [SerializeField] private TMP_Text taskName_TF;
    [SerializeField] private Text taskLabel_TF;
    [SerializeField] private Slider progres_Silder;
    [SerializeField] private int nIndex = 0;
    
    private LTaskEntry entry = null;
    private LHabitEntry habitEntry = null;
    private LProjectEntry pEntry = null;
    private EntryType linkedType = EntryType.NULL;
    #region Unity Memebers
    // Start is called before the first frame update
    void Start()
    {

    }
    #endregion

    #region Public Members
    public string GetTaskName()
    {
        if (taskName_TF != null)
        {
            return taskName_TF.text;
        }

        return "";
    }

    public int Index
    {
        get
        {
            return nIndex;
        }
        set
        {
            nIndex = value;
        }
    }

    public void SetTaskEntry(LTaskEntry entry, LProjectEntry projectEntry)
    {
        this.linkedType = EntryType.DailyTask;
        this.entry = entry;
        this.pEntry = projectEntry;
        taskName_TF.text = entry.taskName;
        taskLabel_TF.text = "Task";
        List<System.DateTime> dateList = new List<System.DateTime>();

        switch ((Repeatition)entry.repeatition)
        {
            case Repeatition.Daily:
                dateList = Utilities.DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), Convert.EntryDateToDateTime(projectEntry.endDate), Repeatition.Daily, entry.repeat_every).ToList();
                break;
            case Repeatition.Weekly:
                dateList = Utilities.DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), Convert.EntryDateToDateTime(projectEntry.endDate), entry.repeat_every, Convert.IntToWeekday(entry.repeatDays)).ToList();
                break;
            case Repeatition.Monthly:
                dateList = Utilities.DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), Convert.EntryDateToDateTime(projectEntry.endDate), Repeatition.Monthly, entry.repeat_every).ToList();
                break;
            default:
                break;
        }

        System.DateTime lastDateTime = System.DateTime.Now.Date;
        if (entry.completedDate.Equals(""))
        {
            lastDateTime = System.DateTime.MinValue;
        }
        else
        {
            lastDateTime = Convert.FDateToDateTime(entry.completedDate);
        }

        foreach(string date in entry.skip_Dates)
        {
            if (date.CompareTo(projectEntry.beginDate) >= 0 && date.CompareTo(projectEntry.endDate) <= 0)
            {
                lastDateTime = Convert.FDateToDateTime(date);
                break;
            }
        }

        List<System.DateTime> checkedList = new List<System.DateTime>();

        switch ((Repeatition)entry.repeatition)
        {
            case Repeatition.Daily:
                checkedList = Utilities.DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), lastDateTime, Repeatition.Daily, entry.repeat_every).ToList();
                break;
            case Repeatition.Weekly:
                checkedList = Utilities.DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), lastDateTime, entry.repeat_every, Convert.IntToWeekday(entry.repeatDays)).ToList();
                break;
            case Repeatition.Monthly:
                checkedList = Utilities.DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), lastDateTime, Repeatition.Monthly, entry.repeat_every).ToList();
                break;
            default:
                break;
        }
        
        entry.progress = dateList.Count == 0 ? 0.0f : (float)(checkedList.Count) / dateList.Count;  
        progres_Silder.value = entry.progress;
    }

    public void SetHabitEntry(LHabitEntry entry, LProjectEntry projectEntry)
    {
        this.linkedType = EntryType.Habit;
        this.habitEntry = entry;
        this.pEntry = projectEntry;
        taskName_TF.text = entry.taskName;
        taskLabel_TF.text = "Habit";
        List<System.DateTime> dateList = new List<System.DateTime>();

        switch ((Repeatition)entry.repeatition)
        {
            case Repeatition.Daily:
                dateList = Utilities.DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), Convert.EntryDateToDateTime(projectEntry.endDate), Repeatition.Daily, entry.repeat_every).ToList();
                break;
            case Repeatition.Weekly:
                dateList = Utilities.DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), Convert.EntryDateToDateTime(projectEntry.endDate), entry.repeat_every, Convert.IntToWeekday(entry.repeatDays)).ToList();
                break;
            case Repeatition.Monthly:
                dateList = Utilities.DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), Convert.EntryDateToDateTime(projectEntry.endDate), Repeatition.Monthly, entry.repeat_every).ToList();
                break;
            default:
                break;
        }

        System.DateTime lastDateTime = System.DateTime.Now.Date;
        if (entry.completedDate.Equals(""))
        {
            lastDateTime = System.DateTime.MinValue;
        }
        else
        {
            lastDateTime = Convert.FDateToDateTime(entry.completedDate);
        }

        foreach (string date in entry.skip_Dates)
        {
            if (date.CompareTo(projectEntry.beginDate) >= 0 && date.CompareTo(projectEntry.endDate) <= 0)
            {
                lastDateTime = Convert.FDateToDateTime(date);
                break;
            }
        }

        List<System.DateTime> checkedList = new List<System.DateTime>();

        switch ((Repeatition)entry.repeatition)
        {
            case Repeatition.Daily:
                checkedList = Utilities.DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), lastDateTime, Repeatition.Daily, entry.repeat_every).ToList();
                break;
            case Repeatition.Weekly:
                checkedList = Utilities.DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), lastDateTime, entry.repeat_every, Convert.IntToWeekday(entry.repeatDays)).ToList();
                break;
            case Repeatition.Monthly:
                checkedList = Utilities.DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), lastDateTime, Repeatition.Monthly, entry.repeat_every).ToList();
                break;
            default:
                break;
        }

        entry.progress = dateList.Count == 0 ? 0.0f : (float)(checkedList.Count) / dateList.Count;
        progres_Silder.value = entry.progress;
    }

    public void DeleteItem()
    {
        if (linkedType == EntryType.DailyTask)
        {
            this.entry.linkedGoalId = "";
            transform.GetComponentInParent<EntryPage>().DeleteLinkTask(this.entry);
        }

        if (linkedType == EntryType.Habit)
        {
            this.habitEntry.linkedGoalId = "";
            transform.GetComponentInParent<EntryPage>().DeleteLinkHabit(this.habitEntry);
        }
        
        Destroy(transform.gameObject);
    }

    #endregion

    #region Private Members
    #endregion
}
