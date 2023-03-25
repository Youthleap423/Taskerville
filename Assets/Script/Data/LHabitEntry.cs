using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LHabitEntry : LEntry
{
    public bool isPositive = false;
    public bool unit = false;
    public bool duration = false;
    public bool timespan = false;
    public bool recurrence = false;
    public string category = "";
    public int numberOfUnit = 0;
    public string unitType = "";
    public string dur_h = "";
    public string dur_m = "";
    public string span_h = "";
    public string span_m = "";
    public string span_start = "";
    public int repeatition = 0;
    public List<int> repeatDays = new List<int>();
    public int repeat_every = 0;
    public int streak = 0;
    
    public int complete_unit = 0;
    public string span_startTime = "";
    public string span_startDate = "";
    public bool bJustToday = true;
    public string linkedGoalId = "";
    public List<string> skip_Dates = new List<string>();
    public List<string> completed_Week = new List<string>();

    public LHabitEntry()
    {
        isPositive = false;
        unit = false;
        begin_date = Convert.DateTimeToFDate(System.DateTime.Now);
        duration = false;
        timespan = false;
        recurrence = false;
        category = "";
        numberOfUnit = 0;
        unitType = "1/4 mile or 1/4 km per unit";
        dur_h = "";
        dur_m = "";
        span_h = "";
        span_m = "";
        span_start = "00:00 AM";
        span_startTime = "0";
        Type = EntryType.Habit;
        repeatition = (int)Repeatition.Daily;
        repeatDays = new List<int>();
        remindAlarm = "00:00 AM";
        complete_unit = 0;
        completed_Week = new List<string>();
        skip_Dates = new List<string>();
        repeat_every = 1;
        bJustToday = true;
        linkedGoalId = "";
}

    public bool IsAvailable(System.DateTime dateTime)
    {
        if (unit == true)
        {
            return numberOfUnit >= (complete_unit - 1);
        }

        if (begin_date == "")
        {
            return false;
        }

        System.DateTime begin_date_DT = Convert.FDateToDateTime(begin_date);

        if (recurrence == true)
        {
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

        return true;
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

    public void OnCompleteTimeSpan()
    {
        if (unit == true)
        {
            complete_unit++;
            span_startTime = "";
        }
        else
        {
            //OnComplete(System.DateTime.Now);
        }
        Update();
    }

    public override void OnComplete(System.DateTime dateTime)
    {
        if (this.isCompleted())
        {
            return;
        }

        base.OnComplete(dateTime);
        streak = Mathf.Clamp(streak + 1, 0, 5);
        if (!isPositive)
        {
            RewardSystem.Instance.OnCompleteNegativeHabit();
            return;
        }

        System.DateTime lastCompletedDate = Convert.FDateToDateTime(begin_date);
        int goldCount = 0;
        if (timespan == true)
        {
            if (unit == true)
            {
                goldCount = complete_unit;
            }
            else
            {
                var timeSpanSecs = (int.Parse(span_h) * 60 + int.Parse(span_m)) * 60;
                var pastSecs = (int)Convert.TimeDifferenceSeconds(span_startDate + "_" + span_startTime, dateTime);

                progress = (float)(pastSecs) / (float)(timeSpanSecs);

                var timeSpanMins = (int.Parse(span_h) * 60 + int.Parse(span_m));
                var pastMins = (int)Convert.TimeDifferenceMinutes(span_startDate + "_" + span_startTime, dateTime);

                goldCount = Mathf.Clamp(Mathf.Min(pastMins, timeSpanMins) / 30, 0, 8);
            }
        }else if (duration == true)
        {
            var durationMins = (int.Parse(dur_h) * 60 + int.Parse(dur_m));
            goldCount = Mathf.Clamp(durationMins / 30, 0, 8);
            if (unit == true)
            {
                goldCount = Mathf.Clamp(goldCount * complete_unit, 0, 8);
            }
        }
        else if (unit == true)
        {
            goldCount = complete_unit;
        }
        if (recurrence)
        {
            NotificationManager.Instance.ReScheduleLocalNotification(this);
        }
        RewardSystem.Instance.OnCompleteWith(EResources.Gold, (float)goldCount);
    }

    public override System.DateTime GetTriggerDate()
    {
        if (recurrence == false)
        {
            return System.DateTime.MinValue;
        }

        var date = System.DateTime.Now;
        var startDays = 0;
        if (remindAlarm != "")
        {
            var dateStr = Convert.DateTimeToFDate(date) + "_" + remindAlarm;
            if (Convert.DetailedStringToDateTime(dateStr).CompareTo(date) < 0)
            {
                startDays = 1;
            }


            for (int days = startDays; days < 36500; days++)
            {
                date = System.DateTime.Now.AddDays(days);
                if (isEnabled(date))
                {
                    dateStr = Convert.DateTimeToFDate(date) + "_" + remindAlarm;
                    return Convert.DetailedStringToDateTime(dateStr);
                }
            }
        }


        return System.DateTime.MinValue;
    }

    public void Update()
    {
        DataManager.Instance.UpdateEntry(this);
        NotificationManager.Instance.ReScheduleLocalNotification(this);
    }

    public override bool isCompleted()
    {
        if (unit == false && timespan == true)
        {
            var endDateTime = getEndDate(span_startDate);
            var isCompleted = Convert.DateTimeToFDate(endDateTime) == completedDate;

            if (isCompleted == false)
            {
                return false;
            }

            

            //if (System.DateTime.Now.CompareTo(endDateTime) < 0)
            //{
            //    return false;
            //}

            var dateTime = System.DateTime.Now;
            System.DateTime startDateTime = getStartDate(Convert.DateTimeToFDate(dateTime));
            startDateTime = startDateTime.AddDays(1);
            return dateTime.CompareTo(startDateTime) <= 0;
            //return true;
        }
        else
        {
            return Utilities.GetFormattedDate().Equals(completedDate);
        }
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

    public void OnCheckCompleted(System.DateTime dateTime)
    {
        if (completedDate == "" || completedDate.CompareTo(Convert.DateTimeToFDate(dateTime)) < 0)
        {
            if (IsAvailable(dateTime))
            {
                if (created_at.CompareTo(Convert.DateTimeToFDate(dateTime)) <= 0)
                {
                    completedDate = Convert.DateTimeToFDate(dateTime);
                    if (isPositive)
                    {
                        skip_Dates.Add(completedDate);
                        streak = 0;
                        RewardSystem.Instance.OnFailed(this);
                    }
                    else
                    {
                        RewardSystem.Instance.OnComplete(this);
                    }
                }
            }
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

    public bool needReset(System.DateTime dateTime)
    {
        if (unit == false && timespan == true)
        {
            //System.DateTime endDateTime = getEndDate(span_startDate);
            //return dateTime.CompareTo(endDateTime) > 0;
            System.DateTime startDateTime = getStartDate(Convert.DateTimeToFDate(dateTime));
            startDateTime = startDateTime.AddDays(1);
            //UIManager.LogError(System.DateTime.Now + " : " + startDateTime + " : " + System.DateTime.Now.CompareTo(startDateTime));
            return dateTime.CompareTo(startDateTime) > 0;
        }
        else
        {
            var dateStr = Convert.DateTimeToFDate(dateTime);
            return dateStr.CompareTo(span_startDate) > 0;
        }
    }

    public void Reset()
    {
        complete_unit = 0;
        progress = 0f;
        span_startDate = Convert.DateTimeToFDate(System.DateTime.Now);
    }

    private System.DateTime getEndDate(System.DateTime startDate)
    {
        var dateStr = Convert.DateTimeToFDate(startDate);
        return getEndDate(dateStr);
    }

    private System.DateTime getEndDate(string startDate)
    {
        System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
        string formatString = "yyyy_MM_dd hh:mm tt";
        var dateStr = startDate + " " + span_startTime;
        System.DateTime startDateTime = System.DateTime.ParseExact(dateStr, formatString, provider);
        System.DateTime endDateTime = startDateTime.AddHours(double.Parse(span_h));
        endDateTime = endDateTime.AddMinutes(double.Parse(span_m));

        return endDateTime;
    }

    private System.DateTime getStartDate(string startDate)
    {
        System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
        string formatString = "yyyy_MM_dd hh:mm tt";
        var dateStr = /*startDate*/ span_startDate + " " + span_startTime;
        System.DateTime startDateTime = System.DateTime.ParseExact(dateStr, formatString, provider);
        
        return startDateTime;
    }
}
