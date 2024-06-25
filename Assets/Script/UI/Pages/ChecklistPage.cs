using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistPage : Page
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

        if (UserViewController.Instance.GetCurrentSetting().game_mode == (int)Game_Mode.Game_Only)
        {
            newButton.SetActive(false);
            CreateAutoTaskList(TaskViewController.Instance.GetAutoToDos());
        }
        else
        {
            newButton.SetActive(true);
            CreateTaskList(TaskViewController.Instance.GetToDos());
        }

        
        
    }

    public void Reload()
    {
        Initialize();
    }

    public void ShowEntryPage()
    {
        transform.parent.GetComponent<NavigationPage>().Show("todo_entry");
    }

    public void Rearrange(bool bFlag)
    {
        List<LToDoEntry> entryList = new List<LToDoEntry>();
        foreach (Transform child in taskListGroup)
        {
            ToDoItem item = child.gameObject.GetComponent<ToDoItem>();
            if (item == null)
            {
                continue;
            }

            LToDoEntry entry = item.GetToDo();
            int itemIndex = child.gameObject.GetComponentInChildren<DragHandler>().getIndex();
            entry.orderId = itemIndex;
            entryList.Add(entry);
        }
        
        TaskViewController.Instance.ArrangeToDo(entryList);
    }

    public void ShowInfoPage()
    {
        TutorialManager.Instance.ShowInfo(1);
    }
    #endregion

    #region Private Members
    private void CreateTaskList(List<LToDoEntry> todoList)
    {
        DeleteTaskList();
        taskListGroup.gameObject.SetActive(true);
        for (int index = 0; index < todoList.Count; index++)
        {
            if (todoList[index].isCompleted())
            {
                continue;
            }
            GameObject subItemObj = GameObject.Instantiate(taskItemPrefab, taskListGroup);
            subItemObj.GetComponent<ToDoItem>().SetToDo(todoList[index], this);
            subItemObj.GetComponentInChildren<DragHandler>().setIndex(index);
            slotComponent.items.Add(subItemObj);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(taskListGroup.transform.GetComponent<RectTransform>());
    }

    private void CreateAutoTaskList(List<LAutoToDo> todoList)
    {
        DeleteTaskList();
        taskListGroup.gameObject.SetActive(true);
        for (int index = 0; index < todoList.Count; index++)
        {
            if (AppManager.Instance.GetCurrentMode() == Game_Mode.Game_Only && !todoList[index].IsAvailable(System.DateTime.Now))
            {
                continue;
            }
            GameObject subItemObj = GameObject.Instantiate(taskItemPrefab, taskListGroup);
            subItemObj.GetComponent<ToDoItem>().SetAutoToDo(todoList[index], this);
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
#if !UNITY_EDITOR
        if (focus)
        {
            Initialize();
        }
#endif
    }
    #endregion
}
