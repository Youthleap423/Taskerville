using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubTaskEntryItem : MonoBehaviour
{
    [SerializeField] private TMP_InputField taskName_IF;
    [SerializeField] private int nIndex = 0;
    [SerializeField] private LSubTask task = new LSubTask();

    #region Unity Memebers
    // Start is called before the first frame update
    void Start()
    {
        taskName_IF.onEndEdit.AddListener(delegate
        {
            UpdateTaskName(taskName_IF);
        });
    }
    #endregion

    #region Public Members
    public string GetTaskName()
    {
        if (taskName_IF != null)
        {
            return taskName_IF.text;
        }

        return "";
    }

    public int Index
    {
        get
        {
            return nIndex;
        }
        set
        {
            nIndex = value;
        }
    }

    public void DeleteItem()
    {
        this.task.SetRemoved(true);
        transform.GetComponentInParent<EntryPage>().DeleteSubTask(this.task);
        Destroy(transform.gameObject);
    }

    public LSubTask Task
    {
        get
        {
            return task;
        }
        set
        {
            this.task = value;
            taskName_IF.text = value.taskName;
        }
    }

    #endregion

    #region Private Members
    private void UpdateTaskName(TMP_InputField input)
    {
        this.task.taskName = input.text;
        transform.GetComponentInParent<EntryPage>().UpdateSubTask(this.task);
    }

    #endregion
}
