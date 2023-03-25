using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LAutoGoal : LEntry
{
    public int completionDays = 1;
    public int repeatDays = 0;
    public int completeAmount = 0;
    public string endDate = "";
    public float happiness = 0f;
    public LAutoGoal()
    {
        completionDays = 1;
        repeatDays = 0;
        endDate = "March 22, 2000";
        Type = EntryType.AutoGoal;
    }

    
    public override bool isCompleted()
    {
        if (completedDate.Equals(""))
        {
            return false;
        }

        if (begin_date == "")
        {
            return true;
        }

        System.DateTime complete_date_DT = Convert.FDateToDateTime(completedDate);
        System.DateTime begin_date_DT = Convert.FDateToDateTime(begin_date);

        if (begin_date_DT.CompareTo(complete_date_DT) > 0)
        {
            return false;
        }else if (begin_date_DT.CompareTo(complete_date_DT) == 0)
        {
            if (repeatDays == 0)
            {
                return true;
            }
        }

        return true;
    }

    public bool IsAvailable(System.DateTime dateTime)
    {
        if (begin_date == "")
        {
            return false;
        }


        return !isCompleted();
        
    }

    public override bool isEnabled(System.DateTime dateTime)
    {
        if (begin_date == "")
        {
            return false;
        }


        if (isCompleted())
        {
            return false;
        }

        System.DateTime begin_date_DT = Convert.FDateToDateTime(begin_date);

        if (dateTime.CompareTo(begin_date_DT) < 0)
        {
            return false;
        }

        //return true;
        System.DateTime end_date_DT = Convert.FDateToDateTime(endDate);//enable this function 2023/02/06

        if (dateTime.CompareTo(end_date_DT) > 0)
        {
            return false;
        }
        return true;
    }
}
