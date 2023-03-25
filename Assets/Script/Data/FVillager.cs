using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[FirestoreData]
public class FVillager: FData
{
    #region Firebase Properties
    [FirestoreProperty]
    public string name { get; set; }

    [FirestoreProperty]
    public float init_amount { get; set; }

    [FirestoreProperty]
    public float meal_amount { get; set; }

    [FirestoreProperty]
    public string meal_date { get; set; }

    [FirestoreProperty]
    public float salary_amount { get; set; }

    [FirestoreProperty]
    public string salary_date { get; set; }

    [FirestoreProperty]
    public float pension_amount { get; set; }

    [FirestoreProperty]
    public string pension_date { get; set; }

    [FirestoreProperty]
    public float hire_price { get; set; }

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
    public string anim_state { get; set; }
    #endregion

    public FVillager()
    {
        init_amount = 0;
        Id = "";
        Pid = "";
        meal_amount = 4;
        meal_date = "";
        salary_amount = 0;
        salary_date = "";
        pension_amount = 0;
        pension_date = "";
        hire_price = 0;
        type = "";
        happiness = 100;
        health = 100;
        posX = posY = posZ = 0;
        anim_state = "";
        created_at = "";
        name = "";
    }

    protected virtual void Init()
    {

    }
}
