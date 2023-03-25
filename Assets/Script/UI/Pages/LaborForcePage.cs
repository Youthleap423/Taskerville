using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LaborForcePage : Page
{
    [SerializeField] private GameObject uiItemPrefab;
    [SerializeField] private Transform ListGroup;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Text priceTF;
    [SerializeField] private CanvasGroup  adamantine_CG;
    [SerializeField] private GameObject notifyDlg;

    private List<CVillager> buyableVillagers = new List<CVillager>();
    private CVillager selectedVillager = null;

    private void Start()
    {
        dropdown.onValueChanged.AddListener(delegate
        {
            SelectVillager(dropdown);
        });
    }

    private void OnEnable()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();

        if (adamantine_CG != null)
        {
            if (AppManager.Instance.GetCurrentMode() == Game_Mode.Task_Only)
            {
                adamantine_CG.alpha = 0.5f;
                adamantine_CG.blocksRaycasts = false;
                adamantine_CG.interactable = false;
            }
            else
            {
                adamantine_CG.alpha = 1.0f;
                adamantine_CG.blocksRaycasts = true;
                adamantine_CG.interactable = true;
            }
        }

        foreach (Transform child in ListGroup.transform)
        {
            Destroy(child.gameObject);
        }

        Dictionary<string, int> villagerCountDic = ResourceViewController.Instance.GetCurrentVillgerDictionary();

        //select labor group member
        List<string> smallNameGroup = new List<string>();
        List<float> smallValueGroup = new List<float>();
        List<EVillagerType> laborTypeGroup = DataManager.Instance.laborTypeGroup;
        foreach (string id in villagerCountDic.Keys)
        {
            CVillager villager = ResourceViewController.Instance.GetCVillager(id);
         
            if (laborTypeGroup.Contains(villager.type))
            {
                smallNameGroup.Add(villager.name);
                smallValueGroup.Add(villagerCountDic[id]);
                if (smallNameGroup.Count == 2)
                {
                    CreateNewLine(smallNameGroup, smallValueGroup);
                    smallNameGroup.Clear();
                    smallValueGroup.Clear();
                }
            }
        }

        if (smallNameGroup.Count > 0)
        {
            CreateNewLine(smallNameGroup, smallValueGroup);
        }
        //select guard group member
        CreateEmptyLine();

        List<EVillagerType> guardTypeGroup = DataManager.Instance.guardTypeGroup;
        smallNameGroup.Clear();
        smallValueGroup.Clear();
  
        foreach (string id in villagerCountDic.Keys)
        {
            CVillager villager = ResourceViewController.Instance.GetCVillager(id);
            if (guardTypeGroup.Contains(villager.type))
            {              
                smallNameGroup.Add(villager.name);
                smallValueGroup.Add(villagerCountDic[id]);
                if (smallNameGroup.Count == 2)
                {
                    CreateNewLine(smallNameGroup, smallValueGroup);
                    smallNameGroup.Clear();
                    smallValueGroup.Clear();
                }
            }
        }

        if (smallNameGroup.Count > 0)
        {
            CreateNewLine(smallNameGroup, smallValueGroup);
        }

        var user = UserViewController.Instance.GetCurrentUser();
        buyableVillagers = ResourceViewController.Instance.GetHirelVillagers();
        if (user.isVegetarian)
        {
            var butcher = buyableVillagers.Find(item => item.id == "19");
            var fisherman = buyableVillagers.Find(item => item.id == "17");
            var rancher = buyableVillagers.Find(item => item.id == "16");
            buyableVillagers.Remove(butcher);
            buyableVillagers.Remove(fisherman);
            buyableVillagers.Remove(rancher);
        }

        if (!user.hasReligion)
        {
            var philosophy = buyableVillagers.Find(item => item.id == "18");
            buyableVillagers.Remove(philosophy);
        }

        dropdown.options.Clear();
        foreach (CVillager villager in buyableVillagers)
        {
            if (villager.name != "Commander")
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(villager.name));
            }
        }

        dropdown.Select();
        SelectVillager(dropdown);
    }

    public void Hire()
    {
        if (selectedVillager != null)
        {
            if (!ResourceViewController.Instance.builtBuildingFor(selectedVillager))
            {
                notifyDlg.SetActive(true);
            }
            
        }
        
    }

    public void OnProceed()
    {
        notifyDlg.SetActive(false);
        ResourceViewController.Instance.HireVillager(selectedVillager, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                Initialize();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }

        });
        
    }

    public void OnNevermind()
    {
        notifyDlg.SetActive(false);
    }

    private void CreateNewLine(List<string> nameList, List<float> valueList)
    {
        GameObject lineObj = GameObject.Instantiate(uiItemPrefab, ListGroup);
        GameObject leftObj = lineObj.transform.Find("left").gameObject;
        if (leftObj != null && nameList.Count > 0)
        {
            leftObj.GetComponent<Text>().text = GetFormattedString(nameList[0], valueList[0]);
        }

        GameObject rightObj = lineObj.transform.Find("right").gameObject;
        if (rightObj != null && nameList.Count == 2)
        {
            rightObj.GetComponent<Text>().text = GetFormattedString(nameList[1], valueList[1]);
        }
    }

    private void CreateEmptyLine()
    {
        GameObject.Instantiate(uiItemPrefab, ListGroup);
    }

    private string GetFormattedString(string name, float value)
    {
        return string.Format("{0} {1}", value, value > 1 ? Utilities.GetPluralWord(name) : name);
    }

    private void SelectVillager(TMP_Dropdown dropdown)
    {
        int index = dropdown.value;
        selectedVillager = buyableVillagers[index];
        priceTF.text = string.Format("{0} gold", (int)selectedVillager.hire_price);
    }
}
