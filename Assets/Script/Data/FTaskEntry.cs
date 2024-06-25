using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[FirestoreData]
public class FTaskEntry: FEntry
{
    [FirestoreProperty]
    public int diffculty { get; set; }

    [FirestoreProperty]
    public int repeatition { get; set; }

    [FirestoreProperty]
    public List<int> repeatDays { get; set; }

    [FirestoreProperty]
    public int repeat_every { get; set; }

    [FirestoreProperty]
    public string remindAlarm { get; set; }

    [FirestoreProperty]
    public string linkedGoalId { get; set; }

    [FirestoreProperty]
    public List<string> skip_Dates { get; set; }

    [FirestoreProperty]
    public List<string> subTasks { get; set; }

    [FirestoreProperty]
    public List<string> completed_Week { get; set; }

    public List<FTask> subTaskList = new List<FTask>();

    public FTaskEntry()
    {
        Type = EntryType.DailyTask;
        diffculty = (int)Difficuly.Easy;
        repeatition = (int)Repeatition.Daily;
        repeatDays = new List<int>();
        remindAlarm = "00:00 AM";
        linkedGoalId = "";        
        subTasks = new List<string>();
        completed_Week = new List<string>();
        skip_Dates = new List<string>();
        repeat_every = 1;
        subTaskList = new List<FTask>();
    }

    public bool IsAvailable(System.DateTime dateTime)
    {
        System.DateTime begin_date_DT = Convert.FDateToDateTime(begin_date);
        if (repeatition == (int)Repeatition.Daily)
        {
            for (System.DateTime day = begin_date_DT.Date; day.Date <= dateTime.Date; day = day.AddDays(repeat_every))
            {
                if (day.Date == dateTime.Date)
                {
                    return true;
                }
            }
            return false;
        }
        else if (repeatition == (int)Repeatition.Weekly)
        {
            if (!repeatDays.Contains((int)dateTime.DayOfWeek))
            {
                return false;
            }
            else
            {
                System.DateTime currentWeekSunday = dateTime.AddDays(-(int)(dateTime.DayOfWeek));
                System.DateTime beginWeekSunday = begin_date_DT.AddDays(-(int)begin_date_DT.DayOfWeek);
                int totalDays = (int)((currentWeekSunday - beginWeekSunday).TotalDays);
                return totalDays % (repeat_every * 7) == 0;
            }
        }
        else if (repeatition == (int)Repeatition.Monthly)
        {
            if (dateTime.Day != begin_date_DT.Day)
            {
                return false;
            }
            for (System.DateTime day = begin_date_DT.Date; day.Date <= dateTime.Date; day = day.AddMonths(repeat_every))
            {
                if (day.Date == dateTime.Date)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            if (dateTime.Day != begin_date_DT.Day | dateTime.Month != begin_date_DT.Month)
            {
                return false;
            }
            for (System.DateTime day = begin_date_DT.Date; day.Date <= dateTime.Date; day = day.AddMonths(repeat_every))
            {
                if (day.Date == dateTime.Date)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool IsCompletedInAWeek()
    {
        string aWeekAgoDateString = Utilities.GetFormattedDate(-7);
        string todayString = Utilities.GetFormattedDate(-7);
        completed_Week = completed_Week.Where(x => x.CompareTo(aWeekAgoDateString) > 0).ToList();
        bool result = false;
        switch ((Repeatition)repeatition)
        {
            case Repeatition.Daily:
                result = completed_Week.Count == 7;
                break;
            case Repeatition.Weekly:
                result = completed_Week.Count == repeatDays.Count;
                break;
            case Repeatition.Monthly:
                if (completedDate.CompareTo(aWeekAgoDateString) > 0 && completedDate.CompareTo(todayString) <= 0)
                {
                    result = true;
                }
                else
                {
                    result = Utilities.DatesList(Convert.FDateToDateTime(begin_date), System.DateTime.Now.AddDays(-6), System.DateTime.Now, Repeatition.Monthly, repeat_every).ToList().Count == 0;
                }
                break;
            case Repeatition.Yearly:
                if (completedDate.CompareTo(aWeekAgoDateString) > 0 && completedDate.CompareTo(todayString) <= 0)
                {
                    result = true;
                }
                else
                {
                    result = Utilities.DatesList(Convert.FDateToDateTime(begin_date), System.DateTime.Now.AddDays(-6), System.DateTime.Now, Repeatition.Monthly, repeat_every).ToList().Count == 0;
                }
                break;
            default:
                break;
        }

        return result;
    }

    public override void OnComplete(System.DateTime dateTime)
    {
        System.DateTime lastCompletedDate = Convert.FDateToDateTime(begin_date);
        if (completedDate != "")
        {
            lastCompletedDate = Convert.FDateToDateTime(completedDate);
        }

        List<System.DateTime> skippedDates = new List<System.DateTime>();

        switch ((Repeatition)repeatition)
        {
            case Repeatition.Daily:
                skippedDates = Utilities.DatesList(Convert.FDateToDateTime(begin_date), lastCompletedDate.AddDays(1), dateTime.AddDays(-1), Repeatition.Daily, repeat_every).ToList();
                break;
            case Repeatition.Weekly:
                skippedDates = Utilities.DatesList(lastCompletedDate.AddDays(1), dateTime.AddDays(-1), repeat_every, Convert.IntToWeekday(repeatDays)).ToList();
                break;
            case Repeatition.Monthly:
                skippedDates = Utilities.DatesList(Convert.FDateToDateTime(begin_date), lastCompletedDate.AddDays(1), dateTime.AddDays(-1), Repeatition.Monthly, repeat_every).ToList();
                break;
            case Repeatition.Yearly:
                skippedDates = Utilities.DatesList(Convert.FDateToDateTime(begin_date), lastCompletedDate.AddDays(1), dateTime.AddDays(-1), Repeatition.Yearly, repeat_every).ToList();
                break;
            default:
                break;
        }

        if (skippedDates.Count > 0)
        {
            skip_Dates.AddRange(Convert.DateTimeToFDate(skippedDates));
        }

        base.OnComplete(dateTime);
        
        if ((Repeatition)repeatition == Repeatition.Daily | (Repeatition)repeatition == Repeatition.Weekly)
        {
            if (completed_Week == null)
            {
                completed_Week = new List<string>();
            }
            if (!completed_Week.Contains(this.completedDate))
            {
                completed_Week.Add(this.completedDate);
            }
            
            //remove complete history 30days ago
            string aWeekAgoDateString = Utilities.GetFormattedDate(-7);
            completed_Week = completed_Week.Where(x => x.CompareTo(aWeekAgoDateString) > 0).ToList();
        }

        /*
        switch ((Repeatition)repeatition)
        {
            case Repeatition.Daily:
                begin_date = Convert.DateTimeToFDate(System.DateTime.Now.AddDays(repeat_every));
                break;
            case Repeatition.Weekly:
                int dayOfWeek = (int)System.DateTime.Now.DayOfWeek;
                int index = repeatDays.FindLast(repeatDay => repeatDay == dayOfWeek);
                if (index == repeatDays.Count - 1)
                {
                    int difference = repeatDays[0] - repeatDays[repeatDays.Count - 1] + repeat_every * 7;
                    begin_date = Convert.DateTimeToFDate(System.DateTime.Now.AddDays(difference));
                }
                else
                {
                    int difference = repeatDays[index + 1] - repeatDays[index];
                    begin_date = Convert.DateTimeToFDate(System.DateTime.Now.AddDays(difference));
                }
                break;
            case Repeatition.Monthly:
                begin_date = Convert.DateTimeToFDate(System.DateTime.Now.AddMonths(repeat_every));
                break;
            case Repeatition.Yearly:
                begin_date = Convert.DateTimeToFDate(System.DateTime.Now.AddYears(repeat_every));
                break;
            default:
                break;
        }
        */
    }

    public override void CancelComplete()
    {
        var todayStr = Convert.DateTimeToFDate(System.DateTime.Now);
        if (completed_Week.Contains(todayStr))
        {
            completed_Week.Remove(todayStr);
        }

        if (completed_Week.Count > 0)
        {
            completedDate = completed_Week.Last();
        }
        else
        {
            completedDate = "";
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