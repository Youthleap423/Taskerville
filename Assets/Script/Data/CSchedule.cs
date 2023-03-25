using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CSchedule : CData
{
    public int bID = 0;
    public int day = 0;
    public List<string> preCompleteIds;
    public bool isBonus;
}

