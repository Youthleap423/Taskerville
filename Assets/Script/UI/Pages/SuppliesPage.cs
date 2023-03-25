using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuppliesPage : Page
{
    [SerializeField] private GameObject resObj;
    [SerializeField] private GameObject meatObj;
    [SerializeField] private GameObject farmObj;
    [SerializeField] private GameObject specialObj;

    [Header("Resrouces")]
    [SerializeField] private Text Iron_TF;
    [SerializeField] private Text Stone_TF;
    [SerializeField] private Text Lumber_TF;
    [SerializeField] private Text Iron_Ore_TF;
    [SerializeField] private Text Logs_TF;
    [SerializeField] private Text Raw_Gold_TF;
    [SerializeField] private Text Raw_Silver_TF;

    [Header("Meat & Livestock")]
    [SerializeField] private Text Cattle_TF;
    [SerializeField] private Text Deer1_TF;
    [SerializeField] private Text Lamb1_TF;
    [SerializeField] private Text Swine1_TF;

    [Space]
    [SerializeField] private Text Beef_TF;
    [SerializeField] private Text Deer_TF;
    [SerializeField] private Text Fish_TF;
    [SerializeField] private Text Lamb_TF;
    [SerializeField] private Text Swine_TF;

    [Space]

    [SerializeField] private Text Beef_unit_TF;
    [SerializeField] private Text Deer_unit_TF;
    [SerializeField] private Text Fish_unit_TF;
    [SerializeField] private Text Lamb_unit_TF;
    [SerializeField] private Text Swine_unit_TF;

    [Header("Farm Goods & Grains")]
    [SerializeField] private Text Apple_TF;
    [SerializeField] private Text Cabbage_TF;
    [SerializeField] private Text Cherries_TF;
    [SerializeField] private Text Grapes_TF;
    [SerializeField] private Text Corn_TF;
    [SerializeField] private Text Eggs_TF;
    [SerializeField] private Text Fava_Beans_TF;
    [SerializeField] private Text Melon_TF;
    [SerializeField] private Text Peaches_TF;
    [SerializeField] private Text Pears_TF;
    [SerializeField] private Text Raspberries_TF;
    [Space]

    [SerializeField] private Text Bread_TF;
    [SerializeField] private Text Barley_TF;
    [SerializeField] private Text Flour_TF;
    [SerializeField] private Text Malted_Barley_TF;
    [SerializeField] private Text Wheat_TF;

    [Space]
    [SerializeField] private Text Apple_unit_TF;
    [SerializeField] private Text Cabbage_unit_TF;
    [SerializeField] private Text Cherries_unit_TF;
    [SerializeField] private Text Grapes_unit_TF;
    [SerializeField] private Text Corn_unit_TF;
    [SerializeField] private Text Eggs_unit_TF;
    [SerializeField] private Text Fava_Beans_unit_TF;
    [SerializeField] private Text Melon_unit_TF;
    [SerializeField] private Text Peaches_unit_TF;
    [SerializeField] private Text Pears_unit_TF;
    [SerializeField] private Text Raspberries_unit_TF;
    [SerializeField] private Text Bread_unit_TF;

    [Header("Specialty")]
    [SerializeField] private Text Ale_TF;
    [SerializeField] private Text Clothes_TF;
    [SerializeField] private Text Clothes_Fine_TF;
    [SerializeField] private Text Garlic_TF;
    [SerializeField] private Text Jewelry_TF;
    [SerializeField] private Text Onion_TF;
    [SerializeField] private Text Spices_TF;
    [SerializeField] private Text Wine_TF;
    private List<LResource> resources = new List<LResource>();
    private void OnEnable()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();

        resObj.SetActive(true);
        meatObj.SetActive(false);
        farmObj.SetActive(false);
        specialObj.SetActive(false);

        resources = ResourceViewController.Instance.GetUserResource();

        Iron_TF.text = GetValue(EResources.Iron);
        Stone_TF.text = GetValue(EResources.Stone);
        Lumber_TF.text = GetValue(EResources.Lumber);
        Iron_Ore_TF.text = GetValue(EResources.Iron_Ore);
        Logs_TF.text = GetValue(EResources.Logs);
        Raw_Gold_TF.text = GetValue(EResources.Raw_Gold);
        Raw_Silver_TF.text = GetValue(EResources.Raw_Silver);

        Cattle_TF.text = GetValue(EResources.Cattle);
        Deer1_TF.text = GetValue(EResources.Deer);
        Lamb1_TF.text = GetValue(EResources.Goat);
        Swine1_TF.text = GetValue(EResources.Swine);


        Beef_TF.text = GetValue(EResources.Cattle_Meat);
        Deer_TF.text = GetValue(EResources.Deer_Meat);
        Fish_TF.text = GetValue(EResources.Fish);
        Lamb_TF.text = GetValue(EResources.Goat_Meat);
        Swine_TF.text = GetValue(EResources.Swine_Meat);

        Beef_unit_TF.text = GetUnit(EResources.Beef);
        Deer_unit_TF.text = GetUnit(EResources.Deer);
        Fish_unit_TF.text = GetUnit(EResources.Fish);
        Lamb_unit_TF.text = GetUnit(EResources.Lamb);
        Swine_unit_TF.text = GetUnit(EResources.Swine);

        Apple_TF.text = GetValue(EResources.Apples);
        Cabbage_TF.text = GetValue(EResources.Cabbage);
        Cherries_TF.text = GetValue(EResources.Cherries);
        Grapes_TF.text = GetValue(EResources.Grapes);
        Corn_TF.text = GetValue(EResources.Corn);
        Eggs_TF.text = GetValue(EResources.Eggs);
        Fava_Beans_TF.text = GetValue(EResources.Fava_Beans);
        Melon_TF.text = GetValue(EResources.Melons); 
        Peaches_TF.text = GetValue(EResources.Peaches);
        Pears_TF.text = GetValue(EResources.Pears);
        Raspberries_TF.text = GetValue(EResources.Raspberries);

        Bread_TF.text = GetValue(EResources.Bread);
        Barley_TF.text = GetValue(EResources.Barley);
        Flour_TF.text = GetValue(EResources.Flour);
        Malted_Barley_TF.text = GetValue(EResources.Malted_Barley);
        Wheat_TF.text = GetValue(EResources.Wheat);

        Apple_unit_TF.text = GetUnit(EResources.Apples);
        Cabbage_unit_TF.text = GetUnit(EResources.Cabbage);
        Cherries_unit_TF.text = GetUnit(EResources.Cherries);
        Grapes_unit_TF.text = GetUnit(EResources.Grapes);
        Corn_unit_TF.text = GetUnit(EResources.Corn);
        Eggs_unit_TF.text = GetUnit(EResources.Eggs);
        Fava_Beans_unit_TF.text = GetUnit(EResources.Fava_Beans);
        Melon_unit_TF.text = GetUnit(EResources.Melons);
        Peaches_unit_TF.text = GetUnit(EResources.Peaches);
        Pears_unit_TF.text = GetUnit(EResources.Pears);
        Raspberries_unit_TF.text = GetUnit(EResources.Raspberries);
        Bread_unit_TF.text = GetUnit(EResources.Bread);


        Ale_TF.text = GetValue(EResources.Ale);
        Clothes_TF.text = GetValue(EResources.Clothes);
        Clothes_Fine_TF.text = GetValue(EResources.Fine_Clothes);
        Garlic_TF.text = GetValue(EResources.Garlic);
        Jewelry_TF.text = GetValue(EResources.Jewelry);
        Onion_TF.text = GetValue(EResources.Onion);
        Spices_TF.text = GetValue(EResources.Spices);
        Wine_TF.text = GetValue(EResources.Wine);
    }

    public void ShowMeatInfo()
    {
        resObj.SetActive(false);
        meatObj.SetActive(true);
        farmObj.SetActive(false);
        specialObj.SetActive(false);
    }

    public void ShowFarmInfo()
    {
        resObj.SetActive(false);
        meatObj.SetActive(false);
        farmObj.SetActive(true);
        specialObj.SetActive(false);
    }

    public void ShowSpecialInfo()
    {
        resObj.SetActive(false);
        meatObj.SetActive(false);
        farmObj.SetActive(false);
        specialObj.SetActive(true);
    }

    public void BackToResource()
    {
        resObj.SetActive(true);
        meatObj.SetActive(false);
        farmObj.SetActive(false);
        specialObj.SetActive(false);
    }
    private string GetValue(EResources resKey)
    {
        return string.Format("{0}", (int)ResourceViewController.Instance.GetResourceValue(resKey, resources));
    }

    private string GetUnit(EResources resKey)
    {
        CResource resource = ResourceViewController.Instance.GetCResource(resKey);
        float value = resource.effect_amount_per_day;
        int num = Utilities.findnum(value.ToString());

        value = num * value;
        if (value > 1)
        {
            return string.Format("{0} = {1} meals", num, (int)value);
        }

        return string.Format("{0} = {1} meal", num, (int)value);
    }
}
