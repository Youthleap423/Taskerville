using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : SingletonComponent<CharacterManager>
{
    [SerializeField] private GameObject fisher;
    [SerializeField] private GameObject woodcutter;
    [SerializeField] private GameObject appleFarmer;

    // Start is called before the first frame update
    override protected void Awake()
    {
        base.Awake();

        fisher.SetActive(false);
        woodcutter.SetActive(false);
        appleFarmer.SetActive(false);
    }

    private void CheckAppleFarmer(LVillager villager)
    {
        if (villager.work_at == 181)
        {
            appleFarmer.SetActive(true);
        }
    }

    private void CheckFisher(LVillager villager)
    {
        fisher.SetActive(true);
    }

    private void CheckWoodCutter(LVillager villager)
    {
        woodcutter.SetActive(true);
    }

    public void CheckVillagers()
    {
        var villagers = ResourceViewController.Instance.GetCurrentVillagers();
        foreach(LVillager villager in villagers)
        {
            if (villager.id == "9")//Farmer
            {
                CheckAppleFarmer(villager);
            }

            if (villager.id == "10")//Woodcutter
            {
                CheckWoodCutter(villager);
            }

            if (villager.id == "17")//Fisher
            {
                CheckFisher(villager);
            }
        }
    }
}
