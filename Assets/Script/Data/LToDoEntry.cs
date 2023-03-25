using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LToDoEntry : LEntry
{
    public int diffculty = 0;
    public string dueDate = "";
    public List<string> checkList = new List<string>();
    private List<LSubTask> subTaskList = new List<LSubTask>();

    public LToDoEntry()
    {
        Type = EntryType.ToDo;
        diffculty = (int)Difficuly.Easy;
        dueDate = "2000_03_20";
        remindAlarm = "00:00 AM";
        checkList = new List<string>();
    }

    public void Update(List<LSubTask> subTaskList)
    {
        this.subTaskList = subTaskList;
        DataManager.Instance.UpdateEntry(this, subTaskList);
        NotificationManager.Instance.ReScheduleLocalNotification(this);
    }

    public override bool isCompleted()
    {
        if (completedDate != "")
        {
            return true;
        }
        return false;
    }

    public bool isCompletedInTime()
    {
        return completedDate.CompareTo(dueDate) <= 0;
    }

    public override bool isEnabled()
    {
        if (isCompleted())
        {
            return false;
        }
        else
        {
            //if (dueDate.CompareTo(Utilities.GetFormattedDate()) >= 0)
            //{
            //    return true;
            //}
            return true;
        }

        //return false;
    }



    public override System.DateTime GetTriggerDate()
    {
        if (remindAlarm != "")
        {
            var dateStr = dueDate + "_" + remindAlarm;
            var date = Convert.DetailedStringToDateTime(dateStr);
            if (isEnabled(date))
            {
                return date;
            }
        }

        return System.DateTime.MinValue;
    }
}
