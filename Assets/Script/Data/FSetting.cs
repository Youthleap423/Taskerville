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
}
