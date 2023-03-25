using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyTaskPage : Page
{
    [SerializeField] private GameObject taskItemPrefab;
    [SerializeField] private Transform taskListGroup;
    [SerializeField] private GameObject newButton;

    [Space(10)]
    [SerializeField] private Slot slotComponent;
    #region Unity_Members
    // Start is called before the first frame update
    void Start()
    {
        

    }

    private void OnEnable()
    {
        slotComponent.OnDragDropFinished += Rearrange;
        Initialize();
    }
    private void OnDisable()
    {
        slotComponent.OnDragDropFinished -= Rearrange;
    }
    #endregion

    #region Public Members
    public override void Initialize()
    {
        base.Initialize();

        if (UserViewController.Instance.GetCurrentSetting().current_mode == (int)Game_Mode.Game_Only)
        {
            newButton.SetActive(false);
        }
        else
        {
            newButton.SetActive(true);
        }

        var dailyTasks = TaskViewController.Instance.GetDailyTasks();
        CreateTaskList(dailyTasks);
    }

    public void ShowTaskEntryPage()
    {
        transform.parent.GetComponent<NavigationPage>().Show("task_entry");
    }

    public void Rearrange(bool bFlag)
    {
        List<LTaskEntry> entryList = new List<LTaskEntry>();
        foreach (Transform child in taskListGroup)
        {
            DailyTaskItem item = child.gameObject.GetComponent<DailyTaskItem>();
            if (item == null)
            {
                continue;
            }

            LTaskEntry entry = item.GetTask();
            int itemIndex = child.gameObject.GetComponentInChildren<DragHandler>().getIndex();
            entry.orderId = itemIndex;
            entryList.Add(entry);
        }

        TaskViewController.Instance.UpdateEntries(entryList);
    }

    public void ShowInfoPage()
    {
        TutorialManager.Instance.ShowInfo(0);
    }
    #endregion

    #region Private Members
    private void CreateTaskList(List<LTaskEntry> dailyTasks)
    {
        DeleteTaskList();
        taskListGroup.gameObject.SetActive(true);
        for (int index = 0; index < dailyTasks.Count; index++)
        {
            if (AppManager.Instance.GetCurrentMode() == Game_Mode.Game_Only && !dailyTasks[index].IsAvailable(System.DateTime.Now)) {
                continue;
            }

            GameObject subItemObj = GameObject.Instantiate(taskItemPrefab, taskListGroup);
            subItemObj.GetComponent<DailyTaskItem>().SetTask(dailyTasks[index]);
            subItemObj.GetComponentInChildren<DragHandler>().setIndex(index);
            slotComponent.items.Add(subItemObj);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(taskListGroup.transform.GetComponent<RectTransform>());
    }

    public void SerializeToJson()
    {
        
    }

    private void DeleteTaskList()
    {
        foreach (Transform child in taskListGroup.transform)
        {
            Destroy(child.gameObject);
        }

        slotComponent.items.Clear();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Initialize();
        }
    }
    #endregion
}