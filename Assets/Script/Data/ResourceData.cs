using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class ResourceData : ScriptableObject
{
	[FormerlySerializedAs("Religions")]
	public List<CReligion> religions;

    public string GetTemplePrefabName(string str)
    {
        foreach(CReligion item in religions)
        {
            if (item.name == str)
            {
                return item.prefabName;
            }
        }

        return "";
    }

    public string GetTempleBuildingName(string str)
    {
        foreach (CReligion item in religions)
        {
            if (item.name == str)
            {
                return item.buildingInfo;
            }
        }

        return "Temple & Church";
    }
}