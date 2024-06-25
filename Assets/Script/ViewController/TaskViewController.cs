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

    public void OnComplete(LHabitEntry entry)
    {
        entry.OnComplete(System.DateTime.Now);
        DataManager.Instance.UpdateEntry(entry);
    }

    public void OnComplete(List<LTaskEntry> fTaskEntries, System.DateTime dateTime)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CheckOffYesterdayTask(fTaskEntries, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
        
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

        RewardSystem.Instance.OnComplete(entry); //2023/05/23 by pooh

        /*
        LUser currentUser = DataManager.Instance.GetCurrentUser();
        currentUser.completed_ToDos += 1;

        if (currentUser.completed_ToDos >= 3)
        {
            currentUser.completed_ToDos -= 3;
            //RewardSystem.Instance.OnThreeToDoComplete();
        }*/
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

    public List<LTaskEntry> GetUnCheckedTasks(System.DateTime dateTime)
    {
        return DataManager.Instance.GetYesterdayTasks();
        /*
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

        return taskList;*/
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


    public void CreateDailyTask(LTaskEntry entry, List<LSubTask> subTasks, System.Action<bool> callback)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CreateDailyTask(entry, subTasks, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);            
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void RemoveDailyTask(LTaskEntry entry, System.Action<bool> callback)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.RemoveDailyTask(entry, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void CompleteDailyTask(LTaskEntry entry, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CompleteDailyTask(entry, (isSuccess, errMsg, reward) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowReward(reward);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void CancelDailyTaskComplete(LTaskEntry entry, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CancelDailyTaskComplete(entry, (isSuccess, errMsg, reward) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowReward(reward);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void ArrangeDailyTask(List<LTaskEntry> entryList, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.ArrangeDailyTask(entryList, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void CompleteSubTask(LSubTask subTask, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CompleteSubTask(subTask, (isSuccess, errMsg, reward) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowReward(reward);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void CancelSubTaskComplete(LSubTask subTask, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CancelSubTaskComplete(subTask, (isSuccess, errMsg, reward) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowReward(reward);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void CreateToDo(LToDoEntry entry, List<LSubTask> subTasks, System.Action<bool> callback)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CreateToDo(entry, subTasks, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void RemoveToDo(LToDoEntry entry, System.Action<bool> callback)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.RemoveToDo(entry, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void CompleteToDo(LToDoEntry entry, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CompleteToDo(entry, (isSuccess, errMsg, reward) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowReward(reward);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void CompleteAutoToDo(LAutoToDo entry, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CompleteAutoToDo(entry.id, (isSuccess, errMsg, reward) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowReward(reward);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void ArrangeToDo(List<LToDoEntry> entryList, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.ArrangeToDo(entryList, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void CreateHabit(LHabitEntry entry, System.Action<bool> callback)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CreateHabit(entry, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void RemoveHabit(LHabitEntry entry, System.Action<bool> callback)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.RemoveHabit(entry, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void CompleteHabit(LHabitEntry entry, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CompleteHabit(entry, (isSuccess, errMsg, reward) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowReward(reward);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void CancelHabitComplete(LHabitEntry entry, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CancelHabitComplete(entry, (isSuccess, errMsg, reward) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowReward(reward);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void ArrangeHabit(List<LHabitEntry> entryList, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.ArrangeHabit(entryList, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void CreateProject(LProjectEntry entry, List<LSubTask> subTasks, System.Action<bool> callback)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CreateProject(entry, subTasks, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void RemoveProject(LProjectEntry entry, System.Action<bool> callback)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.RemoveProject(entry, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void CompleteProject(LProjectEntry entry, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.CompleteProject(entry, (isSuccess, errMsg, reward) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowReward(reward);
            }
            callback?.Invoke(isSuccess);
        });
    }

    public void ArrangeProject(List<LProjectEntry> entryList, System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.ArrangeProject(entryList, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            callback?.Invoke(isSuccess);
        });
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
            var yesterdayTaskList = DataManager.Instance.GetYesterdayTasks();
            Debug.LogError(yesterdayTaskList.Count);
            if (yesterdayTaskList.Count > 0)
            {
                UIManager.Instance.ShowUncheckedTaskDlg();
            }
        }
        

        //UIManager.Instance.ShowReport();

        if (System.DateTime.Now.DayOfWeek == System.DayOfWeek.Sunday && DataManager.Instance.GetCurrentUser().GetDailyTaskDate() != Convert.DateTimeToFDate(System.DateTime.Now)) 
        {
            CheckedOffAllTasksInAWeek();
        }

    }
    #endregion
}
