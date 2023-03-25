using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LArtwork : LData
{
    public string reason = "";

    public LArtwork()
    {
        reason = "";
    }

    public LArtwork(CArtwork artwork, string reason = "")
    {
        id = artwork.id;
        created_at = Convert.DateTimeToFDate(System.DateTime.Now);
        this.reason = reason;
    }
}
