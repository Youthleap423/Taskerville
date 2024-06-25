using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using UnityEditor;
using UnityEngine.Networking;
using UIControllersAndData.Store.Categories.Buildings;
public class AdminLogin : MonoBehaviour
{
    [Space]
    [SerializeField] public InputField username_IF;
    [SerializeField] public InputField password_IF;
    [SerializeField] private Image backgroundImage;

    [SerializeField] public string userId_IF;

    #region Unity_Members
    // Start is called before the first frame update
    void Start()
    {
        /*LoadCSVFile();
        var item = DataManager.Instance.Artifact_Data.artworks[0];
        StartCoroutine(ImageLoader.Start(item.image_path, (sprite =>
        {
            backgroundImage.sprite = sprite;
        })));*/

        DownloadManager.instance.AddQueue("http://159.65.171.191:3000/uploads/artwork/1745204958.jpg", (path, texture) =>
        {
            Debug.LogError("Success to download: " + path);
            backgroundImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        });
    }
    #endregion

    #region Public_Members
    public void OnLogin()
    {
        FAuth.Instance.SignIn("ronniilene@aol.com", "isitg0nnaworkornot", (isSuccess, errMsg, userId) =>
        {
            if (isSuccess)
            {
                Debug.LogError("Ready To Manager Database!!!");
            }
            else
            {
                Debug.LogError(errMsg);
            }
        });
    }
    #endregion
    int index = 0;
    public void CreateInitialDatabase()
    {
        //UploadCBuildingData();
        //CreateABuildingDB();
    }

    private IEnumerator ECallPostAPI(string path, WWWForm form)
    {
        using UnityWebRequest www = UnityWebRequest.Post(path, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("ERROR" + www.downloadHandler.text == "" ? "Server Connection Error: " + www.error : www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Success" + www.downloadHandler.text);
        }
        www.Dispose();
    }

    private void LoadCSVFile()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("paintingvalues(final)");

        DataManager.Instance.Artifact_Data.artworks.Clear();
        for (var i = 0; i < data.Count; i++)
        {
            var name = data[i]["PAINTING_NAME"].ToString();
            var artist_name = data[i]["ARTIST_NAME"].ToString();
            var path = data[i]["PAINTING_IMAGE_PATH"].ToString();
            path = path.Remove(0, 14);
            path = path.Substring(0, path.Length - 4);

            var newArt = new CArtwork();
            newArt.id = i.ToString();
            newArt.name = name;
            newArt.artist_name = artist_name;
            newArt.contactInfo = "www.enelsonduran.com";
            newArt.imageURL = path.Trim();

            DataManager.Instance.Artifact_Data.artworks.Add(newArt);
            //print("PAINTING_NAME " + name + " " +
            //       "ARTIST_NAME " + artist_name + " " +
            //       "PAINTING_IMAGE_PATH " + path);
        }
    }

    private void CreateResourceDB()
    {
        string[] resourceNames = Enum.GetNames(typeof(EResources));
        List<FResource> resList = new List<FResource>();
        foreach (string resName in resourceNames)
        {
            FResource res = new FResource();
            res.type = resName;
            res.collectionId = "Init_Resource";
            res.Id = resName;
            resList.Add(res);
        }
    }
}

public class WResponse
{
    public string type = "";
    public string errcode = "";
    public string errmsg = "";
    public WResponse()
    {
        type = "";
        errcode = "";
        errmsg = "";
    }
}





