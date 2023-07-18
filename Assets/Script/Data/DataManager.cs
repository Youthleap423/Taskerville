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
    [SerializeField] public Sprite backMode_Sprite;
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
    
    private BuildingsCategoryData _buildingsCategoryData;
    private BuildingsCategoryData _roadCategoryData;
    private ResourceData _resourceCategoryData;
    private VillageData _villagerCategoryData;
    private ScheduleData _scheduleData;
    private ArtifactData _artifactData;
    
    public List<EVillagerType> laborTypeGroup = new List<EVillagerType>() { EVillagerType.Farmer, EVillagerType.Woodcutter, EVillagerType.Baker,
                                                                    EVillagerType.Builder, EVillagerType.Miller, EVillagerType.Laborer, EVillagerType.Sawyer, EVillagerType.Quarryman, EVillagerType.Rancher, EVillagerType.Miner};
    public List<EVillagerType> guardTypeGroup = new List<EVillagerType>() { EVillagerType.Commander, EVillagerType.Kinght, EVillagerType.Archer, EVillagerType.Crossbowman };
    public List<EVillagerType> staffTypeGroup = new List<EVillagerType>() { EVillagerType.Mayor, EVillagerType.Administrator, EVillagerType.Currator, EVillagerType.Cabinet };
    public List<DayOfWeek> availableDaysOfShip = new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Saturday };
    public List<EResources> exportResources = new List<EResources>() { EResources.Bread, EResources.Ale, EResources.Lumber, EResources.Stone, EResources.Iron };



    [HideInInspector]public List<int> rareBuildings = new List<int>() { 3, 5, 11, 12, 14, 61, 62, 63, 65};

    [HideInInspector] public Dictionary<DayOfWeek, List<int>> bonusBuildingsForVegetarin = new Dictionary<DayOfWeek, List<int>>() {
        {DayOfWeek.Sunday, new List<int>{3, 5, 12, 61} },
        {DayOfWeek.Monday, new List<int>{12, 11, 3, 61} },
        {DayOfWeek.Tuesday, new List<int>{5, 14, 11, 63} },
        {DayOfWeek.Wednesday, new List<int>{12, 11, 14, 63} },
        {DayOfWeek.Thursday, new List<int>{65, 5} },
        {DayOfWeek.Friday, new List<int>{62, 12} },
        {DayOfWeek.Saturday, new List<int>{14, 5, 63, 3} },
    };

    [HideInInspector]
    public Dictionary<DayOfWeek, List<int>> bonusBuildingsForNotVegetarin = new Dictionary<DayOfWeek, List<int>>() {
        {DayOfWeek.Sunday, new List<int>{3, 5, 61} },
        {DayOfWeek.Monday, new List<int>{12, 11, 61} },
        {DayOfWeek.Tuesday, new List<int>{5, 14, 63} },
        {DayOfWeek.Wednesday, new List<int>{12, 11, 63} },
        {DayOfWeek.Thursday, new List<int>{65} },
        {DayOfWeek.Friday, new List<int>{62} },
        {DayOfWeek.Saturday, new List<int>{14, 63, 3} },
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

    private List<string> tempReport = new List<string>();
    public List<LVillager> CurrentVillagers
    {
        get
        {
            return currentVillagers;
        }

        set
        {
            if (value.Count != 0)
            {
                currentVillagers = value;
            }
            
            DBHandler.SaveToJSON(currentVillagers, villagerFN);
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
            if (value.Count != 0)
            {
                currentBuildings = value;
            }
            DBHandler.SaveToJSON(currentBuildings, buildingFN);
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
            if (value.Count != 0)
            {
                currentResources = value;
            }
            DBHandler.SaveToJSON(currentResources, resourceFN);
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
            if (value.Count != 0)
            {
                currentAutoBuildings = value;
            }
            DBHandler.SaveToJSON(currentAutoBuildings, autoFN);
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
            if (value.Count != 0)
            {
                currentHabits = value;
            }
            DBHandler.SaveToJSON(currentHabits, habitFN);
        }
    }

    public BuildingsCategoryData BuildingsCategoryData
    {
        get
        {
            if (_buildingsCategoryData == null)
            {
                _buildingsCategoryData = Resources.Load<BuildingsCategoryData>(Constants.PATH_FOR_BUILDINGS_CATEGORY_ASSET_LOAD);
            }
            return _buildingsCategoryData;
        }
    }

    public BuildingsCategoryData RoadsCategoryData
    {
        get
        {
            if (_roadCategoryData == null)
            {
                _roadCategoryData = Resources.Load<BuildingsCategoryData>(Constants.PATH_FOR_ROAD_CATEGORY_ASSET_LOAD);
            }
            return _roadCategoryData;
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

    public VillageData VillagersCategoryData
    {
        get
        {
            if (_villagerCategoryData == null)
            {
                _villagerCategoryData = Resources.Load<VillageData>(Constants.PATH_FOR_VILLAGER_CATEGORY_ASSET_LOAD);
            }
            return _villagerCategoryData;
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
            if (value.Count != 0)
            {
                currentDailyTasks = value;
            }
            DBHandler.SaveToJSON(currentDailyTasks, dailyTaskFN);
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
            if (value.Count != 0)
            {
                currentSubTasks = value;
            }
            DBHandler.SaveToJSON(currentSubTasks, subTaskFN);
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
            if (value.Count != 0)
            {
                currentToDos = value;
            }
            DBHandler.SaveToJSON(currentToDos, toDOFN);
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
            if (value.Count != 0)
            {
                currentAutoToDos = value;
            }
            DBHandler.SaveToJSON(currentAutoToDos, autoTodoFN);
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
            if (value.Count != 0)
            {
                currentAutoGoals = value;
            }
            DBHandler.SaveToJSON(currentAutoGoals, autoGoalFN);
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
            if (value.Count != 0)
            {
                currentProjects = value;
            }
            DBHandler.SaveToJSON(currentProjects, projectGoalFN);
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
            if (value.Count != 0)
            {
                currentArtifacts = value;
            }
            DBHandler.SaveToJSON(currentArtifacts, artifactFN);
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
            if (value.Count != 0)
            {
                currentArtworks = value;
            }
            DBHandler.SaveToJSON(currentArtworks, artworkFN);
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


    public void SignOut()
    {
        currentUser = new LUser();
        currentSetting = new LSetting();
        currentUserList.Clear();
        currentVillagers.Clear();
        currentBuildings.Clear();
        currentAutoBuildings.Clear();
        currentResources.Clear();
        currentDailyTasks.Clear();
        currentSubTasks.Clear();
        currentToDos.Clear();
        currentProjects.Clear();
        currentCoalitions.Clear();
        currentHabits.Clear();
        currentAutoToDos.Clear();
        currentAutoGoals.Clear();
        currentArtifacts.Clear();
        currentArtworks.Clear();
        currentFUser = new FUser();

        Clear_LocalData();
        SaveData();
    }

    public void SaveData()
    {
        DBHandler.SaveToJSON(currentVillagers, villagerFN);
        DBHandler.SaveToJSON(currentBuildings, buildingFN);
        DBHandler.SaveToJSON(currentResources, resourceFN);
        DBHandler.SaveToJSON(currentAutoBuildings, autoFN);
        DBHandler.SaveToJSON(currentDailyTasks, dailyTaskFN);
        DBHandler.SaveToJSON(currentSubTasks, subTaskFN);
        DBHandler.SaveToJSON(currentToDos, toDOFN);
        DBHandler.SaveToJSON(currentProjects, projectGoalFN);
        DBHandler.SaveToJSON(currentSetting, settingFN);
        DBHandler.SaveToJSON(currentHabits, habitFN);
        DBHandler.SaveToJSON(currentAutoToDos, autoTodoFN);
        DBHandler.SaveToJSON(currentAutoGoals, autoGoalFN);
        DBHandler.SaveToJSON(currentArtifacts, artifactFN);
        DBHandler.SaveToJSON(currentArtworks, artworkFN);
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

        FirestoreManager.Instance.OnCoalitionUpdated += OnCoalitionUpdated;
    }

    private void OnDestroy()
    {
        FirestoreManager.Instance.OnCoalitionUpdated -= OnCoalitionUpdated;
    }

    #endregion

    #region Public_Members

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

    public FUser GetCurrentFUser()
    {
        return currentFUser;
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
            return JsonHelper.FromJson<LVillager>(fUser.Villager).ToList();
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
            return JsonHelper.FromJson<LBuilding>(fUser.Building).ToList();
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
            return JsonHelper.FromJson<LResource>(fUser.Resource).ToList();
        }

        return new List<LResource>();
    }

    public List<LArtwork> GetOtherUserArtworks(LUser user)
    {
        var fUser = GetFUserFromUser(user);
        if (fUser != null)
        {
            return JsonHelper.FromJson<LArtwork>(fUser.Artwork).ToList();
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

    public BuildingsCategory GetBuilding(int buildingID)
    {
        return BuildingsCategoryData.category.Find(it => it.GetId() == buildingID);
    }

    public void CheckUsername(string username, System.Action<bool, string, FUser, LUser> callback)
    {
        FirestoreManager.Instance.ExistUser(username, callback);
    }

    public void GetDataList<T>(string collectionId, System.Action<bool, string, List<T>> callback) where T : FData
    {
        FirestoreManager.Instance.GetDataList<T>(collectionId, callback);
    }

    public void GetDataList<T>(string collectionId, string fieldName, string value, System.Action<bool, string, List<T>> callback) where T : FData
    {
        FirestoreManager.Instance.GetList<T>(collectionId, fieldName, value, callback);
    }

    public void GetTaskList<T>(EntryType type, System.Action<bool, string, List<T>> callback) where T: FTask
    {
        FirestoreManager.Instance.GetTaskList<T>(type.ToString(), "Pid", currentUser.id, callback);
    }

    public void GetTaskList<T>(EntryType type, string Pid, System.Action<bool, string, List<T>> callback) where T : FTask
    {
        FirestoreManager.Instance.GetTaskList<T>(type.ToString(), "Pid", Pid, callback);
    }

    public void GetSubTasks(ESubEntryType type, string parentTaskId, System.Action<bool, string, List<FTask>> callback)
    {
        FirestoreManager.Instance.GetSubTaskList<FTask>(type.ToString(), "Pid", parentTaskId, callback);
    }

    public void GetPublicMessages(string groupName, System.Action<bool, string, List<FMessage>> callback)
    {
        FirestoreManager.Instance.GetMessages("Messages", groupName, EMessageType.Public.ToString(), callback);
    }

    public void GetPrivateMessages(List<string> list, System.Action<bool, string, List<FMessage>> callback)
    {
        FirestoreManager.Instance.GetMessages("Messages", list, EMessageType.Private.ToString(), callback);
    }


    public void GetInvitations(string userId, System.Action<bool, string, List<FInvitation>> callback)
    {
        FirestoreManager.Instance.GetInvitations("Invitation", userId, callback);
    }

    public void GetInvitations(string userId, string type,  System.Action<bool, string, List<FInvitation>> callback)
    {
        FirestoreManager.Instance.GetInvitations("Invitation", userId, type, callback);
    }

    public void GetTradeInvitations(string userId, System.Action<bool, string, List<FTradeInvitation>> callback)
    {
        FirestoreManager.Instance.GetInvitations("Trade_Invitation", userId, callback);
    }

    public void GetSentTrades(string userId, System.Action<bool, string, List<FTrade>> callback)
    {
        FirestoreManager.Instance.GetTrades("Trade", userId, "Pid", callback);
    }

    public void GetReceiveTrades(string userId, System.Action<bool, string, List<FTrade>> callback)
    {
        FirestoreManager.Instance.GetTrades("Trade", userId, "receiver_Id", callback);
    }

    public void GetTrades(string created_at, System.Action<bool, string, List<FTrade>> callback)
    {
        FirestoreManager.Instance.GetTrades("Trade", created_at, "created_at", callback);
    }

    public void GetReceiveTradeItems(string userId, System.Action<bool, string, List<FTradeItem>> callback)
    {
        FirestoreManager.Instance.GetTradeItems("receiver_Id", userId, callback);
    }

    public void GetSentTradeItems(string userId, System.Action<bool, string, List<FTradeItem>> callback)
    {
        FirestoreManager.Instance.GetTradeItems("Pid", userId, callback);
    }

    public void UpdateTrades(List<FTrade> rTrades, List<FTrade> sTrades, List<FTradeItem> tradeItemList, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.UpdateTrades("Trade", rTrades, sTrades, tradeItemList, callback);
    }

    public void UpdateData<T>(T data, System.Action<bool, string> callback) where T : FData
    {
        FirestoreManager.Instance.UpdateData(data, callback);
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

    public CArtwork GetCArtwork(LArtwork lArtwork)
    {
        return Artifact_Data.artworks.Find(item => item.id == lArtwork.id);
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

    public void CreateVillager(LVillager villager)
    {
        currentVillagers.Add(villager);
        //DBHandler.SaveToJSON(currentVillagers, villagerFN);
    }

    public void LoadUserData(System.Action<bool, string, List<LUser>> callback)
    {
        FirestoreManager.Instance.GetInitData("User", (isSuccess, errMsg, snapshotList) =>
        {
            if (isSuccess)
            {
                this.currentFUserList.Clear();
                this.currentUserList.Clear();
                foreach (DocumentSnapshot snapshot in snapshotList)
                {
                    FUser user = snapshot.ConvertTo<FUser>();
                    this.currentFUserList.Add(user);
                    this.currentUserList.Add(JsonConvert.DeserializeObject<LUser>(user.User));
                }
            }
            callback(isSuccess, errMsg, this.currentUserList);
        });
    }

    public void LoadCoalitionData(System.Action<bool, string, List<FCoalition>> callback)
    {
        if (this.currentCoalitions.Count == 0)
        {
            FirestoreManager.Instance.GetCoalitions("Coalition", (isSuccess, errMsg, snapshotList) =>
            {
                if (isSuccess)
                {
                    this.currentCoalitions.Clear();
                    foreach (DocumentSnapshot snapshot in snapshotList)
                    {
                        FCoalition coalition = snapshot.ConvertTo<FCoalition>();
                        this.currentCoalitions.Add(coalition);
                    }
                }
                callback(isSuccess, errMsg, this.currentCoalitions);
            });
        }
        else
        {
            callback(true, "", this.currentCoalitions);
        }

    }

    public void LoadInitialData()
    {
        InitData data = Resources.Load<InitData>(Constants.PATH_FOR_INIT_DATA_ASSET_LOAD);

        initVillagers.Clear();
        foreach(LVillager villager in data.villagers)
        {
            var newVillager = new LVillager();
            newVillager.id = villager.id;
            newVillager.live_at = villager.live_at;
            newVillager.work_at = villager.work_at;
            newVillager.UID = villager.UID;
            initVillagers.Add(newVillager);
        }

        initBuildings.Clear();
        foreach (LBuilding building in data.buildings)
        {
            var newBuilding = new LBuilding();
            newBuilding.id = building.id;
            newBuilding.progress = building.progress;
            newBuilding.bID = building.bID;
            newBuilding.bTime = building.bTime;
            initBuildings.Add(newBuilding);
        }

        initResources.Clear();
        foreach (LResource resource in data.resources)
        {
            var newResource = new LResource();
            newResource.id = resource.id;
            newResource.current_amount = resource.current_amount;
            initResources.Add(newResource);
        }

        if (CurrentResources.Count == 0)
        {
            CurrentResources = new List<LResource>(initResources);
        }
        else
        {
            foreach(LResource resource in CurrentResources)
            {
                var cResource = ResourcesCategoryData.resources.Find(res => res.id == resource.id);
                if (cResource != null && cResource.bCleanMode == true)
                {
                    initResources.Remove(initResources.Find(item => item.id == resource.id));
                }
            }
            UpdateResource(initResources);
        }

        //var galleryBuilding = CurrentBuildings.Find(item => item.bID == "")
        CurrentVillagers = new List<LVillager>(initVillagers);
        CurrentBuildings = new List<LBuilding>(initBuildings);

        UpdateUser();
    }

    public void LoadLocalData()
    {
        LoadInitialData();

        currentVillagers = DBHandler.ReadListFromJSON<LVillager>(villagerFN);
        currentBuildings = DBHandler.ReadListFromJSON<LBuilding>(buildingFN);
        currentAutoBuildings = DBHandler.ReadListFromJSON<LAutoBuilding>(autoFN);
        currentResources = DBHandler.ReadListFromJSON<LResource>(resourceFN);
        CurrentDailyTasks = DBHandler.ReadListFromJSON<LTaskEntry>(dailyTaskFN);
        CurrentSubTasks = DBHandler.ReadListFromJSON<LSubTask>(subTaskFN);
        CurrentToDos = DBHandler.ReadListFromJSON<LToDoEntry>(toDOFN);
        CurrentProjects = DBHandler.ReadListFromJSON<LProjectEntry>(projectGoalFN);
        currentUser = DBHandler.ReadFromJSON<LUser>(userFN);
        currentSetting = DBHandler.ReadFromJSON<LSetting>(settingFN);
        currentHabits = DBHandler.ReadListFromJSON<LHabitEntry>(habitFN);
        CurrentAutoToDos = DBHandler.ReadListFromJSON<LAutoToDo>(autoTodoFN);
        CurrentAutoGoals = DBHandler.ReadListFromJSON<LAutoGoal>(autoGoalFN);
        CurrentArtifacts = DBHandler.ReadListFromJSON<LArtifact>(artifactFN);
        CurrentArtworks = DBHandler.ReadListFromJSON<LArtwork>(artworkFN);

        UpdateUser();
    }

    public void LoadData(string userId, System.Action<bool, string> callback)
    {
        currentFUser.Id = userId;

        currentVillagers = DBHandler.ReadListFromJSON<LVillager>(villagerFN);
        currentBuildings = DBHandler.ReadListFromJSON<LBuilding>(buildingFN);
        currentAutoBuildings = DBHandler.ReadListFromJSON<LAutoBuilding>(autoFN);
        currentResources = DBHandler.ReadListFromJSON<LResource>(resourceFN);
        CurrentDailyTasks = DBHandler.ReadListFromJSON<LTaskEntry>(dailyTaskFN);
        CurrentSubTasks = DBHandler.ReadListFromJSON<LSubTask>(subTaskFN);
        CurrentToDos = DBHandler.ReadListFromJSON<LToDoEntry>(toDOFN);
        CurrentProjects = DBHandler.ReadListFromJSON<LProjectEntry>(projectGoalFN);
        currentUser = DBHandler.ReadFromJSON<LUser>(userFN);
        currentSetting = DBHandler.ReadFromJSON<LSetting>(settingFN);
        currentHabits = DBHandler.ReadListFromJSON<LHabitEntry>(habitFN);
        CurrentAutoToDos = DBHandler.ReadListFromJSON<LAutoToDo>(autoTodoFN);
        CurrentAutoGoals = DBHandler.ReadListFromJSON<LAutoGoal>(autoGoalFN);
        CurrentArtifacts = DBHandler.ReadListFromJSON<LArtifact>(artifactFN);
        CurrentArtworks = DBHandler.ReadListFromJSON<LArtwork>(artworkFN);

        SerializeUser(false, callback);
    }

    public void LoadData(System.Action<bool, string> callback)
    {
        CurrentVillagers = JsonHelper.FromJson<LVillager>(currentFUser.Villager).ToList();
        CurrentBuildings = JsonHelper.FromJson<LBuilding>(currentFUser.Building).ToList();
        CurrentAutoBuildings = JsonHelper.FromJson<LAutoBuilding>(currentFUser.AutoBuilding).ToList();
        CurrentResources = JsonHelper.FromJson<LResource>(currentFUser.Resource).ToList();
        CurrentDailyTasks = JsonHelper.FromJson<LTaskEntry>(currentFUser.DailyTask).ToList();
        CurrentSubTasks = JsonHelper.FromJson<LSubTask>(currentFUser.SubTask).ToList();
        CurrentToDos = JsonHelper.FromJson<LToDoEntry>(currentFUser.ToDo).ToList();
        CurrentProjects = JsonHelper.FromJson<LProjectEntry>(currentFUser.Project).ToList();
        CurrentHabits = JsonHelper.FromJson<LHabitEntry>(currentFUser.Habit).ToList();
        CurrentAutoToDos = JsonHelper.FromJson<LAutoToDo>(currentFUser.AutoToDo).ToList();
        CurrentAutoGoals = JsonHelper.FromJson<LAutoGoal>(currentFUser.AutoGoal).ToList();
        currentUser = JsonConvert.DeserializeObject<LUser>(currentFUser.User);
        currentSetting = JsonConvert.DeserializeObject<LSetting>(currentFUser.Setting);
        CurrentArtifacts = JsonHelper.FromJson<LArtifact>(currentFUser.Artifact).ToList();
        CurrentArtworks = JsonHelper.FromJson<LArtwork>(currentFUser.Artwork).ToList();

        currentUser.Save();
        currentSetting.Save();
        SerializeUser(false, callback);
    }

    public void LoadInitData<T>(string collectionId, System.Action<bool, string, List<T>> callback) where T: FData
    {
        FirestoreManager.Instance.GetList<T>(collectionId, "Pid", currentUser.id, callback);
    }

    public void LoadInitData<T>(string collectionId, string Pid, System.Action<bool, string, List<T>> callback) where T : FData
    {
        FirestoreManager.Instance.GetList<T>(collectionId, "Pid", Pid, callback);
    }

    public void SerializeUser(bool bForce = false, System.Action<bool, string> callback = null)
    {
        if (currentUser == null || currentUser.id.Equals(""))
        {
            callback(false, "Can't Login");
        }
        else
        {
            UpdateUser();
            currentFUser.Serialize(bForce, callback);
        }
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
        currentFUser = user;
        PlayerPrefs.SetString("UpdatedTime", user.updated_at);
        LoadData(callback);
    }

    public void UpdateNewUser(FUser user)
    {
        currentFUser = user;
        PlayerPrefs.SetString("UpdatedTime", user.updated_at);
        LoadInitialData();
    }

    public void UpdateUser(string fUserId, System.Action<bool, string> callback)
    {
        currentFUser = new FUser();
        currentFUser.Id = fUserId;
        LoadData(fUserId, callback);
    }

    public void UpdateUser()
    {
        if (currentUser == null)
        {
            return;
        }
        var strBuilding = JsonHelper.ToJson(CurrentBuildings.ToArray());
        var strVillager = JsonHelper.ToJson(CurrentVillagers.ToArray());
        var strResource = JsonHelper.ToJson(CurrentResources.ToArray());
        var strAutoBuilding = JsonHelper.ToJson(CurrentAutoBuildings.ToArray());
        var strDailyTasks = JsonHelper.ToJson(CurrentDailyTasks.ToArray());
        var strToDo = JsonHelper.ToJson(CurrentToDos.ToArray());
        var strProject = JsonHelper.ToJson(CurrentProjects.ToArray());
        var strSubTasks = JsonHelper.ToJson(CurrentSubTasks.ToArray());
        var strUser = JsonConvert.SerializeObject(currentUser);
        var strSetting = JsonConvert.SerializeObject(currentSetting);
        var strHabit = JsonHelper.ToJson(CurrentHabits.ToArray());
        var strAutoTodo = JsonHelper.ToJson(CurrentAutoToDos.ToArray());
        var strAutoGoal = JsonHelper.ToJson(CurrentAutoGoals.ToArray());
        var strArtifact = JsonHelper.ToJson(CurrentArtifacts.ToArray());
        var strArtwork = JsonHelper.ToJson(CurrentArtworks.ToArray());

        currentFUser.Update(currentUser.id, strUser, strSetting, strBuilding, strVillager, strResource, strAutoBuilding,strDailyTasks, strToDo, strProject, strSubTasks, strHabit, strAutoTodo, strAutoGoal, strArtifact, strArtwork);
    }

    public void UpdateUser(string firstName, string secondName, string villageName, bool isVegetarian, bool hasReligion, System.Action<bool, string> callback)
    {
        LoadUserData((isSuccess, errMsg, userlist) =>
        {
            var sameVillage = userlist.Find(item => item.Village_Name.ToLower() == villageName.ToLower());
            if (sameVillage != null)
            {
                callback(false, "Village name already exists. Please choose another name.");
                return;
            }

            var sameUserName = userlist.Find(item => (item.First_Name.ToLower() == firstName.ToLower() && item.Last_Name.ToLower() == secondName.ToLower()));
            if (sameUserName != null)
            {
                callback(false, "Mayor name already exists. Please choose another name.");
                return;
            }

            currentUser.Update(firstName, secondName, villageName, isVegetarian, hasReligion);
            callback(true, "");
        });
        
    }

    public void UpdateUser(string created_coalition, string joined_coalition, System.Action<bool, string> callback)
    {
        currentUser.Update(created_coalition, joined_coalition, callback);
    }

    public void UpdateReligion(string religion)
    {
        currentUser.UpdateReligion(religion);
    }

    public void UpdateUserAvatarId(int nId)
    {
        currentUser.UpdateAvatarId(nId);
    }

    public void UpdateSalaryDate(System.DateTime dateTime)
    {
        currentUser.updateSalaryDate(dateTime);
    }

    public void UpdateMaintenanceDate(System.DateTime dateTime)
    {
        currentUser.updateMaintenanceDate(dateTime);
    }

    public void UpdateMealDate(System.DateTime dateTime)
    {
        currentUser.updateMealDate(dateTime);
    }

    public void UpdateCoalition(FCoalition coalition, System.Action<bool, string> callback)
    {
        foreach(FCoalition fCoalition in currentCoalitions)
        {
            if (fCoalition.Id == coalition.Id)
            {
                currentCoalitions.Remove(fCoalition);
                break;
            }
        }

        currentCoalitions.Add(coalition);

        FirestoreManager.Instance.UpdateData(coalition.collectionId, coalition, callback);
    }

    public void UpdateInvitation(FInvitation invitation, FUser user, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.UpdateInvitation(invitation, user, callback);
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

    public void UpdateSetting(LSetting setting)
    {
        currentSetting = setting;
        currentSetting.Save();
    }

    public void UpdateSetting(Game_Mode mode)
    {
        currentSetting.Update(mode);
        currentUser.UpdateModeDate(System.DateTime.Now);
        ChangeMode(mode);
    }

    public void UpdateSetting(Interaction_Mode mode)
    {
        currentSetting.Update(mode);
    }

    public void UpdateSetting(Game_Mode game_mode, Interaction_Mode interaction_mode)
    {
        currentSetting.Update(game_mode, interaction_mode);
        currentUser.UpdateModeDate(System.DateTime.Now);
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
            CResource cResource = ResourcesCategoryData.resources.ToList().Find(cRes => cRes.type.ToString() == key);
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
                        if (GetCurrentSetting().current_mode == (int)Game_Mode.Game_Only)
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
                CResource cResource = ResourcesCategoryData.resources.ToList().Find(cRes => cRes.id == res.id);
                if (cResource.type == key)
                {
                    res.current_amount += resourceDic[key];
                    res.current_amount = Mathf.Max(0f, res.current_amount);
                    res.created_at = Convert.DateTimeToFDate(System.DateTime.Now);
                    if (key == EResources.Happiness)
                    {
                        res.current_amount = Mathf.Min(100f, res.current_amount);
                        if (GetCurrentSetting().current_mode == (int)Game_Mode.Game_Only)
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
            list.AddRange(tempReport.ToList());
            callback(list);
            return;
        }

        GetDailyReportString(strList =>
        {
            PlayerPrefs.SetString("DailyReport", Convert.DateTimeToFDate(System.DateTime.Now));
            var list = strList.ToList();
            list.AddRange(tempReport.ToList());
            callback(list);
        });
    }

    public void AddDailyReport(string s)
    {
        if (PlayerPrefs.GetString("TempDailyReport") != Convert.DateTimeToFDate(System.DateTime.Now))
        {
            tempReport.Clear();
            PlayerPrefs.SetString("TempDailyReport", Convert.DateTimeToFDate(System.DateTime.Now));
        }

        if (!tempReport.Contains(s))
        {
            tempReport.Add(s);
        }
    }

    private List<LBuilding> GetBuiltLBuildings(System.DateTime dateTime)
    {
        return currentBuildings.FindAll(item => item.created_at == Convert.DateTimeToFDate(dateTime) && item.progress == 1.0f);
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
            return buildingCategory.GetName();
        }
        return "Unknown Building";
    }

    public void CreateVillagers(List<FVillager> villagerList, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.createDataList<FVillager>(villagerList, callback);
    }

    public void CreateBuildings(List<LBuilding> buildingList, System.Action<bool, string> callback)
    {
        currentBuildings.AddRange(buildingList);
    }

    public void CreateResources(List<FResource> resourceList, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.createDataList<FResource>(resourceList, callback);
    }

    
    public void CreateCoalition(FCoalition coalition, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.createData(coalition.collectionId, coalition.Id, coalition, callback);
    }

    public void CreateInvitation(FInvitation invitation, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.createData(invitation.collectionId, invitation, callback);
    }

    public void CreateInvitation(FTradeInvitation invitation, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.createData(invitation.collectionId, invitation, callback);
    }

    public void CreateTrade(FTrade trade, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.createData(trade.collectionId, trade, callback);
    }

    public void CreateTrade(FArtTrade trade, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.createData(trade.collectionId, trade, callback);
    }

    public void CreateTradeItem(List<FTradeItem> tradelist, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.createDataList(tradelist, callback);
    }

    public void CreateMessages(FMessage fMessage, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.createData(fMessage.collectionId, fMessage, callback);
    }

    public void CreateBuilding(FBuilding building, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.createData(building.collectionId, building, callback);
    }

    public void RemoveData(FData data, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.RemoveData(data, callback);
    }

    public void RemoveData(List<FData> dataList, System.Action<bool, string> callback)
    {
        FirestoreManager.Instance.RemoveData(dataList, callback);
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

    public void ChangeMode(Game_Mode mode)
    {
        LoadInitialData();
        CurrentAutoBuildings.Clear();
        CurrentDailyTasks.Clear();
        CurrentHabits.Clear();
        CurrentSubTasks.Clear();
        CurrentProjects.Clear();
        currentUser.ClearAllExports();
        if (mode == Game_Mode.Game_Only)
        {
            AITaskManager.Instance.CreateEntries();
        }
        dailyReportStrList.Clear();
        PlayerPrefs.SetString("DRemind", "");
        SaveData();
    }

    #endregion

    #region Private Members

    private List<CResource> GetBuyResourceFromRepublic()
    {
        List<CResource> resultList = new List<CResource>();
        foreach(CResource resource in ResourcesCategoryData.resources)
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
        foreach (CResource resource in ResourcesCategoryData.resources)
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
        foreach (CResource resource in ResourcesCategoryData.resources)
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
        foreach (CResource resource in ResourcesCategoryData.resources)
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
        foreach (CResource resource in ResourcesCategoryData.resources)
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
        foreach (CResource resource in ResourcesCategoryData.resources)
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
        foreach (CResource resource in ResourcesCategoryData.resources)
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

        var mode = (Game_Mode)currentSetting.current_mode;
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

                ArtworkSystem.Instance.LoadArtTrade((isSuccess, errMsg, tradeList) =>
                {
                    if (tradeList != null && tradeList.Count > 0)
                    {
                        foreach(FArtTrade trade in tradeList)
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
    }
    #endregion
}
