using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LAutoToDo : LEntry
{
    public int diffculty = 0;
    public string dueDate = "";    
    public List<string> checkList = new List<string>();
    public List<int> repeatition = new List<int>();
    public int cost_gold = 0;
    public int cost_wood = 0;
    public int cost_iron = 0;

    public LAutoToDo()
    {
        Type = EntryType.AutoToDo;
        diffculty = (int)Difficuly.Easy;
        dueDate = "2000_03_20";
        remindAlarm = "00:00 AM";
        checkList = new List<string>();
        cost_gold = cost_wood = cost_iron = 0;
        repeatition = new List<int>();
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

    public bool IsAvailable(System.DateTime dateTime)
    {
        System.DateTime begin_date_DT = Convert.FDateToDateTime(begin_date);
        if (!repeatition.Contains((int)dateTime.DayOfWeek))
        {
            return false;
        }
        else
        {
            System.DateTime currentWeekSunday = dateTime.AddDays(-(int)(dateTime.DayOfWeek));
            System.DateTime beginWeekSunday = begin_date_DT.AddDays(-(int)begin_date_DT.DayOfWeek);
            int totalDays = (int)((currentWeekSunday - beginWeekSunday).TotalDays);
            return totalDays % 7 == 0;
        }
    }

    public override bool isEnabled(System.DateTime dateTime)
    {
        if (isCompleted())
        {
            return false;
        }
        else
        {
            return IsAvailable(dateTime);
        }
    }
}
