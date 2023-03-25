using Firebase.Firestore;
using UnityEngine;
using System.Collections.Generic;

[FirestoreData]
public class FSetting
{
    [FirestoreProperty]
    public string Id { get; set; }

    [FirestoreProperty]
    public int game_mode { get; set; }

    [FirestoreProperty]
    public int interaction_mode { get; set; }

    [FirestoreProperty]
    public bool shelter_storm { get; set; }

    public FSetting()
    {
        game_mode = (int)Game_Mode.Task_Only;
        interaction_mode = (int)Interaction_Mode.Partial_Management;
        shelter_storm = false;
    }

    public FSetting(string Id, int game_mode, int interaction_mode)
    {
        this.Id = Id;
        this.game_mode = game_mode;
        this.interaction_mode = interaction_mode;
        this.shelter_storm = false;
    }

    public void Load()
    {
        game_mode = PlayerPrefs.GetInt("game_mode");
        interaction_mode = PlayerPrefs.GetInt("interaction_mode");
    }

    public void Save()
    {
        PlayerPrefs.SetInt("game_mode", (int)game_mode);
        PlayerPrefs.SetInt("interaction_mode", (int)interaction_mode);
    }

    public void Update(Game_Mode mode, System.Action<bool, string> callback)
    {
        game_mode = (int)mode;
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("game_mode", (int)mode);

        FirestoreManager.Instance.UpdateData("GameSetting", Id, dic, callback);
    }

    public void Update(Interaction_Mode mode, System.Action<bool, string> callback)
    {
        interaction_mode = (int)mode;

        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("interaction_mode", (int)mode);

        FirestoreManager.Instance.UpdateData("GameSetting", Id, dic, callback);
    }

    public void Update(Game_Mode game_Mode, Interaction_Mode interaction_Mode, System.Action<bool, string> callback)
    {
        this.game_mode = (int)game_Mode;
        this.interaction_mode = (int)interaction_Mode;

        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("game_mode", this.game_mode);
        dic.Add("interaction_mode", this.interaction_mode);
        
        FirestoreManager.Instance.UpdateData("GameSetting", Id, dic, callback);
    }

    public void Update(bool shelter_storm, System.Action<bool, string> callback)
    {
        this.shelter_storm = shelter_storm;

        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("shelter_storm", shelter_storm);

        FirestoreManager.Instance.UpdateData("GameSetting", Id, dic, callback);
    }

}
