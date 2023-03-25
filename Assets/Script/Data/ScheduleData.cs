using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class ScheduleData : ScriptableObject
{
    [FormerlySerializedAs("Schedule")]
    public List<CSchedule> schedules;
}
