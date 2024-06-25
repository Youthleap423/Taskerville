using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.iOS;
using EasyMobile;
using EasyMobile.MiniJSON;

public class NotificationManager : SingletonComponent<NotificationManager>
{
    override protected void Awake()
    {
        base.Awake();
        if (!RuntimeManager.IsInitialized())
        {
            RuntimeManager.Init();
        }
            
    }

    private void Start()
    {
        if (!Notifications.IsInitialized())
        {
            Notifications.SetAppIconBadgeNumber(0);
            Notifications.Init();
        }
        
    }

    public void RescheduleAllTaskNotification()
    {
        RescheduleDailyTaskNotification();
        RescheduleToDoNotification();
        RescheduleHabitNotification();
        RescheduleGoalNotification();
    }

    public void RescheduleTaskNotification(EntryType entryType)
    {
        switch (entryType)
        {
            case EntryType.DailyTask:
                RescheduleDailyTaskNotification();
                break;
            case EntryType.ToDo:
                RescheduleToDoNotification();
                break;
            case EntryType.Habit:
                RescheduleHabitNotification();
                break;
            case EntryType.Project:
                RescheduleGoalNotification();
                break;
            default:
                break;
        }
    }

    public void RescheduleLocalNotification(LEntry entry)
    {
        switch (entry.Type)
        {
            case EntryType.DailyTask:
                var taskEntry = TaskViewController.Instance.GetDailyTask(entry.id);
                ReScheduleLocalNotification(taskEntry);
                break;
            case EntryType.ToDo:
                var toDoEntry = TaskViewController.Instance.GetToDoEntry(entry.id);
                ReScheduleLocalNotification(toDoEntry);
                break;
            case EntryType.Habit:
                var habitEntry = TaskViewController.Instance.GetHabitEntry(entry.id);
                ReScheduleLocalNotification(habitEntry);
                break;
            case EntryType.Project:
                var projectEntry = TaskViewController.Instance.GetProjectEntry(entry.id);
                ReScheduleLocalNotification(projectEntry);
                break;
            default:
                break;
        }
    }

    public void RescheduleDailyTaskNotification()
    {
        var currentSetting = DataManager.Instance.GetCurrentSetting();
        if (currentSetting.alarm_dt == false)
        {
            Notifications.GetPendingLocalNotifications(pendingNotifs =>
            {
                foreach (var req in pendingNotifs)
                {
                    NotificationContent content = req.content;
                    var type = "";
                    if (req.content.userInfo.TryGetValue("type", out object typeObj))
                    {
                        type = System.Convert.ToString(typeObj);
                    };
                    if (type == EntryType.DailyTask.ToString())
                    {
                        
                        Notifications.CancelPendingLocalNotification(req.id);
                    }
                }
            });
        }
        else
        {
            foreach (LTaskEntry entry in DataManager.Instance.CurrentDailyTasks)
            {
                if (entry.repeatition == (int)Repeatition.Daily && entry.repeat_every == 1)
                {
                    ScheduleLocalNotification(entry, entry.GetTriggerDate(), NotificationRepeat.EveryDay);
                }
                else
                {
                    ScheduleLocalNotification(entry, entry.GetTriggerDate());
                }
                
            }
        }
    }

    public void RescheduleToDoNotification()
    {
        var currentSetting = DataManager.Instance.GetCurrentSetting();
        if (currentSetting.alarm_td == false)
        {
            Notifications.GetPendingLocalNotifications(pendingNotifs =>
            {
                foreach (var req in pendingNotifs)
                {
                    NotificationContent content = req.content;
                    var type = "";
                    if (req.content.userInfo.TryGetValue("type", out object typeObj))
                    {
                        type = System.Convert.ToString(typeObj);
                    };
                    if (type == EntryType.ToDo.ToString())
                    {
                        Notifications.CancelPendingLocalNotification(req.id);
                    }
                }
            });
        }
        else
        {
            foreach (LToDoEntry entry in DataManager.Instance.CurrentToDos)
            {
                ScheduleLocalNotification(entry, entry.GetTriggerDate(), NotificationRepeat.EveryDay);
            }
        }
    }

    public void RescheduleHabitNotification()
    {
        var currentSetting = DataManager.Instance.GetCurrentSetting();
        if (currentSetting.alarm_ht == false)
        {
            Notifications.GetPendingLocalNotifications(pendingNotifs =>
            {
                foreach (var req in pendingNotifs)
                {
                    NotificationContent content = req.content;
                    var type = "";
                    if (req.content.userInfo.TryGetValue("type", out object typeObj))
                    {
                        type = System.Convert.ToString(typeObj);
                    };
                    if (type == EntryType.Habit.ToString())
                    {
                        Notifications.CancelPendingLocalNotification(req.id);
                    }
                }
            });
        }
        else
        {
            foreach (LHabitEntry entry in DataManager.Instance.CurrentHabits)
            {
                if (entry.repeatition == (int)Repeatition.Daily && entry.repeat_every == 1 && entry.recurrence)
                {
                    ScheduleLocalNotification(entry, entry.GetTriggerDate(), NotificationRepeat.EveryDay);
                }
                else
                {
                    ScheduleLocalNotification(entry, entry.GetTriggerDate());
                }
            }
        }
    }

    public void RescheduleGoalNotification()
    {
        var currentSetting = DataManager.Instance.GetCurrentSetting();
        if (currentSetting.alarm_goal == false)
        {
            Notifications.GetPendingLocalNotifications(pendingNotifs =>
            {
                foreach (var req in pendingNotifs)
                {
                    NotificationContent content = req.content;
                    var type = "";
                    if (req.content.userInfo.TryGetValue("type", out object typeObj))
                    {
                        type = System.Convert.ToString(typeObj);
                    };
                    if (type == EntryType.Project.ToString())
                    {
                        Notifications.CancelPendingLocalNotification(req.id);
                    }
                }
            });
        }
        else
        {
            foreach (LProjectEntry entry in DataManager.Instance.CurrentProjects)
            {
                ScheduleLocalNotification(entry, entry.GetTriggerDate(), NotificationRepeat.EveryDay);
            }
        }
    }

    public void ScheduleLocalNotification(LEntry entry, DateTime triggerDate, NotificationRepeat repeat = NotificationRepeat.None)
    {
        if (entry.IsRemoved())
        {
            return;
        }

        if (!InitCheck())
        {
            Notifications.Init();
        }

        var notif = new NotificationContent();
        notif.title = "Taskerville";
        notif.subtitle = "Notification";
        notif.body = entry.taskName;

        notif.userInfo = new Dictionary<string, object>();
        notif.userInfo.Add("id", entry.id);
        notif.userInfo.Add("type", entry.Type.ToString());
        //notif.userInfo.Add("entry", JsonUtility.ToJson(entry));
        notif.userInfo.Add("repeat", repeat.ToString());
        notif.categoryId = entry.Type.ToString();
        if (triggerDate != DateTime.MinValue)
        {
            TimeSpan timeSpan = triggerDate < DateTime.Now ? TimeSpan.Zero : triggerDate - DateTime.Now;
            if (repeat == NotificationRepeat.None)
            {
                Notifications.ScheduleLocalNotification(entry.Type.ToString() + entry.id, timeSpan, notif);
            }
            else
            {
                Notifications.ScheduleLocalNotification(entry.Type.ToString() + entry.id, timeSpan, notif, repeat);
            }
        }
    }

    public void RescheduleLocalNotification(NotificationContent content)
    {
        var type = "";
        if (content.userInfo.TryGetValue("type", out object typeObj))
        {
            type = System.Convert.ToString(typeObj);
        };

        if (type == "Unit" || type == EntryType.ToDo.ToString())
        {
            return;
        }

        var id = "";
        if (content.userInfo.TryGetValue("type", out object idObj))
        {
            id = System.Convert.ToString(idObj);
        };

        string repeatStr = "";
        if (content.userInfo.TryGetValue("repeat", out object repeatObj))
        {
            repeatStr = System.Convert.ToString(repeatObj);
        };

        if (repeatStr == NotificationRepeat.EveryDay.ToString())
        {
            return;
        }

        var entry = DataManager.Instance.GetCorrespondingEntry(id, type);

        
        if (entry == null || entry.id == "" /*|| entry.IsRemoved() || entry.isCompleted()*/)
        {
            return;
        }

        RescheduleLocalNotification(entry);
        
        /*
        if (entry.repeat_alarm != 0)
        {
            float times = Mathf.Pow(2, (entry.repeat_alarm - 1));
            content.body = entry.taskName;
            TimeSpan timeSpan = new TimeSpan(0, Mathf.FloorToInt(times * 15), 0); 
            Notifications.ScheduleLocalNotification(timeSpan, content);
        }

        Notifications.GetPendingLocalNotifications(pendingNotifs =>
        {
            UIManager.LogError(pendingNotifs.Length);
            foreach (var req in pendingNotifs)
            {
                NotificationContent content = req.content;

                var strId = "";
                if (req.content.userInfo.TryGetValue("id", out object entryId))
                {
                    strId = System.Convert.ToString(entryId);
                };

                var type = "";
                if (req.content.userInfo.TryGetValue("type", out object typeObj))
                {
                    type = System.Convert.ToString(typeObj);
                };

                UIManager.LogError(strId + " : " + type + ":" + req.repeat.ToString() + ":" + req.nextTriggerDate.ToString() + ":" + req.content.categoryId + ":" + req.id);
            }
        });
        */
    }

    public void ScheduleUnitLocalNotification(LHabitEntry entry)
    {
        if (entry.timespan == false)
        {
            return;
        }

        if (DataManager.Instance.GetCurrentSetting().alarm_ht == false)
        {
            return;
        }

        var notif = new NotificationContent();
        notif.title = "Taskerville";
        notif.subtitle = "Notification";
        notif.body = entry.taskName;

        notif.userInfo = new Dictionary<string, object>();
        notif.userInfo.Add("id", entry.id);
        notif.userInfo.Add("type", "Unit");
        notif.categoryId = "Unit";
        Notifications.ScheduleLocalNotification("Unit" + entry.id, new TimeSpan(System.Convert.ToInt32(entry.span_h), System.Convert.ToInt32(entry.span_m), 0), notif);
    }

    public void ReScheduleLocalNotification(LTaskEntry entry)
    {
        if (!InitCheck())
        {
            Notifications.Init();
        }

        if (entry == null || entry.id == "")
        {
            return;
        }

        CancelPendingLocalNotification(entry, (result) =>
        {
            if (entry.remindAlarm.Trim().Equals(String.Empty))
            {
                return;
            }

            if (DataManager.Instance.GetCurrentSetting().alarm_dt == true)
            {
                if (entry.repeatition == (int)Repeatition.Daily && entry.repeat_every == 1)
                {
                    ScheduleLocalNotification(entry, entry.GetTriggerDate(), NotificationRepeat.EveryDay);
                }
                else
                {
                    ScheduleLocalNotification(entry, entry.GetTriggerDate());
                }
            }
        });
    }

    public void ReScheduleLocalNotification(LToDoEntry entry)
    {
        if (!InitCheck())
        {
            Notifications.Init();
        }


        if (entry == null || entry.id == "")
        {
            return;
        }

        CancelPendingLocalNotification(entry, (result) =>
        {
            if (entry.IsRemoved() || entry.isCompleted())
            {
                return;
            }

            if (entry.remindAlarm.Trim().Equals(String.Empty))
            {
                return;
            }

            if (DataManager.Instance.GetCurrentSetting().alarm_td == true)
            {
                ScheduleLocalNotification(entry, entry.GetTriggerDate());
            }
        });
    }


    public void ReScheduleLocalNotification(LHabitEntry entry)
    {
        if (!InitCheck())
        {
            Notifications.Init();
        }

        if (entry == null || entry.id == "")
        {
            return;
        }

        
        CancelPendingLocalNotification(entry, (result) =>
        {
            if (entry.remindAlarm.Trim().Equals(String.Empty))
            {
                return;
            }

            if (DataManager.Instance.GetCurrentSetting().alarm_ht == true)
            {
                if (entry.repeatition == (int)Repeatition.Daily && entry.repeat_every == 1 && entry.recurrence)
                {
                    ScheduleLocalNotification(entry, entry.GetTriggerDate(), NotificationRepeat.EveryDay);
                }
                else
                {
                    ScheduleLocalNotification(entry, entry.GetTriggerDate());
                }
            }
        });
    }

    public void ReScheduleUnitLocalNotification(LHabitEntry entry)
    {
        if (entry.timespan == false)
        {
            return;
        }

        CancelPendingUnitLocalNotification(entry);
        ScheduleUnitLocalNotification(entry);
    }

    public void ReScheduleLocalNotification(LProjectEntry entry)
    {
        if (!InitCheck())
        {
            Notifications.Init();
        }

        if (entry == null || entry.id == "")
        {
            return;
        }

        CancelPendingLocalNotification(entry, (result) =>
        {
            if (entry.remindAlarm.Trim().Equals(String.Empty))
            {
                return;
            }

            if (DataManager.Instance.GetCurrentSetting().alarm_goal == true)
            {
                ScheduleLocalNotification(entry, entry.GetTriggerDate());
            }
        });
    }

    public void CancelPendingLocalNotification(LEntry entry, System.Action<bool> callback)
    {
        Notifications.GetPendingLocalNotifications(pendingNotifs =>
        {
            var result = false;
            foreach (var req in pendingNotifs)
            {
                NotificationContent content = req.content;

                var strId = "";
                if (req.content.userInfo.TryGetValue("id", out object entryId)) {
                    strId = System.Convert.ToString(entryId);
                };

                var type = "";
                if (req.content.userInfo.TryGetValue("type", out object typeObj))
                {
                    type = System.Convert.ToString(typeObj);
                };

                if (strId != "" && strId == entry.id && type == entry.Type.ToString())
                {
                    CancelPendingLocalNotification(req.id);
                    result = true;
                    break;
                }
            }
            callback(result);
        });
    }

    public void CancelPendingUnitLocalNotification(LHabitEntry entry)
    {
        Notifications.GetPendingLocalNotifications(pendingNotifs =>
        {
            foreach (var req in pendingNotifs)
            {
                NotificationContent content = req.content;

                var strId = "";
                if (req.content.userInfo.TryGetValue("id", out object entryId))
                {
                    strId = System.Convert.ToString(entryId);
                };

                if (content.categoryId == "Unit" && strId != "" && strId == entry.id)
                {
                    Notifications.CancelPendingLocalNotification(req.id);
                }
            }
        });
    }

    public void CancelPendingLocalNotification(string str)
    {
        if (!InitCheck())
            Notifications.Init();

        if (string.IsNullOrEmpty(str))
        {
            return;
        }

        Notifications.CancelPendingLocalNotification(str);
    }

    public void CancelAllPendingLocalNotifications()
    {
        if (!InitCheck())
            Notifications.Init();

        Notifications.CancelAllPendingLocalNotifications();
    }

    public void RemoveAllDeliveredNotifications()
    {
        Notifications.ClearAllDeliveredNotifications();
    }

    bool InitCheck()
    {
        bool isInit = Notifications.IsInitialized();
        return isInit;
    }

    void UpdatePendingNotificationList()
    {
        Notifications.GetPendingLocalNotifications(pendingNotifs =>
        {
            StringBuilder sb = new StringBuilder();
            foreach (var req in pendingNotifs)
            {
                NotificationContent content = req.content;

                sb.Append("ID: " + req.id.ToString() + "\n")
                    .Append("Title: " + content.title + "\n")
                    .Append("Subtitle: " + content.subtitle + "\n")
                    .Append("Body: " + content.body + "\n")
                    .Append("Badge: " + content.badge.ToString() + "\n")
                    .Append("UserInfo: " + Json.Serialize(content.userInfo) + "\n")
                    .Append("CategoryID: " + content.categoryId + "\n")
                    .Append("NextTriggerDate: " + req.nextTriggerDate.ToShortDateString() + "\n")
                    .Append("Repeat: " + req.repeat.ToString() + "\n")
                    .Append("-------------------------\n");
            }

            var listText = sb.ToString();

            // Display list of pending notifications
            
        });
    }

    
}

