using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmSettingPage : Page
{
    [SerializeField] private IOSToggle dailyTask_TG;
    [SerializeField] private IOSToggle toDo_TG;
    [SerializeField] private IOSToggle habit_TG;
    [SerializeField] private IOSToggle goal_TG;

    private LSetting currentSetting = null;
    // Start is called before the first frame update
    void Start()
    {
        dailyTask_TG.OnValueChanged += DailyTask_TG_OnValueChanged;
        toDo_TG.OnValueChanged += ToDo_TG_OnValueChanged;
        habit_TG.OnValueChanged += Habit_TG_OnValueChanged;
        goal_TG.OnValueChanged += Goal_TG_OnValueChanged;
    }

    private void OnEnable()
    {
        currentSetting = DataManager.Instance.GetCurrentSetting();
        dailyTask_TG.SetState(currentSetting.alarm_dt);
        toDo_TG.SetState(currentSetting.alarm_td);
        habit_TG.SetState(currentSetting.alarm_ht);
        goal_TG.SetState(currentSetting.alarm_goal);
    }

    private void Goal_TG_OnValueChanged(bool isOn)
    {
        if (currentSetting != null && currentSetting.alarm_goal != isOn)
        {
            currentSetting.UpdateAlarm(isOn, EntryType.Project);
            NotificationManager.Instance.RescheduleGoalNotification();
        }
    }

    private void Habit_TG_OnValueChanged(bool isOn)
    {
        if (currentSetting != null && currentSetting.alarm_ht != isOn)
        {
            currentSetting.UpdateAlarm(isOn, EntryType.Habit);
            NotificationManager.Instance.RescheduleHabitNotification();
        }
    }

    private void ToDo_TG_OnValueChanged(bool isOn)
    {
        if (currentSetting != null && currentSetting.alarm_td != isOn)
        {
            currentSetting.UpdateAlarm(isOn, EntryType.ToDo);
            NotificationManager.Instance.RescheduleToDoNotification();
        }
    }

    private void DailyTask_TG_OnValueChanged(bool isOn)
    {
        if (currentSetting != null && currentSetting.alarm_dt != isOn)
        {
            currentSetting.UpdateAlarm(isOn, EntryType.DailyTask);
            NotificationManager.Instance.RescheduleDailyTaskNotification();
        }
    }

    public void OnBack()
    {
        GetComponentInParent<NavPage>().Back();
    }
}
