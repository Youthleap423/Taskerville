using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class LSubTask : LTask
{
    public string type = ESubEntryType.SubDailyTask.ToString();

    override public bool isCompleted()
    {
        if (this.type == ESubEntryType.SubDailyTask.ToString())
        {
            return Utilities.GetFormattedDate().Equals(completedDate); 
        }
        else
        {
            return this.completedDate != "";
        }
    }
}
