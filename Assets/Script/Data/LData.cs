using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LData
{
    public string id = "";
    public string created_at = "";

    public LData()
    {
        id = "";
        created_at = Convert.DateTimeToFDate(System.DateTime.Now);
    }

    public virtual void Serialize()
    {

    }
}
