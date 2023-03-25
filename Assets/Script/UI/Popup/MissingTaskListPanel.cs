using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissingTaskListPanel : PopUpDlg
{
    [SerializeField] private GameObject taskItemPrefab;
    [SerializeField] private Transform taskListGroup;
    
    private List<LTaskEntry> fTaskEntries = new List<LTaskEntry>();
    private System.DateTime dateTime = System.DateTime.MinValue;
    private List<MissingTaskItem> missingTaskItems = new List<MissingTaskItem>();

    #region Public Members

    private void OnEnable()
    {
        
    }

    public override void Show()
    {
        base.Show();
        
        var tasks = TaskViewController.Instance.GetUnCheckedTasks(System.DateTime.Now.AddDays(-1));
        Load(tasks, System.DateTime.Now.AddDays(-1));
    }

    public void Load(List<LTaskEntry> taskEntries, System.DateTime dateTime)
    {
        fTaskEntries = taskEntries;
        this.dateTime = dateTime;

        CreateTaskList(taskEntries);
    }

    public void StartMyDay()
    {
        PlayerPrefs.SetString(type.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
        CheckOffAllTasks();
        Back();
    }
    #endregion

    #region Private Members
    private void CreateTaskList(List<LTaskEntry> dailyTasks)
    {
        DeleteTaskList();
        taskListGroup.gameObject.SetActive(true);

        missingTaskItems.Clear();
        for (int index = 0; index < dailyTasks.Count; index++)
        {
            GameObject subItemObj = GameObject.Instantiate(taskItemPrefab, taskListGroup);
            subItemObj.GetComponent<MissingTaskItem>().SetTask(dailyTasks[index]);
            missingTaskItems.Add(subItemObj.GetComponent<MissingTaskItem>());
        }
    }

    private void DeleteTaskList()
    {
        foreach (Transform child in taskListGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CheckOffAllTasks()
    {
        List<LTaskEntry> entries = new List<LTaskEntry>();
        foreach(MissingTaskItem item in missingTaskItems)
        {
            LTaskEntry entry = item.GetTask();
            if (!item.isChecked())
            {
                entry.skip_Dates.Add(Convert.DateTimeToFDate(this.dateTime));
            }
            entries.Add(entry);
        }

        TaskViewController.Instance.OnComplete(entries, this.dateTime);
    }
    #endregion
}
