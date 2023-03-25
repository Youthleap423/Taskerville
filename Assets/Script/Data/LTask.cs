using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class LTask : LData
{
    public string taskName = "";
    public float progress = 0f;
    public float goldCount = 0f;
    public string begin_date = "";
    public string completedDate = "";
    
    private bool bNew = false;
    private bool bRemoved = false;

    public bool IsNew()
    {
        return bNew;
    }

    public bool IsRemoved()
    {
        return bRemoved;
    }

    public void SetNew(bool flag)
    {
        bNew = flag;
    }

    public void SetRemoved(bool flag)
    {
        bRemoved = flag;
    }

    public void SetParent(string pid)
    {
        //Pid = pid;
   
    }

    virtual public void OnComplete()
    {
        completedDate = Utilities.GetFormattedDate();
    }

    virtual public void OnComplete(System.DateTime dateTime)
    {
        completedDate = Convert.DateTimeToFDate(dateTime);
    }

    virtual public void CancelComplete()
    {
        completedDate = "";
    }

    virtual public bool isCompleted()
    {
        return Utilities.GetFormattedDate().Equals(completedDate);
    }

    virtual public bool isEnabled(System.DateTime dateTime)
    {
        if (Utilities.GetFormattedDate() == completedDate)
        {
            return false;
        }

        return true;
    }

    virtual public bool isEnabled()
    {
        //if (this.collectionId == ESubEntryType.SubToDo.ToString())
        //{
        //    return this.completedDate == "";
        //}
        //else
        //{
            return !Utilities.GetFormattedDate().Equals(completedDate);
        //}
        //if (Utilities.GetFormattedDate() == completedDate)
        //{
        //    return false;
        //}

        //return true;
    }
}
