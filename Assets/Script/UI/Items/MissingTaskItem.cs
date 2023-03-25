using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissingTaskItem : MonoBehaviour
{
    [SerializeField] private Toggle complete_Toggle;
    [SerializeField] private Toggle sublist_Toggle;


    [Space]
    [SerializeField] private float offsetY;
    [SerializeField] private float item_height;
    [SerializeField] private TMP_Text taskNameIF;
    [SerializeField] private GameObject subItemPrefab;
    [SerializeField] private Transform subTaskListGroup;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup toggleCanvasGroup;

    private VerticalLayoutGroup subListVLG;
    private CheckAll checkAllComponent;
    private float item_interval;
    private LTaskEntry task = null;
    private List<LSubTask> subTaskList = new List<LSubTask>();
    
    // Start is called before the first frame update
    void Start()
    {
        checkAllComponent = GetComponent<CheckAll>();
        subListVLG = subTaskListGroup.gameObject.GetComponent<VerticalLayoutGroup>();

        sublist_Toggle.onValueChanged.AddListener(delegate
        {
            ShowSubTaskList(sublist_Toggle);
        });

        complete_Toggle.onValueChanged.AddListener(delegate
        {
            OnComplete(complete_Toggle);
        });

    }

    private void OnEnable()
    {
        DeleteSubTasks();
    }

    #region Public Members
    public void ShowSubTaskList(bool bShow)
    {
        if (bShow)
        {
            if (subTaskList.Count == 0 && task != null)
            {
                this.subTaskList = TaskViewController.Instance.GetSubTasks(task);
            }
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
        taskNameIF.text = task.taskName;
        complete_Toggle.isOn = this.task.isCompleted();
        UpdateCanvasGroup(true);
    }

    public LTaskEntry GetTask()
    {
        return this.task;
    }

    public bool isChecked()
    {
        return this.complete_Toggle.isOn;
    }

    #endregion

    #region Private Members

    private void CreateSubTasks()
    {
        item_interval = subListVLG.spacing;

        DeleteSubTasks();

        subTaskListGroup.gameObject.SetActive(true);
        for (int index = 0; index < subTaskList.Count; index++)
        {
            GameObject subItemObj = GameObject.Instantiate(subItemPrefab, subTaskListGroup);
            subItemObj.name = subTaskList[index].taskName;
            subItemObj.GetComponent<SubTaskItem>().SetTask(subTaskList[index]);
            if (checkAllComponent != null)
            {
                checkAllComponent.AddChildToggle(subItemObj.GetComponent<Toggle>());
            }
        }
        RectTransform trans = gameObject.GetComponent<RectTransform>();

        trans.sizeDelta = new Vector2(trans.sizeDelta.x, subTaskList.Count * (item_height + item_interval) + offsetY);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent.gameObject.GetComponent<RectTransform>());
    }

    private void DeleteSubTasks()
    {
        SubTaskItem[] items = subTaskListGroup.GetComponentsInChildren<SubTaskItem>();
        foreach (SubTaskItem item in items)
        {
            Destroy(item.gameObject);
        }
        /*
        foreach (Transform child in subTaskListGroup.transform)
        {
            Destroy(child.gameObject);
        }
        */
        if (checkAllComponent != null)
        {
            checkAllComponent.ClearChildrenToggles();
        }

        RectTransform trans = gameObject.GetComponent<RectTransform>();
        trans.sizeDelta = new Vector2(trans.sizeDelta.x, item_height);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent.gameObject.GetComponent<RectTransform>());
        subTaskListGroup.gameObject.SetActive(false);
    }

    private void ShowSubTaskList(Toggle toggle)
    {
        ShowSubTaskList(toggle.isOn);
    }

    private void UpdateCanvasGroup(bool isEnable)
    {
        if (isEnable)
        {
            canvasGroup.alpha = 1.0f;
            toggleCanvasGroup.blocksRaycasts = true;
            toggleCanvasGroup.interactable = true;
        }
        else
        {
            canvasGroup.alpha = 0.6f;
            toggleCanvasGroup.blocksRaycasts = false;
            toggleCanvasGroup.interactable = false;
        }
    }

    private void OnComplete(Toggle toggle)
    {
        /*
        if (toggle.isOn == true)
        {
            task.OnComplete();

            UIManager.Instance.ShowLoadingBar(true);
            TaskViewController.Instance.OnComplete(task, false, (isSuccess, errMsg) =>
            {
                UIManager.Instance.ShowLoadingBar(false);
                if (isSuccess)
                {
                    UpdateCanvasGroup(false);
                }
                else
                {
                    UIManager.Instance.ShowErrorDlg(errMsg);
                }
            });
        }
        */
    }
    #endregion


}
