using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ToDoItem : EntryItem
{
    private LToDoEntry toDo = null;
    private LAutoToDo autoToDo = null;
    private ChecklistPage parentPage = null;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        sublist_Toggle.onValueChanged.AddListener(delegate
        {
            ShowSubTaskList(sublist_Toggle);
        });

        complete_Toggle.onValueChanged.AddListener(delegate
        {
            OnComplete(complete_Toggle);
        });
    }

    
    #region Public Members
    public override void ShowSubTaskList(bool bShow)
    {
        if (bShow)
        {
            if (subTaskList.Count == 0)
            {
                if (UserViewController.Instance.GetCurrentSetting().game_mode != (int)Game_Mode.Game_Only && toDo != null)
                {
                    Debug.Log(">>>>>>>>>>>>1");
                    this.subTaskList = TaskViewController.Instance.GetSubTasks(toDo);
                }
                else
                {
                    if (autoToDo != null)
                    {
                        Debug.Log(">>>>>>>>>>>>");
                        this.subTaskList = TaskViewController.Instance.GetSubTasks(autoToDo);
                    }
                    
                }
            }
            UpdateTodoTitle(true);
            //taskNameIF.text = this.toDo.taskName + string.Format("\n(Due date: {0})", Convert.FDateToEntryDate(this.toDo.dueDate));
            CreateSubTasks();
        }
        else
        {
            UpdateTodoTitle(false);
            DeleteSubTasks();
        }
    }

    public void SetToDo(LToDoEntry ftoDo, ChecklistPage parentPage)
    {
        this.toDo = ftoDo;
        this.parentPage = parentPage;
        ReloadUI();
    }

    public override void ReloadUI()
    {
        if (this.toDo != null)
        {
            this.subTaskList = TaskViewController.Instance.GetSubTasks(toDo);
            UpdateTodoTitle(false);
            float ratio = UnityEngine.Screen.height / (float)UnityEngine.Screen.width;
            taskNameIF.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1330.0f / ratio, 0f);
            complete_Toggle.isOn = this.toDo.isCompleted();
            UpdateCanvasGroup(this.toDo.isEnabled());
        }

        if (this.autoToDo != null)
        {
            this.subTaskList = TaskViewController.Instance.GetSubTasks(autoToDo);
            UpdateTodoTitle(false);
            float ratio = UnityEngine.Screen.height / (float)UnityEngine.Screen.width;
            taskNameIF.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1330.0f / ratio, 0f);
            complete_Toggle.isOn = this.autoToDo.isCompleted();
            UpdateCanvasGroup(this.autoToDo.isEnabled());
        }
    }

    public void SetAutoToDo(LAutoToDo ftoDo, ChecklistPage parentPage)
    {
        this.autoToDo = ftoDo;
        this.parentPage = parentPage;
        taskNameIF.text = ftoDo.taskName;
        
        float ratio = UnityEngine.Screen.height / (float)UnityEngine.Screen.width;
        taskNameIF.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1330.0f / ratio, 0f);
        complete_Toggle.isOn = this.autoToDo.isCompleted();
        UpdateCanvasGroup(this.autoToDo.isEnabled(System.DateTime.Now));
    }

    public void EditToDo()
    {
        if (UserViewController.Instance.GetCurrentSetting().game_mode != (int)Game_Mode.Game_Only)
        {
            transform.GetComponentInParent<NavigationPage>().Show<ToDoEntryPage>("todo_entry", this.toDo);
        }
    }

    public LToDoEntry GetToDo()
    {
        return this.toDo;
    }
    #endregion
    #region Private Members

    private void UpdateTodoTitle(bool bshowEndDate)
    {
        var titleStr = "";
        var unCompletedTasks = this.subTaskList.FindAll(item => item.isCompleted() == false).ToList();
        if (this.toDo != null)
        {
            if (this.toDo.checkList.Count > 0)
            {
                titleStr = this.toDo.taskName + " {" + unCompletedTasks.Count + "}";
            }
            else
            {
                titleStr = this.toDo.taskName;
            }
            if (bshowEndDate)
            {
                titleStr += string.Format("\n(Due date: {0})", Convert.FDateToEntryDate(this.toDo.dueDate));
            }
        }

        if (this.autoToDo != null)
        {
            if (this.autoToDo.checkList.Count > 0)
            {
                titleStr = this.autoToDo.taskName + " {" + unCompletedTasks.Count + "}";
            }
            else
            {
                titleStr = this.autoToDo.taskName;
            }
            if (bshowEndDate)
            {
                titleStr += string.Format("\n(Due date: {0})", Convert.FDateToEntryDate(this.autoToDo.dueDate));
            }
        }
        
        
        taskNameIF.text = titleStr;
    }

    private void ShowSubTaskList(Toggle toggle)
    {
        ShowSubTaskList(toggle.isOn);
    }

    private void OnComplete(Toggle toggle)
    {
        if (UserViewController.Instance.GetCurrentSetting().game_mode == (int)Game_Mode.Game_Only)
        {
            if (toggle.isOn && autoToDo.isEnabled() == true)
            {
                var costGold = (float)autoToDo.cost_gold;
                var costWood = (float)autoToDo.cost_wood;
                var costIron = (float)autoToDo.cost_iron;
                var curGold = ResourceViewController.Instance.GetResourceValue(EResources.Gold);
                var curWood = ResourceViewController.Instance.GetResourceValue(EResources.Lumber);
                var curIron = ResourceViewController.Instance.GetResourceValue(EResources.Iron);

                if (costGold <= curGold && costWood <= curWood && costIron <= curIron)
                {
                    Dictionary<EResources, float> dic = new Dictionary<EResources, float>();
                    dic.Add(EResources.Gold, -costGold);
                    dic.Add(EResources.Lumber, -costWood);
                    dic.Add(EResources.Iron, -costIron);

                    TaskViewController.Instance.CompleteToDo(toDo, (isSuccess) =>
                    {
                        if (!isSuccess)
                        {
                            UpdateCanvasGroup(false);
                        }
                        else
                        {
                            NotificationManager.Instance.CancelPendingLocalNotification(toDo, (result) =>
                            {
                            });
                            toDo.OnComplete();
                            if (parentPage != null)
                            {
                                parentPage.Reload();
                            }
                        }
                    });
                }
                else
                {
                    UIManager.Instance.ShowErrorDlg("Not Enough Resources to complete");
                }
                
            }
        }
        else
        {
            if (toggle.isOn && toDo.isEnabled() == true)
            {
                TaskViewController.Instance.CompleteToDo(toDo, (isSuccess) =>
                {
                    if (!isSuccess)
                    {
                        UpdateCanvasGroup(false);
                    }
                    else
                    {
                        NotificationManager.Instance.CancelPendingLocalNotification(toDo, (result) =>
                        {
                        });
                        toDo.OnComplete();
                        if (parentPage != null)
                        {
                            parentPage.Reload();
                        }
                    }
                });
            }
        }
        
    }
    #endregion

}
