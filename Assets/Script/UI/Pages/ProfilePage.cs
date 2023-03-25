using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfilePage : Page
{
    [SerializeField] private Text name_TF;
    [SerializeField] private Text village_TF;
    [SerializeField] private Image avatar_Image;

    [Header("Resources")]
    [SerializeField] private Text fruit_TF;
    [SerializeField] private Text stone_TF;
    [SerializeField] private Text iron_TF;
    [SerializeField] private Text wood_TF;
    [SerializeField] private Text culture_TF;

    [SerializeField] private Text village_population_TF;
    [SerializeField] private Text available_Meals_TF;
    [SerializeField] private Text settler_houses_TF;
    [SerializeField] private Text salary_TF;
    [SerializeField] private Text maintenance_TF;

    [SerializeField] private TMP_Text bonuslist_TF;
    private List<LResource> resources = new List<LResource>();
    private LUser user = null;

    #region Unity Members
    private void OnEnable()
    {
        Initialize();
    }
    #endregion

    #region Private Members

    #endregion

    #region Public Members
    public override void Initialize()
    {
        base.Initialize();
        if (user == null)
        {
            Debug.LogError("get user");
            user = UserViewController.Instance.GetCurrentUser();
        }

        name_TF.text = string.Format("Mayor {0}", user.GetFullName());
        village_TF.text = string.Format("Village of {0}", user.Village_Name);
        avatar_Image.sprite = UserViewController.Instance.GetAvatarSprite(user.AvatarId);


        fruit_TF.text = "";
        stone_TF.text = "";
        iron_TF.text = "";
        wood_TF.text = "";
        culture_TF.text = "";

        village_population_TF.text = "";
        available_Meals_TF.text = "";
        salary_TF.text = "";
        maintenance_TF.text = "";

        resources = ResourceViewController.Instance.GetUserResource(user);
        
        fruit_TF.text = string.Format("{0}", (int)ResourceViewController.Instance.GetMealAmount(user));
        stone_TF.text = GetValue(EResources.Stone);
        iron_TF.text = GetValue(EResources.Iron);
        wood_TF.text = GetValue(EResources.Lumber);
        culture_TF.text = GetValue(EResources.Culture);

        village_population_TF.text = string.Format("{0}", ResourceViewController.Instance.GetVillagePopulation(user));
        available_Meals_TF.text =  string.Format("{0:0.0}", ResourceViewController.Instance.GetAvailableMeals(user));
        settler_houses_TF.text = string.Format("{0}", ResourceViewController.Instance.GetGuestsFromInn(user).Count);
        salary_TF.text = string.Format("{0:0.0}", ResourceViewController.Instance.GetDailySalary(user));
        maintenance_TF.text = string.Format("{0:0.0}", ResourceViewController.Instance.GetDailyMaintenance(user));

        var dayOfWeek = Convert.FDateToDateTime(user.mode_at).DayOfWeek;
        bonuslist_TF.text = user.isVegetarian ? DataManager.Instance.bonusListForVegetarin[dayOfWeek] : DataManager.Instance.bonusListForNotVegetarin[dayOfWeek];
        /*
        UIManager.Instance.ShowLoadingBar(true);
        ResourceViewController.Instance.LoadData(user, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                resources = ResourceViewController.Instance.GetUserResource(user);
                goldCoin_TF.text = GetValue(EResources.Gold);
                happiness_TF.text = string.Format("{0}%", (int)ResourceViewController.Instance.GetCurrentResourceValue(EResources.Happiness, user));
                fruit_TF.text = string.Format("{0}", (int)ResourceViewController.Instance.GetMealAmount(user));
                stone_TF.text = GetValue(EResources.Stone);
                iron_TF.text = GetValue(EResources.Iron);
                wood_TF.text = GetValue(EResources.Lumber);

                village_population_TF.text = string.Format("{0}", ResourceViewController.Instance.GetVillagePopulation(user));
                available_Meals_TF.text = string.Format("{0:0.0}", ResourceViewController.Instance.GetAvailableMeals(user));
                meals_types_TF.text = string.Format("{0}", ResourceViewController.Instance.GetMealTypesAmount(user));
                salary_TF.text = string.Format("{0:0.0}", ResourceViewController.Instance.GetDailySalary(user));
                maintenance_TF.text = string.Format("{0:0.0}", ResourceViewController.Instance.GetDailyMaintenance(user));
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
        */
    }

    public void Back()
    {
        gameObject.SetActive(false);
    }

    public void SetUser(LUser user)
    {
        this.user = user;
        Initialize();
    }

    private string GetValue(EResources resKey)
    {
        return string.Format("{0}", (int)ResourceViewController.Instance.GetResourceValue(resKey, resources));
    }

    private string GetValue(List<EResources> resKeys)
    {
        float resValue = 0;
        foreach (EResources resKey in resKeys)
        {
            resValue += ResourceViewController.Instance.GetResourceValue(resKey, resources);
        }

        return string.Format("{0}", (int)resValue);
    }
    #endregion
}
