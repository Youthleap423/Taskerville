using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkToHabitItem : MonoBehaviour
{
    [SerializeField] private Text taskName_TF;

    private LHabitEntry task = null;
    private HabitLinkListPanel parentPanel = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTask(LHabitEntry task, HabitLinkListPanel pt)
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
