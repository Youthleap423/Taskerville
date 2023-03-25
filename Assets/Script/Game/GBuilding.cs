using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UIControllersAndData.Store.Categories.Buildings;
using UnityEditor;

public class GBuilding : MonoBehaviour
{
    [SerializeField] private string buildingObjectId = "0";
    [SerializeField] private string prefabName = "";
    [SerializeField] private string objectName = "";
    [SerializeField] private string groundMaterial = "Grass";
    [SerializeField] private bool demolishAbility = false;
    [SerializeField] private List<LVillager> workers = new List<LVillager>();
    [SerializeField] private List<LVillager> occupants = new List<LVillager>();
    [Space]
    [SerializeField] private bool hasBonusForBuilt = false;

    [Space]
    [SerializeField] private LBuilding lBuilding = null;
    [SerializeField] private bool bQuickBuild = false;
    private BuildingsCategory buildingsCategory = null;
    private BuildingCreator bCreator = null;
    

    public string GetBuildingObjectID
    {
        get
        {
            return buildingObjectId;
        }

        set
        {
            buildingObjectId = value;
        }
    }

    public BuildingsCategory Category
    {
        get
        {
            return buildingsCategory;
        }

        set
        {
            buildingsCategory = value;
        }
    }

    public string GroundMaterial
    {
        get
        {
            return groundMaterial;
        }
        set
        {
            groundMaterial = value;
        }
    }

    public LBuilding Lbuilding
    {
        get
        {
            return lBuilding;
        }

        set
        {
            lBuilding = value;
        }
    }

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

    private void Start()
    {
        Category = DataManager.Instance.GetBuilding(int.Parse(lBuilding.id));
    }

    public int GetBuildingID()
    {
        return int.Parse(lBuilding.id);
    }

    public float GetProgress()
    {
        return lBuilding.progress;
    }

    public string GetName()
    {
        return objectName;
    }

    public List<LVillager> GetWorkers()
    {
        return workers.ToList();
    }

    public List<LVillager> GetOccupants()
    {
        return occupants.ToList();
    }

    public List<LVillager> GetGuests()
    {
        return occupants.FindAll(it => it.work_at != int.Parse(buildingObjectId)).ToList();
    }

    public void RemoveGuest(string UID)
    {
        occupants.RemoveAll(it => it.UID == UID);
    }

    public bool IsDemolishable()
    {
        return demolishAbility;
    }

    public bool IsBuilt()
    {
        return lBuilding.progress >= 1.0f;
    }

    public int GetProducer()
    {
        var sWorker = 0;
        var rWorker = 0;
        var specialVillagers = Category.special_villagers;
        if (specialVillagers.Count == 0)
        {
            return 0;
        }

        foreach(LVillager villager in workers)
        {
            if (villager.canWork(System.DateTime.Now))
            {
                if (ResourceViewController.Instance.GetCVillager(villager.id).type == specialVillagers[0])
                {

                    sWorker++;
                }
                else
                {
                    rWorker++;
                }
            
            }
        }

        var result = 0;
        for(int i = 0; i < sWorker; i++)
        {
            if (rWorker >= (i + 1) * Category.require_villagers.Count)
            {
                result = i + 1;
            }
        }

        return result;
    }

    

    
    public (string, List<EVillagerType>) NeedToHireSpecialist()
    {
        var result = new List<EVillagerType>();
        if (Category.special_villagers.Count == 0 && Category.require_villagers.Count == 0)
        {
            return ("", result);
        }

        var sWorkerIDs = new List<string>();

        foreach(LVillager villager in workers)
        {
            var cVillager = ResourceViewController.Instance.GetCVillager(villager.id);
            foreach(EVillagerType type in Category.special_villagers)
            {
                if (cVillager.type == type)
                {
                    if (!sWorkerIDs.Contains(villager.UID))
                    {
                        sWorkerIDs.Add(villager.UID);
                    }
                }
            }
        }
        
        if (sWorkerIDs.Count < Category.special_villagers.Count)
        {
            if (sWorkerIDs.Count + sWorkerIDs.Count * Category.require_villagers.Count > workers.Count)
            {
                var value = sWorkerIDs.Count + sWorkerIDs.Count * Category.require_villagers.Count - workers.Count;
                for(int i = 0; i < value; i++)
                {
                    result.Add(Category.require_villagers[0]);
                }
                return ("Hire Laborer", result);
            }
            else
            {
                var value = sWorkerIDs.Count + (sWorkerIDs.Count + 1) * Category.require_villagers.Count - workers.Count;
                result.Add(Category.special_villagers[0]);
                for (int i = 0; i < value; i++)
                {
                    result.Add(Category.require_villagers[0]);
                }
                return ("Hire Specialist", result);
            }
        }else{
            if (Category.special_villagers.Count + Category.special_villagers.Count * Category.require_villagers.Count > workers.Count)
            {
                var value = sWorkerIDs.Count + sWorkerIDs.Count * Category.require_villagers.Count - workers.Count;
                for (int i = 0; i < value; i++)
                {
                    result.Add(Category.require_villagers[0]);
                }
                return ("Hire Laborer", result);
            }else if (Category.special_villagers.Count == 0 && Category.require_villagers.Count > workers.Count)
            {
                result.AddRange(Category.require_villagers);
                return ("Hire Laborer", result);
            }
        }
        
        return ("", result);
    }


    public bool CanHouse(string vID)
    {
        if (lBuilding.progress < 1.0f)
        {
            return false;
        }

        
        if (lBuilding.id == "25")//manor
        {
            if (vID == "23")// curator id
            {
                foreach (LVillager villager in occupants)
                {
                    if (villager.id == vID)
                    {
                        return false;
                    }
                }
                return true;
            }

            if (vID == "33" || vID == "36")
            {
                return GetOccupants(occupants) < 4;
            }
        }
        else
        {
            if (vID == "23")// curator id
            {
                return false;
            }

            if (lBuilding.id == "22")//Cottage 2
            {
                return GetOccupants(occupants) < 8;
            }

            if (lBuilding.id == "23")//Cottage 1
            {
                return GetOccupants(occupants) < 4;
            }
        }
        

        return false;
    }

    public int GetOccupants(List<LVillager> villlagers) {
        var result = 0;
        foreach (LVillager villager in villlagers)
        {
            var cVillager = ResourceViewController.Instance.GetCVillager(villager.id);
            result += cVillager.size_of_house;
        }

        return result;
    }
    public void ShowLocationFlag(bool bFlag)
    {
        transform.GetChild(0).gameObject.SetActive(bFlag);
        transform.GetChild(1).gameObject.SetActive(bFlag);
    }


    public void StartToBuild()
    {
        if (lBuilding.progress == 0f)
        {
            BuildManager.Instance.StartToBuild(gameObject);
        }
    }

    public void CancelBuild()
    {
        if (bCreator != null)
        {
            UIManager.LogError("CancelBuilding");
            DestroyImmediate(bCreator.gameObject);
        }
        lBuilding.progress = 0.0f;
        lBuilding.created_at = "";
        transform.GetChild(0).gameObject.SetActive(false);
        ResourceViewController.Instance.RemoveBuilding(lBuilding);
        bCreator = null;
    }

    public void Demolish()
    {
        DestroyImmediate(transform.GetChild(2).gameObject);
        lBuilding.progress = 0.0f;
        lBuilding.created_at = "";
        transform.GetChild(0).gameObject.SetActive(false);
        ResourceViewController.Instance.RemoveBuilding(lBuilding);
    }

    public void Complete()
    {
        lBuilding.progress = 1.0f;
        lBuilding.bID = buildingObjectId;
        lBuilding.created_at = Convert.DateTimeToFDate(System.DateTime.Now);
        transform.GetChild(0).gameObject.SetActive(true);
        LoadBuilding(lBuilding);
        bCreator = null;
        ResourceViewController.Instance.UpdateBuilding(lBuilding);
        if (buildingsCategory != null && buildingsCategory.culturePoint != 0)
        {
            DataManager.Instance.UpdateResource(EResources.Culture, (float)(buildingsCategory.culturePoint));

            if (buildingsCategory.Description == "Unique Building")
            {
                if (UserViewController.Instance.GetCurrentUser().fBU == false)
                {
                    DataManager.Instance.UpdateResource(EResources.Culture, 3f);
                    UserViewController.Instance.GetCurrentUser().fBU = true;
                    UserViewController.Instance.GetCurrentUser().Save();
                }
            }
        }
        
        if (hasBonusForBuilt)
        {
            RewardSystem.Instance.OnBuiltComplete();
        }
    }

    public void OnTapped()
    {
        if (lBuilding.progress > 0f)
        {
            BuildingInfoDlg.Instance.SetInfo(gameObject);
        }else
        {
            StartToBuild();
        }
    }

    public void UpdateProgress(float prog)
    {
        lBuilding.progress = prog;
    }

    public void SetBuildingCreator(BuildingCreator creator)
    {
        lBuilding.bID = buildingObjectId;
        //lBuilding.id = Category.GetId() + "";
        if (lBuilding.created_at == "")
        {
            lBuilding.created_at = Utilities.SystemTimeInMillisecondsString;
        }
        lBuilding.bTime = creator.buildingTime;
        ResourceViewController.Instance.CreateBuilding(lBuilding);
        bCreator = creator;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void RemoveBuilding()
    {
        if (lBuilding.progress == 0.0f)
        {
            return;
        }

        workers.Clear();
        occupants.Clear();

        DestroyImmediate(transform.GetChild(2).gameObject);
    }

    public void LoadBuilding(LBuilding building)
    {
        try
        {
            if (lBuilding.id == "55")
            {
                prefabName = BuildManager.Instance.GetTemplePrefabName();
            }

            GameObject prefabObj = Resources.Load<GameObject>("Building/" + prefabName);
            //GameObject prefabObj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Building/" + prefabName, typeof(GameObject));
            if (prefabObj != null)
            {
                this.lBuilding = building;
                if (BuildManager.Instance.needRender)
                {
                    var obj = Instantiate(prefabObj, transform);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    obj.transform.SetSiblingIndex(2);
                    lBuilding.progress = 1.0f;
                    transform.GetChild(0).gameObject.SetActive(true);
                    var adjustZ = transform.GetComponent<AdjustZ>();
                    if (adjustZ != null)
                    {
                        adjustZ.AdjustStructureZ();
                    }
                }

                var villagers = ResourceViewController.Instance.GetWorkers(int.Parse(buildingObjectId));
                if (villagers.Count != 0)
                {
                    foreach(LVillager villager in villagers)
                    {
                        if (!workers.Contains(villager))
                        {
                            workers.Add(villager);
                        }
                    }
                }

                villagers.Clear();
                villagers = ResourceViewController.Instance.GetOccupants(int.Parse(buildingObjectId));
                if (villagers.Count != 0)
                {
                    foreach (LVillager villager in villagers)
                    {
                        if (!occupants.Contains(villager))
                        {
                            occupants.Add(villager);
                        }
                    }
                }

                CheckBuilders();

            }
            lBuilding.CheckProduce();
        }
        catch
        {
            UIManager.Instance.ShowErrorDlg("Can't found the model - " + prefabName);
        }
        
    }

    public void CheckBuilders()//workers
    {
        var tHireVillager = NeedToHireSpecialist();

        var nonBuilderTrans = transform.Find("nonBuilderObj");
        GameObject nonBuilderObj = nonBuilderTrans != null ? nonBuilderTrans.gameObject : null; 
        

        if (tHireVillager.Item1 == "")
        {
            if (nonBuilderObj != null)
            {
                DestroyImmediate(nonBuilderObj);
            }
        }
        else
        {
            if (nonBuilderObj == null)
            {
                nonBuilderObj = Instantiate(BuildManager.Instance.GetNonBuilderPrefab(), transform);
                nonBuilderObj.name = "nonBuilderObj";
            }
        }
    }

    public void AddVillager(LVillager villager, bool isWorker)
    {
        if (isWorker)
        {
            workers.Add(villager);
        }
        else
        {
            occupants.Add(villager);
        }
    }

    public void RemoveVillager(LVillager villager, bool isWorker)
    {
        if (isWorker)
        {
            workers.Remove(villager);
        }
        else
        {
            occupants.Remove(villager);
        }
    }
}
