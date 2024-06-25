using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Firebase.Firestore;
using UIControllersAndData.Store.Categories.Buildings;
using Assets.Scripts;
using Newtonsoft.Json;

public class DataManager : SingletonComponent<DataManager>
{
    public static event System.Action<bool> OnDataUpdated;

    [SerializeField] public Sprite[] avatar_Images;

    [Header("Sprites")]
    [SerializeField] public Sprite gold_Sprite;
    [SerializeField] public Sprite happy_Sprite;
    [SerializeField] public Sprite set_Sprite;
    [SerializeField] public Sprite wine_Sprite;
    [SerializeField] public Sprite diamond_Sprite;
    [SerializeField] public Sprite ruby_Sprite;
    [SerializeField] public Sprite sapphire_Sprite;
    [SerializeField] public Sprite paint_Sprite;
    [Space]
    [SerializeField]
    private FUser currentFUser = new FUser();
    
    

    [Space]
    [SerializeField] public string buildingFN = "";
    [SerializeField] public string villagerFN = "";
    [SerializeField] public string resourceFN = "";
    [SerializeField] public string dailyTaskFN = "";
    [SerializeField] public string toDOFN = "";
    [SerializeField] public string projectGoalFN = "";
    [SerializeField] public string subTaskFN = "";
    [SerializeField] public string subToDoFN = "";
    [SerializeField] public string subProjectFN = "";
    [SerializeField] public string settingFN = "";
    [SerializeField] public string userFN = "";
    [SerializeField] public string autoFN = "";
    [SerializeField] public string habitFN = "";
    [SerializeField] public string autoTodoFN = "autoToDO";
    [SerializeField] public string autoGoalFN = "autoGoal";
    [SerializeField] public string artifactFN = "artifact";
    [SerializeField] public string artworkFN = "artwork";

    private List<LResource> initResources = new List<LResource>();
    private List<LVillager> initVillagers = new List<LVillager>();
    private List<LBuilding> initBuildings = new List<LBuilding>();

    private List<CResource> initCResources = new List<CResource>();
    private List<CVillager> initCVillagers = new List<CVillager>();
    private List<CBuilding> initCBuildings = new List<CBuilding>();

    private LUser currentUser = new LUser();
    private LSetting currentSetting = new LSetting();
    private List<LUser> currentUserList = new List<LUser>();
    private List<FUser> currentFUserList = new List<FUser>();
    private List<LVillager> currentVillagers = new List<LVillager>();
    private List<LBuilding> currentBuildings = new List<LBuilding>();
    private List<LResource> currentResources = new List<LResource>();
    private List<LTaskEntry> currentDailyTasks = new List<LTaskEntry>();
    private List<LSubTask> currentSubTasks = new List<LSubTask>();
    private List<LToDoEntry> currentToDos = new List<LToDoEntry>();
    private List<LProjectEntry> currentProjects = new List<LProjectEntry>();
    private List<LAutoToDo> currentAutoToDos = new List<LAutoToDo>();
    private List<LAutoGoal> currentAutoGoals = new List<LAutoGoal>();
    private List<LAutoBuilding> currentAutoBuildings = new List<LAutoBuilding>();
    private List<LHabitEntry> currentHabits = new List<LHabitEntry>();
    private List<LArtifact> currentArtifacts = new List<LArtifact>();
    private List<LArtwork> currentArtworks = new List<LArtwork>();
    private List<FCoalition> currentCoalitions = new List<FCoalition>();
    private List<LTaskEntry> yesterdayTasks = new List<LTaskEntry>();
    private List<FTrade> currentTrades = new List<FTrade>();

    private ResourceData _resourceCategoryData;
    private ScheduleData _scheduleData;
    private ArtifactData _artifactData;
    
    public List<EVillagerType> laborTypeGroup = new List<EVillagerType>() { EVillagerType.Farmer, EVillagerType.Woodcutter, EVillagerType.Baker,
                                                                    EVillagerType.Builder, EVillagerType.Miller, EVillagerType.Laborer, EVillagerType.Sawyer, EVillagerType.Quarryman, EVillagerType.Rancher, EVillagerType.Miner};
    public List<EVillagerType> guardTypeGroup = new List<EVillagerType>() { EVillagerType.Commander, EVillagerType.Knight, EVillagerType.Archer, EVillagerType.Crossbowman };
    public List<EVillagerType> staffTypeGroup = new List<EVillagerType>() { EVillagerType.Mayor, EVillagerType.Administrator, EVillagerType.Curator, EVillagerType.Cabinet };
    public List<DayOfWeek> availableDaysOfShip = new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Saturday };
    public List<EResources> exportResources = new List<EResources>() { EResources.Bread, EResources.Ale, EResources.Lumber, EResources.Stone, EResources.Iron };



    [HideInInspector]public List<string> rareBuildings = new List<string>() { "3", "5", "11", "12", "14", "61", "62", "63", "65"};

    [HideInInspector] public Dictionary<DayOfWeek, List<string>> bonusBuildingsForVegetarin = new Dictionary<DayOfWeek, List<string>>() {
        {DayOfWeek.Sunday, new List<string>{"3", "5", "12", "61"} },
        {DayOfWeek.Monday, new List<string>{"12", "11", "3", "61"} },
        {DayOfWeek.Tuesday, new List<string>{"5", "14", "11", "63"} },
        {DayOfWeek.Wednesday, new List<string>{"12", "11", "14", "63"} },
        {DayOfWeek.Thursday, new List<string>{"65", "5"} },
        {DayOfWeek.Friday, new List<string>{"62", "12"} },
        {DayOfWeek.Saturday, new List<string>{"14", "5", "63", "3"} },
    };

    [HideInInspector]
    public Dictionary<DayOfWeek, List<string>> bonusBuildingsForNotVegetarin = new Dictionary<DayOfWeek, List<string>>() {
        {DayOfWeek.Sunday, new List<string>{"3", "5", "61"} },
        {DayOfWeek.Monday, new List<string>{"12", "11", "61"} },
        {DayOfWeek.Tuesday, new List<string>{"5", "14", "63"} },
        {DayOfWeek.Wednesday, new List<string>{"12", "11", "63"} },
        {DayOfWeek.Thursday, new List<string>{"65"} },
        {DayOfWeek.Friday, new List<string>{"62"} },
        {DayOfWeek.Saturday, new List<string>{"14", "63", "3"} },
    };

    [HideInInspector]
    public Dictionary<DayOfWeek, EResources> bonusAnimalForNotVegetarin = new Dictionary<DayOfWeek, EResources>() {
        {DayOfWeek.Sunday, EResources.Goat },
        {DayOfWeek.Monday, EResources.Goat },
        {DayOfWeek.Tuesday, EResources.Swine },
        {DayOfWeek.Wednesday, EResources.Goat },
        {DayOfWeek.Thursday, EResources.Goat },
        {DayOfWeek.Friday, EResources.Swine },
        {DayOfWeek.Saturday, EResources.Swine },
    };
    [HideInInspector]
    public Dictionary<DayOfWeek, String> bonusListForNotVegetarin = new Dictionary<DayOfWeek, String>() {
        {DayOfWeek.Sunday, "Cherries, Goats, Iron, Barley" },
        {DayOfWeek.Monday, "Raspberries, Peaches, Goats, Iron" },
        {DayOfWeek.Tuesday, "Cherries, Grapes, Swine, Stone" },
        {DayOfWeek.Wednesday, "Raspberries, Peaches, Goats, Stone"},
        {DayOfWeek.Thursday, "Gold, Goats" },
        {DayOfWeek.Friday, "Silver, Swine" },
        {DayOfWeek.Saturday, "Grapes, Swine, Stone, Barley" },
    };

    [HideInInspector]
    public Dictionary<DayOfWeek, String> bonusListForVegetarin = new Dictionary<DayOfWeek, String>() {
        {DayOfWeek.Sunday, "Cherries, Raspberries, Iron, Barley" },
        {DayOfWeek.Monday, "Raspberries, Peaches, Barley, Iron" },
        {DayOfWeek.Tuesday, "Cherries, Grapes, Peaches, Stone" },
        {DayOfWeek.Wednesday, "Raspberries, Peaches, Grapes, Stone"},
        {DayOfWeek.Thursday, "Gold, Cherries" },
        {DayOfWeek.Friday, "Silver, Raspberries" },
        {DayOfWeek.Saturday, "Grapes, Cherries, Stone, Barley" },
    };

    [HideInInspector]
    public string HappyId = "19";

    [HideInInspector]
    public List<string> dailyReportStrList = new List<string>();

    private List<LocalReport> tempReport = new List<LocalReport>();

    private bool bTokenRegister = false;
    private DateTime startTime = DateTime.Now;

    public FUser CurrentFUser
    {
        get
        {
            return currentFUser;
        }

        set
        {
            currentFUser = value;
            LoadData();
        }
    }

    public List<LVillager> CurrentVillagers
    {
        get
        {
            return currentVillagers;
        }

        set
        {
            currentVillagers = value;
        }
    }

    public List<LBuilding> CurrentBuildings
    {
        get
        {
            return currentBuildings;
        }

        set
        {
            currentBuildings = value;
        }
    }

    public List<LResource> CurrentResources
    {
        get
        {
            return currentResources;
        }

        set
        {
            currentResources = value;
        }
    }

    public List<LAutoBuilding> CurrentAutoBuildings
    {
        get
        {
            return currentAutoBuildings;
        }

        set
        {
            currentAutoBuildings = value;
        }
    }

    public List<LHabitEntry> CurrentHabits
    {
        get
        {
            return currentHabits;
        }

        set
        {
            currentHabits = value;
        }
    }

    public ResourceData ResourcesCategoryData
    {
        get
        {
            if (_resourceCategoryData == null)
            {
                _resourceCategoryData = Resources.Load<ResourceData>(Constants.PATH_FOR_RESOURCE_CATEGORY_ASSET_LOAD);
            }
            return _resourceCategoryData;
        }
    }

    public ScheduleData Schedule_Data
    {
        get
        {
            if (_scheduleData == null)
            {
                _scheduleData = Resources.Load<ScheduleData>(Constants.PATH_FOR_SCHEDULE_ASSET_LOAD);
            }
            return _scheduleData;
        }
    }

    public ArtifactData Artifact_Data
    {
        get
        {
            if (_artifactData == null)
            {
                _artifactData = Resources.Load<ArtifactData>(Constants.PATH_FOR_ARTIFACT_ASSET_LOAD);
            }
            return _artifactData;
        }
    }

    public List<LTaskEntry> CurrentDailyTasks
    {
        get
        {
            if (currentDailyTasks == null)
            {
                currentDailyTasks = new List<LTaskEntry>();
            }

            return currentDailyTasks;
        }

        set
        {
            currentDailyTasks = value;
        }
    }

    public List<LSubTask> CurrentSubTasks
    {
        get
        {
            if (currentSubTasks == null)
            {
                currentSubTasks = new List<LSubTask>();
            }

            return currentSubTasks;
        }

        set
        {
            currentSubTasks = value;
        }
    }

    public List<LToDoEntry> CurrentToDos
    {
        get
        {
            if (currentToDos == null)
            {
                currentToDos = new List<LToDoEntry>();
            }
            return currentToDos;
        }

        set
        {
            currentToDos = value;
        }
    }

    public List<LAutoToDo> CurrentAutoToDos
    {
        get
        {
            return currentAutoToDos;
        }

        set
        {
            currentAutoToDos = value;
        }
    }

    public List<LAutoGoal> CurrentAutoGoals
    {
        get
        {
            return currentAutoGoals.ToList();
        }

        set
        {
            currentAutoGoals = value;
        }
    }

    public List<LProjectEntry> CurrentProjects
    {
        get
        {
            if (currentProjects == null)
            {
                currentProjects = new List<LProjectEntry>();
            }
            return currentProjects;
        }

        set
        {
            currentProjects = value;
        }
    }

    public List<LArtifact> CurrentArtifacts
    {
        get
        {
            if (currentArtifacts == null)
            {
                currentArtifacts = new List<LArtifact>();
            }
            return currentArtifacts;
        }

        set
        {
            currentArtifacts = value;
        }
    }

    public List<LArtwork> CurrentArtworks
    { 
        get
        {
            if (currentArtworks == null)
            {
                currentArtworks = new List<LArtwork>();
            }
            return currentArtworks;
        }

        set
        {
            currentArtworks = value;
        }
    }

    public List<CBuilding> InitCBuilddings
    {
        get
        {
            return initCBuildings;
        }
    }

    public List<CVillager> InitCVillagers
    {
        get
        {
            return initCVillagers;
        }
    }

    public List<CResource> InitCResources
    {
        get
        {
            return initCResources;
        }
    }

    [ContextMenu("Clear_LocalData")]
    public void Clear_LocalData()
    {
        PlayerPrefs.DeleteAll();
    }

    [ContextMenu("Drop_HP")]
    public void Drop_HP()
    {
        Dictionary<string, float> resourceDic = new Dictionary<string, float>();
        resourceDic.Add(EResources.Happiness.ToString(), -20f);
        UpdateResource(resourceDic, (isSuccess, errMsg) =>
        {
            UIManager.Instance.UpdateTopProfile();
        });
    }


    public void SignOut(System.Action<bool, string> callback)
    {
        ServerManager.Instance.SaveFUser(CurrentFUser, (isSuccess, errMsg, _) =>
        {
            if (isSuccess)
            {
                CurrentFUser = new FUser();
                Clear_LocalData();
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void SaveData()
    {
        //DBHandler.SaveToJSON(currentVillagers, villagerFN);
        //DBHandler.SaveToJSON(currentBuildings, buildingFN);
        //DBHandler.SaveToJSON(currentResources, resourceFN);
        //DBHandler.SaveToJSON(currentAutoBuildings, autoFN);
        //DBHandler.SaveToJSON(currentDailyTasks, dailyTaskFN);
        //DBHandler.SaveToJSON(currentSubTasks, subTaskFN);
        //DBHandler.SaveToJSON(currentToDos, toDOFN);
        //DBHandler.SaveToJSON(currentProjects, projectGoalFN);
        //DBHandler.SaveToJSON(currentSetting, settingFN);
        //DBHandler.SaveToJSON(currentHabits, habitFN);
        //DBHandler.SaveToJSON(currentAutoToDos, autoTodoFN);
        //DBHandler.SaveToJSON(currentAutoGoals, autoGoalFN);
        //DBHandler.SaveToJSON(currentArtifacts, artifactFN);
        //DBHandler.SaveToJSON(currentArtworks, artworkFN);
        SaveFUser();
    }
    #region Unity_Members
    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();

        currentFUser = new FUser();
        initResources = new List<LResource>();
        initVillagers = new List<LVillager>();
        initBuildings = new List<LBuilding>();

        currentUser = new LUser();
        currentSetting = new LSetting();
        currentUserList = new List<LUser>();
        currentVillagers = new List<LVillager>();
        currentBuildings = new List<LBuilding>();
        currentAutoBuildings = new List<LAutoBuilding>();
        currentResources = new List<LResource>();
        currentDailyTasks = new List<LTaskEntry>();
        currentSubTasks = new List<LSubTask>();
        currentToDos = new List<LToDoEntry>();
        currentProjects = new List<LProjectEntry>();
        currentHabits = new List<LHabitEntry>();
        currentAutoToDos = new List<LAutoToDo>();
        currentAutoGoals = new List<LAutoGoal>();
        currentCoalitions = new List<FCoalition>();
        currentArtifacts = new List<LArtifact>();
        currentArtworks = new List<LArtwork>();
        startTime = DateTime.Now;
        //InvokeRepeating(nameof(AutoSave), 300.0f, 1);
    }
    #endregion

    #region Public_Membersunity 

    public void AutoSave()
    {
        if (CurrentFUser.id != "")
        {
            SaveFUser();
        }
    }
    public void HandleAITaskManager()
    {
        var aiTaskManager = GetComponent<AITaskManager>();
        if (AppManager.Instance.GetCurrentMode() == Game_Mode.Game_Only)
        {
            if (aiTaskManager == null)
            {
                aiTaskManager = gameObject.AddComponent<AITaskManager>();
            }
        }
        else
        {
            RemoveAITaskManager();
        }
    }

    public void RemoveAITaskManager()
    {
        var aiTaskManager = GetComponent<AITaskManager>();
        if (aiTaskManager != null)
        {
            Destroy(aiTaskManager);
        }
    }

    public Sprite GetCurrentAvatarSprite()
    {
        return avatar_Images[currentUser.AvatarId];
    }

    public Sprite GetAvatarSprite(int index)
    {
        return avatar_Images[index];
    }

    public List<CResource> GetMarketResourceDic(EMarketType type)
    {
        List<CResource> resList = new List<CResource>();
        switch (type)
        {
            case EMarketType.BuyFromRepublic:
                resList = GetBuyResourceFromRepublic();
                break;
            case EMarketType.BuyFromMerchant:
                resList = GetBuyResourceFromMerchant();
                break;
            case EMarketType.BuytFromCoalition:
                resList = GetBuyResourceFromCoalition();
                break;
            case EMarketType.BuyFromSpecialist:
                resList = GetBuyResourceFromSpecialist();
                break;

            case EMarketType.SellToRepublic:
                resList = GetSellResourceToRepublic();
                break;
            case EMarketType.SellToMerchant:
                resList = GetSellResourceToMerchant();
                break;
            case EMarketType.SellToCoalition:
                resList = GetSellResourceToCoalition();
                break;
            default:
                break;
        }

        return resList;
    }



    public LUser GetCurrentUser()
    {
        return currentUser;
    }

    public LSetting GetCurrentSetting()
    {
        return currentSetting;
    }

    public List<FUser> GetCurrentFUserList()
    {
        return currentFUserList.ToList();
    }

    public List<LVillager> GetCurrentVillagers()
    {
        return currentVillagers;
    }

    public List<LVillager> GetOtherUserVillagers(LUser user)
    {
        var fUser = GetFUserFromUser(user);
        if (fUser != null)
        {
            return fUser.villager;
        }

        return new List<LVillager>();
    }

    public List<LBuilding> GetCurrentBuildings()
    {
        return currentBuildings;
    }

    public List<LBuilding> GetOtherUserBuildings(LUser user)
    {
        var fUser = GetFUserFromUser(user);
        if (fUser != null)
        {
            return fUser.building;
        }

        return new List<LBuilding>();
    }

    public List<LResource> GetCurrentResources()
    {
        return currentResources;
    }

    public List<LResource> GetOtherUserResources(LUser user)
    {
        var fUser = GetFUserFromUser(user);
        if (fUser != null)
        {
            return fUser.resource;
        }

        return new List<LResource>();
    }

    public List<LArtwork> GetOtherUserArtworks(LUser user)
    {
        var fUser = GetFUserFromUser(user);
        if (fUser != null)
        {
            return fUser.artwork;
        }

        return new List<LArtwork>();
    }

    public FUser GetFUserFromUser(LUser user)
    {
        for (int index = 0; index < currentUserList.Count; index++)
        {
            if (user.id == currentUserList[index].id)
            {
                return currentFUserList[index];
            }
        }

        return null;
    }

    public List<LTaskEntry> GetYesterdayTasks()
    {
        return yesterdayTasks;
    }

    public List<LVillager> GetInitVillager()
    {
        return initVillagers;
    }

    public List<LBuilding> GetInitBuilding()
    {
        return initBuildings;
    }

    public List<LResource> GetInitResource()
    {
        return initResources;
    }

    public List<FCoalition> GetCurrentCoalitions()
    {
        return currentCoalitions;
    }

    public string GetSalaryDate()
    {
        return currentUser.GetSalaryDate();
    }

    public string GetMaintenanceDate()
    {
        return currentUser.GetMaintenanceDate();
    }

    public string GetMealDate()
    {
        return currentUser.GetMealDate();
    }

    public LEntry GetCorrespondingEntry(string id, string type)
    {
        var result = new LEntry();
        switch (type)
        {
            case "DailyTask":
                result = CurrentDailyTasks.Find(item => item.id == id);
                break;
            case "ToDo":
                result = CurrentToDos.Find(item => item.id == id);
                break;
            case "Habit":
                result = CurrentHabits.Find(item => item.id == id);
                break;
            case "Project":
                result = CurrentProjects.Find(item => item.id == id);
                break;
            default:
                break;
        }

        return result;
    }

    public void GetEmailWithUserName(string username, System.Action<bool, string, string> callback)
    {
        ServerManager.Instance.GetEmail(username, (isSuccess, errMsg, email) =>
        {
            if (isSuccess)
            {
                email = email.Replace("\"", "");
            }
            callback(isSuccess, errMsg, email);
        });
    }

    public CBuilding GetBuilding(int buildingID)
    {
        return initCBuildings.Find(it => it.id == buildingID.ToString());
    }

    public void GetInvitations(System.Action<bool, string, List<FInvitation>> callback)
    {
        ServerManager.Instance.GetInvitations(CurrentFUser.id, callback);
    }

    public void GetTradeInvitations(System.Action<bool, string, List<FTradeInvitation>> callback)
    {
        ServerManager.Instance.GetTradeInvites(CurrentFUser.id, callback);
    }

    public void GetSentTrades(System.Action<bool, string, List<FTrade>> callback)
    {
        if (currentTrades.Count == 0)
        {
            ServerManager.Instance.GetTrades(CurrentFUser.id, (isSuccess, errMsg, list) =>
            {
                if (isSuccess)
                {
                    currentTrades = list;
                    callback?.Invoke(isSuccess, errMsg, currentTrades.FindAll((item) => item.sender == CurrentFUser.id));
                }
                else
                {
                    callback?.Invoke(isSuccess, errMsg, list);
                }
            });
        }
        else
        {
            callback?.Invoke(true, "Success", currentTrades.FindAll((item) => item.sender == CurrentFUser.id));
        }
        
    }

    public void GetReceiveTrades(System.Action<bool, string, List<FTrade>> callback)
    {
        if (currentTrades.Count == 0)
        {
            ServerManager.Instance.GetTrades(CurrentFUser.id, (isSuccess, errMsg, list) =>
            {
                if (isSuccess)
                {
                    currentTrades = list;
                    callback?.Invoke(isSuccess, errMsg, currentTrades.FindAll((item) => item.receiver == CurrentFUser.id));
                }
                else
                {
                    callback?.Invoke(isSuccess, errMsg, list);
                }
            });
        }
        else
        {
            callback?.Invoke(true, "Success", currentTrades.FindAll((item) => item.receiver == CurrentFUser.id));
        }
    }

    public void CancelTrades(string tradeId, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.CancelTrades(CurrentFUser.id, tradeId, (isSuccess, errMsg, list) =>
        {
            if (isSuccess)
            {
                currentTrades = list;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public LTaskEntry GetTaskEntry(string id)
    {
        return CurrentDailyTasks.Find(dt => dt.id == id);
    }

    public List<LTaskEntry> GetDailyTaskWithLink(string id)
    {
        return CurrentDailyTasks.FindAll(dt => dt.linkedGoalId == id);
    }

    public List<LHabitEntry> GetHabitWithLink(string id)
    {
        return CurrentHabits.FindAll(dt => dt.linkedGoalId == id);
    }

    public LToDoEntry GetToDoEntry(string id)
    {
        return CurrentToDos.Find(dt => dt.id == id);
    }

    public LProjectEntry GetProjectEntry(string id)
    {
        return CurrentProjects.Find(dt => dt.id == id);
    }

    public LHabitEntry GetHabitEntry(string id)
    {
        return CurrentHabits.Find(dt => dt.id == id);
    }

    public LProjectEntry GetProjectEntryWithLink(string linkedGoalId)
    {
        return CurrentProjects.Find(pt => pt.id == linkedGoalId);
    }


    public List<LSubTask> GetSubTasks(List<string> idList)
    {
        return GetTasks(idList, CurrentSubTasks);
    }


    public Sprite GetSprite(EResources resource)
    {
        var result = set_Sprite;

        switch (resource)
        {
            case EResources.Gold:
                result = gold_Sprite;
                break;
            case EResources.Happiness:
                result = happy_Sprite;
                break;
            case EResources.Wine:
                result = wine_Sprite;
                break;
            case EResources.Sapphire:
                result = sapphire_Sprite;
                break;
            case EResources.Diamond:
                result = diamond_Sprite;
                break;
            case EResources.Ruby:
                result = ruby_Sprite;
                break;
            case EResources.Paint:
                result = paint_Sprite;
                break;
            default:
                break;
        }

        return result;
    }

    public List<LSubTask> GetTasks(List<string> idList, List<LSubTask> taskList)
    {
        var result = new List<LSubTask>();
        foreach(string id in idList)
        {
            var lTask = taskList.Find(lt => lt.id == id);
            if (lTask != null)
            {
                result.Add(lTask);
            }
        }

        return result;
    }

    public string GetTemplePrefabName()
    {
        return ResourcesCategoryData.GetTemplePrefabName(currentUser.Religion_Name);
    }

    public string GetTempleBuildingName()
    {
        return ResourcesCategoryData.GetTempleBuildingName(currentUser.Religion_Name);
    }

    public void LoadInitCResources(System.Action<bool, string> callback)
    {
        ServerManager.Instance.LoadCResources((isSuccess, errMsg, list) =>
        {            
            if(isSuccess && list.Count > 0)
            {
                initCResources = list;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void LoadInitCBuildings(System.Action<bool, string> callback)
    {
        ServerManager.Instance.LoadCBuildings((isSuccess, errMsg, list) =>
        {
            if (isSuccess && list.Count > 0)
            {
                initCBuildings = list;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void LoadInitCVillagers(System.Action<bool, string> callback)
    {
        ServerManager.Instance.LoadCVillagers((isSuccess, errMsg, list) =>
        {
            if (isSuccess && list.Count > 0)
            {
                initCVillagers = list;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void LoadCoalitionData(System.Action<bool, string> callback)
    {
        ServerManager.Instance.LoadCoalitions((isSuccess, errMsg,list) =>
        {
            if (isSuccess)
            {
                this.currentCoalitions = list;
            }

            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void LoadInvitations(System.Action<bool, string, List<FInvitation>> callback = null)
    {
        ServerManager.Instance.LoadInvitations(callback);
    }
    public void LoadCArtworks(System.Action<bool, string, List<CArtwork>> callback)
    {
        ServerManager.Instance.LoadCArtworks(callback);
    }

    public void LoadCArtifacts(System.Action<bool, string, List<CArtifact>> callback)
    {
        ServerManager.Instance.LoadCArtifacts(callback);
    }

    public void LoadInitialData()
    {
    }

    public void LoadLocalData()
    {
        
    }

    public void LoadData()
    {
        CurrentVillagers = currentFUser.villager;
        CurrentBuildings = currentFUser.building;
        CurrentAutoBuildings = currentFUser.autoBuilding;
        CurrentResources = currentFUser.resource;
        CurrentDailyTasks = currentFUser.dailyTask;
        CurrentSubTasks = currentFUser.subTask;
        CurrentToDos = currentFUser.toDo;
        CurrentProjects = currentFUser.project;
        currentUser = currentFUser.user;
        currentSetting = currentFUser.setting;
        CurrentHabits = currentFUser.habit;
        CurrentAutoToDos = currentFUser.autoToDo;
        CurrentAutoGoals = currentFUser.autoGoal;
        CurrentArtifacts = currentFUser.artifact;
        CurrentArtworks = currentFUser.artwork;
        yesterdayTasks = currentFUser.uncompletedDailyTasks;
        Debug.LogError("Loadedd Data" + yesterdayTasks.Count);
        GetToken();
        OnDataUpdated(true);
    }

    public void LoadData(System.Action<bool, string> callback)
    {
        
        CurrentVillagers = currentFUser.villager;
        
        currentUser.Save();
        currentSetting.Save();
        SerializeUser(false, callback);
    }

    public void SerializeUser(bool bForce = false, System.Action<bool, string> callback = null)
    {
        SaveFUser(callback);
    }

    public void UpdateUser(LUser user)
    {
        currentUser = user;
        currentUser.Save();
    }

    public void UpdateUser(string userId, string username, string email)
    {
        currentUser = new LUser();
        currentUser.Update(userId, username, email);
    }

    public void UpdateUser(FUser user, System.Action<bool, string> callback)
    {
        try
        {
            this.CurrentFUser = user;
            PlayerPrefs.SetString("UpdatedTime", user.updated_at);
            LoadData(callback);

        }catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
        
    }

    public void UpdateNewUser(FUser user)
    {
        currentFUser = user;
        PlayerPrefs.SetString("UpdatedTime", user.updated_at);
        LoadInitialData();
    }

    public void CreateFUser(string email, string password, string username,  System.Action<bool, string> callback)
    {
        ServerManager.Instance.CreateFUser(email, password, username, (isSuccess, errrMsg, fUser) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = fUser;
                LoadData();
            }
            callback(isSuccess, errrMsg);
        });

    }

    public void GetFUser(string fUserId, System.Action<bool, string> callback)
    {
        ServerManager.Instance.GetFUser(fUserId, (isSuccess, errrMsg, fUser) =>
        {
            if (isSuccess)
            {
                currentFUser = fUser;
                LoadData();
            }
            callback(isSuccess, errrMsg);
        });
        
    }

    public void UpdateUser(string firstName, string secondName, string villageName, bool isVegetarian, bool hasReligion, System.Action<bool, string> callback)
    {
        ServerManager.Instance.UpdatePersonalInfo(currentFUser.id, currentFUser.user.AvatarId, firstName, secondName, villageName, isVegetarian, hasReligion, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void UpdateUser(string created_coalition, string joined_coalition, System.Action<bool, string> callback)
    {
        CurrentFUser.user.Update(created_coalition, joined_coalition, callback);
    }

    public void UpdateReligion(string religion)
    {
        CurrentFUser.user.UpdateReligion(religion);
    }

    public void UpdateUserAvatarId(int nId)
    {
        CurrentFUser.user.UpdateAvatarId(nId);
    }

    public void UpdateSalaryDate(System.DateTime dateTime)
    {
        CurrentFUser.user.updateSalaryDate(dateTime);
    }

    public void UpdateMaintenanceDate(System.DateTime dateTime)
    {
        CurrentFUser.user.updateMaintenanceDate(dateTime);
    }

    public void UpdateMealDate(System.DateTime dateTime)
    {
        CurrentFUser.user.updateMealDate(dateTime);
    }

    public void UpdateCoalition(FCoalition coalition, System.Action<bool, string> callback)
    {
        foreach(FCoalition fCoalition in currentCoalitions)
        {
            if (fCoalition.id == coalition.id)
            {
                currentCoalitions.Remove(fCoalition);
                break;
            }
        }

        currentCoalitions.Add(coalition);

    }

    public void CreateDailyTask(LTaskEntry entry, List<LSubTask> subTasks, System.Action<bool, string> callback)
    {
        ServerManager.Instance.CreateDailyTask(CurrentFUser.id, entry, subTasks, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void RemoveDailyTask(LTaskEntry entry, System.Action<bool, string> callback)
    {
        var subTasks = GetSubTasks(entry.subTasks);
        ServerManager.Instance.RemoveDailyTask(CurrentFUser.id, entry, subTasks, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void CompleteDailyTask(LTaskEntry entry, System.Action<bool, string, Dictionary<EResources, float>> callback = null)
    {
        var subTasks = GetSubTasks(entry.subTasks);

        ServerManager.Instance.CompleteDailyTask(CurrentFUser.id, entry, subTasks, (isSuccess, errMsg, result) =>
        {
            if (isSuccess)
            {
                CurrentFUser = result.fUser;
                callback?.Invoke(isSuccess, errMsg, result.reward);
            }
            else
            {
                callback?.Invoke(isSuccess, errMsg, null);
            }
        });
    }

    public void CancelDailyTaskComplete(LTaskEntry entry, System.Action<bool, string, Dictionary<EResources, float>> callback = null)
    {
        var subTasks = GetSubTasks(entry.subTasks);

        ServerManager.Instance.CancelDailyTaskComplete(CurrentFUser.id, entry, subTasks, (isSuccess, errMsg, result) =>
        {
            if (isSuccess)
            {
                CurrentFUser = result.fUser;
                callback?.Invoke(isSuccess, errMsg, result.reward);
            }
            else
            {
                callback?.Invoke(isSuccess, errMsg, null);
            }
        });
    }

    public void ArrangeDailyTask(List<LTaskEntry> entryList, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.ArrangeDailyTask(CurrentFUser.id, entryList, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void CheckOffYesterdayTask(List<LTaskEntry> entryList, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.CheckOffYesterdayTask(CurrentFUser.id, entryList, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void CompleteSubTask(LSubTask subTask, System.Action<bool, string, Dictionary<EResources, float>> callback = null)
    {
        ServerManager.Instance.CompleteSubTask(CurrentFUser.id, subTask, (isSuccess, errMsg, result) =>
        {
            if (isSuccess)
            {
                CurrentFUser = result.fUser;
                callback?.Invoke(isSuccess, errMsg, result.reward);
            }
            else
            {
                callback?.Invoke(isSuccess, errMsg, null);
            }
        });
    }

    public void CancelSubTaskComplete(LSubTask subTask, System.Action<bool, string, Dictionary<EResources, float>> callback = null)
    {
        ServerManager.Instance.CancelSubTaskComplete(CurrentFUser.id, subTask, (isSuccess, errMsg, result) =>
        {
            if (isSuccess)
            {
                CurrentFUser = result.fUser;
                callback?.Invoke(isSuccess, errMsg, result.reward);
            }
            else
            {
                callback?.Invoke(isSuccess, errMsg, null);
            }
        });
    }

    public void CreateToDo(LToDoEntry entry, List<LSubTask> subTasks, System.Action<bool, string> callback)
    {
        ServerManager.Instance.CreateToDo(CurrentFUser.id, entry, subTasks, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void RemoveToDo(LToDoEntry entry, System.Action<bool, string> callback)
    {
        var subTasks = GetSubTasks(entry.checkList);
        ServerManager.Instance.RemoveToDo(CurrentFUser.id, entry, subTasks, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void CompleteToDo(LToDoEntry entry, System.Action<bool, string, Dictionary<EResources, float>> callback = null)
    {
        var subTasks = GetSubTasks(entry.checkList);

        ServerManager.Instance.CompleteToDo(CurrentFUser.id, entry, subTasks, (isSuccess, errMsg, result) =>
        {
            if (isSuccess)
            {
                CurrentFUser = result.fUser;
                callback?.Invoke(isSuccess, errMsg, result.reward);
            }
            else
            {
                callback?.Invoke(isSuccess, errMsg, null);
            }
        });
    }

    public void CompleteAutoToDo(string todoId, System.Action<bool, string, Dictionary<EResources, float>> callback = null)
    {
        ServerManager.Instance.CompleteAutoToDo(CurrentFUser.id, todoId, (isSuccess, errMsg, result) =>
        {
            if (isSuccess)
            {
                CurrentFUser = result.fUser;
                callback?.Invoke(isSuccess, errMsg, result.reward);
            }
            else
            {
                callback?.Invoke(isSuccess, errMsg, null);
            }
        });
    }

    public void ArrangeToDo(List<LToDoEntry> entryList, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.ArrangeToDo(CurrentFUser.id, entryList, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void CreateHabit(LHabitEntry entry, System.Action<bool, string> callback)
    {
        ServerManager.Instance.CreateHabit(CurrentFUser.id, entry, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void RemoveHabit(LHabitEntry entry, System.Action<bool, string> callback)
    {
        ServerManager.Instance.RemoveHabit(CurrentFUser.id, entry, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void CompleteHabit(LHabitEntry entry, System.Action<bool, string, Dictionary<EResources, float>> callback = null)
    {
        ServerManager.Instance.CompleteHabit(CurrentFUser.id, entry, (isSuccess, errMsg, result) =>
        {
            if (isSuccess)
            {
                CurrentFUser = result.fUser;
                callback?.Invoke(isSuccess, errMsg, result.reward);
            }
            else
            {
                callback?.Invoke(isSuccess, errMsg, null);
            }
        });
    }

    public void CancelHabitComplete(LHabitEntry entry, System.Action<bool, string, Dictionary<EResources, float>> callback = null)
    {
        ServerManager.Instance.CancelHabitComplete(CurrentFUser.id, entry, (isSuccess, errMsg, result) =>
        {
            if (isSuccess)
            {
                CurrentFUser = result.fUser;
                callback?.Invoke(isSuccess, errMsg, result.reward);
            }
            else
            {
                callback?.Invoke(isSuccess, errMsg, null);
            }
        });
    }

    public void ArrangeHabit(List<LHabitEntry> entryList, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.ArrangeHabit(CurrentFUser.id, entryList, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void CreateProject(LProjectEntry entry, List<LSubTask> subTasks, System.Action<bool, string> callback)
    {
        ServerManager.Instance.CreateProject(CurrentFUser.id, entry, subTasks, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void CompleteProject(LProjectEntry entry, System.Action<bool, string, Dictionary<EResources, float>> callback = null)
    {
        var subTasks = GetSubTasks(entry.subProjects);

        ServerManager.Instance.CompleteProject(CurrentFUser.id, entry, subTasks, (isSuccess, errMsg, result) =>
        {
            if (isSuccess)
            {
                CurrentFUser = result.fUser;
                callback?.Invoke(isSuccess, errMsg, result.reward);
            }
            else
            {
                callback?.Invoke(isSuccess, errMsg, null);
            }
        });
    }


    public void RemoveProject(LProjectEntry entry, System.Action<bool, string> callback)
    {
        var subTasks = GetSubTasks(entry.subProjects);
        ServerManager.Instance.RemoveProject(CurrentFUser.id, entry, subTasks, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }
    
    public void ArrangeProject(List<LProjectEntry> entryList, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.ArrangeProject(CurrentFUser.id, entryList, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void SaveFUser(System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.SaveFUser(CurrentFUser, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void SetToken(string token)
    {
        ServerManager.Instance.SetToken(CurrentFUser.id, token, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
                bTokenRegister = true;
            }
        });
    }

    public void GetToken()
    {
#if !UNITY_EDITOR
        StartCoroutine("GetTokenAsync");
#endif
    }

    private IEnumerator GetTokenAsync()
    {
        var task = Firebase.Messaging.FirebaseMessaging.GetTokenAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError("GetToken encountered an error: " + task.Exception);
        }
        else
        {
            SetTokenValue(task.Result);
        }
    }

    public void SetTokenValue(string token)
    {
        if (PlayerPrefs.GetString("TToken") != token)
        {
            PlayerPrefs.SetString("TToken", token);
            SetToken(token);
        }
    }

    public void UpdateEntry(LTaskEntry entry, List<LSubTask> subTasks)
    {

        foreach (LSubTask lTask in subTasks)
        {
            var prevTaskId = CurrentSubTasks.FindIndex(dt => dt.id == lTask.id);
            if (prevTaskId < 0)
            {
                CurrentSubTasks.Add(lTask);
            }
            else
            {
                CurrentSubTasks[prevTaskId] = lTask;
            }
        }
        CurrentSubTasks.RemoveAll(dt => dt.IsRemoved());

        DBHandler.SaveToJSON(currentSubTasks, subTaskFN);


        var prevEntryId = CurrentDailyTasks.FindIndex(dt => dt.id == entry.id);
        
        if (prevEntryId < 0)
        {
            currentDailyTasks.Add(entry);
        }
        else
        {
            CurrentDailyTasks[prevEntryId] = entry;
        }
        CurrentDailyTasks.RemoveAll(dt => dt.IsRemoved());

        DBHandler.SaveToJSON(currentDailyTasks, dailyTaskFN);
    }

    public void UpdateEntry(LToDoEntry entry, List<LSubTask> subTasks)
    {

        foreach (LSubTask lTask in subTasks)
        {
            var prevTaskId = CurrentSubTasks.FindIndex(dt => dt.id == lTask.id);
            if (prevTaskId < 0)
            {
                CurrentSubTasks.Add(lTask);
            }
            else
            {
                CurrentSubTasks[prevTaskId] = lTask;
            }
        }
        CurrentSubTasks.RemoveAll(dt => dt.IsRemoved());

        DBHandler.SaveToJSON(currentSubTasks, subTaskFN);


        var prevEntryId = CurrentToDos.FindIndex(dt => dt.id == entry.id);
        
        if (prevEntryId < 0)
        {
            CurrentToDos.Add(entry);
        }
        else
        {
            CurrentToDos[prevEntryId] = entry;
        }
        CurrentToDos.RemoveAll(dt => dt.IsRemoved());

        DBHandler.SaveToJSON(currentToDos, toDOFN);
    }

    public void UpdateEntry(LAutoToDo entry, List<LSubTask> subTasks)
    {
        foreach (LSubTask lTask in subTasks)
        {
            var prevTaskId = CurrentSubTasks.FindIndex(dt => dt.id == lTask.id);
            if (prevTaskId < 0)
            {
                CurrentSubTasks.Add(lTask);
            }
            else
            {
                CurrentSubTasks[prevTaskId] = lTask;
            }
        }
        CurrentSubTasks.RemoveAll(dt => dt.IsRemoved());

        DBHandler.SaveToJSON(currentSubTasks, subTaskFN);


        var prevEntryId = CurrentAutoToDos.FindIndex(dt => dt.id == entry.id);

        if (prevEntryId < 0)
        {
            CurrentAutoToDos.Add(entry);
        }
        else
        {
            CurrentAutoToDos[prevEntryId] = entry;
        }
        CurrentAutoToDos.RemoveAll(dt => dt.IsRemoved());
    }

    public void UpdateEntry(LProjectEntry entry, List<LSubTask> subTasks, List<LTaskEntry> lTaskEntries)
    {

        foreach (LSubTask lTask in subTasks)
        {
            var prevTaskId = CurrentSubTasks.FindIndex(dt => dt.id == lTask.id);
            if (prevTaskId < 0)
            {
                CurrentSubTasks.Add(lTask);
            }
            else
            {
                CurrentSubTasks[prevTaskId] = lTask;
            }
        }
        CurrentSubTasks.RemoveAll(dt => dt.IsRemoved());

        DBHandler.SaveToJSON(currentSubTasks, subTaskFN);

        foreach(LTaskEntry lTaskEntry in lTaskEntries)
        {
            var linkedTaskId = CurrentDailyTasks.FindIndex(dt => dt.id == lTaskEntry.id);

            if (linkedTaskId >= 0)
            {
                CurrentDailyTasks[linkedTaskId] = lTaskEntry;
            }
        }

        DBHandler.SaveToJSON(currentDailyTasks, dailyTaskFN);


        var prevEntryId = CurrentProjects.FindIndex(dt => dt.id == entry.id);

        if (prevEntryId < 0)
        {
            CurrentProjects.Add(entry);
        }
        else
        {
            CurrentProjects[prevEntryId] = entry;
        }
        CurrentProjects.RemoveAll(dt => dt.IsRemoved());

        DBHandler.SaveToJSON(currentProjects, projectGoalFN);
    }

    public void UpdateEntry(LAutoGoal entry)
    {

        var prevEntryId = CurrentAutoGoals.FindIndex(dt => dt.id == entry.id);

        if (prevEntryId < 0)
        {
            CurrentAutoGoals.Add(entry);
        }
        else
        {
            CurrentAutoGoals[prevEntryId] = entry;
        }

        DBHandler.SaveToJSON(currentProjects, projectGoalFN);
    }

    public void UpdateEntry(LSubTask entry)
    {
        var prevTaskId = CurrentSubTasks.FindIndex(dt => dt.id == entry.id);
        if (prevTaskId < 0)
        {
            CurrentSubTasks.Add(entry);
        }
        else
        {
            CurrentSubTasks[prevTaskId] = entry;
        }

        CurrentSubTasks.RemoveAll(dt => dt.IsRemoved());

        DBHandler.SaveToJSON(currentSubTasks, subTaskFN);
    }

    public void UpdateEntry(LHabitEntry entry)
    {
        var prevTaskId = CurrentHabits.FindIndex(dt => dt.id == entry.id);
        if (prevTaskId < 0)
        {
            CurrentHabits.Add(entry);
        }
        else
        {
            CurrentHabits[prevTaskId] = entry;
        }

        CurrentHabits.RemoveAll(dt => dt.IsRemoved());
        DBHandler.SaveToJSON(currentSubTasks, subTaskFN);
    }

    public void UpdateArt(LArtifact artifact)
    {
        var prevIndex = CurrentArtifacts.ToList().FindIndex(item => item.id == artifact.id);

        if (prevIndex < 0)
        {
            CurrentArtifacts.Add(artifact);
        }
        else
        {
            CurrentArtifacts[prevIndex] = artifact;
        }
    }

    public void UpdateArt(LArtwork artwork, bool bRemove = false)
    {
        if (artwork == null)
        {
            return;
        }
        
        if (bRemove)
        {
            CurrentArtworks.Remove(artwork);
            return;
        }

        var prevIndex = CurrentArtworks.ToList().FindIndex(item => item.id == artwork.id);
        if (prevIndex < 0)
        {
            CurrentArtworks.Add(artwork);
        }
        else
        {
            CurrentArtworks[prevIndex] = artwork;
        }
    }

    public void ResetDates(System.DateTime dateTime)
    {
        currentUser.resetDates(dateTime);
    }

    public void UpdateSetting(Game_Mode mode, System.Action<bool, string> callback)
    {
        ServerManager.Instance.ChangeGameMode(currentFUser.id, mode, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                CurrentFUser = user;
                LoadData();
                //dailyReportStrList.Clear();
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void UpdateSetting(Interaction_Mode mode)
    {
        currentSetting.Update(mode);
    }

    public void UpdateSetting(bool shelter_storm)
    {
        currentSetting.Update(shelter_storm);
    }

    public void UpdateResource(LResource res)
    {
        var oldResource = currentResources.Find(it => it.id == res.id);
        if (oldResource != null)
        {
            currentResources.Remove(oldResource);
        }
        currentResources.Add(res);

        //DBHandler.SaveToJSON(currentResources, resourceFN);
    }

    public void UpdateResource(List<LResource> resList)
    {
        foreach(LResource res in resList)
        {
            var oldResource = currentResources.Find(it => it.id == res.id);
            if (oldResource != null)
            {
                currentResources.Remove(oldResource);
            }
            currentResources.Add(res);
        }
        
        //DBHandler.SaveToJSON(currentResources, resourceFN);
    }

    public void UpdateResource(EResources res, float amount)
    {
        var dic = new Dictionary<string, float>();
        dic.Add(res.ToString(), amount);
        UpdateResource(dic, null);
    }

    public void UpdateResource(Dictionary<string, float> resourceDic, System.Action<bool, string> callback)
    {
        var updateResList = new List<LResource>();
        foreach (string key in resourceDic.Keys)
        {
            CResource cResource = initCResources.ToList().Find(cRes => cRes.type.ToString() == key);
            if (cResource != null)
            {
                var res = currentResources.ToList().Find(item => item.id == cResource.id);
                if (res != null)
                {
                    res.current_amount += resourceDic[key];
                    res.current_amount = Mathf.Max(0f, res.current_amount);
                    res.created_at = Convert.DateTimeToFDate(System.DateTime.Now);
                    if (key == EResources.Happiness.ToString())//Happiness
                    {
                        res.current_amount = Mathf.Min(100f, res.current_amount);
                        if (GetCurrentSetting().game_mode == (int)Game_Mode.Game_Only)
                        {
                            AITaskManager.Instance.CheckOnCompleteWithHappiness(res.current_amount);
                        }
                        //currentUser.updateHappiness(res.current_amount, resourceDic[key]);
                    }
                }
                else
                {
                    res = new LResource();
                    res.id = cResource.id;
                    res.current_amount = resourceDic[key];
                }
                updateResList.Add(res);
            }
        }
        UpdateResource(updateResList);
        if (callback != null)
        {
            callback(true, "");
        }
    }

    public void BuyResource(Dictionary<EResources, float> resourceDic, System.Action<bool, string> callback)
    {
        var updateResList = new List<LResource>();

        foreach (EResources key in resourceDic.Keys)
        {
            foreach (LResource res in currentResources.ToList())
            {
                CResource cResource = initCResources.ToList().Find(cRes => cRes.id == res.id);
                if (cResource.type == key)
                {
                    res.current_amount += resourceDic[key];
                    res.current_amount = Mathf.Max(0f, res.current_amount);
                    res.created_at = Convert.DateTimeToFDate(System.DateTime.Now);
                    if (key == EResources.Happiness)
                    {
                        res.current_amount = Mathf.Min(100f, res.current_amount);
                        if (GetCurrentSetting().game_mode == (int)Game_Mode.Game_Only)
                        {
                            AITaskManager.Instance.CheckOnCompleteWithHappiness(res.current_amount);
                        }
                        //currentUser.updateHappiness(res.current_amount, resourceDic[key]);
                    }
                    if (cResource.marketable_count > 0 && resourceDic[key] > 0)
                    {
                        res.OnPurchased();
                    }

                    updateResList.Add(res);
                }
            }
        }
        UpdateResource(updateResList);
        if (callback != null){
            callback(true, "");
        }
    }

    public void GetDailyReport(System.Action<List<string>> callback)
    {
        if (PlayerPrefs.GetString("DailyReport") == Convert.DateTimeToFDate(System.DateTime.Now) && dailyReportStrList.Count > 0)
        {
            var list = dailyReportStrList.ToList();
            list.AddRange(GetLocalReportList(System.DateTime.Now));
            callback(list);
            return;
        }

        GetDailyReportString(strList =>
        {
            PlayerPrefs.SetString("DailyReport", Convert.DateTimeToFDate(System.DateTime.Now));
            var list = strList.ToList();
            list.AddRange(GetLocalReportList(System.DateTime.Now));
            callback(list);
        });
    }

    public void AddDailyReport(string s)
    {
        var date = Convert.DateTimeToFDate(System.DateTime.Now);
        var samereport = tempReport.Find(x => x.date == date && x.report == s);
        if (samereport == null)
        {
            tempReport.Add(new LocalReport(s, date));
        }
    }

    private List<LBuilding> GetAllBuiltLBuildings()
    {
        return currentBuildings.FindAll(item => item.progress == 1.0f);
    }

    private string GetBuildingName(LBuilding lBuilding)
    {
        var buildingCategory = GetBuilding(int.Parse(lBuilding.id));
        if (buildingCategory != null)
        {
            return buildingCategory.name;
        }
        return "Unknown Building";
    }

    
    public void CreateCoalition(string coalitionName, string timeZone, System.Action<bool, string> callback)
    {
        ServerManager.Instance.CreateCoalition(CurrentFUser.id, coalitionName, timeZone, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void GetCoalitionMembers(System.Action<bool, string, List<LUser>> callback)
    {
        ServerManager.Instance.GetCoalitionMembers(currentFUser.id, (isSuccess, errMsg, list) =>
        {
            if (isSuccess)
            {
                currentFUserList.Clear();
                currentUserList.Clear();
                foreach(FUser fUser in list)
                {
                    currentFUserList.Add(fUser);
                    currentUserList.Add(fUser.user);
                }
            }
            callback?.Invoke(isSuccess, errMsg, currentUserList);
        });
    }

    public List<LUser> GetCurrentMemberList()
    {
        return currentUserList;
    }

    public void LeaveCoalition(System.Action<bool, string> callback)
    {
        ServerManager.Instance.LeaveCoalition(CurrentFUser.id, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void OpenCoalition(System.Action<bool, string> callback)
    {
        ServerManager.Instance.OpenCoalition(CurrentFUser.id, (isSuccess, errMsg, list) =>
        {
            if (isSuccess)
            {
                this.currentCoalitions = list;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void CloseCoalition(System.Action<bool, string> callback)
    {
        ServerManager.Instance.CloseCoalition(CurrentFUser.id, (isSuccess, errMsg, list) =>
        {
            if (isSuccess)
            {
                this.currentCoalitions = list;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void SendJoinInvite(string coalitionName, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.SendJoinInvite(CurrentFUser.id, coalitionName, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void InviteVillagerToCoalition(string villagerName, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.InviteVillagerToCoalition(CurrentFUser.id, villagerName, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void InviteUserToCoalition(string invitee, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.InviteUserToCoalition(CurrentFUser.id, invitee, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void AcceptCoalitionInvite(string inviteId, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.AcceptCoalitionInvite(CurrentFUser.id, inviteId, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void RejectCoalitionInvite(string inviteId, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.RejectCoalitionInvite(CurrentFUser.id, inviteId, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void KickCoalitionMember(string memberId, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.KickCoalitionMember(CurrentFUser.id, memberId, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void Excavate(System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.Excavate(CurrentFUser.id, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void CompleteExcavate(string artifactId, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.CompleteExcavate(CurrentFUser.id, artifactId, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void PickArtwork(LArtwork artwork, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.PickArtwork(CurrentFUser.id, JsonUtility.ToJson(artwork), (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);

        });
    }

    public void GetCoalitionMessage(string coalition, System.Action<bool, string, List<FMessage>> callback = null)
    {
        ServerManager.Instance.GetCoalitionMessage(coalition, callback);
    }

    public void GetPrivateMessage(System.Action<bool, string, List<FMessage>> callback = null)
    {
        ServerManager.Instance.GetPrivateMessage(CurrentFUser.id, callback);
    }

    public void SendCoalitionMessage(string content, System.Action<bool, string, FMessage> callback = null)
    {
        ServerManager.Instance.SendCoalitionMessage(CurrentFUser.id, content, callback);
    }

    public void SendPrivateMessage(string receiver, string content, System.Action<bool, string, FMessage> callback = null)
    {
        ServerManager.Instance.SendPrivateMessage(CurrentFUser.id, receiver, content, callback);
    }

    public void SendTradeInvite(string receiver, EResources resource1, float amount1, EResources resource2, float amount2, ETradeRepeat repeat, ETradeInviteType type, System.Action<bool, string, FInvitation> callback = null)
    {
        ServerManager.Instance.SendTradeInvite(CurrentFUser.id, receiver, resource1.ToString(), amount1, resource2.ToString(), amount2, (int)repeat, type, callback);
    }

    public void AcceptTradeInvite(string inviteId, System.Action<bool, string, FInvitation> callback = null)
    {
        ServerManager.Instance.AcceptTradeInvite(CurrentFUser.id, inviteId, callback);
    }

    public void RejectTradeInvite(string inviteId, System.Action<bool, string, FInvitation> callback = null)
    {
        ServerManager.Instance.RejectTradeInvite(CurrentFUser.id, inviteId, callback);
    }

    public void GetTradeInvites(System.Action<bool, string, List<FTradeInvitation>> callback = null)
    {
        ServerManager.Instance.GetTradeInvites(CurrentFUser.id, callback);
    }

    public void PostArtTrades(string paint, string artist1, string artist2, System.Action<bool, string, FArtTrade> callback = null)
    {
        ServerManager.Instance.PostArtTrades(CurrentFUser.id, paint, artist1, artist2, callback);
    }

    public void SubmitArtTrades( string tradeId, string paint, string artist, System.Action<bool, string, FArtTrade> callback = null)
    {
        ServerManager.Instance.SubmitArtTrades(CurrentFUser.id, tradeId, paint, artist, callback);
    }

    public void AcceptArtTrades( string tradeId, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.AcceptArtTrades(CurrentFUser.id, tradeId, (isSuccess, errMsg, user) =>
        {
            if (isSuccess)
            {
                this.CurrentFUser = user;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void RejectArtTrades( string tradeId, System.Action<bool, string, FArtTrade> callback = null)
    {
        ServerManager.Instance.RejectArtTrades(CurrentFUser.id, tradeId, callback);
    }

    public void RemoveArtTrades( string tradeId, System.Action<bool, string, FArtTrade> callback = null)
    {
        ServerManager.Instance.RemoveArtTrades(CurrentFUser.id, tradeId, callback);
    }

    public void GetAllArtTrades(System.Action<bool, string, List<FArtTrade>> callback = null)
    {
        ServerManager.Instance.GetAllArtTrades(callback);
    }

    public void GetSendArtTrades( System.Action<bool, string, List<FArtTrade>> callback = null)
    {
        ServerManager.Instance.GetSendArtTrades(CurrentFUser.id, callback);
    }

    public void GetReceiveArtTrades(System.Action<bool, string, List<FArtTrade>> callback = null)
    {
        ServerManager.Instance.GetReceiveArtTrades(CurrentFUser.id, callback);
    }

    public void GetPublicMessages(string coalitionName, System.Action<bool, string, List<FMessage>> callback)
    {
        ServerManager.Instance.GetCoalitionMessage(coalitionName, callback);
    }

    public void GetPrivateMessages(System.Action<bool, string, List<FMessage>> callback)
    {
        ServerManager.Instance.GetPrivateMessage(CurrentFUser.id, callback);
    }

    public void StartToBuild(string bId, string cBuildingId, bool bQuick, string createdAt, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.StartToBuild(CurrentFUser.id, bId, cBuildingId, bQuick, createdAt, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void CompleteBuilding(string bId, string createdAt, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.CompleteBuilding(CurrentFUser.id, bId, createdAt, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void CancelBuilding(string bId, string cBuildingId, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.CancelBuilding(CurrentFUser.id, bId, cBuildingId, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void ConvertQuickBuilding(string bId, string cBuildingId, string createdAt, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.ConvertQuickBuilding(CurrentFUser.id, bId, cBuildingId, createdAt, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

    public void ExchangeResource(EResources type1, float amount1, EResources type2, float amount2, System.Action<bool, string> callback = null)
    {
        ServerManager.Instance.ExchangeResource(CurrentFUser.id, type1, amount1, type2, amount2, (isSuccess, errMsg, fUser) =>
        {
            if (isSuccess)
            {
                CurrentFUser = fUser;
            }
            callback?.Invoke(isSuccess, errMsg);
        });
    }

        public void UpdateVillagers(List<LVillager> fVillagers)
    {
        foreach(LVillager villager in fVillagers)
        {
            UpdateVillager(villager);
        }
    }

    public void UpdateBuildings(List<LBuilding> fBuildings)
    {
        currentBuildings = fBuildings;
    }

    public void UpdateVillager(LVillager villager)
    {
        var oldVillager = currentVillagers.Find(it => it.UID == villager.UID);
        if (oldVillager != null)
        {
            currentVillagers.Remove(oldVillager);
        }
        currentVillagers.Add(villager);
    }

    public void UpdateAutoBuilding(LAutoBuilding building)
    {
        var oldBuilding = currentAutoBuildings.Find(it => it.id == building.id);
        if (oldBuilding != null)
        {
            currentAutoBuildings.Remove(oldBuilding);
        }
        currentAutoBuildings.Add(building);
    }

    public void RemoveVillager(LVillager villager)
    {
        currentVillagers.Remove(villager);
    }

    public void UpdateBuilding(LBuilding building)
    {
        var prevID = currentBuildings.FindIndex(lb => lb.bID == building.bID);
        if (prevID >= 0)
        {
            currentBuildings.RemoveAt(prevID);
        }
        currentBuildings.Add(building);
    }

    public void UpdateResources(List<LResource> fResources)
    {
        currentResources = fResources;
    }

    
    public void AutoLoggedIn(string userId)
    {
        
    }

#endregion

#region Private Members

    private List<CResource> GetBuyResourceFromRepublic()
    {
        List<CResource> resultList = new List<CResource>();
        foreach(CResource resource in initCResources)
        {
            if (resource.buy_price_from_republic > 0)
            {
                resultList.Add(resource);
            }
        }

        return resultList;
    }

    private List<CResource> GetBuyResourceFromMerchant()
    {
        List<CResource> resultList = new List<CResource>();
        foreach (CResource resource in initCResources)
        {
            if (resource.buy_price_from_merchant > 0)
            {
                resultList.Add(resource);
            }
        }

        return resultList;
    }

    private List<CResource> GetBuyResourceFromCoalition()
    {
        List<CResource> resultList = new List<CResource>();
        foreach (CResource resource in initCResources)
        {
            if (resource.buy_price_from_coalition > 0)
            {
                resultList.Add(resource);
            }
        }

        return resultList;
    }

    private List<CResource> GetBuyResourceFromSpecialist()
    {
        List<CResource> resultList = new List<CResource>();
        foreach (CResource resource in initCResources)
        {
            if (resource.buy_price_from_specialist > 0)
            {
                resultList.Add(resource);
            }
        }

        return resultList;
    }

    private List<CResource> GetSellResourceToRepublic()
    {
        List<CResource> resultList = new List<CResource>();
        foreach (CResource resource in initCResources)
        {
            if (resource.sell_price_to_republic > 0)
            {
                resultList.Add(resource);
            }
        }

        return resultList;
    }

    private List<CResource> GetSellResourceToMerchant()
    {
        List<CResource> resultList = new List<CResource>();
        foreach (CResource resource in initCResources)
        {
            if (resource.sell_price_to_merchant > 0)
            {
                resultList.Add(resource);
            }
        }

        return resultList;
    }

    private List<CResource> GetSellResourceToCoalition()
    {
        List<CResource> resultList = new List<CResource>();
        foreach (CResource resource in initCResources)
        {
            if (resource.sell_price_to_coalition > 0)
            {
                resultList.Add(resource);
            }
        }

        return resultList;
    }
#endregion


#region Firestore Events
    private void OnCoalitionUpdated(List<FCoalition> coalitions)
    {
        this.currentCoalitions.Clear();
        foreach(FCoalition coalition in coalitions)
        {
            if (coalition.isMemberOf(currentUser.id))
            {
                JoinedCoalition(coalition);
            }
            this.currentCoalitions.Add(coalition);
        }

    }

    private void JoinedCoalition(FCoalition coalition)
    {
        if (currentUser.fJC == false)
        {
            UpdateResource(EResources.Culture, 10f);
            currentUser.fJC = true;
            currentUser.Save();
        }
        currentUser.UpdateJoinCoalition(coalition.name);
    }

    private List<string> GetLocalReportList(System.DateTime dateTime)
    {
        var result = new List<string>();
        var date = Convert.DateTimeToFDate(dateTime);
        tempReport.RemoveAll(x => x.date != date);

        foreach (LocalReport rt in tempReport)
        {
            result.Add(rt.report);
        }

        return result;
    }

    private void GetDailyReportString(System.Action<List<string>> callback)
    {

        dailyReportStrList.Clear();
        var day = System.DateTime.Now.DayOfWeek;
        if (availableDaysOfShip.Contains(day))
        {
            dailyReportStrList.Add("The Merchant's ship has docked.");
        }
        else
        {
            dailyReportStrList.Add("Merchant will not be docking today.");
        }

        var mode = (Game_Mode)currentSetting.game_mode;
        var bTodayGoal = false;
        if (mode == Game_Mode.Task_Only)
        {
            var goals = currentAutoGoals.FindAll(item => item.endDate == Convert.DateTimeToEntryDate(System.DateTime.Now));
            bTodayGoal = goals.Count > 0;

        }
        else
        {
            var goals = currentProjects.FindAll(item => item.endDate == Convert.DateTimeToEntryDate(System.DateTime.Now));
            bTodayGoal = goals.Count > 0;
        }

        if (bTodayGoal)
        {
            dailyReportStrList.Add("A Goal is due today.");
        }
        //TODO
        //'You've traded a Painting(coalition-see 'Trade') ---- if you or a coalition member made an offer to trade a piece of artwork, this pops up if the trade is agreed
        //to and successful(all modes)
        //
        //'You've traded a Painting(noncoalition- see 'Art Exchange')--- if you or a non-coalition member made an offer to trade a piece of artwork, this pops up if
        //the trade is agreed to and successful(all modes)

        //var lBuildings = GetBuiltLBuildings(System.DateTime.Now.AddDays(-1));
        var lBuildings = GetAllBuiltLBuildings();

        foreach (LBuilding lBuilding in lBuildings)
        {
            var tuple = lBuilding.NeedToHireSpecialist();
            var buildingName = GetBuildingName(lBuilding);
            if (tuple.Item1 != "")
            {
                if (mode != Game_Mode.Task_Only)
                {
                    dailyReportStrList.Add(string.Format("{0} has been constructed and specialists must be hired(press building)", buildingName));
                }
            }
            else
            {
                if (lBuilding.created_at.Equals(Convert.DateTimeToFDate(System.DateTime.Now.AddDays(-1))))
                {
                    dailyReportStrList.Add(string.Format("{0} has been constructed.", buildingName));
                }
            }
        }

        if (mode != Game_Mode.Task_Only)
        {
            if (ResourceViewController.Instance.GetVillagePopulation() >= 60 && ResourceViewController.Instance.GetMealAmount() < 200.0f)
            {
                dailyReportStrList.Add("Food is low, more farms are needed.");
            }


            if (currentArtifacts.Count > 0)
            {
                var museum = currentBuildings.Find(item => item.bID == "94");

                if (museum == null || museum.progress < 1.0f)
                {
                    dailyReportStrList.Add("In order to see your Artifacts, a Museum must be built.");
                }
            }

            if (CurrentArtworks.Count > 0)
            {
                var gallery = currentBuildings.Find(item => item.bID == "95");
                if (gallery == null || gallery.progress < 1.0f)
                {
                    dailyReportStrList.Add("In order to see your Artwork, a Gallery must be built");
                }
            }

            var guests = ResourceViewController.Instance.GetGuestsFromInn();
            var curatorGuests = guests.FindAll(item => item.id == "23");
            if (curatorGuests.Count > 0)//exist curator in villager inn
            {
                dailyReportStrList.Add("Housing needed (build Manor)");
            }

            if (guests.Count > curatorGuests.Count)
            {
                dailyReportStrList.Add("Housing needed (build Cottage)");
            }
        }
        
        if (ResourceViewController.Instance.GetCurrentResourceValue(EResources.Culture) >= 40)
        {
            dailyReportStrList.Add("You have reached 40 culture and can begin building Unique Structures.");
        }

        if (ArtifactSystem.Instance.GetRemainDaysUntilAvailable() <= 0)
        {
            dailyReportStrList.Add("Your archaeologist is now available for a new excavation.");
        }

        InvitationViewController.Instance.LoadInvitation((isSuccess, errMsg, invitationList) =>
        {
            var invitation = invitationList.Find(item => item.state == EState.Created.ToString() && item.isOutOfDate() == false);
            if (invitation != null)
            {
                dailyReportStrList.Add("You have new notifications(see 'Connections')");
            }

            InvitationViewController.Instance.LoadTradeInvitation((isSuccess, errMsg, tradeInvitationList) =>
            {
                if (tradeInvitationList != null && tradeInvitationList.Count > 0)
                {
                    dailyReportStrList.Add("You have new trade offers (check 'Member Offers' under 'Trade')");
                }

                TradeViewController.Instance.LoadSentTrades((isSuccess, errMsg, tradeList) =>
                {
                    if (tradeList != null && tradeList.Count > 0)
                    {
                        foreach (FTrade trade in tradeList)
                        {
                            int result = trade.created_at.CompareTo(Convert.DateTimeToFDate(DateTime.Now.AddDays(-1)));
                            if ((result == 0) /*|| (result < 0 && trade.update_at.Equals(string.Empty))*/)
                            {
                                dailyReportStrList.Add("Your trade offer has been accepted.  See ‘members offer’under 'Trade'");
                                break;
                            }
                        }
                    }
                    ArtworkSystem.Instance.LoadSendArtTrades((isSuccess, errMsg, tradeList) =>
                    {
                        if (tradeList != null && tradeList.Count > 0)
                        {
                            foreach (FArtTrade trade in tradeList)
                            {
                                if (trade.state == EState.Created.ToString())
                                {
                                    dailyReportStrList.Add("Your art exchange has been accepted. See ‘notifications’under 'Connections'");
                                    break;
                                }
                            }
                        }
                        callback(dailyReportStrList);
                    });
                });
                
            });
        });
    }
#endregion
}
