using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LProjectEntry : LEntry
{
    public string beginDate = "";
    public string endDate = "";
    public List<string> subProjects = new List<string>();
    public List<string> linkedTasks = new List<string>();
    public List<string> linkedHabits = new List<string>();

    public LProjectEntry()
    {
        Type = EntryType.Project;
        beginDate = "March 22, 2000";
        endDate = "March 22, 2000";
        subProjects = new List<string>();
        linkedTasks = new List<string>();
        linkedHabits = new List<string>();
    }

    public void Update(List<LSubTask> subProjectList, List<LTaskEntry> linkedTaskList)
    {
        DataManager.Instance.UpdateEntry(this, subProjectList, linkedTaskList);
        NotificationManager.Instance.ReScheduleLocalNotification(this);
        //TODO - FirestoreManager.Instance.CreateEntry(this, subProjectList, linkedTaskList, callback);
    }

    public override bool isCompleted()
    {
        return !completedDate.Equals("");
    }

    public override System.DateTime GetTriggerDate()
    {
        if (remindAlarm != "")
        {
            var dateStr = Convert.EntryDateToFDate(endDate) + "_" + remindAlarm;
            var date = Convert.DetailedStringToDateTime(dateStr);
            if (isEnabled(date))
            {
                return date;
            }
        }

        return System.DateTime.MinValue;
    }
}
