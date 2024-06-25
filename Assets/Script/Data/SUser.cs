using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SUser
{
    public string Id = "";
    public string User = "";
    public string Setting = "";
    public string DailyTask = "";
    public string ToDo = "";
    public string Project = "";
    public string SubTask = "";
    public string Building = "";
    public string Resource = "";
    public string Villager = "";
    public string AutoBuilding = "";
    public string AutoToDo = "";
    public string AutoGoal = "";
    public string Habit = "";
    public string Artifact = "";
    public string Artwork = "";
    public string created_at = "";
    public string updated_at = "";

    public SUser(FUser fUser)
    {
        this.Id = fUser.id;
        this.User = JsonUtility.ToJson(fUser.user);
        this.Setting = JsonUtility.ToJson(fUser.setting);
        this.DailyTask = JsonHelper.ToJson(fUser.dailyTask.ToArray());
        this.ToDo = JsonHelper.ToJson(fUser.toDo.ToArray());
        this.Project = JsonHelper.ToJson(fUser.project.ToArray());
        this.SubTask = JsonHelper.ToJson(fUser.subTask.ToArray());
        this.Building = JsonHelper.ToJson(fUser.building.ToArray());
        this.Resource = JsonHelper.ToJson(fUser.resource.ToArray());
        this.Villager = JsonHelper.ToJson(fUser.villager.ToArray());
        this.AutoBuilding = JsonHelper.ToJson(fUser.autoBuilding.ToArray());
        this.AutoToDo = JsonHelper.ToJson(fUser.autoToDo.ToArray());
        this.AutoGoal = JsonHelper.ToJson(fUser.autoGoal.ToArray());
        this.Habit = JsonHelper.ToJson(fUser.habit.ToArray());
        this.Artifact = JsonHelper.ToJson(fUser.artifact.ToArray());
        this.Artwork = JsonHelper.ToJson(fUser.artwork.ToArray());
        this.created_at = fUser.created_at;
        this.updated_at = fUser.updated_at;
    }
}