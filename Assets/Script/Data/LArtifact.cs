using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LArtifact : LData
{
    public float progress = 0f;
    public bool isExchanged = false;
    public int dig = 0;

    public LArtifact()
    {
        progress = 0f;
        dig = 0;
        isExchanged = false;
    }

    public LArtifact(CArtifact artifact)
    {
        id = artifact.id;
        progress = 0f;
        dig = int.Parse(artifact.id) % 29;
        isExchanged = false;
    }

    public void Completed()
    {
        created_at = Convert.DateTimeToDetailedString(System.DateTime.Now);//change date to end date
        progress = 1.0f;
    }
}