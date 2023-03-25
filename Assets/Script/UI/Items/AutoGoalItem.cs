using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoGoalItem : EntryItem
{
    [SerializeField] private TMP_Text remaining_TF;
    [SerializeField] private TMP_Text startDate_TF;
    [SerializeField] private TMP_Text dueDate_TF;
    [SerializeField] private TMP_Text gold_TF;
    [SerializeField] private TMP_Text happiness_TF;
    private LAutoGoal autoGoalEntry = null;
    private ProjectGoalsPage parentPage = null;
    private List<int> exportGoalIds = new List<int>() { 1, 5, 8, 9, 10 };
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();


    }

    #region Public Members
  
    public void SetProject(LAutoGoal project, ProjectGoalsPage parentPage)
    {
        this.autoGoalEntry = project;
        this.parentPage = parentPage;
        taskNameIF.text = project.taskName;
        float ratio = UnityEngine.Screen.height / (float)UnityEngine.Screen.width;
        taskNameIF.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1330.0f / ratio, 0f);
        complete_Toggle.isOn = this.autoGoalEntry.isCompleted();
        startDate_TF.text = Convert.FDateToEntryDate(project.begin_date);
        dueDate_TF.text = Convert.FDateToEntryDate(project.endDate);
        gold_TF.text = "" + project.goldCount;
        happiness_TF.text = project.happiness + "%";
        UpdateCanvasGroup(this.autoGoalEntry.isEnabled(System.DateTime.Now));
        var exportIndex = AITaskManager.Instance.GetExportGoalIndex(project.id);
        if (exportIndex >= 0)
        {
            var exportValue = AITaskManager.Instance.GetExportValue(exportIndex);
            if (exportValue >= project.completeAmount)
            {
                remaining_TF.gameObject.SetActive(false);
                gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 230);
            }
            else
            {
                remaining_TF.gameObject.SetActive(true);
                gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 280);
                remaining_TF.text = string.Format("[{0} remaining to export]", project.completeAmount - exportValue);
            }
        }
        else
        {
            gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 230);
            remaining_TF.gameObject.SetActive(false);
        }
    }

    
    public LAutoGoal GetProject()
    {
        return this.autoGoalEntry;
    }
    #endregion
    
}
