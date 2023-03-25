using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UIControllersAndData.Store.Categories.Buildings;

public class AITaskManager : SingletonComponent<AITaskManager>
{
    private List<LTaskEntry> initDailyTasks = new List<LTaskEntry>();
    private List<LAutoToDo> initToDos = new List<LAutoToDo>();
    private List<LHabitEntry> initHabits = new List<LHabitEntry>();
    private List<LAutoGoal> initGoals = new List<LAutoGoal>();
    private List<LTask> initSubTasks = new List<LTask>();
    private List<int> rareResourceBuildingIds = new List<int>(){61, 61, 63, 63, 65, 62, 63};
    private List<int> rareBuildingTypesVegetrain = new List<int>() { 5, 12, 14, 11, 5, 12, 14 };
    private List<int> rareBuildingTypesNotVegetrain = new List<int>() { 5, 12, 14, 11, 0, 0, 14 };
    private List<string> exportGoalIds = new List<string>() { "1", "5", "8", "9", "10" };
    private List<EResources> exportResources = new List<EResources>() { EResources.Bread, EResources.Ale, EResources.Lumber, EResources.Stone, EResources.Iron };
    private int rareBuildingId = 0;
    private int rareBuildingTypes = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetData();
    }

    public void CreateEntries()
    {
        GetData();
        InitData data = Resources.Load<InitData>(Constants.PATH_FOR_INIT_DATA_ASSET_LOAD);
        initDailyTasks = new List<LTaskEntry>(data.dailyTasks.ToList());
        initToDos = new List<LAutoToDo>(data.todos.ToList());
        initHabits = new List<LHabitEntry>(data.habits.ToList());
        initGoals = new List<LAutoGoal>(data.goals.ToList());
        initSubTasks = new List<LTask>(data.subTasks.ToList());

        CreateDailyTask();
        CreateTodo();
        CreateSubTasks();
        CreateHabit();
        CreateGoal();
    }

    public void CompleteGoal(string id)
    {
        var goal = DataManager.Instance.CurrentAutoGoals.ToList().Find(it => it.id == id);
        if (goal == null)
        {
            return;
        }

        CompleteGoal(goal);
    }

    public void CompleteGoal(LAutoGoal goal)
    {
        var beginDate = Convert.FDateToDateTime(goal.begin_date);
        var endDate = beginDate.AddDays(goal.completionDays);
        RewardSystem.Instance.OnComplete(goal);
        if (System.DateTime.Now.CompareTo(endDate) > 0)
        {
            RewardSystem.Instance.OnFailed(goal);
        }
        
        goal.completedDate = Convert.DateTimeToFDate(System.DateTime.Now);
        if (goal.repeatDays > 0)
        {
            beginDate = System.DateTime.Now.AddDays(goal.repeatDays);
            goal.begin_date = Convert.DateTimeToFDate(beginDate);
            if (goal.id == "4")
            {
                goal.completeAmount = Mathf.Clamp(goal.completeAmount + 5, 80, 100);
                goal.taskName = "Raise Happiness to " + goal.completeAmount + "%";
            }
        }
        DataManager.Instance.UpdateEntry(goal);
    }

    public void CheckOnCompleteWithHappiness(float happy)
    {
        var goal = DataManager.Instance.CurrentAutoGoals.Find(it => it.id == "4");
        if (goal == null)
        {
            return;
        }

        if (!goal.IsAvailable(System.DateTime.Now))
        {
            return;
        }

        if (happy >= goal.completeAmount)
        {
            CompleteGoal(goal);
        }

    }
    public void CheckOnCompleteWithExports()
    {
        var user = UserViewController.Instance.GetCurrentUser();
        for(int index = 0; index <exportGoalIds.Count; index++)
        {
            var id = exportGoalIds[index];
            var goal = DataManager.Instance.CurrentAutoGoals.Find(it => it.id == id);
            if (goal == null)
            {
                continue;
            }

            if (!goal.IsAvailable(System.DateTime.Now))
            {
                continue;
            }
            var curRes = exportResources[index];
            var curAmount = (int)user.GetExport(curRes);
            if (curAmount >= goal.completeAmount)
            {
                CompleteGoal(id);
                user.ClearExport(curRes);
            }
        }
    }

    public int GetExportGoalIndex(string id)
    {
        return exportGoalIds.FindLastIndex((item) => item == id);
    }

    public int GetExportValue(int index)
    {
        var user = UserViewController.Instance.GetCurrentUser();
        var curRes = exportResources[index];
        return (int)user.GetExport(curRes);
    }

    public void CheckOnCompleteWithBuilding(BuildingsCategory building)
    {
        var goal = DataManager.Instance.CurrentAutoGoals.Find(it => it.id == "3");
        if (goal != null && building.id == goal.completeAmount)
        {
            CompleteGoal(goal);
            return;
        }
        
        goal = DataManager.Instance.CurrentAutoGoals.Find(it => it.id == "2");
        if (goal != null &&  building.id == goal.completeAmount)
        {
            CompleteGoal(goal);
        }
    }

    public void CheckOnStartWithVillager(LVillager villager)
    {
        var cVillager = ResourceViewController.Instance.GetCVillager(villager.id);
        if (cVillager.type == EVillagerType.Sawyer)
        {
            EnableGoal("8");
            return;
        }
        if (cVillager.type == EVillagerType.Blacksmith)
        {
            EnableGoal("10");
            return;
        }
    }

    public void CheckOnStartWithBuilding(LBuilding building)
    {
        var cBuilding = ResourceViewController.Instance.GetCBuilding(building.id);
        CheckOnCompleteWithBuilding(cBuilding);
        if (cBuilding.type == EBuildingType.Tavern)
        {
            EnableGoal("5");
            return;
        }

        if (cBuilding.type == EBuildingType.Quarry)
        {
            EnableGoal("9");
            return;
        }
    }
    private void CreateDailyTask()
    {
        var list = new List<LTaskEntry>();
        foreach (LTaskEntry entry in initDailyTasks.ToList())
        {
            var newEntry = new LTaskEntry();
            newEntry.id = entry.id;
            newEntry.taskName = entry.taskName;
            newEntry.orderId = entry.orderId;
            newEntry.goldCount = entry.goldCount;
            newEntry.diffculty = entry.diffculty;
            newEntry.repeatition = entry.repeatition;
            newEntry.repeat_every = entry.repeat_every;
            newEntry.subTasks = entry.subTasks.ToList();
            newEntry.repeatDays = entry.repeatDays.ToList();
            newEntry.created_at = Convert.DateTimeToFDate(System.DateTime.Now);
            newEntry.begin_date = newEntry.created_at;
            list.Add(newEntry);
        }
        DataManager.Instance.CurrentDailyTasks = list;
    }

    private void CreateTodo()
    {
        var list = new List<LAutoToDo>();
        foreach (LAutoToDo entry in initToDos.ToList())
        {
            var newEntry = new LAutoToDo();
            newEntry.id = entry.id;
            newEntry.taskName = entry.taskName;
            newEntry.orderId = entry.orderId;
            newEntry.goldCount = entry.goldCount;
            newEntry.diffculty = entry.diffculty;
            newEntry.checkList = entry.checkList.ToList();
            newEntry.repeatition = entry.repeatition.ToList();
            newEntry.cost_gold = entry.cost_gold;
            newEntry.cost_iron = entry.cost_iron;
            newEntry.cost_wood = entry.cost_wood;
            newEntry.created_at = Convert.DateTimeToFDate(System.DateTime.Now);
            newEntry.begin_date = newEntry.created_at;
            list.Add(newEntry);
        }
        DataManager.Instance.CurrentAutoToDos = list;
    }

    private void CreateHabit()
    {
        var list = new List<LHabitEntry>();
        foreach (LHabitEntry entry in initHabits.ToList())
        {
            var newEntry = new LHabitEntry();
            newEntry.id = entry.id;
            newEntry.taskName = entry.taskName;
            newEntry.orderId = entry.orderId;
            newEntry.goldCount = entry.goldCount;
            newEntry.isPositive = entry.isPositive;
            newEntry.created_at = Convert.DateTimeToFDate(System.DateTime.Now);
            newEntry.begin_date = newEntry.created_at;
            newEntry.bJustToday = entry.bJustToday;
            list.Add(newEntry);
        }
        DataManager.Instance.CurrentHabits = list;
    }

    private void CreateGoal()
    {
        var list = new List<LAutoGoal>();
        foreach (LAutoGoal entry in new List<LAutoGoal>(initGoals))
        {
            var newEntry = new LAutoGoal();
            newEntry.id = entry.id;
            newEntry.taskName = entry.taskName;
            newEntry.orderId = entry.orderId;
            newEntry.goldCount = entry.goldCount;
            newEntry.repeatDays = entry.repeatDays;
            newEntry.completeAmount = entry.completeAmount;
            newEntry.completionDays = entry.completionDays;
            newEntry.created_at = Convert.DateTimeToFDate(System.DateTime.Now);
            var startDate = System.DateTime.Now;
            //entry.begin_date = entry.created_at;
            switch (newEntry.id)
            {
                case "1":
                    newEntry.begin_date = newEntry.created_at;
                    break;
                case "2":
                    if (rareBuildingTypes != 0)
                    {
                        newEntry.begin_date = newEntry.created_at;
                        newEntry.completeAmount = rareBuildingTypes;
                        newEntry.taskName = "Build " + DataManager.Instance.BuildingsCategoryData.category.Find(it => it.id == rareBuildingTypes).GetName();
                    }
                    break;    
                case "3":
                    if (rareBuildingId == 63)
                    {
                        newEntry.taskName = "Build Quarry";
                    }
                    else
                    {
                        newEntry.taskName = "Build Mine";
                    }
                    newEntry.completeAmount = rareBuildingId;
                    newEntry.begin_date = newEntry.created_at;
                    break;
                case "4":
                    newEntry.begin_date = Convert.DateTimeToFDate(System.DateTime.Now.AddDays(7));
                    startDate = System.DateTime.Now.AddDays(7);
                    break;
                default:
                    break;
            }
            newEntry.endDate = Convert.DateTimeToFDate(startDate.AddDays(entry.completionDays));
            list.Add(newEntry);
        }
        DataManager.Instance.CurrentAutoGoals = list;
    }

    private void GetData()
    {
        var user = UserViewController.Instance.GetCurrentUser();
        var startDate = Convert.FDateToDateTime(user.mode_at);

        rareBuildingId = rareResourceBuildingIds[(int)startDate.DayOfWeek];
        if (user.isVegetarian)
        {
            rareBuildingTypes = rareBuildingTypesVegetrain[(int)startDate.DayOfWeek];
        }
        else
        {
            rareBuildingTypes = rareBuildingTypesNotVegetrain[(int)startDate.DayOfWeek];
        }
    }
    private void EnableGoal(string id)
    {
        var goal = DataManager.Instance.CurrentAutoGoals.ToList().Find(it => it.id == id);
        if (goal == null)
        {
            return;
        }
        goal.begin_date = Convert.DateTimeToFDate(System.DateTime.Now);
        goal.endDate = Convert.DateTimeToFDate(System.DateTime.Now.AddDays(goal.completionDays));
        DataManager.Instance.UpdateEntry(goal);
    }

    private void CreateSubTasks()
    {
        var list = new List<LSubTask>();
        foreach (LTask entry in initSubTasks)
        {
            var newEntry = new LSubTask();
            newEntry.id = entry.id;
            newEntry.taskName = entry.taskName;
            newEntry.goldCount = entry.goldCount;
            newEntry.created_at = Convert.DateTimeToFDate(System.DateTime.Now);
            newEntry.begin_date = newEntry.created_at;
            list.Add(newEntry);
        }
        DataManager.Instance.CurrentSubTasks = list;
    }

}
