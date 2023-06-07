using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtworkSystem : SingletonComponent<ArtworkSystem>
{
    public event System.Action<bool, LArtwork, string> artwork_picked = delegate { };
    private List<string> _artistList = new List<string>();
    private List<FArtTrade> _fArtTrades = new List<FArtTrade>();
    public string selectedArtist = ""; 
         
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        
    }

    private void Test()
    {
        var resData = DataManager.Instance.Artifact_Data.artworks;
        var cartwork = resData.Find(item => item.id == "423");
        var lartwork = new LArtwork(cartwork, EArtworkReason.Trade.ToString());
        UpdateArt(lartwork);
        artwork_picked(true, lartwork, "");
    }

    private CArtwork SelectArtwork()
    {
        var allData = GetAllArtworks();

        var resData = DataManager.Instance.Artifact_Data.artworks;
        
        var selectableData = new List<CArtwork>();

        foreach (CArtwork data in resData)
        {
            if (!allData.Exists(item => item.id == data.id))
            {
                selectableData.Add(data);
            }
        }

        if (selectableData.Count == 0)
        {
            return null;
        }

        var rndIndex = Random.Range(0, selectableData.Count - 1);
        return selectableData.ElementAt(rndIndex);
    }

    public void CheckArtwork()
    {
        var allData = GetAllArtworks();
        
        if (allData.Count == 0)
        {
            Pick(EArtworkReason.Init);
        }
    }

    public void CheckHappinessMilestone(float happyValue)
    {
        switch (happyValue)
        {
            case float f when (f >= 65.0 && f < 70.0):
                Pick(EArtworkReason.Happy65);
                break;
            case float f when (f >= 70.0 && f < 75.0):
                Pick(EArtworkReason.Happy70);
                break;
            case float f when (f >= 75.0 && f < 80.0):
                Pick(EArtworkReason.Happy75);
                break;
            case float f when (f >= 80.0 && f < 85.0):
                Pick(EArtworkReason.Happy80);
                break;
            case float f when (f >= 85.0 && f < 90.0):
                Pick(EArtworkReason.Happy85);
                break;
            case float f when (f >= 90.0 && f < 95.0):
                Pick(EArtworkReason.Happy90);
                break;
            case float f when (f >= 95.0 && f < 100.0):
                Pick(EArtworkReason.Happy95);
                break;
            case float f when (f == 100.0):
                Pick(EArtworkReason.Happy100);
                break;
            default:
                break;
        }
    }

    public void CheckPopulationMilestone(int population)
    {
        switch (population)
        {
            case int f when (f >= 65 && f < 75):
                Pick(EArtworkReason.Population65);
                break;
            case int f when (f >= 75 && f < 85):
                Pick(EArtworkReason.Population75);
                break;
            case int f when (f >= 85 && f < 95):
                Pick(EArtworkReason.Population85);
                break;
            case int f when (f >= 95 && f < 100):
                Pick(EArtworkReason.Population95);
                break;
            case int f when (f >= 100):
                Pick(EArtworkReason.Population100);
                break;
            default:
                break;
        }
    }

    public void CheckBuildingMilestone(string bID, List<LVillager> villagers = null)
    {
        switch (bID)
        {
            case "95":
                if (villagers != null && villagers.Find(item => item.id == "23") != null)
                {
                    Pick(EArtworkReason.Build_Gallery);
                }
                break;
            case "94":
                if (villagers != null && villagers.Find(item => item.id == "23") != null)
                {
                    Pick(EArtworkReason.Build_Museum);
                }
                break;
            case "100":
                if (villagers != null && villagers.Find(item => item.id == "37") != null)
                {
                    Pick(EArtworkReason.Build_Religious);
                }
                break;
            case "183":
                if (villagers != null && villagers.Find(item => item.id == "22") != null)
                {
                    Pick(EArtworkReason.Build_HerbGarden);
                }
                break;
            case "99":
                Pick(EArtworkReason.Build_Park);
                break;
            case "71":
                Pick(EArtworkReason.Build_Cemetery);
                break;
            default:
                break;

        }
    }

    public void CheckBuildingMilestone(List<LVillager> villagers)
    {

    }


    public void Pick(EArtworkReason reason)
    {
        var allData = GetAllArtworks();
        var resData = DataManager.Instance.Artifact_Data.artworks;

        if (allData.Count == resData.Count)
        {
            artwork_picked(false, null, "You've got all artworks.");
            return;
        }

        if (reason != EArtworkReason.Buy && reason != EArtworkReason.Trade && reason != EArtworkReason.WeeklyTaskComplete)
        {
            foreach (LArtwork work in allData)
            {
                if (work.reason == reason.ToString())
                {
                    return;
                }
            }
        }

        var cartwork = SelectArtwork();
        var lartwork = new LArtwork(cartwork, reason.ToString());
        UpdateArt(lartwork);
        DataManager.Instance.UpdateResource(EResources.Culture, 1.0f);
        artwork_picked(true, lartwork, "");

        var artCount = 0;
        foreach (LArtwork artwork in GetAllArtworks())
        {
            var cArtwork = GetCArtwork(artwork);
            if (cartwork.artist_name == cArtwork.artist_name)
            {
                artCount = artCount + 1;
            }
        }

        if (artCount % 4 == 0)
        {
            int nTime = artCount / 4;
            DataManager.Instance.UpdateResource(EResources.Culture, (float)(nTime * 16));
            DataManager.Instance.UpdateResource(EResources.Ruby, 2.0f);
        }
    }

    public void Trade(FArtTrade artTrade)
    {
        CArtwork tradedCArtwork = DataManager.Instance.Artifact_Data.artworks.Find(item => item.artist_name == artTrade.artistName1 && item.name == artTrade.painting1);
        LArtwork tradedLArtwork = GetAllArtworks().Find(item => item.id == tradedCArtwork.id);
        RemoveArt(tradedLArtwork);

        CArtwork cArtwork = DataManager.Instance.Artifact_Data.artworks.Find(item => item.artist_name == artTrade.artistName2 && item.name == artTrade.painting2);
        var lartwork = new LArtwork(cArtwork, EArtworkReason.Trade.ToString());
        UpdateArt(lartwork);
        artwork_picked(true, lartwork, "");
    }

    public CArtwork GetCArtwork(LArtwork lArtwork)
    {
        var allCArtData = DataManager.Instance.Artifact_Data.artworks;

        return allCArtData.Find(item => item.id == lArtwork.id);
    }

    public List<LArtwork> GetAllArtworks()
    {
        return DataManager.Instance.CurrentArtworks.ToList();
    }

    public List<CArtwork> GetAllCArtworks()
    {
        var result = new List<CArtwork>();
        foreach(LArtwork artwork in DataManager.Instance.CurrentArtworks.ToList())
        {
            result.Add(GetCArtwork(artwork));
        }

        return result;
    }

    public List<CArtwork> GetSelectedCArtworks(string artist)
    {
        var allArtworks = GetAllCArtworks().ToList();
        if (selectedArtist == "")
        {
            return allArtworks;
        }

        return allArtworks.FindAll(item => item.artist_name == artist);
    }

    public List<string> GetAllArtists()
    {
        if (_artistList.Count > 0)
        {
            return _artistList;
        }
        foreach(CArtwork cArtwork in DataManager.Instance.Artifact_Data.artworks)
        {
            if (cArtwork.artist_name == "Willem van Leen")
            {
                Debug.LogError(cArtwork.name + ":" + cArtwork.id);

            }

            if (!_artistList.Contains(cArtwork.artist_name))
            {
                _artistList.Add(cArtwork.artist_name);
            }
        }

        _artistList.Sort();

        return _artistList;
    }
    
    public void UpdateArt(LArtwork artwork)
    {
        DataManager.Instance.UpdateArt(artwork);
    }

    public void RemoveArt(LArtwork artwork)
    {
        DataManager.Instance.UpdateArt(artwork);
    }

    /// <summary>
    /// Art Trade
    /// </summary>
    public void LoadArtTrades(System.Action<bool, string> callback)
    {
        //if (_fArtTrades.Count > 0)
        //{
        //    callback(true, "");
        //    return;
        //}
        DataManager.Instance.GetDataList<FArtTrade>("ArtTrade", (isSuccess, errMsg, tradeList) =>
        {
            if (isSuccess)
            {
                _fArtTrades = tradeList.ToList();
            }
            callback(isSuccess, errMsg);
        });
    }

    public void LoadArtTrade(System.Action<bool, string, List<FArtTrade>> callback)
    {
        DataManager.Instance.GetDataList("ArtTrade", "Pid", UserViewController.Instance.GetCurrentUser().id, callback);
    }

    public List<FArtTrade> GetAllArtTrades()
    {
        return _fArtTrades;
    }

    public List<FArtTrade> GetSelectedArtTrades()
    {
        return _fArtTrades.FindAll(item => item.artistName1 == selectedArtist).ToList();
    }

    public void RemoveArtTrade(FArtTrade trade, System.Action<bool, string> callback)
    {
        DataManager.Instance.RemoveData(trade, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                LoadArtTrades(callback);
            }
            else
            {
                callback(isSuccess, errMsg);
            }
        });
    }

    public void PostToNexus(LArtwork artwork, string seekArtistName, System.Action<bool, string> callback)
    {
        var cArtwork = GetCArtwork(artwork);
        var newPost = new FArtTrade(cArtwork);
        var currentUser = UserViewController.Instance.GetCurrentUser();
        newPost.Pid = currentUser.id;
        newPost.artistName2 = seekArtistName;
        newPost.readers.Add(currentUser.GetFullName());
        DataManager.Instance.CreateTrade(newPost, callback);
    }

}
