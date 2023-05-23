using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtifactSystem : SingletonComponent<ArtifactSystem>
{
    [HideInInspector]public Dictionary<EResources, float> artifactRes = new Dictionary<EResources, float>();

    private int completeMins = 720;
    private LArtifact currentArtifact = null;
    private float currentProgress = 0f;
    // Start is called before the first frame update
    void Start()
    {
        artifactRes.Add(EResources.Gold, -30f);
        artifactRes.Add(EResources.Meal, -60f);
        artifactRes.Add(EResources.Lumber, -2f);
        artifactRes.Add(EResources.Iron, -2f);
        artifactRes.Add(EResources.Stone, -2f);

        currentArtifact = GetCurrentArtifact();
    }

    void Update()
    {
        if (currentArtifact != null)
        {
            var mins = (float)((System.DateTime.Now - Convert.DetailedStringToDateTime(currentArtifact.created_at)).TotalMinutes);
            currentProgress = mins / completeMins;
            if (currentProgress >= 1.0f)
            {
                CompleteExcavate(currentArtifact);
            }
        }
        else
        {
            currentProgress = 0f;
        }
    }

    private CArtifact SelectArtifact(EResources type)
    {
        var allData = GetAllArtifacts();

        var resData = DataManager.Instance.Artifact_Data.commonList;
        if (type == EResources.Artifact_UnCommon)
        {
            resData = DataManager.Instance.Artifact_Data.uncommonList;
        }
        else if (type == EResources.Artifact_Rare)
        {
            resData = DataManager.Instance.Artifact_Data.rareList;
        }

        var selectableData = new List<CArtifact>();

        foreach (CArtifact data in resData)
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

    private void AutoChangeFromCommonToUnCommon()
    {
        var allData = GetAllArtifacts().FindAll(item => item.progress >= 1.0f);
        var commonData = allData.FindAll(item => int.Parse(item.id) <= 150);
        if (commonData.Count >= 4)
        {
            var unCommonArtifact = SelectArtifact(EResources.Artifact_UnCommon);
            if (unCommonArtifact != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    commonData[i].isExchanged = true;
                    UpdateArt(commonData[i]);
                }
            }
            var lartifact = new LArtifact(unCommonArtifact);

            lartifact.created_at = Convert.DateTimeToDetailedString(System.DateTime.Now);
            lartifact.progress = 1.0f;
            UpdateArt(lartifact);
        }
        AutoChangeFromUnCommonToRare();
    }

    private void AutoChangeFromUnCommonToRare()
    {
        var allData = GetAllArtifacts().FindAll(item => item.progress >= 1.0f);
        var uncommonData = allData.FindAll(item => int.Parse(item.id) >= 151 && int.Parse(item.id) <= 250);
        if (uncommonData.Count >= 2)
        {
            var rareArtifact = SelectArtifact(EResources.Artifact_Rare);
            if (rareArtifact != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    uncommonData[i].isExchanged = true;
                    UpdateArt(uncommonData[i]);
                }
            }
            var lartifact = new LArtifact(rareArtifact);

            lartifact.created_at = Convert.DateTimeToDetailedString(System.DateTime.Now);
            lartifact.progress = 1.0f;
            UpdateArt(lartifact);
        }

    }

    public void CheckArtifacts()
    {
        var allData = GetAllArtifacts().FindAll(item => item.progress < 1.0f);
        if (allData.Count == 0)
        {
            CheckAutoExcavate();
            return;
        }

        foreach(LArtifact artifact in allData)
        {
            var mins = (float)((System.DateTime.Now - Convert.DetailedStringToDateTime(artifact.created_at)).TotalMinutes);
            var progress = mins / completeMins;
            if (progress >= 1.0f)
            {
                CompleteExcavate(artifact);
                UIManager.Instance.ShowExcavationDlg();
            }
        }
        
    }

    public float GetCurrentProgress()
    {
        return currentProgress;
    }

    public void Excavate()
    {
        var allData = GetAllArtifacts();

        if (allData.Count == 300)
        {
            UIManager.Instance.ShowErrorDlg("You've excavated all artifacts.");
            return;
        }

        var commonData = allData.FindAll(item => int.Parse(item.id) <= 150);

        var cartifact = new CArtifact();
        
        if (commonData.Count >= 20 && commonData.Count % 20 == 0)
        {
            if (commonData.Count >= 100 && commonData.Count % 100 == 0)
            {
                cartifact = SelectArtifact(EResources.Artifact_Rare);
            }
            else
            {
                cartifact = SelectArtifact(EResources.Artifact_UnCommon);
            }
        }else 
        {
            cartifact = SelectArtifact(EResources.Artifact_Common);
        }

        if (cartifact == null)
        {
            UIManager.Instance.ShowErrorDlg("We've not found which artifact to excavate");
            return;
        }

        var lartifact = new LArtifact(cartifact);

        lartifact.created_at = Convert.DateTimeToDetailedString(System.DateTime.Now);

        if (ResourceViewController.Instance.CheckResource(artifactRes) == false)
        {
            UIManager.Instance.ShowErrorDlg("Not enough resources to excavate an artifact.");
            return;
        }

        ResourceViewController.Instance.UpdateResource(artifactRes, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                UpdateArt(lartifact);
                //BuildManager.Instance.ExcavateArchealogicalDig(lartifact.dig);
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public List<LArtifact> GetAllArtifacts()
    {
        return DataManager.Instance.CurrentArtifacts.ToList();
    }

    public CArtifact GetCArtifact(LArtifact lArtifact)
    {
        var allCArtData = DataManager.Instance.Artifact_Data.commonList;
        allCArtData.AddRange(DataManager.Instance.Artifact_Data.uncommonList);
        allCArtData.AddRange(DataManager.Instance.Artifact_Data.rareList);
        
        return allCArtData.Find(item => item.id == lArtifact.id);
    }

    public LArtifact GetLastestCompletedArtifact()
    {
        var items = GetAllArtifacts().FindAll(item => item.isExchanged == false && item.progress >= 1.0f).ToList().OrderBy(item => item.created_at);
        return items.Last();
    }

    public LArtifact GetCurrentArtifact()
    {
        var result = DataManager.Instance.CurrentArtifacts.ToList().Find(item => item.progress < 1f);
        return result;
    }

    public EActionState IsReadyForNewArtifact(bool auto = false)
    {
        if (GetCurrentArtifact() != null)
        {
            return EActionState.Progess;
        }

        var allArtifacts = GetAllArtifacts();

        if (allArtifacts.Count == 0)
        {
            return EActionState.Ready;
        }

        //return EActionState.Ready;//TEST case

        var lastArtifact = allArtifacts.OrderBy(item => item.created_at).Last();
        if (lastArtifact == null)
        {
            return EActionState.Ready;
        }

        var today = System.DateTime.Now.Date;
        var lastDate = Convert.DetailedStringToDateTime(lastArtifact.created_at);
        var days = (today - lastDate).Days;

        
        if (!auto) {
            return days >= 7 ? EActionState.Ready : EActionState.Wait;
        }

        //auto mode -- excavate artifact per month 
        var lastDayOfMonth = new System.DateTime(today.Year, today.Month, System.DateTime.DaysInMonth(today.Year, today.Month));
        if (today == lastDayOfMonth.Date)
        {
            return EActionState.Ready;
        }

        if (today.Month != lastDate.Month)
        {
            return EActionState.Ready;
        }

        return EActionState.Wait;
    }

    public void UpdateArt(LArtifact artifact)
    {
        UserViewController.Instance.GetCurrentUser().updateDates(EDates.Artifact.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
        DataManager.Instance.UpdateArt(artifact);
        currentArtifact = GetCurrentArtifact();
    }

    public void CompleteExcavate(LArtifact artifact)
    {
        artifact.Completed();
        UpdateArt(artifact);
        AutoChangeFromCommonToUnCommon();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompletedExcavate();
        }
        UIManager.Instance.ShowExcavationDlg();
    }

    public void CheckAutoExcavate()
    {
        
        if (AppManager.Instance.GetCurrentMode() == Game_Mode.Task_Only)
        {
            var allAritifacts = GetAllArtifacts();
            if (allAritifacts.Count == 0)
            {
                Excavate();
                return;
            }
        }

        if (IsReadyForNewArtifact(true) == EActionState.Ready)
        {
            Excavate();
        }
    }
}
