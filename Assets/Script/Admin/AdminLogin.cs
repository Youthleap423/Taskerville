using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using UnityEditor;

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
        LoadCSVFile();
        var item = DataManager.Instance.Artifact_Data.artworks[0];
        StartCoroutine(ImageLoader.Start(item.image_path, (sprite =>
        {
            backgroundImage.sprite = sprite;
        })));
    }
    #endregion

    #region Public_Members
    public void OnLogin()
    {
        FAuth.Instance.SignIn("test@test.com", "123456", (isSuccess, errMsg, userId) =>
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
    
    public void CreateInitialDatabase()
    {
        UpdateDB();
        //CreateABuildingDB();
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
            newArt.image_path = path.Trim();

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
        FirestoreManager.Instance.createDataList(resList, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                Debug.LogError("Success!!!");
            }
            else
            {
                Debug.LogError(errMsg);
            }
        });
    }

    private void CreateBuildingDB()
    {
        string[] buildingNames = Enum.GetNames(typeof(EBuildingType));
        List<FBuilding> buildingList = new List<FBuilding>();
        foreach (string buildingName in buildingNames)
        { 
            FBuilding building = new FBuilding();
            building.type = buildingName;
            building.collectionId = "Init_Building";
            building.Id = buildingName;
            buildingList.Add(building);
        }
        FirestoreManager.Instance.createDataList(buildingList, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                Debug.LogError("Success!!!");
            }
            else
            {
                Debug.LogError(errMsg);
            }
        });
    }

    private void CreateABuildingDB()
    {
        List<FBuilding> buildingList = new List<FBuilding>();
        
            FBuilding building = new FBuilding();
            building.type = EBuildingType.Obelisk.ToString();
            building.collectionId = "Init_Building";
            building.Id = "Obelisk";
            buildingList.Add(building);
        
        FirestoreManager.Instance.createDataList(buildingList, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                Debug.LogError("Success!!!");
            }
            else
            {
                Debug.LogError(errMsg);
            }
        });
    }

    private void CreateVillagerDB()
    {
        string[] villagerNames = Enum.GetNames(typeof(EVillagerType));
        List<FVillager> villagerList = new List<FVillager>();
        foreach (string villagerName in villagerNames)
        {
            FVillager villager = new FVillager();
            villager.type = villagerName;
            villager.collectionId = "Init_Villager";
            villager.Id = villagerName;
            villagerList.Add(villager);
        }
        FirestoreManager.Instance.createDataList(villagerList, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                Debug.LogError("Success!!!");
            }
            else
            {
                Debug.LogError(errMsg);
            }
        });
    }

    private void AddToVillagerDB()
    {
        string[] villagerNames = new string[] { EVillagerType.Charger.ToString(), EVillagerType.Merchant.ToString()};
        List<FVillager> villagerList = new List<FVillager>();
        foreach (string villagerName in villagerNames)
        {
            FVillager villager = new FVillager();
            villager.type = villagerName;
            villager.collectionId = "Init_Villager";
            villager.Id = villagerName;
            villagerList.Add(villager);
        }
        FirestoreManager.Instance.createDataList(villagerList, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                Debug.LogError("Success!!!");
            }
            else
            {
                Debug.LogError(errMsg);
            }
        });
    }

    private void UpdateDB()
    {
        List<FBuilding> fResources = new List<FBuilding>();
        FirestoreManager.Instance.GetInitData("Init_Building", (isSuccess, errMsg, snapshotList) =>
        {
            if (isSuccess)
            {
                long index = 0;
                foreach (DocumentSnapshot snapshot in snapshotList)
                {
                    FBuilding resource = snapshot.ConvertTo<FBuilding>();
                    resource.Pid = "FEbTT5jNcUVHN0CLUv3v7xLJviy1";
                    resource.Id = "FEbTT5jNcUVHN0CLUv3v7xLJviy1" + "_" + resource.Id;
                    resource.collectionId = "Buildings";
                    fResources.Add(resource);
                }

                FirestoreManager.Instance.createDataList<FBuilding>(fResources, (isSuccess, errMsg) =>
                {
                    Debug.LogError("Success:>>>" + isSuccess.ToString());
                });
            }
            Debug.LogError(errMsg);
        });
    }
}
