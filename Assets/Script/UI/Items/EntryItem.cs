using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EntryItem : MonoBehaviour
{
    [SerializeField] public Toggle complete_Toggle;
    [SerializeField] public Toggle sublist_Toggle;


    [Space]
    [SerializeField] public float offsetY;
    [SerializeField] public float item_height;
    [SerializeField] public Text taskNameIF;
    [SerializeField] public GameObject subItemPrefab;
    [SerializeField] public Transform subTaskListGroup;
    [SerializeField] public CanvasGroup canvasGroup;
    [SerializeField] public CanvasGroup toggleCanvasGroup;
    [SerializeField] public CanvasGroup subTasksCanvasGroup;

    protected VerticalLayoutGroup subListVLG;
    protected CheckAll checkAllComponent;
    protected float item_interval;
    protected List<LSubTask> subTaskList = new List<LSubTask>();

    protected virtual void Start()
    {
        checkAllComponent = GetComponent<CheckAll>();
        if (subTaskListGroup != null)
        {
            subListVLG = subTaskListGroup.gameObject.GetComponent<VerticalLayoutGroup>();
        }
        
    }

    protected virtual void OnEnable()
    {
        DeleteSubTasks();
    }

    virtual public void ShowSubTaskList(bool bShow)
    {

    }

    virtual public void ReloadUI()
    {

    }

    protected void UpdateCanvasGroup(bool isEnable)
    {
        canvasGroup.alpha = isEnable ? 1.0f: 0.6f;
        if (subTasksCanvasGroup != null)
        {
            subTasksCanvasGroup.blocksRaycasts = isEnable;
            subTasksCanvasGroup.interactable = isEnable;
        }
    }


    protected void CreateSubTasks()
    {
        item_interval = subListVLG.spacing;

        DeleteSubTasks();

        subTaskListGroup.gameObject.SetActive(true);
        for (int index = 0; index < subTaskList.Count; index++)
        {
            GameObject subItemObj = GameObject.Instantiate(subItemPrefab, subTaskListGroup);
            subItemObj.name = subTaskList[index].taskName;
            subItemObj.GetComponent<SubTaskItem>().SetTask(subTaskList[index], this);
            if (checkAllComponent != null)
            {
                checkAllComponent.AddChildToggle(subItemObj.GetComponent<Toggle>());
            }
        }
        RectTransform trans = gameObject.GetComponent<RectTransform>();

        trans.sizeDelta = new Vector2(trans.sizeDelta.x, subTaskList.Count * (item_height + item_interval) + offsetY);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent.gameObject.GetComponent<RectTransform>());
    }

    protected void DeleteSubTasks()
    {
        if (subTaskListGroup != null)
        {
            SubTaskItem[] items = subTaskListGroup.GetComponentsInChildren<SubTaskItem>();
            foreach (SubTaskItem item in items)
            {
                Destroy(item.gameObject);
            }

            if (checkAllComponent != null)
            {
                checkAllComponent.ClearChildrenToggles();
            }

            RectTransform trans = gameObject.GetComponent<RectTransform>();
            trans.sizeDelta = new Vector2(trans.sizeDelta.x, item_height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent.gameObject.GetComponent<RectTransform>());
            subTaskListGroup.gameObject.SetActive(false);
        }
    }
}
