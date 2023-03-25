using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskLinkListPanel : MonoBehaviour
{
    [SerializeField] private GameObject taskItemPrefab;
    [SerializeField] private Transform taskListGroup;


    #region Unity Members
    // Start is called before the first frame update
    private void OnEnable()
    {
        var taskList = TaskViewController.Instance.GetDailyTasks();
        CreateTaskList(taskList);
    }

    private void OnDisable()
    {
        
    }
    #endregion


    #region Public Members

    public void SelectedTask(LTaskEntry task)
    {
        task.SetRemoved(false);
        transform.parent.GetComponent<ProjectEntryPage>().SelectedTask(task);
    }

    public void SelectedTask(LHabitEntry task)
    {
        task.SetRemoved(false);
        transform.parent.GetComponent<ProjectEntryPage>().SelectedTask(task);
    }
    #endregion

    #region Private Members
    private void CreateTaskList(List<LTaskEntry> taskList)
    {
        DeleteTaskList();
        taskListGroup.gameObject.SetActive(true);
        for (int index = 0; index < taskList.Count; index++)
        {
            LTaskEntry taskEntry = taskList[index];
            if (taskEntry.linkedGoalId == "" && (Repeatition)taskEntry.repeatition != Repeatition.Yearly)
            {
                GameObject subItemObj = GameObject.Instantiate(taskItemPrefab, taskListGroup);
                subItemObj.GetComponent<LinkToTaskItem>().SetTask(taskList[index], this);
            }
        }
    }

    private void DeleteTaskList()
    {
        foreach (Transform child in taskListGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion
}
