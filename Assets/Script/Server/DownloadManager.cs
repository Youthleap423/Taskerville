using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void DownloadCallback(string path, Texture2D texture);

public class DownloadManager : MonoBehaviour
{
    public static DownloadManager instance;
    private Queue<DownloadJob> downloadQueue = new Queue<DownloadJob>();
    public Dictionary<string, Texture2D> imageData = new Dictionary<string, Texture2D>();
    private int multiThreadCount = 1;
    private int currentThreadCount = 0;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        currentThreadCount = 0;
    }
    public void AddQueue(string path, DownloadCallback callback)
    {
        var job = new DownloadJob(path, callback);
        downloadQueue.Enqueue(job);
    }

    private void Update()
    {
        if (downloadQueue.Count == 0)
        {
            return;
        }

        if (multiThreadCount >= currentThreadCount)
        {
            StartCoroutine(EDownloadImage());
        }
    }
    private IEnumerator EDownloadImage()
    {
        currentThreadCount++;

        DownloadJob job = downloadQueue.Dequeue() as DownloadJob;
        if (job == null)
        {
            currentThreadCount--;
            yield break;
        }

        if (imageData.ContainsKey(job.path))
        {
            job.callback(job.path, imageData[job.path]);
            currentThreadCount--;
            yield break;
        }
        //var compressPath = "https://api.adminpanel.ruffyworld.com/image?url=https://ruffy.mypinata.cloud/ipfs/QmUr83MZNr7sfYnWMgVbLR62CnF75rykYCTWfZe5Bivt7G/MetaRuffy_Land_Plot.jpeg&w=640&q=5";// string.Format("https://api.adminpanel.ruffyworld.com/image?url={0}&w=320&q=20", job.path);
        //UnityWebRequest www = UnityWebRequestTexture.GetTexture(compressPath);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(job.path);
        www.timeout = 90;
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.downloadHandler.text);
            currentThreadCount--;
            job.callback(job.path, null);
            yield break;
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture; //DownloadHandlerTexture.GetContent(www);
     
            yield return new WaitForEndOfFrame();

            if (!imageData.ContainsKey(job.path))
            {
                imageData.Add(job.path, texture);
            }
            job.callback(job.path, imageData[job.path]);
            Resources.UnloadUnusedAssets();
            currentThreadCount--;
        }
        www.Dispose();
    }

    /* var compressPath = string.Format("https://api.adminpanel.ruffyworld.com/image?url={0}&w=320&q=20", job.path);
        UnityWebRequest www = UnityWebRequest.Get(compressPath);
        www.timeout = 90;

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            currentThreadCount--;
            yield break;
        }
        else
        {
            Texture2D texture = new Texture2D(2, 2);
            try
            {               
                texture.LoadImage(System.Convert.FromBase64String(www.downloadHandler.text));
                texture.Apply();
            }
            catch
            {
                currentThreadCount--;
                yield break;
            }

            yield return new WaitForEndOfFrame();
            
            //texture.Compress(true);
            //texture.Apply();

    */
}

public sealed class DownloadJob
{
    public string path { get; set; }
    public DownloadCallback callback { get; set; }

    public DownloadJob(string path, DownloadCallback callback) {
        this.path = path;
        this.callback = callback;
    }
}
