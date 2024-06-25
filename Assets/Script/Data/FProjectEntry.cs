using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class FProjectEntry : FEntry
{
    [FirestoreProperty]
    public string beginDate { get; set; }

    [FirestoreProperty]
    public string endDate { get; set; }

    [FirestoreProperty]
    public List<string> subProjects { get; set; }

    [FirestoreProperty]
    public List<string> linkedTasks { get; set; }


    public FProjectEntry()
    {
        Type = EntryType.Project;
        beginDate = "March 22, 2000";
        endDate = "March 22, 2000";
        subProjects = new List<string>();
        linkedTasks = new List<string>();
    }

    public override bool isCompleted()
    {
        return !completedDate.Equals("");
    }
}
