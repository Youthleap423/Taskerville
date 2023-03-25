using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class VillageData : ScriptableObject
{
	[FormerlySerializedAs("Villager")]
    public List<CVillager> villagers;
}