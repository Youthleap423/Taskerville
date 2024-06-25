using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LSetting : LData
{
    public int game_mode = 0;
    public int interaction_mode = 0;
    public bool shelter_storm = false;
    public bool alarm_dt = true;
    public bool alarm_td = true;
    public bool alarm_ht = true;
    public bool alarm_goal = true;

    public LSetting()
    {
        game_mode = (int)Game_Mode.Task_And_Game;
        interaction_mode = (int)Interaction_Mode.Manual_Management;
        shelter_storm = false;
    }

    public LSetting(int game_mode, int interaction_mode)
    {
        this.game_mode = game_mode;
        this.interaction_mode = interaction_mode;
        this.shelter_storm = false;
    }

    public void Save()
    {
        DBHandler.SaveToJSON(this, DataManager.Instance.settingFN);
    }

    public void Update(Game_Mode mode)
    {
        this.game_mode = (int)mode;
        Save();
    }

    public void Update(Interaction_Mode mode)
    {
        interaction_mode = (int)mode;
        Save();
    }

    public void Update(Game_Mode game_Mode, Interaction_Mode interaction_Mode)
    {
        this.game_mode = (int)game_Mode;
        this.interaction_mode = (int)interaction_Mode;
        Save();
    }

    public void Update(bool shelter_storm)
    {
        this.shelter_storm = shelter_storm;
        Save();
    }

    public void UpdateAlarm(bool bFlag, EntryType entryType)
    {
        switch (entryType)
        {
            case EntryType.DailyTask:
                alarm_dt = bFlag;
                break;
            case EntryType.ToDo:
                alarm_td = bFlag;
                break;
            case EntryType.Habit:
                alarm_ht = bFlag;
                break;
            case EntryType.Project:
                alarm_goal = bFlag;
                break;
            default:
                break;
        }
        Save();
    }
}
