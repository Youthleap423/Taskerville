using Firebase.Firestore;

[FirestoreData]
public class FData 
{
    [FirestoreProperty]
    public string Id { get; set; }

    [FirestoreProperty]
    public string collectionId { get; set; }

    [FirestoreProperty]
    public string Pid { get; set; }

    [FirestoreProperty]
    public string created_at { get; set; }

    public FData()
    {
        Id = "";
        collectionId = "";
        Pid = "";
        created_at = Convert.DateTimeToFDate(System.DateTime.Now);
    }

    
}
