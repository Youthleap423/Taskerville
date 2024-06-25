using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CBuilding: CData
{
    //public float maintenance_amount = 0f;
    //public string maintenance_date = "";
    //public float init_amount = 0f;
    //public float villager_amout = 0f;
    //public List<string> currentVillagers = new List<string>();
    //public float happiness = 100f;
    //public float health = 100f;
    //public float posX = 0f;
    //public float posY = 0f;
    //public float posZ = 0f;
    //public float progress = 0f;
    //public int asset_id = 0;
    //public int building_id = 0;
    //public string build_at = "";
    //public bool isRemovable = false;
    public EBuildingType type = EBuildingType.AdminOffice;
    public float maintenance_amount = 0.0f;
    public string description = "";
    public float goldAmount = 0.0f;
    public float lumberAmount = 0.0f;
    public float stoneAmount = 0.0f;
    public float ironAmount = 0.0f;
    public float timeToBuild = 0.0f;
    public float QTimeToBuild = 1.0f;
    public int culturePoint = 0;
    public EResources QResType = EResources.Ruby;
    public float QResAmount = 1.0f;
    public string IdOfStoreIcon = "";
    public bool isStatic = false;
    public EConstructionDlgType requirement = EConstructionDlgType.NoBuilder;
    public List<EVillagerType> specialVillagers = new List<EVillagerType>();
    public List<EVillagerType> requireVillagers = new List<EVillagerType>();
    public List<LProduct> products = new List<LProduct>();
    public List<LProduct> requires = new List<LProduct>();
}
