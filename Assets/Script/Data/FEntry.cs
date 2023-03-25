using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;

[FirestoreData]
public class FEntry : FTask
{
    [FirestoreProperty]
    public long orderId { get; set; }

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

    
}
