using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TaskViewController : SingletonComponent<TaskViewController>
{
    private System.DateTime lastGetDate = System.DateTime.MinValue.Date;

#region Public Members
    public LTaskEntry GetDailyTask(string taskId)
    {
        var result = DataManager.Instance.GetTaskEntry(taskId);
        if (result.id == "")
        {
            return null;
        }
        return result;
    }

    public List<LTaskEntry> GetDailyTaskWithLink(string id)
    {
        return DataManager.Instance.GetDailyTaskWithLink(id).ToList();
    }

    public List<LHabitEntry> GetHabitWithLink(string id)
    {
        return DataManager.Instance.GetHabitWithLink(id).ToList();
    }

    public LToDoEntry GetToDoEntry(string taskId)
    {
        var result = DataManager.Instance.GetToDoEntry(taskId);
        if (result.id == "")
        {
            return null;
        }
        return result;
    }

    public LProjectEntry GetProjectEntry(string taskId)
    {
        var result = DataManager.Instance.GetProjectEntry(taskId);
        if (result.id == "")
        {
            return null;
        }
        return result;
    }

    public LHabitEntry GetHabitEntry(string taskId)
    {
        return DataManager.Instance.GetHabitEntry(taskId);
    }

    public LProjectEntry GetProjectEntryWithLink(string linkedGoalId)
    {
        return DataManager.Instance.GetProjectEntryWithLink(linkedGoalId);
    }

    public List<LSubTask> GetSubTasks(LTaskEntry entry)
    {
        return DataManager.Instance.GetSubTasks(entry.subTasks).ToList();
    }

    public List<LSubTask> GetSubTasks(LAutoToDo entry)
    {
        return DataManager.Instance.GetSubTasks(entry.checkList).ToList();
    }

    public List<LSubTask> GetSubTasks(LToDoEntry entry)
    {
        return DataManager.Instance.GetSubTasks(entry.checkList).ToList();
    }

    public List<LSubTask> GetSubTasks(LProjectEntry entry)
    {
        return DataManager.Instance.GetSubTasks(entry.subProjects).ToList();
    }

    public List<LTaskEntry> GetDailyTasks()
    {
        var result = DataManager.Instance.CurrentDailyTasks.ToList();
        return result.OrderBy(t => t.orderId).ToList();
    }

    public List<LTaskEntry> GetDailyTasks(System.DateTime dateTime)
    {
        var taskList = DataManager.Instance.CurrentDailyTasks.ToList();
        List<LTaskEntry> list = new List<LTaskEntry>();
        foreach (LTaskEntry entry in taskList)
        {
            if (entry.IsAvailable(dateTime))
            {
                list.Add(entry);
            }
        }

        return taskList;
    }

    public List<LToDoEntry> GetToDos()
    {
        return DataManager.Instance.CurrentToDos.OrderBy(t => t.orderId).ToList();
    }

    public List<LAutoToDo> GetAutoToDos()
    {
        return DataManager.Instance.CurrentAutoToDos.OrderBy(t => t.orderId).ToList();
    }

    public List<LHabitEntry> GetHabits()
    { 
        return DataManager.Instance.CurrentHabits.OrderBy(t => t.orderId).ToList();
    }

    public List<LProjectEntry> GetProjects()
    {
        return DataManager.Instance.CurrentProjects.OrderBy(t => t.orderId).ToList();
    }

    public List<LAutoGoal> GetAutoGoals()
    {
        //return DataManager.Instance.CurrentAutoGoals.OrderBy(t => t.orderId).ToList();
        return DataManager.Instance.CurrentAutoGoals.OrderBy(t => t.endDate).ToList();
    }

    public void GetSubTasks(ESubEntryType type, string parentTaskId, System.Action<bool, string, List<FTask>> callback)
    {
        DataManager.Instance.GetSubTasks(type, parentTaskId, callback);
    }

    public void UpdateEntries(List<LTaskEntry> list)
    {
        var oldList = DataManager.Instance.CurrentDailyTasks.ToList();
        foreach (LTaskEntry entry in list)
        {
            var task = oldList.Find(it => it.id == entry.id);
            if (task != null)
            {
                oldList.Remove(task);
            }
            oldList.Add(task);
        }
        
        DataManager.Instance.CurrentDailyTasks = oldList;
    }

    public void UpdateEntries(List<LToDoEntry> list)
    {
        DataManager.Instance.CurrentToDos = list;
    }

    public void UpdateEntries(List<LProjectEntry> list)
    {
        DataManager.Instance.CurrentProjects = list;
    }

    public void UpdateEntries(List<LHabitEntry> list)
    {
        DataManager.Instance.CurrentHabits = list;
    }

    public void UpdateEntry(LTaskEntry entry, List<LSubTask> subTasks)
    {
        DataManager.Instance.UpdateEntry(entry, subTasks);
    }

    public void UpdateEntry(LToDoEntry entry, List<LSubTask> subTasks)
    {
        DataManager.Instance.UpdateEntry(entry, subTasks);
    }

    public void UpdateEntry(LAutoToDo entry, List<LSubTask> subTasks)
    {
        DataManager.Instance.UpdateEntry(entry, subTasks);
    }

    public void UpdateEntry(LHabitEntry entry)
    {
        DataManager.Instance.UpdateEntry(entry);
    }

    public void UpdateEntry(LSubTask entry)
    {
        DataManager.Instance.UpdateEntry(entry);
    }

    public void OnComplete(LSubTask entry, bool hasReward = true)
    {
        entry.OnComplete();
        UpdateEntry(entry);
        if (hasReward)
        {
            RewardSystem.Instance.OnComplete(entry);
        }
    }

    public void CancelComplete(LSubTask entry)
    {
        entry.CancelComplete();
        RewardSystem.Instance.CancelComplete(entry);
        UpdateEntry(entry);
    }

    public void OnComplete(LTaskEntry entry, bool hasReward)
    {
        entry.OnComplete(System.DateTime.Now);
        var subTasks = GetSubTasks(entry);
        foreach (LTask fTask in subTasks)
        {
            if (fTask.isCompleted() == false)
            {
                if (hasReward)
                {
                    RewardSystem.Instance.OnComplete(fTask);
                }
                fTask.OnComplete();
            }
        }

        UpdateEntry(entry, subTasks);

        if (hasReward)
        {
            RewardSystem.Instance.OnComplete(entry);
            /*
            if (entry.IsCompletedInAWeek())
            {
                RewardSystem.Instance.OnWeekTaskComplete();
            }*/

        }

        LUser currentUser = DataManager.Instance.GetCurrentUser();
        currentUser.completed_Tasks += 1;

        if (currentUser.completed_Tasks >= 5)
        {
            currentUser.completed_Tasks -= 5;
            //RewardSystem.Instance.OnFiveTaskComplete();
        }

        DataManager.Instance.UpdateUser(currentUser);
    }

    public void OnComplete(LHabitEntry entry)
    {
        entry.OnComplete(System.DateTime.Now);
        DataManager.Instance.UpdateEntry(entry);
    }

    public void OnComplete(List<LTaskEntry> fTaskEntries, System.DateTime dateTime)
    {
        //TODO
        
        foreach (LTaskEntry fTaskEntry in fTaskEntries)
        {
            fTaskEntry.OnComplete(dateTime);
        }

        foreach (LTaskEntry fTaskEntry in fTaskEntries)
        {
            if (fTaskEntry.skip_Dates.Contains(Convert.DateTimeToFDate(dateTime)))
            {
                //failed to dailytask
                RewardSystem.Instance.OnFailed(fTaskEntry);
            }
            else
            {
                RewardSystem.Instance.OnComplete(fTaskEntry);
                foreach (string fTaskId in fTaskEntry.subTasks)
                {
                    RewardSystem.Instance.OnComplete(1);
                }
            }

        }
        UpdateEntries(fTaskEntries);

        if (IsAllTaskCompleted(System.DateTime.Now.AddDays(-1)))
        {
            if (AppManager.Instance.GetCurrentMode() == (int)Game_Mode.Task_Only)
            {
                UserViewController.Instance.UpdateSetting(false);
            }

            DataManager.Instance.GetCurrentUser().updateDailyTaskDate(System.DateTime.Now.AddDays(-1));

            RewardSystem.Instance.OnAllDailyTaskComplete();
        }
    }

    public void OnComplete(LToDoEntry entry)
    {
        entry.OnComplete();
        var subTasks = GetSubTasks(entry);
        foreach (LSubTask fTask in subTasks)
        {
            if (fTask.completedDate == "")
            {
                RewardSystem.Instance.OnComplete(fTask);
                fTask.OnComplete();
            }
            fTask.SetRemoved(true);
        }
        entry.SetRemoved(true);
        entry.Update(subTasks);

        if (entry.isCompletedInTime())
        {
            RewardSystem.Instance.OnComplete(entry);
        }

        LUser currentUser = DataManager.Instance.GetCurrentUser();
        currentUser.completed_ToDos += 1;

        if (currentUser.completed_ToDos >= 3)
        {
            currentUser.completed_ToDos -= 3;
            //RewardSystem.Instance.OnThreeToDoComplete();
        }
    }

    public void OnComplete(LAutoToDo entry)
    {
        entry.OnComplete(System.DateTime.Now);
        var subTasks = GetSubTasks(entry);
        foreach (LTask fTask in subTasks)
        {
            if (fTask.isCompleted() == false)
            {
                RewardSystem.Instance.OnComplete(fTask);
                fTask.OnComplete();
            }
        }

        UpdateEntry(entry, subTasks);

        if (entry.isCompletedInTime())
        {
            RewardSystem.Instance.OnComplete(entry);
        }
    }

    public void OnComplete(LProjectEntry entry)
    {
        entry.OnComplete(System.DateTime.Now);
        bool hasCompletedByDueDate = false;
        if (Convert.FDateToDateTime(entry.completedDate).CompareTo(Convert.EntryDateToDateTime(entry.endDate)) <= 0)
        {
            hasCompletedByDueDate = true;
        }

        var subTasks = GetSubTasks(entry);
        foreach (LSubTask fTask in subTasks)
        {
            if (fTask.completedDate == "")
            {
                if (hasCompletedByDueDate)
                {
                    RewardSystem.Instance.OnComplete(fTask);
                }
                fTask.OnComplete();
            }
            fTask.SetRemoved(true);
        }
        entry.SetRemoved(true);

        var taskEntryList = GetDailyTaskWithLink(entry.id);

        if (hasCompletedByDueDate)
        {
            foreach (LTaskEntry fTaskEntry in taskEntryList)
            {
                List<string> skipDates = new List<string>();
                foreach (string date in fTaskEntry.skip_Dates)
                {
                    if (date.CompareTo(entry.beginDate) >= 0 && date.CompareTo(entry.endDate) <= 0)
                    {
                        skipDates.Add(date);
                    }
                }
                if (skipDates.Count == 0)
                {
                    RewardSystem.Instance.OnCompleteTaskEntryWithoutSkip(fTaskEntry);
                }
                fTaskEntry.linkedGoalId = "";
            }

            var habitEntryList = GetHabitWithLink(entry.id);
            foreach (LHabitEntry habitEntry in habitEntryList)
            {
                List<string> skipDates = new List<string>();
                foreach (string date in habitEntry.skip_Dates)
                {
                    if (date.CompareTo(entry.beginDate) >= 0 && date.CompareTo(entry.endDate) <= 0)
                    {
                        skipDates.Add(date);
                    }
                }
                if (skipDates.Count == 0)
                {
                    RewardSystem.Instance.OnCompleteHabitEntryWithoutSkip(habitEntry);
                }
                habitEntry.linkedGoalId = "";
                UpdateEntry(habitEntry);
            }
        }
        else
        {
            var habitEntryList = GetHabitWithLink(entry.id);
            foreach (LHabitEntry habitEntry in habitEntryList)
            {
                habitEntry.linkedGoalId = "";
                UpdateEntry(habitEntry);
            }
        }

        
        

        RewardSystem.Instance.OnComplete(entry);
        entry.Update(subTasks, taskEntryList);
    }

    public void CancelComplete(LTaskEntry entry)
    {
        
        RewardSystem.Instance.CancelComplete(entry);
        entry.CancelComplete();

        UpdateEntry(entry, new List<LSubTask>());
    }

    public List<LTaskEntry> GetUnCheckedTasks(System.DateTime dateTime)
    {
        List<LTaskEntry> taskList = new List<LTaskEntry>();

        var dailyTasks = GetDailyTasks();

        lastGetDate = System.DateTime.Now.Date;
        foreach (LTaskEntry fTaskEntry in dailyTasks)
        {
            if (fTaskEntry.completedDate.Equals("") && fTaskEntry.begin_date.Equals(Convert.DateTimeToFDate(System.DateTime.Now)))
            {
                continue;
            }

            if (fTaskEntry.completedDate.CompareTo(Convert.DateTimeToFDate(dateTime)) >= 0)
            {
                continue;
            }
            if (fTaskEntry.isEnabled(dateTime))
            {
                taskList.Add(fTaskEntry);
            }
        }

        return taskList;
    }

    public void CheckHabits(System.DateTime dateTime)
    {
        var habitList = GetHabits();
        foreach (LHabitEntry habitEntry in habitList)
        {
            habitEntry.OnCheckCompleted(dateTime);

            if (habitEntry.needReset(dateTime) == true)
            {
                if (habitEntry.bJustToday == true)
                {
                    habitEntry.SetRemoved(true);
                }
                //else
                //{
                //    habitEntry.Reset();
                //}

                UpdateEntry(habitEntry);
            }
        }
    }

    public bool IsAllTaskCompleted(System.DateTime dateTime)
    {
        if (DataManager.Instance.GetCurrentUser().GetDailyTaskDate() == Convert.DateTimeToFDate(dateTime))
        {
            return false;
        }

        var dailyTasks = GetDailyTasks();

        lastGetDate = System.DateTime.Now.Date;
        foreach (LTaskEntry fTaskEntry in dailyTasks)
        {
            if (fTaskEntry.IsAvailable(dateTime))
            {
                if (fTaskEntry.completedDate.Equals(""))
                {
                    return false;
                }

                if (fTaskEntry.skip_Dates.Contains(Convert.DateTimeToFDate(dateTime)))
                {
                    return false;
                }

                if (fTaskEntry.completedDate == Convert.DateTimeToFDate(dateTime))
                {
                    continue;
                }
                if (fTaskEntry.isEnabled(dateTime))
                {
                    return false;
                }
            }
        }

        return dailyTasks.Count == 0 ? false : true;
    }

    public void CheckDaily()
    {
        CheckDailyTask();
        CheckHabits(System.DateTime.Now.AddDays(-1));
    }

    

    #endregion

    #region Private Members
    private void CheckedOffAllTasksInAWeek()
    {
        foreach(LTaskEntry entry in GetDailyTasks())
        {
            if (!entry.IsCompletedInAWeek())
            {
                return;
            }
        }
        foreach(LHabitEntry entry in GetHabits())
        {
            if (!entry.IsCompletedInAWeek())
            {
                return;
            }
        }

        var currentUser = UserViewController.Instance.GetCurrentUser();
        var lastCheckedDate = Convert.FDateToDateTime(currentUser.GetDate(EDates.WeeklyTaskHabit));
        if (Utilities.GetDays(lastCheckedDate) >= 7)
        {
            UserViewController.Instance.GetCurrentUser().updateWeeklyTaskDate(System.DateTime.Now);
            ArtworkSystem.Instance.Pick(EArtworkReason.WeeklyTaskComplete);
            //RewardSystem.Instance.OnCompleteAllTasksInAWeek();
        }
    }

    private void CheckDailyTask()
    {

        var mode = AppManager.Instance.GetCurrentMode();

        if (UserViewController.Instance.GetCurrentUser().mode_at != Convert.DateTimeToFDate(System.DateTime.Now))
        {
            if (IsAllTaskCompleted(System.DateTime.Now.AddDays(-1)))
            {
                if (mode == (int)Game_Mode.Task_Only)
                {
                    UserViewController.Instance.UpdateSetting(false);
                }

                DataManager.Instance.GetCurrentUser().updateDailyTaskDate(System.DateTime.Now.AddDays(-1));

                RewardSystem.Instance.OnAllDailyTaskComplete();

            }
            else
            {
                var yesterdayTaskList = GetUnCheckedTasks(System.DateTime.Now.AddDays(-1));

                if (yesterdayTaskList.Count > 0)
                {
                    if (mode == (int)Game_Mode.Task_Only)
                    {
                        var dailyTasks = GetDailyTasks();
                        var uncompleteness = (float)(yesterdayTaskList.Count) / (float)(dailyTasks.Count);
                        if (uncompleteness > 0.5f)
                        {
                            UserViewController.Instance.UpdateSetting(true);
                        }
                    }
                    UIManager.Instance.ShowUncheckedTaskDlg();
                }
            }
        }
        

        UIManager.Instance.ShowReport();

        if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Sunday && DataManager.Instance.GetCurrentUser().GetDailyTaskDate() != Convert.DateTimeToFDate(System.DateTime.Now)) 
        {
            CheckedOffAllTasksInAWeek();
        }

    }
    #endregion
}
