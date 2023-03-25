using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasuryPage : Page
{
    [SerializeField] private Text bankName_TF;
    [SerializeField] private Text gold_TF;

    [Header("Gems")]
    [SerializeField] private Text sapphire_TF;
    [SerializeField] private Text ruby_TF;
    [SerializeField] private Text diamond_TF;
    
    [Header("Revenue")]
    [SerializeField] private Text adamantine_TF;
    [SerializeField] private Text task_TF;
    [SerializeField] private Text sold_TF;
    [SerializeField] private Text tRevenue_TF;

    [Header("Excursion")]
    //[SerializeField] private Text adamantine_TF;
    [SerializeField] private Text vMaintenance_TF;
    [SerializeField] private Text excursion_TF;
    [SerializeField] private Text buy_TF;
    [SerializeField] private Text specialist_TF;
    [SerializeField] private Text tExcursion_TF;
    // Start is called before the first frame update
    private void OnEnable()
    {
        Initialize();
    }


    public override void Initialize()
    {
        base.Initialize();

        bankName_TF.text = string.Format("Bank of {0}", UserViewController.Instance.GetCurrentUser().Village_Name);

        sapphire_TF.text = string.Format("{0}", (int)ResourceViewController.Instance.GetCurrentResourceValue(EResources.Sapphire));
        ruby_TF.text = string.Format("{0}", (int)ResourceViewController.Instance.GetCurrentResourceValue(EResources.Ruby));
        diamond_TF.text = string.Format("{0}", (int)ResourceViewController.Instance.GetCurrentResourceValue(EResources.Diamond));

        gold_TF.text = string.Format("{0:0.0}", ResourceViewController.Instance.GetCurrentResourceValue(EResources.Gold));
        adamantine_TF.text = string.Format("{0:0.#}", ResourceViewController.Instance.GetGoldFromHappiness());
        sold_TF.text = string.Format("{0:0.#}", Mathf.Abs(UserViewController.Instance.GetCurrentUser().GetSales(System.DateTime.Now)));
        
        var taskList = TaskViewController.Instance.GetDailyTasks(System.DateTime.Now);
        var amount = 0.0f;
        foreach(LTaskEntry fTaskEntry in taskList)
        {
            amount += fTaskEntry.goldCount;
            amount += fTaskEntry.subTasks.Count;
        }
        this.task_TF.text = string.Format("{0:0.#}", amount);
        var totalRevenue = float.Parse(adamantine_TF.text) + float.Parse(sold_TF.text) + float.Parse(task_TF.text);
        tRevenue_TF.text = string.Format("{0:0.#}", totalRevenue);
            

        excursion_TF.text = string.Format("{0:0.#}", 0.0f);
        buy_TF.text = string.Format("{0:0.#}", Mathf.Abs(UserViewController.Instance.GetCurrentUser().GetBuy(System.DateTime.Now)));
        vMaintenance_TF.text = string.Format("{0:0.#}", ResourceViewController.Instance.GetDailyMaintenance());
        specialist_TF.text = string.Format("{0:0.0}", ResourceViewController.Instance.GetDailySalary());
        var totalExcursion = float.Parse(excursion_TF.text) + float.Parse(buy_TF.text) + float.Parse(vMaintenance_TF.text) + float.Parse(specialist_TF.text);
        tExcursion_TF.text = string.Format("{0:0.#}", totalExcursion);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
