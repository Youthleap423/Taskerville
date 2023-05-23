using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerInn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CheckDatesForVillager();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (true)
        {
            CheckDatesForVillager();
        }
    }

    private void CheckDatesForVillager()
    {
        var guests = new List<LVillager>();
        var gBuilding = GetComponent<GBuilding>();
        var workerIDs = new List<string>();
        foreach(LVillager villager in gBuilding.GetWorkers())
        {
            workerIDs.Add(villager.UID);
        }

        foreach(LVillager villager in gBuilding.GetOccupants())
        {
            if (!workerIDs.Contains(villager.UID))
            {
                guests.Add(villager);
            }
        }

        foreach(LVillager villager in guests)
        {
            if (villager.id == "36" || villager.id == "33")
            {
                continue;
            }
            var cVillager = ResourceViewController.Instance.GetCVillager(villager.id);
            var createDate = Convert.FDateToDateTime(villager.created_at);
            var days = (int)Utilities.GetDays(createDate);
            if (days > cVillager.stay_dates)
            {
                //leave
                BuildManager.Instance.RemoveVillager(villager);
                UIManager.Instance.ShowErrorDlg(cVillager.name + " has left your village in disgust due to lack of housing.");
            }
        }
    }
}
