using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[FirestoreData]
public class FBuilding : FData
{
    #region Firebase Properties
    [FirestoreProperty]
    public string name { get; set; }

    [FirestoreProperty]
    public float maintenance_amount { get; set; }

    [FirestoreProperty]
    public string maintenance_date { get; set; }

    [FirestoreProperty]
    public float init_amount { get; set; }

    [FirestoreProperty]
    public float villager_amout { get; set; }

    [FirestoreProperty]
    public List<string> currentVillagers { get; set; }

    [FirestoreProperty]
    public string type { get; set; }

    [FirestoreProperty]
    public float happiness { get; set; }

    [FirestoreProperty]
    public float health { get; set; }

    [FirestoreProperty]
    public float posX { get; set; }

    [FirestoreProperty]
    public float posY { get; set; }

    [FirestoreProperty]
    public float posZ { get; set; }

    [FirestoreProperty]
    public float progress { get; set; }

    [FirestoreProperty]
    public string category_id { get; set; }

    [FirestoreProperty]
    public int asset_id { get; set; }

    [FirestoreProperty]
    public int building_id { get; set; }

    [FirestoreProperty]
    public string build_at { get; set; }

    [FirestoreProperty]
    public bool isRemovable { get; set; }
    #endregion

    public FBuilding()
    {
        maintenance_amount = 0;
        maintenance_date = "";
        villager_amout = 0;
        init_amount = 0;
        currentVillagers = new List<string>();
        type = "";
        happiness = 100;
        health = 100;
        created_at = Convert.DateTimeToFDate(System.DateTime.Now);
        posX = posX = posZ = 0;
        progress = 0;
        name = "";
        category_id = "buildings";
        asset_id = 0;
        building_id = 0;
        build_at = "";
        isRemovable = true;
    }
}
