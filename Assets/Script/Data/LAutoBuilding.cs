using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LAutoBuilding : LData
{
    public bool isCompleted = false;

    public LAutoBuilding()
    {
        isCompleted = false;
    }

    public LAutoBuilding(CSchedule schedule)
    {
        id = schedule.id;
        isCompleted = false;
    }

    public LAutoBuilding(string id, bool bComplete)
    {
        this.id = id;
        this.isCompleted = bComplete;
    }

}