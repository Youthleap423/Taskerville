using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class FToDoEntry : FEntry
{
    [FirestoreProperty]
    public int diffculty { get; set; }

    [FirestoreProperty]
    public string dueDate { get; set; }

    [FirestoreProperty]
    public string remindAlarm { get; set; }

    [FirestoreProperty]
    public List<string> checkList { get; set; }

    public List<FTask> subTaskList = new List<FTask>();

    public FToDoEntry()
    {
        Type = EntryType.ToDo;
        diffculty = (int)Difficuly.Easy;
        dueDate = "2000_03_20";
        remindAlarm = "00:00 AM";
        checkList = new List<string>();
    }

    public override bool isCompleted()
    {
        if (completedDate != "")
        {
            return true;
        }
        return false;
    }

    public override bool isEnabled()
    {
        if (isCompleted())
        {
            return false;
        }
        else
        {
            //if (dueDate.CompareTo(Utilities.GetFormattedDate()) >= 0)
            //{
            //    return true;
            //}
            return true;
        }

        //return false;
    }

}
