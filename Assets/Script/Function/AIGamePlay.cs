using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AIGamePlay : SingletonComponent<AIGamePlay>
{
    [HideInInspector]
    private Dictionary<DayOfWeek, int> bonusBuildingsForVegetarin = new Dictionary<DayOfWeek, int>() {
        {DayOfWeek.Sunday, 53 },
        {DayOfWeek.Monday, 81 },
        {DayOfWeek.Tuesday, 52 },
        {DayOfWeek.Wednesday, 63 },
        {DayOfWeek.Thursday, 53 },
        {DayOfWeek.Friday, 81 },
        {DayOfWeek.Saturday, 41 }
    };

    [HideInInspector]
    public Dictionary<DayOfWeek, int> bonusBuildingsForNotVegetarin = new Dictionary<DayOfWeek, int>() {
        {DayOfWeek.Sunday, 53 },
        {DayOfWeek.Monday, 81 },
        {DayOfWeek.Tuesday, 52 },
        {DayOfWeek.Wednesday, 63 },
        {DayOfWeek.Thursday, 62 },
        {DayOfWeek.Friday, 62 },
        {DayOfWeek.Saturday, 41 }
    };

    [HideInInspector]
    public Dictionary<DayOfWeek, int> bonusMineBuildings = new Dictionary<DayOfWeek, int>() {
        {DayOfWeek.Sunday, 249 },
        {DayOfWeek.Monday, 249 },
        {DayOfWeek.Tuesday, 251 },
        {DayOfWeek.Wednesday, 251 },
        {DayOfWeek.Thursday, 250 },
        {DayOfWeek.Friday, 252 },
        {DayOfWeek.Saturday, 251 }
    };
    LUser currentUser = null;

    private void Start()
    {
        currentUser = DataManager.Instance.GetCurrentUser();
        OnConstruct();
        //    ResourceViewController.Instance.UpdateResource(EResources.Stone.ToString(), 500, (isSucess, errMsg) =>
        //    {
        //        Debug.LogError(isSucess);
        //    });
    }

    public void OnConstruct()
    {
        var currentBuildingList = DataManager.Instance.CurrentAutoBuildings.ToList();

        if (currentBuildingList.Count == 21)
        {
            //complete to construct all buildings
            return;
        }
        for (int i = 0; i < 2; i++)
        {
            var schedule = OnSelectSchedule();
            
            if (schedule == null)
            {
                continue;
            }
            
            if (schedule.id == "5" || schedule.id == "12")
            {
                var day = Convert.FDateToDateTime(currentUser.mode_at).DayOfWeek;
                if (currentUser.isVegetarian)
                {
                    schedule.bID = bonusBuildingsForVegetarin[day];
                }
                else
                {
                    schedule.bID = bonusBuildingsForNotVegetarin[day];
                }
            }

            if (schedule.id == "17")
            {
                var day = Convert.FDateToDateTime(currentUser.mode_at).DayOfWeek;
                schedule.bID = bonusMineBuildings[day];
            }

            BuildManager.Instance.StartToBuild(schedule);
        }
    }

    public void CompleteBuild(string bID)
    {
        var scheduleList = DataManager.Instance.Schedule_Data.schedules.OrderBy(item => item.id);
        foreach (CSchedule schedule in scheduleList)
        {
            if (schedule.bID.ToString() == bID)
            {
                DataManager.Instance.UpdateAutoBuilding(new LAutoBuilding(schedule.id, true));
                break;
            }
        }

        OnConstruct();
    }

    private CSchedule OnSelectSchedule()
    {
        var currentBuildingList = DataManager.Instance.CurrentAutoBuildings.Select(item => item.id).ToList();
        var completedBuildingList = DataManager.Instance.CurrentAutoBuildings.FindAll(item => item.isCompleted).Select(item => item.id).ToList();

        var scheduleList = DataManager.Instance.Schedule_Data.schedules.OrderBy(item => item.id);
        var days = currentUser.GetAgesAsDays();

        var gold = ResourceViewController.Instance.GetCurrentResourceValue(EResources.Gold);
        var lumber = ResourceViewController.Instance.GetCurrentResourceValue(EResources.Lumber);
        var stone = ResourceViewController.Instance.GetCurrentResourceValue(EResources.Stone);
        var iron = ResourceViewController.Instance.GetCurrentResourceValue(EResources.Iron);
        foreach (CSchedule schedule in scheduleList)
        {
            if (days < schedule.day)
            {
                //UIManager.LogError(schedule.id + ":Dayout");
                continue;
            }

            var gBuilding = BuildManager.Instance.GetGBuilding(schedule.bID.ToString());
            if (gBuilding == null || gBuilding.Lbuilding == null)
            {
                continue;
            }

            var cBuilding = ResourceViewController.Instance.GetCBuilding(gBuilding.Lbuilding.id);
            if (cBuilding == null)
            {
                continue;
            }

            if (schedule.id == "21")
            {
                if (UserViewController.Instance.GetCurrentUser().hasReligion == false)//for the temple
                {
                    continue;
                }
            }

            if (schedule.id == "19")//iron forge
            {
                var ironMine = ResourceViewController.Instance.GetCurrentBuildings().Find(item => item.bID == "249");
                if (ironMine == null)
                {
                    continue;
                }
            }

            if (gold < cBuilding.goldAmount || lumber < cBuilding.lumberAmount || stone < cBuilding.stoneAmount || iron < cBuilding.ironAmount)
            {
                //UIManager.LogError(schedule.id + ":NotEnoughResource");
                continue;
            }
            
            if (currentBuildingList.Contains(schedule.id))
            {
                //UIManager.LogError(schedule.id + ":is Being Built");
                continue;
            }

            var preBuilt = true;
            foreach(string id in schedule.preCompleteIds)
            {
                if (!completedBuildingList.Contains(id))
                {
                    preBuilt = false;
                    break;
                }
            }
            if (preBuilt)
            {
                return schedule;
            }
        }
        return null;
    }
}
