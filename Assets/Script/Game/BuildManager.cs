using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIControllersAndData.Store.Categories.Buildings;
using System;
using System.Linq;

public class BuildManager : SingletonComponent<BuildManager>
{
    [SerializeField] private int builder_Count;
    [SerializeField] private List<GBuilding> allBuildings;
    [SerializeField] private Transform buildingGroup;

    [Space]
    [Header("Builders")]
    [SerializeField] public bool needRender = false;
    [SerializeField] private GameObject contructionBuildingObj;
    [SerializeField] private GameObject contructionIronMineObj;
    [SerializeField] private GameObject contructionSilverMineObj;
    [SerializeField] private GameObject contructionQuarryMineObj;
    [SerializeField] private GameObject nonBuilderPrefab;

    [Space]
    [SerializeField] private GameObject merchantShip;

    [Space]
    [SerializeField] private GBuilding villagerInn;
    [SerializeField] private GBuilding BeachInn;

    [Space]
    [Header("Mine Deposit")]
    [SerializeField] private GameObject ironObj;
    [SerializeField] private GameObject siliverObj;
    [SerializeField] private GameObject goldObj;
    [SerializeField] private GameObject quarry;

    [Space]
    [Header("Digs")]
    [SerializeField] private List<GameObject> archealogicalDigs;

    private int current_builder_count = 0;
    private List<GBuilding> flaggedGBuldings = new List<GBuilding>();
    private bool bQuickBuild = false;
    private List<string> celeBuildingIds = new List<string>() { "15", "27", "28", "55", "61", "62", "63", "65", "43", "44", "24", "58"};
    private bool bIsLoaded = false;

    public bool QuickBuild
    {
        get
        {
            return bQuickBuild;
        }

        set
        {
            bQuickBuild = value;
        }
    }
    //[MenuItem("Tools/Action**")]
    //static void CreateImagesTable()
    //{
    //    foreach (Transform child in GameObject.Find("GroupBuildings(Init)").transform)
    //    {
    //        GBuilding gBuilding = child.GetComponent<GBuilding>();
    //        Debug.LogError(gBuilding.GetBuildingObjectID + "-------" + gBuilding.GetBuildingID());
    //    }
    //}

    private void Start()
    {
        bIsLoaded = false;
        allBuildings = buildingGroup.GetComponentsInChildren<GBuilding>().ToList();
        //Debug.LogError(allBuildings.Count);
        if (!needRender)
        {
            StartCoroutine(LoadBuilding(allBuildings));
        }
    }

    private List<GameObject> GetBuildingObjects(int buildingID)
    {
        List<GameObject> result = new List<GameObject>();
        foreach (GBuilding gBuilding in allBuildings)
        {
            if (gBuilding.GetBuildingID() == buildingID)
            {
                result.Add(gBuilding.gameObject);
            }
        }
        return result;
    }

    private List<GameObject> GetUnBuiltObjects(List<GameObject> objList)
    {
        List<GameObject> result = new List<GameObject>();
        foreach (GameObject obj in objList)
        {
            GBuilding gBuilding = obj.GetComponent<GBuilding>();
            if (gBuilding == null)
            {
                continue;
            }
            if (gBuilding.GetProgress() == 0.0f)
            {
                result.Add(obj);
            }
        }
        return result;
    }

    private IEnumerator LoadBuilding(List<GBuilding> gBuildings)
    {
        if (AppManager.Instance.excavationDigIndex != -1)
        {
            ShowDigOnMap(AppManager.Instance.excavationDigIndex);
            AppManager.Instance.excavationDigIndex = -1;
        }

        foreach (GBuilding gBuilding in gBuildings)
        {
            yield return new WaitForEndOfFrame();

            if (gBuilding.GetBuildingObjectID != "0")
            {
                if (gBuilding.Lbuilding.progress < 0.99f)
                {
                    ContinueToBuild(gBuilding);
                }
                else
                {
                    gBuilding.LoadBuilding(gBuilding.Lbuilding);
                }
            }
        }

        yield return new WaitForFixedUpdate();

        bIsLoaded = true;
        CheckVisibility();
        CharacterManager.Instance.CheckVillagers();
        GameManager.Instance.BuildingsLoaded();
        
    }

    private GameObject GetConstructionObj(LBuilding building)
    {
        var result = contructionBuildingObj;
        switch (building.id)
        {
            case "61":
                result = contructionIronMineObj;
                break;
            case "62":
            case "65":
                result = contructionSilverMineObj;
                break;
            case "63":
                result = contructionQuarryMineObj;
                break;
            default:
                break;
        }
        
        return result;
    }

    private void ContinueToBuild(GBuilding building)
    {
        LBuilding lbuidling = building.Lbuilding;
        var Construction = Instantiate(GetConstructionObj(lbuidling), building.transform);
        Construction.transform.SetSiblingIndex(2);
        Construction.transform.localPosition = Vector3.zero;
        if (lbuidling.GetBuildingType() == EBuildingType.Building)
        {
            BuildingCreator bCreator = Construction.GetComponent<BuildingCreator>();
            bCreator.SetGBuilding(building);
            bCreator.buildingTime = lbuidling.bTime;

            if (lbuidling.created_at != "")
            {
                var passedSeconds = (float)((double.Parse(Utilities.SystemTimeInMillisecondsString) - double.Parse(lbuidling.created_at)) / 1000);
                bCreator.progress = Mathf.Clamp01((float)(passedSeconds) / 60.0f / (float)(bCreator.buildingTime));
            }

            building.SetBuildingCreator(bCreator);
            Construction.GetComponent<ConstructionPath>().ResetPath();
        }
        else
        {
            MineCreator bCreator = Construction.GetComponent<MineCreator>();
            bCreator.SetGBuilding(building);
            bCreator.buildingTime = lbuidling.bTime;

            if (lbuidling.created_at != "")
            {
                var passedSeconds = (float)((double.Parse(Utilities.SystemTimeInMillisecondsString) - double.Parse(lbuidling.created_at)) / 1000);
                bCreator.progress = Mathf.Clamp01((float)(passedSeconds) / 60.0f / (float)(bCreator.buildingTime));
            }

            building.SetBuildingCreator(bCreator);
        }
        
        current_builder_count++;
    }

    private int HouseVillager(LVillager villager, GBuilding building)
    {
        villager.live_at = int.Parse(building.GetBuildingObjectID);
        building.AddVillager(villager, false);
        villagerInn.RemoveGuest(villager.UID);
        ResourceViewController.Instance.UpdateVillager(villager);
        var cVillager = ResourceViewController.Instance.GetCVillager(villager.id);
        return cVillager.size_of_house;
    }

    public GameObject GetNonBuilderPrefab()
    {
        return nonBuilderPrefab;
    }

    public List<GBuilding> GetBuiltGBuildings(System.DateTime dateTime)
    {
        return allBuildings.FindAll(item => item.Lbuilding.created_at == Convert.DateTimeToFDate(dateTime) && item.Lbuilding.progress == 1.0f);
    }

    public bool isBuiingBuilt(string buildingId)
    {
        var gBuilding = allBuildings.Find(item => item.GetBuildingObjectID == buildingId);
        if (gBuilding == null || gBuilding.Lbuilding == null)
        {
            return false;
        }

        return gBuilding.Lbuilding.progress > 0.99f;
    }

    public void CheckHouseForVillagers(GBuilding gBuilding)
    {
        var id = gBuilding.Lbuilding.id;

        if (id == "22" || id == "23" || id == "25")
        {
            var guests = ResourceViewController.Instance.GetGuestsFromInn();
            var spouses = guests.FindAll(it => it.id == "33");
            var childs = guests.FindAll(it => it.id == "36");
            var laborers = guests.FindAll(it => it.id == "32");
            var curators = guests.FindAll(it => it.id == "23");
            var specials = guests.FindAll(it => it.id != "23" && it.id != "36" && it.id != "32" && it.id != "33");

            if (id == "25")
            {
                if (curators.Count > 0)
                {
                    HouseVillager(curators[0], gBuilding);
                }
                if (spouses.Count > 0)
                {
                    HouseVillager(spouses[0], gBuilding);
                    HouseVillager(childs[0], gBuilding);
                }
            }
            else if (id == "23")
            {
                var house_size = 4;
                if (specials.Count > 0)
                {
                    house_size -= HouseVillager(specials[0], gBuilding);
                }

                if (spouses.Count > 0)
                {
                    house_size -= HouseVillager(spouses[0], gBuilding);
                    house_size -= HouseVillager(childs[0], gBuilding);
                }

                if (house_size > 0)
                {
                    do
                    {
                        if (laborers.Count == 0)
                        {
                            break;
                        }
                        var laborer = laborers[0];
                        house_size -= HouseVillager(laborer, gBuilding);
                        laborers.Remove(laborer);
                    } while (house_size > 0);
                }
            }
            else
            {
                var house_size = 8;
                for(int i = 0; i < 2; i++)
                {
                    if (specials.Count > 0)
                    {
                        house_size -= HouseVillager(specials[0], gBuilding);
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    if (spouses.Count > 0)
                    {
                        house_size -= HouseVillager(spouses[0], gBuilding);
                        house_size -= HouseVillager(childs[0], gBuilding);
                    }
                }

                if (house_size > 0)
                {
                    do
                    {
                        if (laborers.Count == 0)
                        {
                            break;
                        }
                        var laborer = laborers[0];
                        house_size -= HouseVillager(laborer, gBuilding);
                        laborers.Remove(laborer);
                    } while (house_size > 0);
                }
            }
            
        }
    }

    public void AddVillager(LVillager villager)
    {
        var work_building = allBuildings.Find(b => b.GetBuildingObjectID == villager.work_at.ToString());
        if (work_building != null)
        {
            work_building.AddVillager(villager, true);
        }

        var live_building = allBuildings.Find(b => b.GetBuildingObjectID == villager.live_at.ToString());
        if (live_building != null)
        {
            live_building.AddVillager(villager, false);
        }
    }

    public void RemoveVillager(LVillager villager)
    {
        var work_building = allBuildings.Find(b => b.GetBuildingObjectID == villager.work_at.ToString());
        if (work_building != null)
        {
            work_building.RemoveVillager(villager, true);
        }

        var live_building = allBuildings.Find(b => b.GetBuildingObjectID == villager.live_at.ToString());
        if (live_building != null)
        {
            live_building.RemoveVillager(villager, false);
        }
        ResourceViewController.Instance.RemoveVillager(villager);
    }

    public void ExcavateArchealogicalDig(int index)
    {
        foreach(GameObject obj in archealogicalDigs)
        {
            obj.SetActive(false);
        }
        archealogicalDigs[index].SetActive(true);
    }

    public void ShowDigOnMap(int index)
    {
        CameraController.Instance.MoveCamera(archealogicalDigs[index].transform.position);
    }

    public int GetLiveBuildingId(string vID)
    {
        int result = 80;

        var currentBuildings = allBuildings.Find(it => it.CanHouse(vID));
        if (currentBuildings != null)
        {
            result = int.Parse(currentBuildings.GetBuildingObjectID);
        }

        return result;
    }

    public GBuilding GetGBuilding(string buildingId)
    {
        return allBuildings.Find((item) => item.GetBuildingObjectID == buildingId);
    }

    public string GetTemplePrefabName()
    {
        return DataManager.Instance.GetTemplePrefabName();
    }

    public bool AnyFreeBuilder()
    {
        return current_builder_count < builder_Count;
    }

    public void CancelBuiling(GameObject buildingObj)
    {
        GBuilding gBuilding = buildingObj.GetComponent<GBuilding>();
        if (gBuilding != null)
        {
            if (gBuilding.GetProgress() < 1.0f)
            {
                gBuilding.CancelBuild();
                current_builder_count--;
            }
            else
            {
                gBuilding.Demolish();
            }
            
        }
    }

    public void HireSpecialist(GameObject buildingObj, List<EVillagerType> eVillagers)
    {
        GBuilding gBuilding = buildingObj.GetComponent<GBuilding>();
        if (gBuilding != null)
        {
            ResourceViewController.Instance.HireVillager(eVillagers, int.Parse(gBuilding.GetBuildingObjectID), int.Parse(gBuilding.GetBuildingObjectID), (isSuccess, errMsg, newVillagerList) =>
            {
                if (isSuccess)
                {
                    ArtworkSystem.Instance.CheckBuildingMilestone(gBuilding.GetBuildingObjectID, newVillagerList);
                    var logMsg = "";
                    foreach(LVillager villager in newVillagerList)
                    {
                        if (villager.live_at == 0)
                        {
                            var live_at = GetLiveBuildingId(villager.id);
                            if (live_at == 80)
                            {
                                if (villager.id == "23")
                                {
                                    logMsg = "Your curator needs a manor built. They will stay at the Inn for another day before leaving your village.";
                                }
                                else
                                {
                                    if (logMsg == "")
                                    {
                                        logMsg = "Housing is needed. Specialists and laborers can only afford the Inn for another two days before leaving your village.";
                                    }
                                }
                                
                            }
                            villager.live_at = live_at;
                        }
                        AddVillager(villager);
                        ResourceViewController.Instance.UpdateVillager(villager);
                    }
                    if (logMsg != "")
                    {
                        UIManager.Instance.ShowErrorDlg(logMsg);
                    }
                    gBuilding.CheckBuilders();
                }
                else
                {
                    UIManager.Instance.ShowErrorDlg(errMsg);
                }
            });

        }
    }

    public void CheckVisibility()
    {
        if (!bIsLoaded)
        {
            return;
        }

        var dayOfWeek = System.DateTime.Now.DayOfWeek;
        //enable/disable Merchant Ship
        merchantShip.SetActive(DataManager.Instance.availableDaysOfShip.Contains(dayOfWeek));
    }



    public void CompleteBuild(GameObject buildingObj)
    {
        GBuilding gBuilding = buildingObj.GetComponent<GBuilding>();
        if (gBuilding != null)
        {
            gBuilding.Complete();
            CheckHouseForVillagers(gBuilding);
            ArtworkSystem.Instance.CheckBuildingMilestone(gBuilding.GetBuildingObjectID);
            if (AppManager.Instance.GetCurrentMode() == Game_Mode.Task_Only)
            {
                var tHireVillager = gBuilding.NeedToHireSpecialist();

                if (tHireVillager.Item1 != "")
                {
                    HireSpecialist(buildingObj, tHireVillager.Item2);
                }
                AIGamePlay.Instance.CompleteBuild(gBuilding.GetBuildingObjectID);
            }

            if (AppManager.Instance.GetCurrentMode() == Game_Mode.Game_Only)
            {
                AITaskManager.Instance.CheckOnStartWithBuilding(gBuilding.Lbuilding);
            }
            
        }

        
        current_builder_count--;
    }

    

    public void LoadBuildings()
    {
        var buildings = DataManager.Instance.GetCurrentBuildings();
        
        ironObj.SetActive(false);
        siliverObj.SetActive(false);
        goldObj.SetActive(false);
        quarry.SetActive(false);

        var user = UserViewController.Instance.GetCurrentUser();
        var day = Convert.FDateToDateTime(user.mode_at).DayOfWeek;
        var bonusItems = DataManager.Instance.bonusBuildingsForVegetarin[day];


        if (bonusItems.Contains(61))
        {
            ironObj.SetActive(true);
        }

        if (bonusItems.Contains(62))
        {
            siliverObj.SetActive(true);
        }

        if (bonusItems.Contains(63))
        {
            quarry.SetActive(true);
        }

        if (bonusItems.Contains(65))
        {
            goldObj.SetActive(true);
        }

        var gBuildings = new List<GBuilding>();
        foreach (LBuilding building in buildings)
        {
            var gBuilding = allBuildings.Find(b => b.GetBuildingObjectID == building.bID);
            gBuilding.Lbuilding = building;
            gBuildings.Add(gBuilding);
        }

        //For Test

        StartCoroutine(LoadBuilding(gBuildings));



        var currentArtifact = ArtifactSystem.Instance.GetCurrentArtifact();
        if (currentArtifact != null)
        {
            ExcavateArchealogicalDig(currentArtifact.dig);
        }
    }

    public void RemoveBuildings()
    {
        foreach(GBuilding building in allBuildings)
        {
            building.RemoveBuilding();
        }
    }


    public void StartToBuild(GameObject buildingObj)
    {
        foreach(GBuilding building in flaggedGBuldings)
        {
            building.ShowLocationFlag(false);
        }

        GBuilding gBuilding = buildingObj.GetComponent<GBuilding>();
        LBuilding lbuidling = gBuilding.Lbuilding;

        var category = DataManager.Instance.BuildingsCategoryData.category.Find(item => item.GetId().ToString() == lbuidling.id);

        if (category == null)
        {
            UIManager.Instance.ShowErrorDlg("Can't find the data of this building");
            return;
        }

        var resourceDic = new Dictionary<EResources, float>();
        if (QuickBuild)
        {
            //12/14/2022 when a person uses a ruby to build a structure, the cost to build should be reduced to 1/2 of original => free 
            var resDic = new Dictionary<EResources, float>();
            //resDic.Add(EResources.Gold, Mathf.Floor(-category.GoldAmount / 2));
            //resDic.Add(EResources.Lumber, Mathf.Floor(-category.LumberAmount / 2));
            //resDic.Add(EResources.Stone, Mathf.Floor(-category.StoneAmount / 2));
            //resDic.Add(EResources.Iron, Mathf.Floor(-category.IronAmount / 2));
            resDic.Add(category.QResType, -category.QResAmount);

            if (ResourceViewController.Instance.CheckResource(resDic))
            {
                resourceDic = resDic;
            }
            else
            {
                UIManager.Instance.ShowErrorDlg("Not enough resources to construct this building.");
                return;
            }
        }
        else
        {
            var resDic = new Dictionary<EResources, float>();
            resDic.Add(EResources.Gold, -category.GoldAmount);
            resDic.Add(EResources.Lumber, -category.LumberAmount);
            resDic.Add(EResources.Stone, -category.StoneAmount);
            resDic.Add(EResources.Iron, -category.IronAmount);

            if (ResourceViewController.Instance.CheckResource(resDic))
            {
                resourceDic = resDic;
            }
            else
            {
                UIManager.Instance.ShowErrorDlg("Not enough resources to construct this building.");
                return;
            }
        }


        ResourceViewController.Instance.UpdateResource(resourceDic, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                var Construction = Instantiate(GetConstructionObj(gBuilding.Lbuilding), buildingObj.transform);
                Construction.transform.SetSiblingIndex(2);
                Construction.transform.localPosition = Vector3.zero;
                if (lbuidling.GetBuildingType() == EBuildingType.Building)
                {
                    BuildingCreator bCreator = Construction.GetComponent<BuildingCreator>();
                    bCreator.SetGBuilding(gBuilding);
                    bCreator.buildingTime = QuickBuild ? 1 : gBuilding.Category.TimeToBuild;

                    bCreator.progress = 0f;
                    gBuilding.SetBuildingCreator(bCreator);
                    Construction.GetComponent<ConstructionPath>().ResetPath();
                }
                else
                {
                    MineCreator bCreator = Construction.GetComponent<MineCreator>();
                    bCreator.SetGBuilding(gBuilding);
                    bCreator.buildingTime = QuickBuild ? 1 : gBuilding.Category.TimeToBuild;

                    bCreator.progress = 0f;
                    gBuilding.SetBuildingCreator(bCreator);
                }
                gBuilding.QuickBuild = QuickBuild;
                current_builder_count++;
                if (celeBuildingIds.Contains(lbuidling.id))
                {
                    AudioManager.Instance.PlayFXSound(AudioManager.Instance.specialConstructionClip);
                }
                else
                {
                    AudioManager.Instance.PlayFXSound(new List<AudioClip>() { AudioManager.Instance.constructionClip});
                }
                
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public void StartToBuild(BuildingsCategory category, int uniqueBuildingIndex, System.Action<bool, string> callback)
    {
        if (!AnyFreeBuilder())
        {
            callback(false, "There are no free builders.");
            return;
        }

        GBuilding gBuilding = allBuildings.Find(item => item.GetBuildingID() == category.GetId()); //GetUnBuiltObjects(GetBuildingObjects(category.GetId()));
        
        if (gBuilding == null)
        {
            callback(false, "Can't find the building");
            return;
        }

        LBuilding lbuidling = gBuilding.Lbuilding;

        if (lbuidling.progress == 1.0f)
        {
            callback(false, "You have already constructed this building.");
            return;
        }

        if (lbuidling.progress != 0.0f)
        {
            callback(false, "You've already started to construct this building.");
            return;
        }

        var requireItems = ResourceViewController.Instance.GetResourceValue(category.QResType);
        if (requireItems < category.QResAmount)
        {
            callback(false, "Not enough Rubys to construct this building.");
            return;
        }

        ResourceViewController.Instance.UpdateResource(category.QResType.ToString(), category.QResAmount, (isSuccess, errMsg) =>
        {
            var Construction = Instantiate(GetConstructionObj(gBuilding.Lbuilding), gBuilding.gameObject.transform);
            Construction.transform.SetSiblingIndex(2);
            Construction.transform.localPosition = Vector3.zero;
            BuildingCreator bCreator = Construction.GetComponent<BuildingCreator>();
            bCreator.SetGBuilding(gBuilding);
            bCreator.buildingTime = 1;
            gBuilding.QuickBuild = QuickBuild;

            bCreator.progress = 0f;
            gBuilding.SetBuildingCreator(bCreator);
            Construction.GetComponent<ConstructionPath>().ResetPath();
            gBuilding.Category = category;
            CameraController.Instance.MoveCamera(gBuilding.transform.position);

            current_builder_count++;
            callback(isSuccess, errMsg);
            AudioManager.Instance.PlayFXSound(AudioManager.Instance.specialConstructionClip);
        });
    }

    public void StartToBuild(CSchedule schedule)
    {
        if (!AnyFreeBuilder())
        {
            return;
        }

        GBuilding gBuilding = allBuildings.Find(item => item.GetBuildingObjectID == schedule.bID.ToString());
        if (gBuilding == null && gBuilding.Lbuilding == null)
        {
            return;
        }

        LBuilding lbuidling = gBuilding.Lbuilding;

        var category = DataManager.Instance.BuildingsCategoryData.category.Find(item => item.GetId().ToString() == lbuidling.id);

        if (category == null)
        {
            UIManager.Instance.ShowErrorDlg("Can't find the data of this building");
            return;
        }


        var resDic = new Dictionary<EResources, float>();
        resDic.Add(EResources.Gold, -category.GoldAmount);
        resDic.Add(EResources.Lumber, -category.LumberAmount);
        resDic.Add(EResources.Stone, -category.StoneAmount);
        resDic.Add(EResources.Iron, -category.IronAmount);

        ResourceViewController.Instance.UpdateResource(resDic, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                var Construction = Instantiate(GetConstructionObj(gBuilding.Lbuilding), gBuilding.transform);
                Construction.transform.SetSiblingIndex(2);
                Construction.transform.localPosition = Vector3.zero;
                gBuilding.QuickBuild = QuickBuild;
                if (lbuidling.GetBuildingType() == EBuildingType.Building)
                {
                    BuildingCreator bCreator = Construction.GetComponent<BuildingCreator>();
                    bCreator.SetGBuilding(gBuilding);
                    bCreator.buildingTime = QuickBuild ? 1 : gBuilding.Category.TimeToBuild;

                    bCreator.progress = 0f;
                    gBuilding.SetBuildingCreator(bCreator);
                }
                else
                {
                    MineCreator bCreator = Construction.GetComponent<MineCreator>();
                    bCreator.SetGBuilding(gBuilding);
                    bCreator.buildingTime = QuickBuild ? 1 : gBuilding.Category.TimeToBuild;

                    bCreator.progress = 0f;
                    gBuilding.SetBuildingCreator(bCreator);
                }
                Construction.GetComponent<ConstructionPath>().ResetPath();
                current_builder_count++;
                DataManager.Instance.UpdateAutoBuilding(new LAutoBuilding(schedule.id, false));
            }
        });
    }

    public void ConvertToQuickBuild(GameObject buildingObj)
    {
        var gBuilding = buildingObj.GetComponent<GBuilding>();

        if (gBuilding == null || gBuilding.Lbuilding == null)
        {
            return;
        }

        var category = DataManager.Instance.BuildingsCategoryData.category.Find(item => item.GetId().ToString() == gBuilding.Lbuilding.id);
        if (category == null)
        {
            return;
        }

        var requireItems = ResourceViewController.Instance.GetResourceValue(category.QResType);
        if (requireItems < category.QResAmount)
        {
            UIManager.Instance.ShowErrorDlg("Not enough Rubys to construct this building.");
            return;
        }

        var resDic = new Dictionary<EResources, float>();
        resDic.Add(EResources.Gold, Mathf.Floor(category.GoldAmount));
        resDic.Add(EResources.Lumber, Mathf.Floor(category.LumberAmount));
        resDic.Add(EResources.Stone, Mathf.Floor(category.StoneAmount));
        resDic.Add(EResources.Iron, Mathf.Floor(category.IronAmount));
        resDic.Add(category.QResType, -category.QResAmount);

        ResourceViewController.Instance.UpdateResource(resDic, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                if (gBuilding.Lbuilding.GetBuildingType() == EBuildingType.Building)
                {
                    BuildingCreator bCreator = buildingObj.GetComponentInChildren<BuildingCreator>();
                    if (bCreator == null)
                    {
                        return;
                    }
                    bCreator.buildingTime = 1;
                    bCreator.progress = 0f;
                    gBuilding.SetBuildingCreator(bCreator);

                }
                else
                {
                    MineCreator bCreator = buildingObj.GetComponentInChildren<MineCreator>();
                    if (bCreator == null)
                    {
                        return;
                    }
                    bCreator.buildingTime = 1;
                    bCreator.progress = 0f;
                    gBuilding.SetBuildingCreator(bCreator);
                }
                gBuilding.Lbuilding.created_at = Utilities.SystemTimeInMillisecondsString;
                gBuilding.QuickBuild = true;
                

            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public void SelectPosition(BuildingsCategory category, System.Action<bool, string> callback)
    {
        foreach (GBuilding building in flaggedGBuldings)
        {
            building.ShowLocationFlag(false);
        }
        flaggedGBuldings.Clear();

        List<GameObject> selectedObjs = GetUnBuiltObjects(GetBuildingObjects(category.GetId())); //GetUnBuiltObjects(GetBuildingObjects(category.GetId()));
        if (selectedObjs.Count == 0)
        {
            callback(false, "You have already constructed this building.");
            return;
        }

        if (selectedObjs.Count == 1)
        {
            selectedObjs[0].GetComponent<GBuilding>().Category = category;
            CameraController.Instance.MoveCamera(selectedObjs[0].transform.position);
            StartToBuild(selectedObjs[0]);
        }
        else
        {
            flaggedGBuldings.Clear();

            foreach (GameObject obj in selectedObjs)
            {
                GBuilding gBuilding = obj.GetComponent<GBuilding>();
                gBuilding.Category = category;
                flaggedGBuldings.Add(gBuilding);

                gBuilding.ShowLocationFlag(true);
            }
        }

        callback(true, ""); 
    }

}
