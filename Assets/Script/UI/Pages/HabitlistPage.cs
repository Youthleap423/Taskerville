using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HabitlistPage : Page
{
    [SerializeField] private GameObject taskItemPrefab;
    [SerializeField] private Transform taskListGroup;
    [SerializeField] private GameObject newButton;
    [SerializeField] private GameObject infoGlowEffectObj;

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
        TaskManager.OnDeviceOrientationChanged += onDeviceOrientationChanged;
        Initialize();
    }
    private void OnDisable()
    {
        slotComponent.OnDragDropFinished -= Rearrange;
        TaskManager.OnDeviceOrientationChanged -= onDeviceOrientationChanged;
    }

    private void onDeviceOrientationChanged(ScreenOrientation orientation)
    {
        Initialize();
    }
    #endregion

    #region Public Members
    public override void Initialize()
    {
        base.Initialize();

        //PlayerPrefs.SetString("NotifyPage", "");

        infoGlowEffectObj.SetActive(UserViewController.Instance.GetCurrentUser().GetPlayerAgesAsDays() < 7);

        if (UserViewController.Instance.GetCurrentSetting().current_mode == (int)Game_Mode.Game_Only)
        {
            newButton.SetActive(false);
        }
        else
        {
            newButton.SetActive(true);
        }

        var habitList = TaskViewController.Instance.GetHabits();
        foreach (LHabitEntry habitEntry in habitList)
        {
            if (habitEntry.needReset(System.DateTime.Now) == true)
            {
                
                habitEntry.Reset();

                if (habitEntry.bJustToday == true)
                {
                    habitEntry.SetRemoved(true);
                }
                TaskViewController.Instance.UpdateEntry(habitEntry);
            }
        }
        
        CreateTaskList(TaskViewController.Instance.GetHabits());
    }

    public void Reload()
    {
        Initialize();
    }

    public void ShowEntryPage()
    {
        transform.parent.GetComponent<NavigationPage>().Show("habit_entry");
    }

    public void Rearrange(bool bFlag)
    {
        List<LHabitEntry> entryList = new List<LHabitEntry>();
        foreach (Transform child in taskListGroup)
        {
            HabitItem item = child.gameObject.GetComponent<HabitItem>();
            if (item == null)
            {
                continue;
            }

            LHabitEntry entry = item.GetTask();
            int itemIndex = child.gameObject.GetComponentInChildren<DragHandler>().getIndex();
            entry.orderId = itemIndex;
            entryList.Add(entry);
        }
        
        TaskViewController.Instance.UpdateEntries(entryList);
    }

    public void ShowInfoPage()
    {
        TutorialManager.Instance.ShowInfo(2);
    }
    #endregion

    #region Private Members
    private void CreateTaskList(List<LHabitEntry> habitList)
    {
        DeleteTaskList();
        taskListGroup.gameObject.SetActive(true);
        for (int index = 0; index < habitList.Count; index++)
        {
            GameObject subItemObj = GameObject.Instantiate(taskItemPrefab, taskListGroup);
            subItemObj.GetComponent<HabitItem>().SetTask(habitList[index]);
            subItemObj.GetComponentInChildren<DragHandler>().setIndex(index);
            slotComponent.items.Add(subItemObj);

        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(taskListGroup.transform.GetComponent<RectTransform>());
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
