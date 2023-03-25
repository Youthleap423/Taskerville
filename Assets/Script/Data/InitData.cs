using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class InitData : ScriptableObject
{
	public List<LResource> resources;
	public List<LBuilding> buildings;
	public List<LVillager> villagers;
	public List<LTaskEntry> dailyTasks;
	public List<LTask> subTasks;
	public List<LAutoToDo> todos;
	public List<LHabitEntry> habits;
	public List<LAutoGoal> goals;
}
