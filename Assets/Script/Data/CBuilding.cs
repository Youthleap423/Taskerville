using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CBuilding: CData
{
    public float maintenance_amount = 0f;
    public string maintenance_date = "";
    public float init_amount = 0f;
    public float villager_amout = 0f;
    public List<string> currentVillagers = new List<string>();
    public float happiness = 100f;
    public float health = 100f;
    public float posX = 0f;
    public float posY = 0f;
    public float posZ = 0f;
    public float progress = 0f;
    public int asset_id = 0;
    public int building_id = 0;
    public string build_at = "";
    public bool isRemovable = false;

}
