using UnityEngine;
using System.Collections;
using Firebase.Firestore;

public class FirestorageManager : SingletonComponent<FirestorageManager>
{
    [SerializeField] private Firebase.Storage.FirebaseStorage storage;
    
    
    // Use this for initialization
    void Start()
    {
        storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DownloadAllAvatarImages()
    {
        Firebase.Storage.StorageReference avatar_storage_ref = storage.GetReference("Avatar");
        
    }

    public void DownloadInitDBFile()
    {
        /*
        Firebase.Storage.StorageReference db_reference = storage.GetReferenceFromUrl("gs://carilson-designs.appspot.com/DB/taskerville_test.db");
        string localDBPath = Application.streamingAssetsPath + "/test.db";

        db_reference.GetFileAsync(localDBPath).ContinueWith(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.LogError("FileDownloaded");
            }
            else
            {
                Debug.LogError("File Download Error: " + task.Exception.ToString());
            }
        });
        */
    }

    public void UploadFile()
    {
        //byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
        ////Editing Metadata
        //var newMetadata = new MetadataChange();
        //newMetadata.ContentType = "image/jpeg";
        //Firebase.Storage.StorageReference storageReference = storage.GetReferenceFromUrl("gs://carilson-designs.appspot.com");
        //Firebase.Storage.StorageReference uploadRef = storageReference.Child("Artifact/newFile.jpeg");
        //Debug.Log("File upload started");
        //uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) => {
        //    if (task.IsFaulted || task.IsCanceled)
        //    {
        //        Debug.Log(task.Exception.ToString());
        //    }
        //    else
        //    {
        //        Debug.Log("File Uploaded Successfully!");
        //    }
        //});
    }
}
