using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Buildings;

public class ResourceViewController : SingletonComponent<ResourceViewController>
{
    public List<LVillager> fVillagers = new List<LVillager>();
    public List<LResource> fResources = new List<LResource>();
    public List<LBuilding> fBuildings = new List<LBuilding>();
    public List<EResources> specialItems = new List<EResources> {EResources.Clothes, EResources.Fine_Clothes, EResources.Garlic, EResources.Jewelry, EResources.Onion, EResources.Spices };
    public void ClearData()
    {
        fVillagers.Clear();
        fResources.Clear();
        fBuildings.Clear();
        DataManager.Instance.CurrentVillagers = fVillagers;
        DataManager.Instance.CurrentBuildings = fBuildings;
        DataManager.Instance.CurrentResources = fResources;
    }

    public void CheckDailyMission()
    {
        CheckProduction();
        CheckHappinessReward();
        PaySalary();
        PayMaintenance();
        PayMeal();
        Assistance();
    }

    public void CheckProduction()
    {
        foreach (LBuilding building in GetCurrentBuildings())
        {
            if (building.progress >= 1.0f)
            {
                building.CheckProduce();
            }
        }
    }

    public void CheckHappinessReward()
    {
        List<LResource> fResources = GetUserResource();
        Dictionary<string, float> dic = new Dictionary<string, float>();
        string happyId = DataManager.Instance.HappyId;
        foreach (LResource lResource in fResources)
        {
            CResource resource = GetCResource(lResource.id); 
            EResources key = resource.type;
            int days = lResource.Effect_Out();
            if (days > 0)
            {
                if (dic.ContainsKey(lResource.id))
                {
                    dic[lResource.id] -= (days * resource.market_amount);
                }
                else
                {
                    dic[lResource.id] = -(days * resource.market_amount);
                }

                

                if (dic.ContainsKey(happyId))
                {
                    dic[happyId] -= (days * resource.effect_amount_per_day);
                }
                else
                {
                    dic[happyId] = -(days * resource.effect_amount_per_day);
                }
            }

            if (lResource.purchasedAt.Count == 0 && resource.marketable_count > 0)
            {
                dic[lResource.id] = -lResource.current_amount;
            }
        }
        

        
        UpdateResource(dic, (isSuccess, errMsg) =>
        {
            RewardSystem.Instance.GiveHappinessReward(GetGoldFromHappiness());
        });
        
    }

    public float GetGoldFromHappiness()
    {
        LUser currentUser = DataManager.Instance.GetCurrentUser();
        float happy = GetCurrentResourceValue(EResources.Happiness);
        float result = 0.0f;
        switch (happy)
        {
            case float f when (f >= 70.0 && f < 75.0):
                if (currentUser.dates.ContainsKey(EDates.Happy7075.ToString()))
                {
                    System.DateTime dateTime = Convert.FDateToDateTime(currentUser.dates[EDates.Happy7075.ToString()]);
                    double days = (System.DateTime.Now - dateTime).TotalDays;
                    if (days >= 7)
                    {
                        result = 75.0f;
                    }
                }
                break;
            case float f when (f >= 75.0 && f < 80.0):
                if (currentUser.dates.ContainsKey(EDates.Happy7580.ToString()))
                {
                    System.DateTime dateTime = Convert.FDateToDateTime(currentUser.dates[EDates.Happy7580.ToString()]);
                    double days = (System.DateTime.Now - dateTime).TotalDays;
                    if (days >= 7)
                    {
                        result = 150.0f;
                    }
                }
                break;
            case float f when (f >= 80.0 && f < 85.0):
                if (currentUser.dates.ContainsKey(EDates.Happy8085.ToString()))
                {
                    System.DateTime dateTime = Convert.FDateToDateTime(currentUser.dates[EDates.Happy8085.ToString()]);
                    double days = (System.DateTime.Now - dateTime).TotalDays;
                    if (days >= 7)
                    {
                        result = 175.0f;
                    }
                }
                break;
            case float f when (f >= 85.0 && f < 90.0):
                if (currentUser.dates.ContainsKey(EDates.Happy8590.ToString()))
                {
                    System.DateTime dateTime = Convert.FDateToDateTime(currentUser.dates[EDates.Happy8590.ToString()]);
                    double days = (System.DateTime.Now - dateTime).TotalDays;
                    if (days >= 7)
                    {
                        result = 200.0f;
                    }
                }
                break;
            case float f when (f >= 90.0 && f < 95.0):
                if (currentUser.dates.ContainsKey(EDates.Happy9095.ToString()))
                {
                    System.DateTime dateTime = Convert.FDateToDateTime(currentUser.dates[EDates.Happy9095.ToString()]);
                    double days = (System.DateTime.Now - dateTime).TotalDays;
                    if (days >= 7)
                    {
                        result = 225.0f;
                    }
                }
                break;
            case float f when (f >= 95.0):
                if (currentUser.dates.ContainsKey(EDates.Happy95Above.ToString()))
                {
                    System.DateTime dateTime = Convert.FDateToDateTime(currentUser.dates[EDates.Happy95Above.ToString()]);
                    double days = (System.DateTime.Now - dateTime).TotalDays;
                    if (days >= 7)
                    {
                        result = 250.0f;
                    }
                }
                break;
            default:
                break;
        }

        return result;
    }


    public List<LResource> GetUserResource(LUser user = null)
    {
        if (user != null && user != UserViewController.Instance.GetCurrentUser())
        {
            return DataManager.Instance.GetOtherUserResources(user).ToList();
        }
        else
        {
            return DataManager.Instance.GetCurrentResources().ToList();
        }
    }

    public List<LResource> GetInitialResource()
    {
        return DataManager.Instance.GetInitResource().ToList();
    }

    public List<LVillager> GetInitialVillagers()
    {
        return DataManager.Instance.GetInitVillager().ToList();
    }

    public List<CResource> GetMarketResource(EMarketType type)
    {
        return DataManager.Instance.GetMarketResourceDic(type).ToList();
    }

    public List<LVillager> GetCurrentVillagers(LUser user = null)
    {
        if (user != null && user != UserViewController.Instance.GetCurrentUser())
        {
            return DataManager.Instance.GetOtherUserVillagers(user).ToList();
        }
        else
        {
            return DataManager.Instance.GetCurrentVillagers().ToList();
        }
        
    }

    public List<CVillager> GetHirelVillagers()
    {
        return DataManager.Instance.VillagersCategoryData.villagers.Where(vil => vil.hire_price > 0).ToList();
    }

    public List<LBuilding> GetCurrentBuildings(LUser user = null)
    {
        if (user != null && user != UserViewController.Instance.GetCurrentUser())
        {
            return DataManager.Instance.GetOtherUserBuildings(user).ToList();
        }
        else
        {
            return DataManager.Instance.GetCurrentBuildings().ToList();
        }
        
    }

    public List<LBuilding> GetInitBuildings()
    {
        return DataManager.Instance.GetInitBuilding().ToList();
    }

    public List<EResources> GetStandardFruitKeys()
    {
        return new List<EResources>() { EResources.Apples, EResources.Pears, EResources.Melons};
    }

    public List<EResources> GetRareFruitKeys()
    {
        return new List<EResources>() { EResources.Grapes, EResources.Cherries, EResources.Raspberries, EResources.Peaches };
    }

    public List<EResources> GetGrainKeys()
    {
        return new List<EResources>() { EResources.Bread, EResources.Corn, EResources.Fava_Beans, EResources.Eggs, EResources.Cabbage };
    }

    public List<EResources> GetStandardMeatKeys()
    {
        return new List<EResources>() { EResources.Beef, EResources.Deer, EResources.Fish};
    }

    public List<EResources> GetRareMeatKeys()
    {
        return new List<EResources>() { EResources.Lamb, EResources.Swine};
    }

    public List<EResources> GetSortedMealKeys()
    {
        List<EResources> eResources = new List<EResources>();
        eResources.AddRange(GetGrainKeys());
        eResources.AddRange(GetStandardFruitKeys());
        eResources.AddRange(GetStandardMeatKeys());
        eResources.AddRange(GetRareFruitKeys());
        eResources.AddRange(GetRareMeatKeys());

        return eResources;
    }

    public int GetVillagePopulation(LUser user = null)
    {
        return GetCurrentVillagers(user).Count;
    }

    public int GetMealConsumeVillaerPopulation(LUser user = null)
    {
        return GetVillagePopulation(user) - 30;
        //List<LVillager> currentVillagers = GetCurrentVillagers(user);
        //int result = 0;
        //foreach (LVillager villager in currentVillagers)
        //{
        //    CVillager cVillager = GetCVillager(villager.id);
        //    if (cVillager.meal_amount > 0f)
        //    {
        //        result++;
        //    }
        //}

        //return result;
    }
    public float GetAvailableMeals(LUser user = null)
    {
        return GetMealAmount(user) / GetMealConsumeVillaerPopulation(user);
    }

    public int GetMealTypesAmount(LUser user = null)
    {
        int result = 0;
        foreach(LResource lResource in GetUserResource(user))
        {
            CResource cResource = GetCResource(lResource.id);
            if (cResource.effect_type == EEffect_Type.Meal && lResource.current_amount > 0f)
            {
                result++;
            }
        }
        return result;
    }

    public float GetMealAmount(LUser user = null)
    {
        List<LResource> currentResources = GetUserResource(user);
        float result = 0;
        foreach (LResource res in currentResources)
        {
            CResource cResource = GetCResource(res.id);
            if (cResource.effect_type == EEffect_Type.Meal)
            {
                result += (cResource.effect_amount_per_day * res.current_amount);
            }
        }

        return result;
    }

    public float GetDailySalary(LUser user = null)
    {
        List<LVillager> currentVillagers = GetCurrentVillagers(user);
        float result = 0;
        foreach (LVillager villager in currentVillagers)
        {
            CVillager cVillager = GetCVillager(villager.id);
            result += (cVillager.salary_amount / 7);
        }

        return result;
    }

    public float GetDailyMaintenance(LUser user = null)
    {
        List<LBuilding> currentBuildings = GetCurrentBuildings(user);
        float result = 0;
        foreach (LBuilding building in currentBuildings)
        {
            BuildingsCategory cBuilding = GetCBuilding(building.id);
            result += (2.0f / 7);
        }
        //result += GetDailyPension();
        result -= 2.0f / 7;
        return result;
    }

    public float GetDailyPension(LUser user = null)
    {
        List<LVillager> currentVillagers = GetCurrentVillagers(user);
        float result = 0;
        foreach (LVillager villager in currentVillagers)
        {
            CVillager cVillager = GetCVillager(villager.id);
            result += (cVillager.pension_amount / 7);
        }

        return result;
    }

    public float GetDailyMeal(LUser user = null)
    {
        //List<LVillager> currentVillagers = GetCurrentVillagers(user);
        //float result = 0;
        //foreach (LVillager villager in currentVillagers)
        //{
        //    CVillager cVillager = GetCVillager(villager.id);
        //    result += (cVillager.meal_amount / 7);
        //}
        return GetMealConsumeVillaerPopulation(user) * 3.0f;
        //return result;
    }

    public float GetCurrentResourceValue(EResources type, LUser user = null)
    {
        return GetResourceValue(type, GetUserResource(user));
    }

    public float GetCurrentResourceValue(LUser user, EResources type)
    {
        return GetResourceValue(type, GetUserResource(user));
    }

    public float GetResourceValue(EResources type, List<LResource> resList)
    {
        foreach (LResource res in resList)
        {
            CResource cResource = GetCResource(res.id);
            if (cResource.type == type)
            {
                return res.current_amount;
            }
        }

        return 0;
    }

    public float GetResourceValue(EResources type)
    {
        List<LResource> resList = GetUserResource();

        if (type == EResources.Meal)
        {
            return GetMealAmount();
        }

        foreach (LResource res in resList)
        {
            CResource cResource = GetCResource(res.id);
            if (cResource.type == type)
            {
                return res.current_amount;
            }
        }

        return 0;
    }

    public LResource GetResource(EResources type, List<LResource> resList)
    {
        foreach (LResource res in resList)
        {
            CResource cResource = GetCResource(res.id);
            if (cResource.type == type)
            {
                return res;
            }
        }

        return new LResource();
    }

    public LResource GetCurrentResource(EResources type)
    {
        foreach (LResource res in GetUserResource())
        {
            CResource cResource = GetCResource(res.id);
            if (cResource.type == type)
            {
                return res;
            }
        }

        return new LResource();
    }

    public CResource GetCResource(string id)
    {
        return DataManager.Instance.ResourcesCategoryData.resources.Find(res => res.id == id);
    }

    public CResource GetCResource(EResources type)
    {
        return DataManager.Instance.ResourcesCategoryData.resources.Find(res => res.type == type);
    }

    public CVillager GetCVillager(string id)
    {
        return DataManager.Instance.VillagersCategoryData.villagers.Find(vil => vil.id == id);
    }

    public BuildingsCategory GetCBuilding(string id)
    {
        return DataManager.Instance.BuildingsCategoryData.category.Find(building => building.GetId().ToString() == id);
    }

    public List<LVillager> GetWorkers(int id)
    {
        return DataManager.Instance.CurrentVillagers.FindAll(vil => vil.work_at == id);
    }

    public List<LVillager> GetOccupants(int id)
    {
        return DataManager.Instance.CurrentVillagers.FindAll(vil => vil.live_at == id);
    }

    public Dictionary<string, int> GetCurrentVillgerDictionary()
    {
        Dictionary<string, int> villagerCountDic = new Dictionary<string, int>();
        foreach (LVillager villager in GetCurrentVillagers())
        {
            if (villagerCountDic.ContainsKey(villager.id))
            {
                villagerCountDic[villager.id]++;
            }
            else
            {
                villagerCountDic[villager.id] = 1;
            }
        }

        return villagerCountDic;
    }

    public List<LVillager> GetVillagers(EVillagerType type)
    {
        var result = new List<LVillager>();

        foreach(LVillager lVillager in GetCurrentVillagers())
        {
            CVillager cVillager = GetCVillager(lVillager.id);
            if (cVillager.type == type)
            {
                result.Add(lVillager);
            }
        }

        return result;
    }

    public void BuyResource(Dictionary<EResources, float> dic, EResources resource, System.Action<bool, string> callback)
    {
        DataManager.Instance.BuyResource(dic, (isSuccess, errMsg) =>
        {
            if (dic.ContainsKey(EResources.Gold))
            {
                var amount = dic[EResources.Gold];
                if (amount > 0){
                    DataManager.Instance.GetCurrentUser().updateSales(System.DateTime.Now, amount);
                    RewardSystem.Instance.GivesSellReward(resource);
                }
                else
                {
                    DataManager.Instance.GetCurrentUser().updateBuy(System.DateTime.Now, amount);
                    RewardSystem.Instance.GivesBuyReward(resource);
                }
            }
            ArtworkSystem.Instance.CheckHappinessMilestone(GetResourceValue(EResources.Happiness));
            UIManager.Instance.UpdateTopProfile();
            callback(isSuccess, errMsg);
        });
    }

    public bool builtBuildingFor(CVillager villager)
    {
        if (villager.buildingIds.Count == 0)
        {
            return true;
        }

        var buildings = GetCurrentBuildings();
        foreach(LBuilding building in buildings)
        {
            if (villager.buildingIds.Contains(int.Parse(building.bID))/*TODO && building.canHouse() == true*/)
            {
                return true;
            }
        }

        return false;
    }

    public void HireVillager(CVillager villager, System.Action<bool, string> callback)
    {
        if (villager == null)
        {
            callback(false, "Please select a villager to hire");
            return;
        }

        //DataManager.Instance.HireVillager(villager, (isSuccess, errMsg) =>
        //{
        //    UIManager.Instance.UpdateTopProfile();
        //    callback(isSuccess, errMsg);
        //});
        
    }

    public void HireVillager(List<EVillagerType> types, int work_at, int live_at, System.Action<bool, string, List<LVillager>> callback)
    {
        var vList = new List<LVillager>();
        
        var hirePrice = 0f;
        var UID = GetCurrentVillagers().Count;
        foreach (EVillagerType type in types)
        {
            var cVillager = DataManager.Instance.VillagersCategoryData.villagers.Find(it => it.type == type);
            if (cVillager == null)
            {           
                callback(false, "Cannot hire a selected specialist", vList);
                return;
            }
            hirePrice += cVillager.hire_price;
            UID++;
            var sVillager = new LVillager();
            sVillager.UID = string.Format("{0}_{1}", UID, Utilities.SystemTimeInMillisecondsString);
            sVillager.work_at = work_at;
            if (type == EVillagerType.Laborer)
            {
                sVillager.id = "32";//laborer
                sVillager.created_at = Utilities.GetFormattedDate(0);
                //newLaborer.live_at = live_at;
            }else if (type == EVillagerType.Currator)
            {
                sVillager.id = cVillager.id;
            }
            else
            {
                sVillager.id = cVillager.id;
                sVillager.live_at = live_at;
            }
            vList.Add(sVillager);
        }
        
        float goldAmount = GetCurrentResourceValue(EResources.Gold);
        if (goldAmount < hirePrice)
        {
            callback(false, "Not enough gold to hire a specialist", vList);
            return;
        }

        if (AppManager.Instance.GetCurrentMode() == Game_Mode.Task_Only)
        {
            callback(true, "", vList);
        }
        else
        {
            UpdateResource(EResources.Gold.ToString(), -hirePrice, (isSuccess, errMsg) =>
            {
                callback(isSuccess, errMsg, vList);
            });
        }
    }

    public void UpdateVillager(LVillager villager)
    {
        
        var currentUser = DataManager.Instance.GetCurrentUser();
        DataManager.Instance.UpdateVillager(villager);
        var population = DataManager.Instance.GetCurrentVillagers().Count();
        if (currentUser.population + 7 <= population)
        {
            //generate spouse and child
            GenerateSpouseAndChild();
            currentUser.UpdatePopulation(population);
        }

        ArtworkSystem.Instance.CheckPopulationMilestone(DataManager.Instance.GetCurrentVillagers().Count());
        if (AppManager.Instance.GetCurrentMode() == Game_Mode.Game_Only)
        {
            AITaskManager.Instance.CheckOnStartWithVillager(villager);
        }
        
        CharacterManager.Instance.CheckVillagers();
    }

    public void GenerateSpouseAndChild()
    {
        var vList = new List<LVillager>();
        var types = new List<EVillagerType>() { EVillagerType.Spouse, EVillagerType.Child };
        var UID = GetCurrentVillagers().Count;
        var buildingId = BuildManager.Instance.GetLiveBuildingId("33");
        foreach (EVillagerType type in types)
        {
            var cVillager = DataManager.Instance.VillagersCategoryData.villagers.Find(it => it.type == type);
            if (cVillager == null)
            {
                return;
            }
            UID++;
            var sVillager = new LVillager();
            sVillager.UID = string.Format("{0}_{1}", UID, Utilities.SystemTimeInMillisecondsString);
            
            sVillager.id = cVillager.id;
            sVillager.live_at = buildingId;
            BuildManager.Instance.AddVillager(sVillager);
            DataManager.Instance.UpdateVillager(sVillager);
        }
    }

    public void RemoveVillager(LVillager villager)
    {
        DataManager.Instance.RemoveVillager(villager);
    }

    public void LoadLocalData()
    {
        DataManager.Instance.LoadLocalData();
    }

    public void PaySalary()
    {
        float salaryAmount = GetDailySalary();
        float goldAmount = GetCurrentResourceValue(EResources.Gold);
        
        System.DateTime lastSalaryDate = Convert.FDateToDateTime(DataManager.Instance.GetSalaryDate());
        int days = (int)Utilities.GetDays(lastSalaryDate, System.DateTime.Now);

        if (days == 0)
        {
            return;
        }
        /*
        int payableDays = (int)Mathf.Ceil(goldAmount / salaryAmount);
        int payDays = Mathf.Min(days, payableDays);
        float fee = (float)payDays * salaryAmount;
        goldAmount -= fee;
        //DataManager.Instance.UpdateSalaryDate(Utilities.GetDate(lastSalaryDate, (double)payDays));
        DataManager.Instance.UpdateSalaryDate(System.DateTime.Now);
        if (payDays < days)
        {
            //TODO - no enough gold to pay
            UIManager.Instance.ShowErrorDlg("Not Enough Gold To Pay Salary");
        }
        UpdateResource(EResources.Gold.ToString(), -fee, (isSuccess, errMsg) =>
        {
        });
        */
        if (goldAmount < salaryAmount)
        {
            if (PlayerPrefs.GetString("Salary") != Convert.DateTimeToFDate(System.DateTime.Now))
            {
                UpdateResource(EResources.Gold.ToString(), -goldAmount, (isSuccess, errMsg) =>
                {
                    UIManager.Instance.ShowErrorDlg("Not Enough Gold To Pay Salaries");
                    DataManager.Instance.UpdateSalaryDate(System.DateTime.Now);
                    PlayerPrefs.SetString("Salary", Convert.DateTimeToFDate(System.DateTime.Now));
                });
            }

            return;
        }

        UpdateResource(EResources.Gold.ToString(), -salaryAmount, (isSuccess, errMsg) =>
        {
            DataManager.Instance.UpdateSalaryDate(System.DateTime.Now);
        });
    }

    public void PayMaintenance()
    {
        float maintenanceAmount = GetDailyMaintenance();
        float goldAmount = GetCurrentResourceValue(EResources.Gold);

        System.DateTime lastPaidDate = Convert.FDateToDateTime(DataManager.Instance.GetMaintenanceDate());
        int days = (int)Utilities.GetDays(lastPaidDate, System.DateTime.Now);

        if (days == 0)
        {
            return;
        }
        /*
        int payableDays = (int)Mathf.Ceil(goldAmount / maintenanceAmount);
        int payDays = Mathf.Min(days, payableDays);
        float fee = (float)payDays * maintenanceAmount;
        goldAmount -= fee;
        //DataManager.Instance.UpdateMaintenanceDate(Utilities.GetDate(lastPaidDate, (double)payDays));
        DataManager.Instance.UpdateMaintenanceDate(System.DateTime.Now);
        if (payDays < days)
        {
            //TODO - no enough gold to pay
            UIManager.Instance.ShowErrorDlg("Not Enough Gold To Pay Maintenance");
        }

        UpdateResource(EResources.Gold.ToString(), -fee, (isSuccess, errMsg) =>
        {
            
        });
        */
        if (goldAmount < maintenanceAmount)
        {
            if (PlayerPrefs.GetString("DailyDropHappiness") != Convert.DateTimeToFDate(System.DateTime.Now))
            {
                UpdateResource(EResources.Happiness.ToString(), -2f, (isSuccess, errMsg) =>
                {
                    UIManager.Instance.ShowErrorDlg("Not Enough Gold To Pay Maintenance");
                    PlayerPrefs.SetString("DailyDropHappiness", Convert.DateTimeToFDate(System.DateTime.Now));
                });
            }

            return;
        }

        UpdateResource(EResources.Gold.ToString(), -maintenanceAmount, (isSuccess, errMsg) =>
        {
            DataManager.Instance.UpdateMaintenanceDate(System.DateTime.Now);
        });
    }

    public void PayMeal()
    {
        float mealAmount = GetDailyMeal();

        System.DateTime lastPaidDate = Convert.FDateToDateTime(DataManager.Instance.GetMealDate());
        int days = (int)Utilities.GetDays(lastPaidDate, System.DateTime.Now);

        if (days == 0)
        {
            return;
        }

        PayMeal(mealAmount);
    }

    public void PayMeal(float amout)
    {
        var mealAmount = amout;
        List<LResource> currentResources = DataManager.Instance.GetCurrentResources().ToList().OrderBy(item => item.created_at).ToList();
        Dictionary<EResources, float> updateList = new Dictionary<EResources, float>();

        foreach (LResource resource in currentResources)
        {
            CResource cResource = GetCResource(resource.id);
            if (cResource != null && cResource.effect_type == EEffect_Type.Meal)
            {
                float meals = cResource.effect_amount_per_day * resource.current_amount;
                if (meals <= 0)
                {
                    continue;
                }

                if (mealAmount >= meals)
                {
                    updateList.Add(cResource.type, -resource.current_amount);
                    mealAmount -= meals;
                }
                else
                {
                    updateList.Add(cResource.type, -mealAmount / cResource.effect_amount_per_day);
                    mealAmount = 0;
                    break;
                }
            }
        }

        UpdateResource(updateList, (isSuccess, errMsg) =>
        {
            DataManager.Instance.UpdateMealDate(System.DateTime.Now);
        });

        PaySpeicalItems(10.0f);

        if (mealAmount > 0)
        {
            if (PlayerPrefs.GetString("DailyDropHappiness") != Convert.DateTimeToFDate(System.DateTime.Now))
            {
                UpdateResource(EResources.Happiness.ToString(), -2f, (isSuccess, errMsg) =>
                {
                    UIManager.Instance.ShowErrorDlg("Not Enough Gold To Pay Maintenance");
                    PlayerPrefs.SetString("DailyDropHappiness", Convert.DateTimeToFDate(System.DateTime.Now));
                });
            }

            return;
        }
    }

    public void PaySpeicalItems(float amount)
    {
        var totalAmount = 0f;
        Dictionary<EResources, float> updateList = new Dictionary<EResources, float>();

        foreach (EResources res in specialItems)
        {
            var curRes = GetCurrentResourceValue(res);
            if (totalAmount + curRes >= amount)
            {
                updateList.Add(res, -(curRes - (amount - totalAmount)));
                totalAmount = amount;
            }
            else
            {
                updateList.Add(res, -curRes);
                totalAmount += curRes;
            }
        }

        var curWineValue = GetCurrentResourceValue(EResources.Wine);
        updateList.Add(EResources.Wine, curWineValue >= 10.0f ? -10.0f : -curWineValue);

        var curAleValue = GetCurrentResourceValue(EResources.Ale);
        updateList.Add(EResources.Ale, curAleValue >= 40.0f ? -40.0f : -curAleValue);

        UpdateResource(updateList, (isSuccess, errMsg) =>
        {
            //vanish off list at 10 items per day, no matter how much is purchased 01/25/2023
        });
    }

    public void CreateBuilding(LBuilding building)
    {
        var buildings = DataManager.Instance.GetCurrentBuildings();
        var index = buildings.FindIndex(lb => lb.id == building.id);
        if (index >= 0)
        {
            UpdateBuilding(building);
        }
        else
        {
            buildings.Add(building);
            DataManager.Instance.CurrentBuildings = buildings;
        }
    }

    public bool IsBuildingBuilt(string buildingId)
    {
        var building = GetCurrentBuildings().Find(item => item.bID == buildingId);
        if (building == null || building.progress < 1.0f)
        {
            return false;
        }
        return true;
    }

    public void UpdateBuilding(LBuilding building)
    {
        var buildings = DataManager.Instance.GetCurrentBuildings();
        var prevID = buildings.FindIndex(lb => lb.bID == building.bID);
        if (prevID >= 0)
        {
            buildings.RemoveAt(prevID);
        }
        buildings.Add(building);
        DataManager.Instance.CurrentBuildings = buildings;
    }

    public void RemoveBuilding(LBuilding building)
    {
        var buildings = DataManager.Instance.GetCurrentBuildings();
        var prevID = buildings.FindIndex(lb => lb.id == building.id);
        if (prevID >= 0)
        {
            buildings.RemoveAt(prevID);
        }

        DataManager.Instance.CurrentBuildings = buildings;
    }

    public void OnExchange(EResources from, float fAmount, EResources to, float tAmount, System.Action<bool, string> callback = null)
    {
        var resourceDic = new Dictionary<EResources, float>();
        var totalAmount = GetResourceValue(from);
        if (totalAmount < fAmount)
        {
            return;
        }
        resourceDic.Add(from, -fAmount);
        resourceDic.Add(to, tAmount);
        UpdateResource(resourceDic, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                UIManager.Instance.ShowRewardMessage("You've got ", to.ToString().ToLower(), DataManager.Instance.GetSprite(to), tAmount);
            }
            if (callback != null)
            {
                callback(isSuccess, errMsg);
            }
        });
    }

    public void OnAutomaticExchange()
    {
        //sapphire
        
        if (GetResourceValue(EResources.Sapphire) >= 5.0f)
        {
            OnExchange(EResources.Sapphire, 5f, EResources.Ruby, 1f);
            UIManager.Instance.ShowRewardMessage("You've got", "a ruby", DataManager.Instance.ruby_Sprite, 1);
        }
    }

    public bool CheckResource(Dictionary<EResources, float> resourceDic)
    {
        var result = true;
        foreach(EResources res in resourceDic.Keys)
        {
            if (GetResourceValue(res) < Mathf.Abs(resourceDic[res]))
            {
                result = false;
                break;
            }
        }

        return result;
    }

    public void UpdateResource(Dictionary<EResources, float> resourceDic, System.Action<bool, string> callback)
    {
        Dictionary<string, float> resourceStrDic = new Dictionary<string, float>();

        foreach(EResources key in resourceDic.Keys)
        {
            resourceStrDic.Add(key.ToString(), resourceDic[key]);
        }


        UpdateResource(resourceStrDic, callback);
    }

    public void UpdateResource(Dictionary<string, float> resourceDic, System.Action<bool, string> callback)
    {
        if (resourceDic.Keys.Contains(EResources.Meal.ToString()))
        {
            PayMeal(Mathf.Abs(resourceDic[EResources.Meal.ToString()]));
            resourceDic.Remove(EResources.Meal.ToString());
        }

        DataManager.Instance.UpdateResource(resourceDic, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                OnAutomaticExchange();
            }
            ArtworkSystem.Instance.CheckHappinessMilestone(GetResourceValue(EResources.Happiness));
            UIManager.Instance.UpdateTopProfile();
            callback(isSuccess, errMsg);
        });
    }

    public void UpdateResource(string resourceName, float amount, System.Action<bool, string> callback)
    {
        Dictionary<string, float> resourceDic = new Dictionary<string, float>();
        resourceDic.Add(resourceName, amount);
        UpdateResource(resourceDic, callback);
    }


    public (string, List<EVillagerType>) NeedToHireSpecialist(LBuilding lBuilding)
    {
        var Category = DataManager.Instance.GetBuilding(int.Parse(lBuilding.id));
        var result = new List<EVillagerType>();
        if (Category.special_villagers.Count == 0 && Category.require_villagers.Count == 0)
        {
            return ("", result);
        }

        var sWorkerIDs = new List<string>();

        var workers = ResourceViewController.Instance.GetCurrentVillagers().FindAll(item => item.work_at.ToString() == lBuilding.bID);
        foreach (LVillager villager in workers)
        {
            var cVillager = ResourceViewController.Instance.GetCVillager(villager.id);
            foreach (EVillagerType type in Category.special_villagers)
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
                for (int i = 0; i < value; i++)
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
        }
        else
        {
            if (Category.special_villagers.Count + Category.special_villagers.Count * Category.require_villagers.Count > workers.Count)
            {
                var value = sWorkerIDs.Count + sWorkerIDs.Count * Category.require_villagers.Count - workers.Count;
                for (int i = 0; i < value; i++)
                {
                    result.Add(Category.require_villagers[0]);
                }
                return ("Hire Laborer", result);
            }
            else if (Category.special_villagers.Count == 0 && Category.require_villagers.Count > workers.Count)
            {
                result.AddRange(Category.require_villagers);
                return ("Hire Laborer", result);
            }
        }

        return ("", result);
    }

    public List<LProduct> GetAllProducts()
    {
        var productList = new List<LProduct>();
        foreach (LBuilding building in GetCurrentBuildings())
        {
            //if (building.id != "18")//butcher shop
            //{
                productList.AddRange(building.GetProducts());
            //}
        }

        return productList;
    }

    public List<LVillager> GetGuestsFromInn(LUser user = null)
    {
        return GetCurrentVillagers(user).FindAll(item => item.live_at == 80 && item.work_at != 80).ToList();//80 is villagerInn building index
    }

    public void DestroyBuilding(LBuilding building)
    {
        //TODO-remove building data
    }


    public void ChangeMode(Game_Mode game_mode)
    {
        DataManager.Instance.ChangeMode(game_mode);
    }



    /////////////////////////////////////////////////////////////////////////////////////
    //////////////                   Task app only mode                //////////////////
    /////////////////////////////////////////////////////////////////////////////////////
    public void Assistance()
    {

        if (AppManager.Instance.GetCurrentMode() == Game_Mode.Task_Only)
        {
            var user = UserViewController.Instance.GetCurrentUser();
            if (user.GetAgesAsDays() > 45)
            {
                return;
            }

            if (user.hasGotAssist())
            {
                return;
            }

            var resDic = new Dictionary<EResources, float>();
            resDic.Add(EResources.Gold, 65);
            resDic.Add(EResources.Lumber, 25);
            resDic.Add(EResources.Stone, 15);
            resDic.Add(EResources.Iron, 10);

            UpdateResource(resDic, (isSuccess, errMsg) =>
            {
                if (isSuccess)
                {
                    user.updateDates(EDates.Assist.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                }
            });
        }
    }
}
