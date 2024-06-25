using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;
using Newtonsoft.Json;
using Firebase.Extensions;

public class FirestoreManager : SingletonComponent<FirestoreManager>
{
    [SerializeField] private FirebaseFirestore db = null;

    #region Properties
    public System.Action<string> OnFirestoreFailed;
    public System.Action<FUser> OnFirestoreSucceeded;
    public System.Action<bool, string> OnFireStoreUploadListResult;

    public System.Action<List<FCoalition>> OnCoalitionUpdated;
    #endregion

    #region Unity_Members
    // Start is called before the first frame update
    private int nListUploadCompleteCount = 0;
    
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        db.Collection("Coalition").Listen(querySnapShot =>
        {
            if (querySnapShot != null)
            {
                List<FCoalition> coalitions = new List<FCoalition>();
                foreach (DocumentSnapshot document in querySnapShot.Documents)
                {
                    coalitions.Add(document.ConvertTo<FCoalition>());
                }
                OnCoalitionUpdated(coalitions);
            }
        });
    }

    
    #endregion

}
