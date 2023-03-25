using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LVillager : LData
{
    public float happiness = 100f;
    public float health = 100f;
    public float posX = 0f;
    public float posY = 0f;
    public float posZ = 0f;
    public string anim_state = "";
    public int live_at = 0;
    public int work_at = 0;
    public string UID = "0";

    public bool canWork(System.DateTime dateTime) 
    {
        var createTime = Convert.FDateToDateTime(created_at).Date;
        
        if (createTime.CompareTo(dateTime.Date) < 0)
        {
            return true;
        }

        return false;
    }
}
