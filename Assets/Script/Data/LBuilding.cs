using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class LBuilding : LData
{
    public float happiness = 100f;
    public float health = 100f;
    public float progress = 0f;
    public string bID = "";
    public string produce_at = "";
    public int bTime = 1;

    protected List<LProduct> productList = new List<LProduct>();
    protected List<LProduct> requireList = new List<LProduct>();

    public LBuilding()
    {
        created_at = "";
        produce_at = "";
    }

    public void UpdateProgress(float progress)
    {
        this.progress = progress;
    }

    public void UpdateProduceTime(string strDateTime)
    {
        produce_at = strDateTime;
        ResourceViewController.Instance.UpdateBuilding(this);
    }

    public void UpdateProduceTime(int days)
    {
        var date = Convert.FDateToDateTime(produce_at).AddDays(days);
        produce_at = Convert.DateTimeToFDate(date);
        ResourceViewController.Instance.UpdateBuilding(this);
    }

    public EBuildingType GetBuildingType()//for construction model
    {
        var result = EBuildingType.Building;
        switch (id)
        {
            case "61":
                result = EBuildingType.Mine_Iron;
                break;
            case "62":
            case "65":
                result = EBuildingType.Quarry;
                break;
            case "63":
                result = EBuildingType.Quarry;
                break;
            default:
                break;
        }

        return result;
    }
    //public bool canHouse()
    //{
    //    //TODO
    //    if (currentVillagers.Count == 0)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }

    //}

    public (string, List<EVillagerType>) NeedToHireSpecialist()
    {
        return ResourceViewController.Instance.NeedToHireSpecialist(this);
    }

    public int GetProducer()
    {
        var sWorker = 0;
        var rWorker = 0;

        var Category = DataManager.Instance.GetBuilding(int.Parse(id));
        var specialVillagers = Category.special_villagers;
        if (specialVillagers.Count == 0)
        {
            return 0;
        }

        var workers = ResourceViewController.Instance.GetCurrentVillagers().FindAll(item => item.work_at.ToString() == bID);

        foreach (LVillager villager in workers)
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
        for (int i = 0; i < sWorker; i++)
        {
            if (rWorker >= (i + 1) * Category.require_villagers.Count)
            {
                result = i + 1;
            }
        }

        return result;
    }

    public List<LProduct> GetProducts()
    {
        var result = new List<LProduct>();
        if (productList.Count == 0)
        {
            var Category = DataManager.Instance.GetBuilding(int.Parse(id));
            productList = Category.productList;
            requireList = Category.requireList;
        }

        var producerCount = GetProducer();
        if (productList.Count == 0 || producerCount == 0)
        {
            return result;
        }

        foreach(LProduct lProduct in productList.ToList())
        {
            var amount = lProduct.amount * (float)producerCount;
            result.Add(new LProduct(lProduct.type, amount, lProduct.duration));
        }
        if (id == "1" && UserViewController.Instance.GetCurrentUser().isVegetarian == false)//animal ranch
        {
            var dayOfWeek = Convert.FDateToDateTime(UserViewController.Instance.GetCurrentUser().mode_at).DayOfWeek;
            var bonusAnimal = DataManager.Instance.bonusAnimalForNotVegetarin[dayOfWeek];

            if (bonusAnimal == EResources.Goat)
            {
                result.Add(new LProduct(bonusAnimal, 2, 1));
            }
            else
            {
                result.Add(new LProduct(bonusAnimal, 1, 1));
            }
        }
        
        return result;
    }


    public void CheckProduce()
    {
        if (id == "18")//butcher shop
        {
            CheckProduceForButcherShop();
            return;
        }

        var Category = DataManager.Instance.GetBuilding(int.Parse(id));
        productList = Category.productList;

        if (productList.Count == 0)
        {
            return;
        }

        var producerCount = GetProducer();
        if (producerCount == 0)
        {
            UpdateProduceTime("");
            return;
        }
        if (produce_at == "")
        {
            UpdateProduceTime(Convert.DateTimeToFDate(System.DateTime.Now));
            return;
        }

        if (id == "1")//animal ranch
        {
            if (UserViewController.Instance.GetCurrentUser().isVegetarian == false)
            {
                if (productList.Count == 1)
                {
                    var dayOfWeek = Convert.FDateToDateTime(UserViewController.Instance.GetCurrentUser().mode_at).DayOfWeek;
                    var bonusAnimal = DataManager.Instance.bonusAnimalForNotVegetarin[dayOfWeek];
                    if (bonusAnimal == EResources.Goat)
                    {
                        productList.Add(new LProduct(bonusAnimal, 2, 1));
                    }
                    else
                    {
                        productList.Add(new LProduct(bonusAnimal, 1, 1));
                    }
                }
            }
        }

        var dateTime = Convert.FDateToDateTime(produce_at);
        var dateDiff = (int)Utilities.GetDays(dateTime);

        for (int index = 0; index < producerCount; index++)
        {
            var resDic = new Dictionary<EResources, float>();
            foreach (LProduct resProduct in requireList)
            {
                var curAmont = ResourceViewController.Instance.GetCurrentResourceValue(resProduct.type);
                if (curAmont < resProduct.amount)
                {
                    if (Category.special_villagers.Count > 0)
                    {
                        DataManager.Instance.AddDailyReport(string.Format("{0} has no {1} required for {2}", Category.GetName(), resProduct.type.ToString(), Category.special_villagers[0].ToString()));
                    }
                    return;
                }
                resDic.Add(resProduct.type, -resProduct.amount);
            }

            int multiplies = 1;
            int duration = 1;
            foreach (LProduct product in productList)
            {
                duration = Mathf.Max(duration, product.duration);
                multiplies = dateDiff / product.duration;
                if (multiplies > 0)
                {
                    resDic.Add(product.type, product.amount * multiplies);
                }
            }

            if (resDic.Keys.Count > 0)
            {
                ResourceViewController.Instance.UpdateResource(resDic, (isSuccess, errMsg) =>
                {
                    if (isSuccess)
                    {
                        RewardSystem.Instance.GivesProduceReward(resDic.Keys.ToList());
                        UpdateProduceTime(multiplies * duration);
                    }
                });
            }
        }
    }

    public void CheckProduceForButcherShop()
    {
        if (productList.Count == 0)
        {
            return;
        }

        var producerCount = GetProducer();
        if (producerCount == 0)
        {
            UpdateProduceTime("");
            return;
        }

        if (produce_at == "")
        {
            UpdateProduceTime(Convert.DateTimeToFDate(System.DateTime.Now));
            return;
        }

        var dateTime = Convert.FDateToDateTime(produce_at);
        var dateDiff = (int)Utilities.GetDays(dateTime);

        var resDic = new Dictionary<EResources, float>();
        var resType = EResources.Meat;
        var productType = EResources.Meat;
        var productAmount = 30.0f;

        for (int days = 0; days < dateDiff; days++)
        {
            for (int index = 0; index < producerCount; index++)
            {

                foreach (LProduct resProduct in requireList)
                {
                    var curAmont = ResourceViewController.Instance.GetCurrentResourceValue(resProduct.type);
                    if (curAmont >= resProduct.amount)
                    {
                        resType = resProduct.type;
                        resDic.Add(resProduct.type, -resProduct.amount);
                        break;
                    }
                }

                switch (resType)
                {
                    case EResources.Cattle:
                        productType = EResources.Cattle_Meat;
                        break;
                    case EResources.Goat:
                        productType = EResources.Goat_Meat;
                        break;
                    case EResources.Deer:
                        productType = EResources.Deer_Meat;
                        break;
                    case EResources.Swine:
                        productType = EResources.Swine_Meat;
                        break;
                    default:
                        break;
                }


                if (dateDiff >= productList[0].duration && productType != EResources.Meat)
                {
                    resDic.Add(productType, productAmount);
                }
            }
        }

        ResourceViewController.Instance.UpdateResource(resDic, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                RewardSystem.Instance.GivesProduceReward(resDic.Keys.ToList());
                UpdateProduceTime(Convert.DateTimeToFDate(System.DateTime.Now));
            }
        });
    }
}
