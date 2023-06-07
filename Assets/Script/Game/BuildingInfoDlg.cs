using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UIControllersAndData.Store;
using TMPro;

/**
 * Basic class for panel to upgrade the selected panel
 */
public class BuildingInfoDlg : MonoBehaviour
{
    public static BuildingInfoDlg Instance;

    [SerializeField] private GameObject _buildingInfoPanel;
    [SerializeField] private GameObject _quickBuildPanel;
    [SerializeField] private TMP_Text titleTF;
    [SerializeField] private TMP_Text quickTitleTF;
    [SerializeField] private TMP_Text descTF;
    [SerializeField] private Button demolishBtn;
    [SerializeField] private TMP_Text saveTF;
    private string _msg = "Would you like to upgrade the {0} to the {1} level. It costs {2} {3}.";

    private GameObject _buildingObj;
    private (string, List<EVillagerType>) tHireVillager = ("", new List<EVillagerType>());

    /**
     * Awake is called before the Start
     */
    void Awake()
    {
        Instance = this;

//#if UNITY_EDITOR
//        //saveTF.gameObject.SetActive(true);
//        demolishBtn.gameObject.SetActive(true);
//#endif
    }

    /**
     * Sets info to upgrade the selected building
     */
    public void SetInfo(GameObject obj)
    {
        GBuilding gBuilding = obj.GetComponent<GBuilding>();
        LBuilding lBuilding = gBuilding.Lbuilding;

        titleTF.text = gBuilding.GetName();
        quickTitleTF.text = gBuilding.GetName();

        if (gBuilding.Category != null)
        {
            if (gBuilding.GetBuildingObjectID == "100") // Temple
            {
                titleTF.text = DataManager.Instance.GetTempleBuildingName();
            }
            else
            {
                titleTF.text = gBuilding.Category.GetName();
            }
            
        }

        tHireVillager = gBuilding.NeedToHireSpecialist();

        if (tHireVillager.Item1 == "")
        {
            demolishBtn.gameObject.SetActive(false);
        }
        else
        {
            demolishBtn.gameObject.SetActive(true);
            demolishBtn.transform.GetComponentInChildren<TMP_Text>().text = tHireVillager.Item1;
        }
        
        if (AppManager.Instance.GetCurrentMode() == Game_Mode.Task_Only)
        {
            demolishBtn.gameObject.SetActive(false);
        }

        var str = "";
        switch (lBuilding.id)
        {
            case "33"://Castle
                str = "Workers & Occupants: \n\t 1 Mayor \n\t 1 Administrator \n\t 2 Cabinet Members \n\t 3 Spouses \n\t 3 Children \n\t 4 Castle Servants";
                break;
            case "37"://Barracks
                str = "Occupants & Defense Force:" + MakeString(gBuilding.GetWorkers()); 
                break;
            case "19":
            case "20":
            case "55":
            case "58":
                str = "Workers & Occupants:" + MakeString(gBuilding.GetWorkers());
                break;
            case "64":
            case "24":
                str = MakeString(gBuilding.GetWorkers(), gBuilding.GetOccupants(), gBuilding.GetBuildingObjectID);
                break;
            default:
                str = MakeString(gBuilding.GetWorkers(), gBuilding.GetOccupants());
                break;
        }
        descTF.text = str;
        _buildingObj = obj;
        if (lBuilding.progress < 0.99f)
        {
            if (gBuilding.QuickBuild == false)
            {
                _quickBuildPanel.SetActive(true);
            }
        }
        else
        {
            _buildingInfoPanel.SetActive(true);
        }
    }

    private string MakeString(List<LVillager> workers)
    {
        var dic = new Dictionary<string, int>();
        foreach(LVillager lVillager in workers)
        {
            var key = lVillager.id;
            if (dic.ContainsKey(lVillager.id))
            {
                dic[key]++;
            }
            else
            {
                dic[key] = 1;
            }
        }
        var result = "";
        
        foreach (string id in dic.Keys)
        {
            var cVillager = ResourceViewController.Instance.GetCVillager(id);
            if (cVillager != null)
            {
                result += "\n\t " + dic[id].ToString();
                if (dic[id] > 1)
                {
                    result += " " + Utilities.GetPluralWord(cVillager.name);
                }
                else
                {
                    result += " " + cVillager.name;
                }
            }
        }

        return result;
    }

    private string MakeString(List<LVillager> workers, List<LVillager> occupants, string buildingID)
    {
        var guest = occupants.FindAll(it => it.work_at != int.Parse(buildingID)).ToList();
        occupants = occupants.FindAll(it => it.work_at == int.Parse(buildingID)).ToList();

        var result = "";
        if (occupants.Count > 0)
        {
            result = "Occupants:" + MakeString(occupants) + "\n";
        }

        var dic = new Dictionary<string, int>();
        foreach (LVillager lVillager in occupants)
        {
            workers.RemoveAll(it => it.id == lVillager.id);
        }

        if (workers.Count > 0)
        {
            result += "Workers:" + MakeString(workers) + "\n"; 
        }

        if (guest.Count > 0)
        {
            result += "Guests:" + MakeString(guest);
        }

        return result;
    }

    private string MakeString(List<LVillager> workers, List<LVillager> occupants)
    {
        var result = "";
        if (occupants.Count > 0)
        {
            result = "Occupants:" + MakeString(occupants) + "\n\n";
        }

        var dic = new Dictionary<string, int>();
        foreach (LVillager lVillager in occupants)
        {
            workers.RemoveAll(it => it.id == lVillager.id);
        }

        if (workers.Count > 0)
        {
            result += "Workers:" + MakeString(workers);
        }

        return result;
    }

    [UsedImplicitly]
    public void OkHandler()
    {
        if (_buildingObj != null)
        {
            BuildManager.Instance.HireSpecialist(_buildingObj, tHireVillager.Item2);
            //BuildManager.Instance.CancelBuiling(_buildingObj);
            //TODO - add resource 30% of material

        }

        HidePanel();
    }

    public void DoQuickBuild()
    {
        if (_buildingObj != null)
        {
            BuildManager.Instance.ConvertToQuickBuild(_buildingObj);
        }

        HidePanel();
    }

    public void HidePanel()
    {
        _buildingInfoPanel.SetActive(false);
        _quickBuildPanel.SetActive(false);
    }
}
