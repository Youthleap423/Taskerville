using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class FUser: Data
{
    public string id = "";
    public LUser user = new LUser();
    public LSetting setting = new LSetting();
    public List<LTaskEntry> dailyTask = new List<LTaskEntry>();
    public List<LToDoEntry> toDo = new List<LToDoEntry>();
    public List<LProjectEntry> project = new List<LProjectEntry>();
    public List<LSubTask> subTask = new List<LSubTask>();
    public List<LBuilding> building = new List<LBuilding>();
    public List<LResource> resource = new List<LResource>();
    public List<LVillager> villager = new List<LVillager>();
    public List<LAutoBuilding> autoBuilding = new List<LAutoBuilding>();
    public List<LAutoToDo> autoToDo = new List<LAutoToDo>();
    public List<LAutoGoal> autoGoal = new List<LAutoGoal>();
    public List<LHabitEntry> habit = new List<LHabitEntry>();
    public List<LArtifact> artifact = new List<LArtifact>();
    public List<LArtwork> artwork = new List<LArtwork>();
    public List<LTaskEntry> uncompletedDailyTasks = new List<LTaskEntry>();
    public string created_at { get; set; }
    public string updated_at { get; set; }
    public int timeDiffH = 0;
    public string token = "";

    public FUser()
    { 
        created_at = Convert.DateTimeToFDate(System.DateTime.Now);
        updated_at = "";
    }

    public FUser(string Id)
    {
        this.id = Id;

        created_at = Convert.DateTimeToFDate(System.DateTime.Now);
        updated_at = "";
    }

    public LUser GetLUser()
    {
        return this.user;
    }

    public void SetLUser(LUser user)
    {
        this.user = user;
    }

    public void Serialize(bool bForce = false, System.Action<bool, string> callback = null)
    {
        /*
        var strToday = Convert.DateTimeToFDate(System.DateTime.Now);
        var updated_at = PlayerPrefs.GetString("UpdateTime");
        if (bForce == true || updated_at.Equals("") || string.Compare(updated_at, strToday) < 0)
        {
            this.updated_at = strToday;

            FirestoreManager.Instance.CreateUserData(this, (isSuccess, err) =>
            {
                if (isSuccess)
                {
                    PlayerPrefs.SetString("UpdateTime", strToday);
                }
                callback(isSuccess, err);
            });
        }
        else
        {
            callback(true, "");
        }*/
    }
}
