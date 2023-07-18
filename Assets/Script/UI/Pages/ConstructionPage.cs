using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIControllersAndData.Store.Categories.Buildings;
using Assets.Scripts.UIControllersAndData.Images;


public class ConstructionPage : Page
{
    [SerializeField] private GameObject normalBuildingUIObj;
    [SerializeField] private GameObject uniqueBuildingUIObj;
    [SerializeField] private GameObject mapEventReceiverObj;

    [Space]
    [SerializeField] private Dropdown dropDown;
    [SerializeField] private Image buildingImage;

    [Header("Available Resource")]
    [SerializeField] private Text goldATF;
    [SerializeField] private Text lumberATF;
    [SerializeField] private Text stoneATF;
    [SerializeField] private Text ironATF;

    [Header("Demand Resource")]
    [SerializeField] private Text goldDTF;
    [SerializeField] private Text lumberDTF;
    [SerializeField] private Text stoneDTF;
    [SerializeField] private Text ironDTF;

    [Header("Dialogs")]
    [SerializeField] private GameObject NoBuilderDlg;
    [SerializeField] private GameObject AskSpecialistDlg;
    [SerializeField] private GameObject AskCuratorDlg;
    [SerializeField] private GameObject ImmediateBuildDlg;

    [Header("Unique")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Dropdown uniqueDropDown;

    private List<BuildingsCategory> buildingsCategories = new List<BuildingsCategory>();
    private List<BuildingsCategory> uniqueBuildingsCategories = new List<BuildingsCategory>();
    private BuildingsCategory selectedCategory = null;
    private List<GameObject> buildingConstructionList = new List<GameObject>();
    private List<BuildingCreator> buildingCreatorList = new List<BuildingCreator>();
    private int selectedUniqueBuildingIndex = 0;
    #region Unity Members
    // Start is called before the first frame update

    void Start()
    {
        CloseBuildingObjUIs();

        dropDown.onValueChanged.AddListener(delegate
        {
            SelectBuilding(dropDown);

        });

        uniqueDropDown.onValueChanged.AddListener(delegate
        {
            SelectUniqueBuilding(uniqueDropDown);
        });
    }

    public void NoBuilderDlg_buttonPressedAction(bool arg2)
    {
        NoBuilderDlg.SetActive(false);
        OnDialogAction(EConstructionDlgType.NoBuilder, arg2);
    }

    public void AskSpecialistDlg_buttonPressedAction(bool arg2)
    {
        AskSpecialistDlg.SetActive(false);
        OnDialogAction(EConstructionDlgType.AskSpecialist, arg2);
    }

    public void AskCuratorDlg_buttonPressedAction(bool arg2)
    {
        AskCuratorDlg.SetActive(false);
        OnDialogAction(EConstructionDlgType.AskCurator, arg2);
    }
    public void ImmediateBuildDlg_buttonPressedAction(bool arg2)
    {
        ImmediateBuildDlg.SetActive(false);
        OnDialogAction(EConstructionDlgType.ImmediateBuild, arg2);
    }

    public void ShowNormalBuildingObjUI()
    {
        if (normalBuildingUIObj.activeSelf == true)
        {
            CloseBuildingObjUIs();
        }
        else
        {
            normalBuildingUIObj.SetActive(true);
            uniqueBuildingUIObj.SetActive(false);
            mapEventReceiverObj.SetActive(true);
            Initialize();
            CameraController.Instance.movingBuilding = false;
        }
        
    }

    public void ShowUniqueBuildingObjUI()
    {
        if (uniqueBuildingUIObj.activeSelf == true)
        {
            CloseBuildingObjUIs();
        }
        else
        {
            normalBuildingUIObj.SetActive(false);
            uniqueBuildingUIObj.SetActive(true);
            mapEventReceiverObj.SetActive(true);
            Initialize();
            CameraController.Instance.movingBuilding = false;
        }
    }

    public void CloseConstructionMenu()
    {
        CloseBuildingObjUIs();
    }

    private void CloseBuildingObjUIs()
    {
        normalBuildingUIObj.SetActive(false);
        uniqueBuildingUIObj.SetActive(false);
        mapEventReceiverObj.SetActive(false);
        CameraController.Instance.movingBuilding = true;
    }
    private void OnDialogAction(EConstructionDlgType arg1, bool arg2)
    {
        switch (arg1)
        {
            case EConstructionDlgType.NoBuilder:
                break;
            case EConstructionDlgType.AskSpecialist:
                if (arg2)
                {
                    ShowImmediateDlg();
                    
                }
                //else
                //{
                //    PlaceOnMap();
                //}
                break;
            case EConstructionDlgType.AskCurator:
                if (arg2)
                {
                    ShowImmediateDlg();
                }
                else
                {
                    PlaceOnMap();
                }
                break;
            case EConstructionDlgType.ImmediateBuild:
                if (arg2)
                {
                    BuildManager.Instance.QuickBuild = true;
                }
                PlaceOnMap();
                break;
            default:
                break;
        }
    }

    private void ShowImmediateDlg()
    {
        ImmediateBuildDlg.GetComponent<ConstructionDlg>().SetType(selectedCategory.QResType);
        ImmediateBuildDlg.SetActive(true);
    }
    private void OnDestroy()
    {
        //AppManager.Instance.updateConstruction -= Instance_updateConstruction;
    }

   
    #endregion

    #region Public Members
    public void ShowSpeedBuild()
    {
        transform.parent.GetComponent<NavPage>().Show("speed_build");
    }

    public override void Initialize()
    {
        base.Initialize();

        if (ResourceViewController.Instance.GetCurrentResourceValue(EResources.Culture) >= 40f)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        goldATF.text = string.Format("{0}", (int)ResourceViewController.Instance.GetCurrentResourceValue(EResources.Gold));
        lumberATF.text = string.Format("{0}", (int)ResourceViewController.Instance.GetCurrentResourceValue(EResources.Lumber));
        stoneATF.text = string.Format("{0}", (int)ResourceViewController.Instance.GetCurrentResourceValue(EResources.Stone));
        ironATF.text = string.Format("{0}", (int)ResourceViewController.Instance.GetCurrentResourceValue(EResources.Iron));

        var user = UserViewController.Instance.GetCurrentUser();

        buildingsCategories = DataManager.Instance.BuildingsCategoryData.category.FindAll(item => item.isStatic == false).OrderBy(item => item.GetName()).ToList();

        
        var dayOfWeek = Convert.FDateToDateTime(user.mode_at).DayOfWeek;
        var bonusList = user.isVegetarian ? DataManager.Instance.bonusBuildingsForVegetarin[dayOfWeek] : DataManager.Instance.bonusBuildingsForNotVegetarin[dayOfWeek];
        buildingsCategories.RemoveAll(item => DataManager.Instance.rareBuildings.Contains(item.id) && !bonusList.Contains(item.id));

        if (user.hasReligion == false)
        {
            var temple = buildingsCategories.Find(item => item.id == 55);
            buildingsCategories.Remove(temple);
        }

        if (user.isVegetarian == true)
        {
            var ranch = buildingsCategories.Find(item => item.id == 1);
            var fishercabin = buildingsCategories.Find(item => item.id == 19);
            var butcher = buildingsCategories.Find(item => item.id == 18);
            buildingsCategories.Remove(ranch);
            buildingsCategories.Remove(fishercabin);
            buildingsCategories.Remove(butcher);
        }

        buildingsCategories.RemoveAll(item => BuildManager.Instance.hasExistBuildingToConstruct(item.GetId()) == false);//2023/05/24 by pooh - remove item that all contructed 
        dropDown.options.Clear();

        foreach (BuildingsCategory item in buildingsCategories)
        {
            dropDown.options.Add(new Dropdown.OptionData(item.GetName()));
        }

        if (normalBuildingUIObj.activeSelf == true)
        {
            if (dropDown.options.Count > 0)
            {
                dropDown.value = 0;
                dropDown.RefreshShownValue();
                this.SelectBuilding(0);
            }
        }
        

        uniqueDropDown.options.Clear();
        uniqueBuildingsCategories = DataManager.Instance.BuildingsCategoryData.category.FindAll(item => item.type == EBuildingType.Unique).OrderBy(item => item.GetName()).ToList();
        uniqueBuildingsCategories.RemoveAll(item => BuildManager.Instance.hasExistBuildingToConstruct(item.GetId()) == false);//2023/05/24 by pooh - remove item that all contructed 
        foreach (var category in uniqueBuildingsCategories)
        {
            uniqueDropDown.options.Add(new Dropdown.OptionData(category.GetName()));
        }

        if (uniqueBuildingUIObj.activeSelf == true)
        {
            if (uniqueDropDown.options.Count > 0)
            {
                uniqueDropDown.value = 0;
                uniqueDropDown.RefreshShownValue();
                SelectUniqueBuilding(0);
            }
        }
    }

    public void OnConstruct()
    {
        BuildManager.Instance.QuickBuild = false;
        
        if (!BuildManager.Instance.AnyFreeBuilder())
        {
            NoBuilderDlg.SetActive(true);
        }
        else
        {
            if (selectedCategory.requirement == EConstructionDlgType.AskCurator)
            {
                AskCuratorDlg.SetActive(true);
            }
            else if (selectedCategory.requirement == EConstructionDlgType.AskSpecialist)
            {
                AskSpecialistDlg.SetActive(true);
            }
            else
            {
                if (BuildManager.Instance.IsEnoughResource(selectedCategory, true) == false)
                {
                    ShowImmediateDlg();
                }
                else
                {
                    PlaceOnMap();
                }
            }
        }
    }

    public void PlaceOnMap()
    {
        CloseBuildingObjUIs();
        if (selectedCategory != null)
        {
            BuildManager.Instance.SelectPosition(selectedCategory, (isSuccess, err) => {
                if (!isSuccess)
                {
                    UIManager.Instance.ShowErrorDlg(err);
                }
            });
        }
    }

    public void PlaceUniqueOnMap()
    {
        CloseBuildingObjUIs();
        if (selectedCategory != null)
        {
            BuildManager.Instance.StartToBuild(selectedCategory, selectedUniqueBuildingIndex, (isSuccess, errMsg) =>
            {
                if (!isSuccess)
                {
                    UIManager.Instance.ShowErrorDlg(errMsg);
                }
            });
        }
    }
    #endregion

    private void SelectBuilding(Dropdown dp)
    {
        SelectBuilding(dp.value);
    }

    private void SelectUniqueBuilding(Dropdown dp)
    {
        SelectUniqueBuilding(dp.value);
    }

    private void SelectBuilding(int index)
    {
        var category = buildingsCategories[index];
        selectedCategory = category;
        //var storeImage = ImageControler.GetImage(category.IdOfStoreIcon);
        //buildingImage.sprite = storeImage;


        goldDTF.text = string.Format("{0}", category.GoldAmount);
        lumberDTF.text = string.Format("{0}", category.LumberAmount);
        stoneDTF.text = string.Format("{0}", category.StoneAmount);
        ironDTF.text = string.Format("{0}", category.IronAmount);
    }

    private void SelectUniqueBuilding(int index)
    {
        var category = uniqueBuildingsCategories[index];
        selectedCategory = category;
        selectedUniqueBuildingIndex = index;
    }

}
