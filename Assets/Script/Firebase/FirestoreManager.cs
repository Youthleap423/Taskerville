using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;
using Newtonsoft.Json;

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

    #region Public_Members
    public void CreateUserData(FUser user, System.Action<bool, string> callback)
    {
        //TODO- change structure. 
        StartCoroutine(CreateData("User", user.Id, user, callback));
    }

    public void createSettingData(string docId, FSetting setting)
    {
        ExistSetting(docId, (isSuccess, errMsg) =>
        {
            if (!isSuccess)
            {
                CreateData("GameSetting", docId, setting);
            }
            //else
            //{
            //    //UpdateData("GameSetting", docId, setting);
            //}
        });
    }

    

    public void createData<T>(string collectionId, string docId, T data)
    {
        ExistData(collectionId, docId, (isSuccess, errMsg) =>
        {
            if (!isSuccess)
            {
                CreateData(collectionId, docId, data); 
            }
            //else
            //{
            //    UpdateData(collectionId, docId, data);
            //}
        });
    }

    public void createData(string collectionId, string docId, object data, System.Action<bool, string> callback)
    {
        StartCoroutine(CreateData(collectionId, docId, data, callback));
    }

    public void createData<T>(string collectionId, T data, System.Action<bool, string> callback) where T: FData
    {
        StartCoroutine(CreateData(collectionId, data, callback));
    }

    public void createDataList<T>(List<T> datas, System.Action<bool, string> callback) where T : FData
    {
        StartCoroutine(CreateDataList(datas, callback));
    }


    public void CreateEntry<T>(T entry, List<FTask> subEntryList, System.Action<bool, string> callback) where T: FEntry
    {
        StartCoroutine(CreateDataList(entry.Type.ToString(), entry.Id, entry, subEntryList, callback));
    }

    public void CreateEntry<T>(T entry, List<FTask> subEntryList, List<FTaskEntry> linkedTaskList, System.Action<bool, string> callback) where T : FEntry
    {
        StartCoroutine(CreateDataList(entry.Type.ToString(), entry.Id, entry, subEntryList, linkedTaskList, callback));
    }

    public void CreateEntry<T>(T entry, List<FTask> subEntryList, IDictionary<string, object> dic, System.Action<bool, string> callback) where T : FEntry
    {
        StartCoroutine(CreateDataList(entry.Type.ToString(), entry.Id, entry, subEntryList, dic, callback));
    }

    public void DeleteData(string collectionId, string docId, System.Action<bool, string> callback)
    {
        StartCoroutine(Enumerator_DeleteDocument(collectionId, docId, callback));
    }

    public void ExistUser(string username, System.Action<bool, string, FUser, LUser> callback)
    {
        StartCoroutine(Enumerator_ExistUser(username, callback));
    }

    public void ExistData(string collectionId, string docId, System.Action<bool, string> callback)
    {
        StartCoroutine(Enumerator_ExistDocument(collectionId, docId, callback));
    }

    public void ExistSetting(string docId, System.Action<bool, string> callback)
    {
        StartCoroutine(Enumerator_ExistDocument("GameSetting", docId, callback));
    }

    public void GetEntryData<T>(string collectionId, string docId, System.Action<bool, string, T> callback) where T: FEntry
    {
        StartCoroutine(Enumerator_GetEntryData(collectionId, docId, callback));
    }

    public void GetTaskList<T>(string collectionId, System.Action<bool, string, List<T>> callback) where T : FTask
    {
        StartCoroutine(Enumerator_GetTaskList(collectionId, callback));
    }

    public void GetDataList<T>(string collectionId, System.Action<bool, string, List<T>> callback) where T : FData
    {
        StartCoroutine(Enumerator_GetDataList(collectionId, callback));
    }

    public void GetList<T>(string collectionId, string fieldName, string value, System.Action<bool, string, List<T>> callback) where T : FData
    {
        StartCoroutine(Enumerator_GetList(collectionId, fieldName, value, callback));
    }

    public void GetTaskList<T>(string collectionId, string fieldName, string value, System.Action<bool, string, List<T>> callback) where T: FTask
    {
        StartCoroutine(Enumerator_GetTaskList(collectionId, fieldName, value, callback));
    }

    public void GetSubTaskList<T>(string collectionId, string fieldName, string value, System.Action<bool, string, List<T>> callback) where T : FTask
    {
        StartCoroutine(Enumerator_GetSubTaskList(collectionId, fieldName, value, callback));
    }
    

    public void GetData<T>(string collectionId, string docId, System.Action<bool, string, T> callback) where T: new()
    {
        StartCoroutine(Enumerator_GetData(collectionId, docId, callback));
    }

    public void GetMessages(string collectionId, string groupName, string type, System.Action<bool, string, List<FMessage>> callback)
    {
        StartCoroutine(Enumerator_GetMessages(collectionId, groupName, type, callback)); ;
    }

    public void GetMessages(string collectionId, List<string> list, string type, System.Action<bool, string, List<FMessage>> callback)
    {
        StartCoroutine(Enumerator_GetMessages(collectionId, list, type, callback)); ;
    }

    public void GetInvitations(string collectionId, string userId, System.Action<bool, string, List<FInvitation>> callback)
    {
        StartCoroutine(Enumerator_GetInvitations(collectionId, userId, callback)); ;
    }

    public void GetInvitations(string collectionId, string userId, string invite_type, System.Action<bool, string, List<FInvitation>> callback)
    {
        StartCoroutine(Enumerator_GetInvitations(collectionId, userId, invite_type, callback)); ;
    }

    public void GetInvitations(string collectionId, string userId, System.Action<bool, string, List<FTradeInvitation>> callback)
    {
        StartCoroutine(Enumerator_GetInvitations(collectionId, userId, callback)); ;
    }

    public void GetTrades(string collectionId, string userId, string fieldName, System.Action<bool, string, List<FTrade>> callback)
    {
        StartCoroutine(Enumerator_GetData(collectionId, fieldName, userId, callback)); ;
    }

    public void GetTradeItems(string userId, string fieldName, System.Action<bool, string, List<FTradeItem>> callback)
    {
        StartCoroutine(Enumerator_GetData("TradeItem", fieldName, userId, callback)); ;
    }

    public void UpdateTrades(string collectionId, List<FTrade> rTrades, List<FTrade> sTrades, List<FTradeItem> itemList,  System.Action<bool, string> callback)
    {
        StartCoroutine(Enumerator_UpdateTrades(collectionId, rTrades, sTrades, itemList, callback)); ;
    }

    public void GetCoalitions(string collectionId, System.Action<bool, string, IEnumerable<DocumentSnapshot>> callback)
    {
        StartCoroutine(Enumerator_GetCoalitions(collectionId, callback)); ;
    }

    public void UpdateData(string collectinnId, string docId, IDictionary<string, object> updateDic, System.Action<bool, string> callback)
    {
        StartCoroutine(Enumerator_UpdateData(collectinnId, docId, updateDic, callback));
    }

    public void UpdateData(string collectinnId, FCoalition coalition, System.Action<bool, string> callback)
    {
        StartCoroutine(Enumerator_UpdateData(collectinnId, coalition, callback));
    }

    public void UpdateData<T>(T data, System.Action<bool, string> callback) where T : FData
    {
        StartCoroutine(Enumerator_UpdateData(data, callback));
    }

    public void UpdateData(List<FTaskEntry> fTaskEntries, System.DateTime dateTime, System.Action<bool, string> callback)
    {
        StartCoroutine(Enumerator_UpdateData(fTaskEntries, dateTime, callback));
    }

    public void UpdateEntries<T>(List<T> entryList, System.Action<bool, string> callback) where T: FEntry
    {
        StartCoroutine(Enumerator_UpdateEntries<T>(entryList, callback));
    }

    public void UpdateInvitation(FInvitation invitation, FUser user, System.Action<bool, string> callback)
    {
        StartCoroutine(Enumerator_UpdateData(invitation, user, callback));
    }

    public void GetInitData(string collectionId, System.Action<bool, string, IEnumerable<DocumentSnapshot>> callback)
    {
        StartCoroutine(Enumerator_GetInitData(collectionId, callback));
    }

    public void RemoveData(FData data, System.Action<bool, string> callback)
    {
        StartCoroutine(Enumerator_RemoveData(data, callback));
    }

    public void RemoveData(List<FData> dataList, System.Action<bool, string> callback)
    {
        StartCoroutine(Enumerator_RemoveDataList(dataList, callback));
    }

    #endregion

    #region Private_Members

    private void CreateData(string collectionId, string docId, object data)
    {
        db.Collection(collectionId).Document(docId).SetAsync(data).ContinueWith(task =>
        {
            if (!task.IsCanceled && !task.IsFaulted)
            {
                UIManager.LogError("Successfully created data");
            }
            else
            {
                UIManager.LogError("Failed to create data" + task.Exception);
            }
        });
    }

    private IEnumerator Enumerator_RemoveData(FData data, System.Action<bool, string> callback)
    {
        var task = db.Collection(data.collectionId).Document(data.Id).DeleteAsync();

        yield return new WaitUntil(() => task.IsCompleted);
        
        if (!task.IsCanceled && !task.IsFaulted)
        {
            if (callback != null)
            {
                callback(true, "");
            }
            
        }
        else
        {
            if (callback != null)
            {
                callback(false, "Failed to remove data" + task.Exception);
            }
        }
    }

    private IEnumerator Enumerator_RemoveDataList(List<FData> dataList, System.Action<bool, string> callback)
    {
        WriteBatch batch = db.StartBatch();

        foreach (FData data in dataList)
        {
            batch.Delete(db.Collection(data.collectionId).Document(data.Id));
        }

        var task = batch.CommitAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            callback(false, "Failed to remove data" + task.Exception);
        }
        else
        {
            callback(true, "");
        }
    }

    private IEnumerator CreateData<T>(string collectionId, T data, System.Action<bool, string> callback) where T:FData
    {
        DocumentReference addedDocRef = db.Collection(collectionId).Document();
        data.Id = addedDocRef.Id;
        var task = addedDocRef.SetAsync(data);

        yield return new WaitUntil(() => task.IsCompleted);

        if (!task.IsCanceled && !task.IsFaulted)
        {
            callback(true, "");
        }
        else
        {
            callback(false, "Failed to create data" + task.Exception);
        }
    }

    private IEnumerator CreateData(string collectionId, string docId, object data, System.Action<bool, string> callback)
    {
        var task = db.Collection(collectionId).Document(docId).SetAsync(data);

        yield return new WaitUntil(() => task.IsCompleted);

        if (!task.IsCanceled && !task.IsFaulted)
        {
            callback(true, "");
        }
        else
        {
            callback(false, "Failed to create data" + task.Exception);
        }
    }

    private IEnumerator CreateDataList<T>(List<T> datas, System.Action<bool, string> callback) where T : FData
    {
        WriteBatch batch = db.StartBatch();

        foreach (T data in datas)
        {
            batch.Set(db.Collection(data.collectionId).Document(data.Id), data);
        }

        var task = batch.CommitAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            callback(false, "Failed to create data" + task.Exception);
        }
        else
        {
            callback(true, "");
        }
    }

    private IEnumerator CreateDataList(string collectionId, string docId, FEntry entry, List<FTask> subEntryList, System.Action<bool, string> callback)
    {
        WriteBatch batch = db.StartBatch();

        foreach (FTask ftask in subEntryList)
        {
            if (ftask.isNew)
            {
                if (!ftask.isRemoved)
                {
                    batch.Set(db.Collection("Sub" + collectionId).Document(ftask.Id), ftask);
                }
            }
            else
            {
                if (ftask.isRemoved)
                {
                    batch.Delete(db.Collection("Sub" + collectionId).Document(ftask.Id));
                }
                else
                {
                    batch.Set(db.Collection("Sub" + collectionId).Document(ftask.Id), ftask);
                }
            }
            
        }

        if (entry.isRemoved)
        {
            batch.Delete(db.Collection(collectionId).Document(docId));
        }
        else
        {
            batch.Set(db.Collection(collectionId).Document(docId), entry);
        }
        
        
        var task = batch.CommitAsync();
        
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            callback(false, "Failed to create data" + task.Exception);
        }
        else
        {
            callback(true, "");
        }
    }

    private IEnumerator CreateDataList(string collectionId, string docId, FEntry entry, List<FTask> subEntryList, List<FTaskEntry> linkedTaskList, System.Action<bool, string> callback)
    {
        WriteBatch batch = db.StartBatch();

        foreach (FTask ftask in subEntryList)
        {
            if (ftask.isNew)
            {
                if (!ftask.isRemoved)
                {
                    batch.Set(db.Collection("Sub" + collectionId).Document(ftask.Id), ftask);
                }
            }
            else
            {
                if (ftask.isRemoved)
                {
                    batch.Delete(db.Collection("Sub" + collectionId).Document(ftask.Id));
                }
                else
                {
                    batch.Set(db.Collection("Sub" + collectionId).Document(ftask.Id), ftask);
                }
            }

        }

        if (entry.isRemoved)
        {
            batch.Delete(db.Collection(collectionId).Document(docId));
        }
        else
        {
            batch.Set(db.Collection(collectionId).Document(docId), entry);
        }

        foreach(FTaskEntry taskEntry in linkedTaskList)
        {
            batch.Set(db.Collection(EntryType.DailyTask.ToString()).Document(taskEntry.Id), taskEntry);
        }


        var task = batch.CommitAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            callback(false, "Failed to create data" + task.Exception);
        }
        else
        {
            callback(true, "");
        }
    }

    private IEnumerator CreateDataList(string collectionId, string docId, object entry, List<FTask> subEntryList, IDictionary<string, object> dic, System.Action<bool, string> callback)
    {
        WriteBatch batch = db.StartBatch();

        foreach (FTask ftask in subEntryList)
        {
            batch.Set(db.Collection("Sub" + collectionId).Document(ftask.Id), ftask);
        }

        batch.Set(db.Collection(collectionId).Document(docId), entry);
        batch.Update(db.Collection(collectionId).Document(docId), dic);

        var task = batch.CommitAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            callback(false, "Failed to create data" + task.Exception);
        }
        else
        {
            callback(true, "");
        }
    }


    private IEnumerator Enumerator_UpdateData(string collectionId, string docId, IDictionary<string, object> updateDic, System.Action<bool, string> callback)
    {
        var task = db.Collection(collectionId).Document(docId).UpdateAsync(updateDic);
        
        yield return new WaitUntil(() => task.IsCompleted);

        if (!task.IsCanceled && !task.IsFaulted)
        {
            callback(true, "");
        }
        else
        {
            callback(false, "Failed to update" + task.Exception);
        }
    }

    private IEnumerator Enumerator_UpdateData(string collectionId, FCoalition coalition, System.Action<bool, string> callback)
    {
        var task = db.Collection(collectionId).Document(coalition.Id).SetAsync(coalition);

        yield return new WaitUntil(() => task.IsCompleted);

        if (!task.IsCanceled && !task.IsFaulted)
        {
            callback(true, "");
        }
        else
        {
            callback(false, "Failed to update" + task.Exception);
        }
    }

    private IEnumerator Enumerator_UpdateData<T>(T data, System.Action<bool, string> callback) where T : FData
    {
        var collectionId = data.collectionId;
        var task = db.Collection(collectionId).Document(data.Id).SetAsync(data);

        yield return new WaitUntil(() => task.IsCompleted);

        if (!task.IsCanceled && !task.IsFaulted)
        {
            callback(true, "");
        }
        else
        {
            callback(false, "Failed to update" + task.Exception);
        }
    }

    private IEnumerator Enumerator_UpdateData(List<FTaskEntry> fTaskEntries, System.DateTime dateTime, System.Action<bool, string> callback)
    {
        WriteBatch batch = db.StartBatch();

        foreach (FTaskEntry ftaskEntry in fTaskEntries)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("completedDate", Convert.DateTimeToFDate(dateTime));
            dic.Add("completed_Week", ftaskEntry.completed_Week);
            dic.Add("skip_Dates", ftaskEntry.skip_Dates);
            batch.Update(db.Collection(ftaskEntry.collectionId).Document(ftaskEntry.Id), dic);
            foreach(string fTaskId in ftaskEntry.subTasks)
            {
                Dictionary<string, object> subTaskDic = new Dictionary<string, object>();
                subTaskDic.Add("completedDate", Convert.DateTimeToFDate(dateTime));
                batch.Update(db.Collection("Sub" + ftaskEntry.collectionId).Document(fTaskId), subTaskDic);
            }
        }

        var task = batch.CommitAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (!task.IsCanceled && !task.IsFaulted)
        {
            callback(true, "");
        }
        else
        {
            callback(false, "Failed to update" + task.Exception);
        }
    }

    private IEnumerator Enumerator_UpdateEntries<T>(List<T> entryList, System.Action<bool, string> callback) where T: FEntry
    {
        WriteBatch batch = db.StartBatch();

        foreach (T fEntry in entryList)
        {
            batch.Set(db.Collection(fEntry.collectionId).Document(fEntry.Id), fEntry);
        }

        var task = batch.CommitAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (!task.IsCanceled && !task.IsFaulted)
        {
            callback(true, "");
        }
        else
        {
            callback(false, "Failed to update" + task.Exception);
        }
    }

    private IEnumerator Enumerator_UpdateData(FInvitation invitation, FUser user, System.Action<bool, string> callback)
    {
        WriteBatch batch = db.StartBatch();

        batch.Set(db.Collection(invitation.collectionId).Document(invitation.Id), invitation);
        batch.Set(db.Collection("User").Document(user.Id), user);
        
        var task = batch.CommitAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (!task.IsCanceled && !task.IsFaulted)
        {
            callback(true, "");
        }
        else
        {
            callback(false, "Failed to update" + task.Exception);
        }
    }

    private IEnumerator Enumerator_UpdateTrades(string collectionId, List<FTrade> rTrades, List<FTrade> sTrades, List<FTradeItem> tradeItemList, System.Action<bool, string> callback)
    {
        WriteBatch batch = db.StartBatch();
        CollectionReference colRef = db.Collection(collectionId);

        foreach (FTrade trade in rTrades)
        {
            batch.Set(colRef.Document(trade.Id), trade);
        }

        foreach (FTrade trade in sTrades)
        {
            batch.Set(colRef.Document(trade.Id), trade);
        }

        CollectionReference tradeItemColRef = db.Collection("TradeItem");
        foreach (FTradeItem item in tradeItemList)
        {
            batch.Delete(tradeItemColRef.Document(item.Id));
        }

        var task = batch.CommitAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (!task.IsCanceled && !task.IsFaulted)
        {
            callback(true, "");
        }
        else
        {
            callback(false, "Failed to update" + task.Exception);
        }
    }

    private IEnumerator Enumerator_GetEntryData<T>(string collectionId, string docId, System.Action<bool, string, T> callback) where T : FEntry
    {
        DocumentReference docRef = db.Collection(collectionId).Document(docId);
        var task = docRef.GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, null);
        }
        else
        {
            T data = task.Result.ConvertTo<T>();
            data.isNew = false;

            callback(true, "", data);
        }
    }

    private IEnumerator Enumerator_GetData<T>(string collectionId, string docId, System.Action<bool, string, T> callback) where T : new()
    {
        var task = db.Collection(collectionId).Document(docId).GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);


        if (task.Exception != null)
        {
            callback(false, "Error:" + task.Exception.ToString(), new T());
        }
        else
        {
            if (task.Result.Exists == true)
            {
                T result = task.Result.ConvertTo<T>();
                callback(true, "", result);
            }
            else
            {
                callback(false, "Not found Data", new T());
            }
        }
    }

    private IEnumerator Enumerator_GetTaskList<T>(string collectionId, System.Action<bool, string, List<T>> callback) where T : FTask
    {
        CollectionReference colRef = db.Collection(collectionId);
        var task = colRef.GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        List<T> objectList = new List<T>();

        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, objectList);
        }
        else
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
            {
                T data = documentSnapshot.ConvertTo<T>();
                data.isNew = false;
                objectList.Add(data);
            }
            callback(true, "", objectList);
        }
    }

    private IEnumerator Enumerator_GetDataList<T>(string collectionId, System.Action<bool, string, List<T>> callback) where T : FData
    {
        CollectionReference colRef = db.Collection(collectionId);
        var task = colRef.GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        List<T> objectList = new List<T>();

        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, objectList);
        }
        else
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
            {
                T data = documentSnapshot.ConvertTo<T>();
                objectList.Add(data);
            }
            callback(true, "", objectList);
        }
    }

    private IEnumerator Enumerator_GetTaskList<T>(string collectionId, string fieldName, string value, System.Action<bool, string, List<T>> callback) where T: FTask
    {
        CollectionReference colRef = db.Collection(collectionId);
        var task = colRef.WhereEqualTo(fieldName, value).OrderBy("orderId").GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        List<T> objectList = new List<T>();

        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, objectList);
        }
        else
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
            {
                T data = documentSnapshot.ConvertTo<T>();
                data.isNew = false;//means from the server
                objectList.Add(data);
            }
            callback(true, "", objectList);
        }

}

    private IEnumerator Enumerator_GetSubTaskList<T>(string collectionId, string fieldName, string value, System.Action<bool, string, List<T>> callback) where T : FTask
    {
        CollectionReference colRef = db.Collection(collectionId);
        var task = colRef.WhereEqualTo(fieldName, value).GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        List<T> objectList = new List<T>();

        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, objectList);
        }
        else
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
            {
                T data = documentSnapshot.ConvertTo<T>();
                data.isNew = false;//means from the server
                objectList.Add(data);
            }
            callback(true, "", objectList);
        }
    }

    private IEnumerator Enumerator_GetMessages(string collectionId, string groupName, string type, System.Action<bool, string, List<FMessage>> callback)
    {
        CollectionReference colRef = db.Collection(collectionId);
        var task = colRef.OrderBy("created_at").WhereEqualTo("receiver_Id", groupName).WhereEqualTo("type", type).GetSnapshotAsync();
        
        yield return new WaitUntil(() => task.IsCompleted);

        List<FMessage> messageList = new List<FMessage>();

        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, messageList);
        }
        else
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
            {
                FMessage fMessage = documentSnapshot.ConvertTo<FMessage>();
                messageList.Add(fMessage);
            }
            callback(true, "", messageList);
        }
    }

    private IEnumerator Enumerator_GetMessages(string collectionId, List<string> list, string type, System.Action<bool, string, List<FMessage>> callback)
    {
        CollectionReference colRef = db.Collection(collectionId);
        var task = colRef.OrderBy("created_at")
            .WhereEqualTo("members", list)
            .WhereEqualTo("type", type).GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        List<FMessage> messageList = new List<FMessage>();

        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, messageList);
        }
        else
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
            {
                FMessage fMessage = documentSnapshot.ConvertTo<FMessage>();
                messageList.Add(fMessage);
            }
            callback(true, "", messageList);
        }
    }


    private IEnumerator Enumerator_GetInvitations(string collectionId, string userId, System.Action<bool, string, List<FInvitation>> callback)
    {
        CollectionReference colRef = db.Collection(collectionId);
        var task = colRef.WhereEqualTo("receiver_Id", userId).GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        List<FInvitation> invitationList = new List<FInvitation>();

        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, invitationList);
        }
        else
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
            {
                FInvitation fInvitation = documentSnapshot.ConvertTo<FInvitation>();
                invitationList.Add(fInvitation);
            }
            callback(true, "", invitationList);
        }
    }

    private IEnumerator Enumerator_GetInvitations(string collectionId, string userId, string invite_type, System.Action<bool, string, List<FInvitation>> callback)
    {
        CollectionReference colRef = db.Collection(collectionId);
        var task = colRef.WhereEqualTo("receiver_Id", userId).WhereEqualTo("type", invite_type).WhereEqualTo("state", "Agreed").OrderBy("reply_at").GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        List<FInvitation> invitationList = new List<FInvitation>();

        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, invitationList);
        }
        else
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
            {
                FInvitation fInvitation = documentSnapshot.ConvertTo<FInvitation>();
                invitationList.Add(fInvitation);
            }
            callback(true, "", invitationList);
        }
    }

    private IEnumerator Enumerator_GetInvitations(string collectionId, string userId, System.Action<bool, string, List<FTradeInvitation>> callback)
    {
        CollectionReference colRef = db.Collection(collectionId);
        var task = colRef.WhereEqualTo("receiver_Id", userId).GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        List<FTradeInvitation> invitationList = new List<FTradeInvitation>();

        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, invitationList);
        }
        else
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
            {
                FTradeInvitation fInvitation = documentSnapshot.ConvertTo<FTradeInvitation>();
                invitationList.Add(fInvitation);
            }
            callback(true, "", invitationList);
        }
    }

    private IEnumerator Enumerator_GetData<T>(string collectionId, string paramId, string paramValue, System.Action<bool, string, List<T>> callback)
    {
        CollectionReference colRef = db.Collection(collectionId);
        var task = colRef.WhereEqualTo(paramId, paramValue).GetSnapshotAsync();
        
        yield return new WaitUntil(() => task.IsCompleted);

        List<T> list = new List<T>();
        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, list);
        }
        else
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
            {
                T trade = documentSnapshot.ConvertTo<T>();
                list.Add(trade);
            }
            callback(true, "", list);
        }
    }

    private IEnumerator Enumerator_GetCoalitions(string collectionId, System.Action<bool, string, IEnumerable<DocumentSnapshot>> callback)
    {
        CollectionReference colRef = db.Collection(collectionId);
        var task = colRef.GetSnapshotAsync();//colRef.WhereEqualTo("isOpen", true).GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, null);
        }
        else
        {
            callback(true, "", task.Result.Documents);
        }
    }


    private IEnumerator Enumerator_ExistUser(string username, System.Action<bool, string, FUser, LUser> callback)
    {
        var task = db.Collection("User").GetSnapshotAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        string sRet = "";
        bool bRet = false;
        if (task.Exception != null)
        {
            sRet = "Failed to get user list from firebase" + task.Exception;
            callback(false, sRet, null, null);
        }
        else
        {
            
            foreach(DocumentSnapshot snapshot in task.Result.Documents)
            {
                FUser user = snapshot.ConvertTo<FUser>();
                LUser lUser = JsonConvert.DeserializeObject<LUser>(user.User);
                if (lUser != null && lUser.UserName == username)
                {
                    bRet = true;
                    callback(true, "", user, lUser);
                    break;
                }
            }
            if (!bRet)
            {
                sRet = "There is no user called " + username;
                callback(false, sRet, null, null);
            }
        }
    }

    private IEnumerator Enumerator_ExistDocument(string collectionId, string docId, System.Action<bool, string> callback)
    {
        var task = db.Collection(collectionId).GetSnapshotAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        string sRet = "";
        bool bRet = false;
        if (task.Exception != null)
        {
            sRet = "Failed to get user list from firebase" + task.Exception;
            callback(false, sRet);
        }
        else
        {

            foreach (DocumentSnapshot snapshot in task.Result.Documents)
            {
                if (snapshot.Id == docId)
                {
                    bRet = true;
                    callback(true, "");
                    break;
                }
            }
            if (!bRet)
            {
                sRet = "There is no document id: " + docId;
                callback(false, sRet);
            }
        }
    }

    private IEnumerator Enumerator_DeleteDocument(string collectionId, string docId, System.Action<bool, string> callback)
    {
        var task = db.Collection(collectionId).Document(docId).DeleteAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            callback(false, "Failed to remove from firebase" + task.Exception);
        }
        else
        {
            callback(true, "");
        }
    }

    private IEnumerator Enumerator_GetInitData(string collectionId, System.Action<bool, string, IEnumerable<DocumentSnapshot>> callback)
    {
        var task = db.Collection(collectionId).GetSnapshotAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            callback(false, "Failed to get initial data from firebase" + task.Exception, null);
        }
        else
        {            
            callback(true, "", task.Result.Documents);
        }
    }

    private IEnumerator Enumerator_GetList<T>(string collectionId, string fieldName, string value, System.Action<bool, string, List<T>> callback) where T : FData
    {
        CollectionReference colRef = db.Collection(collectionId);
        var task = colRef.WhereEqualTo(fieldName, value).GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        List<T> objectList = new List<T>();

        if (task.Exception != null)
        {
            callback(false, "GetData encountered an error:" + task.Exception, objectList);
        }
        else
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
            {
                T data = documentSnapshot.ConvertTo<T>();
                objectList.Add(data);
            }
            callback(true, "", objectList);
        }
    }

    #endregion

}
