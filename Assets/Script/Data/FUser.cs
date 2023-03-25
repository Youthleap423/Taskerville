using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

[FirestoreData]
public class FUser
{
    #region Firebase Properties
    [FirestoreProperty]
    public string Id { get; set; }

    [FirestoreProperty]
    public string User { get; set; }

    [FirestoreProperty]
    public string Setting { get; set; }

    [FirestoreProperty]
    public string DailyTask { get; set; }

    [FirestoreProperty]
    public string ToDo { get; set; }

    [FirestoreProperty]
    public string Project { get; set; }

    [FirestoreProperty]
    public string SubTask { get; set; }

    [FirestoreProperty]
    public string Building { get; set; }

    [FirestoreProperty]
    public string Resource { get; set; }

    [FirestoreProperty]
    public string Villager { get; set; }

    [FirestoreProperty]
    public string AutoBuilding { get; set; }

    [FirestoreProperty]
    public string AutoToDo { get; set; }

    [FirestoreProperty]
    public string AutoGoal { get; set; }

    [FirestoreProperty]
    public string Habit { get; set; }

    [FirestoreProperty]
    public string Artifact { get; set; }

    [FirestoreProperty]
    public string Artwork { get; set; }

    [FirestoreProperty]
    public string created_at { get; set; }

    [FirestoreProperty]
    public string updated_at { get; set; }
    //[FirestoreProperty]
    //public string Description { get; set; }
    #endregion

    public FUser()
    {
        Id = "";
        User = "";
        Setting = "";
        DailyTask = "";
        ToDo = "";
        Project = "";
        SubTask = "";
        Building = "";
        Resource = "";
        Villager = "";
        AutoBuilding = "";
        AutoToDo = "";
        AutoGoal = "";
        Habit = "";
        Artifact = "";
        Artwork = "";
        created_at = Convert.DateTimeToFDate(System.DateTime.Now);
        updated_at = "";
    }

    public FUser(string Id)
    {
        this.Id = Id;
        User = "";
        Setting = "";
        DailyTask = "";
        ToDo = "";
        Project = "";
        SubTask = "";
        Building = "";
        Resource = "";
        Villager = "";
        AutoBuilding = "";
        AutoToDo = "";
        AutoGoal = "";
        Habit = "";
        Artifact = "";
        Artwork = "";
        created_at = Convert.DateTimeToFDate(System.DateTime.Now);
        updated_at = "";
    }

    public void Update(string id, string user, string setting, string building, string villager, string resource,  string autoBuidling,string dailyTask, string todo, string project, string subtask, string habit, string autoToDo, string autoGoal, string artifact = "", string artwork = "")
    {
        Id = id;
        User = user;
        Setting = setting;
        Building = building;
        Villager = villager;
        AutoBuilding = autoBuidling;
        Resource = resource;
        DailyTask = dailyTask;
        ToDo = todo;
        Project = project;
        SubTask = subtask;
        Habit = habit;
        Artifact = artifact;
        Artwork = artwork;
        this.AutoToDo = autoToDo;
        this.AutoGoal = autoGoal;
    }

    public LUser GetLUser()
    {
        return JsonConvert.DeserializeObject<LUser>(this.User);
    }

    public void SetLUser(LUser user)
    {
        this.User = JsonConvert.SerializeObject(user);
    }

    public void Serialize(bool bForce = false, System.Action<bool, string> callback = null)
    {
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
        }
    }
}
