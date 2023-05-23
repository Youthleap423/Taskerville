using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEntry : LTask
{
    public long orderId = -1;
    public int repeat_alarm = 0;
    public string remindAlarm = "";
    private EntryType type = EntryType.NULL;

    virtual public EntryType Type
    {
        set
        {
            type = value;
        }

        get
        {
            return type;
        }
    }

    public virtual System.DateTime GetTriggerDate()
    {
        return System.DateTime.MinValue;
    }
}
