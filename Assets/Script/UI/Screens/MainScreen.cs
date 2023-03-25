using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEditor;

public class MainScreen : IScreen
{
    [Space]
    [SerializeField] public Image avatar_image;
    [SerializeField] public TextMeshProUGUI fullname_TMP;

    [Space]
    [SerializeField] public Toggle gear_Toggle;

    [Space]
    [SerializeField] public TextMeshProUGUI nameTMP;
    [SerializeField] public TextMeshProUGUI firstLetterTMP;


    [SerializeField] public TextMeshProUGUI small_nameTMP;
    [SerializeField] public TextMeshProUGUI small_firstLetterTMP;

    [Header("Navigator")]
    [SerializeField] public MainTabPage mainTabPage;
    [SerializeField] public EntryTabPage taskTabPage;
    #region Unity Members

    // Use this for initialization
    void Start()
    {
        gear_Toggle.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged(gear_Toggle);
        });
        Load();

        AppManager.Instance.OnNotificationOpened += Instance_OnNotificationOpened;
    }

    private void OnDestroy()
    {
        if (AppManager.Instance != null)
        {
            AppManager.Instance.OnNotificationOpened -= Instance_OnNotificationOpened;
        }
    }

    private void Instance_OnNotificationOpened(string obj)
    {
        OnNotificationOpend();
    }
    #endregion

    #region Public Members
    override public void Show(bool back, bool immediate)
    {
        base.Show(back, immediate);
        if (back == true)
        {
            //show choose avatar screen from setting, and backed from choose avatar screen
            PageManager.Instance.Show("setting");
        }
    }

    public void OnNotificationOpend()
    {
        string notifyPageId = PlayerPrefs.GetString("NotifyPage");
        
        switch (notifyPageId)
        {
            case "DailyTask":
                mainTabPage.ShowPage("task_management");
                taskTabPage.ShowPage("daily_task");
                break;
            case "ToDo":
                mainTabPage.ShowPage("task_management");
                taskTabPage.ShowPage("checklist");
                break;
            case "Habit":
            case "Unit":
                mainTabPage.ShowPage("task_management");
                taskTabPage.ShowPage("habitlist");
                break;
            case "Project":
                mainTabPage.ShowPage("task_management");
                taskTabPage.ShowPage("project_goals");
                break;
            default:
                break;
        }
    }

    public void Load()
    {
        LUser currentUser = DataManager.Instance.GetCurrentUser();

        avatar_image.sprite = DataManager.Instance.GetCurrentAvatarSprite();
        fullname_TMP.text = currentUser.GetFullName();

        var villageName = currentUser.Village_Name;
        villageName = villageName.Substring(0, Convert.Min(villageName.Length, 12));
        var villageFirstLetter = villageName.Length > 0 ? villageName.Substring(0, 1) : "";
        nameTMP.text = villageName.ToUpper();
        firstLetterTMP.text = villageFirstLetter.ToUpper();
        small_nameTMP.text = villageName.ToUpper();
        small_firstLetterTMP.text = villageFirstLetter.ToUpper();

        OnNotificationOpend();
        //GameManager.Instance.CheckAllDailyUpdate();
        /*
        if (TaskViewController.Instance.IsAllTaskCompleted(System.DateTime.Now.AddDays(-1)))
        {
            DataManager.Instance.GetCurrentUser().updateDailyTaskDate(System.DateTime.Now.AddDays(-1));
            RewardSystem.Instance.OnAllDailyTaskComplete();
        }

        var yesterdayTaskList = TaskViewController.Instance.GetUnCheckedTasks(System.DateTime.Now.AddDays(-1));
        if (yesterdayTaskList.Count > 0)
        {
            UIManager.Instance.ShowUncheckedTaskDlg(yesterdayTaskList);
        }
        ResourceViewController.Instance.CheckDailyMission();
        TradeViewController.Instance.CheckTrades();
        */
        //if (ScreenManager.Instance.PreviousScreenId == "splash" || ScreenManager.Instance.PreviousScreenId == "login")
        //{
        //    if (AppManager.Instance.GetCurrentMode() == Game_Mode.Task_Only)
        //    {
        //        PageManager.Instance.Show("daily_report");
        //    }
        //    else
        //    {
        //        PageManager.Instance.Show("main");
        //    }

        //    if (TaskViewController.Instance.IsAllTaskCompleted(System.DateTime.Now.AddDays(-1))){
        //        DataManager.Instance.GetCurrentUser().updateDailyTaskDate(System.DateTime.Now.AddDays(-1));
        //        RewardSystem.Instance.OnAllDailyTaskComplete();
        //    }

        //    var yesterdayTaskList = TaskViewController.Instance.GetUnCheckedTasks(System.DateTime.Now.AddDays(-1));
        //    if (yesterdayTaskList.Count > 0)
        //    {
        //        UIManager.Instance.ShowUncheckedTaskDlg(yesterdayTaskList);
        //    }
        //    ResourceViewController.Instance.CheckDailyMission();
        //    TradeViewController.Instance.CheckTrades();
        //}
        //else
        //{
        //    PageManager.Instance.Show("prompt");
        //}

    }

#endregion

#region Private Members
    private void ToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            PageManager.Instance.Show("setting_nav");
        }
        else
        {
            PageManager.Instance.Back();
        }
    }
#endregion
}
