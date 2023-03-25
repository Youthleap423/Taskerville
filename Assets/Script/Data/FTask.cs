using Firebase.Firestore;

[FirestoreData]
public class FTask: FData
{
    
    [FirestoreProperty]
    public string taskName { get; set; }

    [FirestoreProperty]
    public float progress { get; set; }

    [FirestoreProperty]
    public float goldCount { get; set; }

    [FirestoreProperty]
    public string begin_date { get; set; }

    [FirestoreProperty]
    public string completedDate { get; set; }

    public bool isNew { get; set; }
    public bool isRemoved { get; set; }

    public FTask()
    {
        Id = "";
        Pid = "";
        taskName = "";
        goldCount = 0;
        progress = 0.0f;
        isNew = true;
        isRemoved = false;
        completedDate = "";
        collectionId = "";
        begin_date = Convert.DateTimeToFDate(System.DateTime.Now);
    }

    public void SetParent(string pid)
    {
        Pid = pid;
        Id = string.Format("{0}_{1}", pid, Utilities.SystemTimeInMillisecondsString);
    }

    virtual public void OnComplete()
    {
        completedDate = Utilities.GetFormattedDate();
    }

    virtual public void OnComplete(System.DateTime dateTime)
    {
        completedDate = Convert.DateTimeToFDate(dateTime);
    }

    virtual public void CancelComplete()
    {

    }

    virtual public bool isCompleted()
    {
        if (this.collectionId == ESubEntryType.SubToDo.ToString())
        {
            return this.completedDate != "";
        }else
        {
            return Utilities.GetFormattedDate().Equals(completedDate);
        }
    }

    

    virtual public bool isEnabled(System.DateTime dateTime)
    {
        if (Utilities.GetFormattedDate() == completedDate)
        {
            return false;
        }

        return true;
    }

    virtual public bool isEnabled()
    {
        if (this.collectionId == ESubEntryType.SubToDo.ToString())
        {
            return this.completedDate == "";
        }
        else
        {
            return !Utilities.GetFormattedDate().Equals(completedDate);
        }
        //if (Utilities.GetFormattedDate() == completedDate)
        //{
        //    return false;
        //}

        //return true;
    }
}