using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkToTaskItem : MonoBehaviour
{
    [SerializeField] private Text taskName_TF;

    private LTaskEntry task = null;
    private TaskLinkListPanel parentPanel = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTask(LTaskEntry task, TaskLinkListPanel pt)
    {
        this.task = task;
        this.parentPanel = pt;
        taskName_TF.text = task.taskName;
    }

    public void OnSelectTask()
    {
        if (this.parentPanel != null)
        {
            this.parentPanel.SelectedTask(task);
        }
    }
}
