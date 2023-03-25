using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DailyTaskItem : EntryItem
{
    private LTaskEntry task = null;

    protected override void Start()
    {
        base.Start();
        sublist_Toggle.onValueChanged.AddListener(delegate
        {
            ShowSubTaskList(sublist_Toggle);
        });

        complete_Toggle.onValueChanged.AddListener(delegate
        {
            OnComplete(complete_Toggle);
        });
    }

    #region Public Members
    public override void ShowSubTaskList(bool bShow)
    {
        if (bShow)
        {
            CreateSubTasks();
        }
        else
        {
            DeleteSubTasks();
        }
    }

    public void SetTask(LTaskEntry task)
    {
        this.task = task;
        ReloadUI();
    }

    override public void ReloadUI()
    {
        this.subTaskList = TaskViewController.Instance.GetSubTasks(task);
        var uncompletedSubTasks = this.subTaskList.FindAll(item => item.isCompleted() == false).ToList();
        if (task.subTasks.Count > 0)
        {
            taskNameIF.text = task.taskName + " {" + uncompletedSubTasks.Count + "}";
        }
        else
        {
            taskNameIF.text = string.Format("{0}", task.taskName);
        }


        float ratio = UnityEngine.Screen.height / (float)UnityEngine.Screen.width;
        taskNameIF.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1330.0f / ratio, 0f);
        complete_Toggle.isOn = this.task.isCompleted();
        if (UserViewController.Instance.GetCurrentSetting().shelter_storm)
        {
            UpdateCanvasGroup(false);
        }
        else
        {
            UpdateCanvasGroup(this.task.isEnabled(System.DateTime.Now));
            var isAvailable = this.task.IsAvailable(System.DateTime.Now);
            toggleCanvasGroup.blocksRaycasts = isAvailable;
            toggleCanvasGroup.interactable = isAvailable;
        }
    }

    public LTaskEntry GetTask()
    {
        return this.task;
    }

    public void EditTask()
    {
        if (UserViewController.Instance.GetCurrentSetting().current_mode != (int)Game_Mode.Game_Only)
        {
            transform.GetComponentInParent<NavigationPage>().Show<TaskEntryPage>("task_entry", this.task);
        }
    }

    #endregion
    #region Private Members


    private void ShowSubTaskList(Toggle toggle)
    {
        ShowSubTaskList(toggle.isOn);
    }

    private void OnComplete(Toggle toggle)
    {
        if (toggle.isOn == true && task.isEnabled() == true)
        {
            TaskViewController.Instance.OnComplete(task, true);
            UpdateCanvasGroup(false);
        }

        if (toggle.isOn == false)
        {
            TaskViewController.Instance.CancelComplete(task);
            UpdateCanvasGroup(true);
        }
        ReloadUI();
    }
    #endregion

    

}
