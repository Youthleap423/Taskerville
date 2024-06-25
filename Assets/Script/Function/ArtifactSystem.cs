using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtifactSystem : SingletonComponent<ArtifactSystem>
{
    private int completeMins = 720;
    private LArtifact currentArtifact = null;
    private float currentProgress = 0f;

    private List<CArtifact> allArtifacts = new List<CArtifact>();
    // Start is called before the first frame update
    void Start()
    {
        LoadArtifacts();
        currentArtifact = GetCurrentArtifact();
    }

    private List<CArtifact> GetAllCArtifacts()
    {
        if (allArtifacts.Count == 0)
        {
            LoadArtifacts();
        }
        return allArtifacts;
    }

    private void LoadArtifacts()
    {
        DataManager.Instance.LoadCArtifacts((isSuccess, errMsg, list) =>
        {
            if (isSuccess)
            {
                allArtifacts = list;
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    void Update()
    {
        /*
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
        }*/
    }

    private CArtifact SelectArtifact(EResources type)
    {
        var allData = GetAllArtifacts();

        var resData = GetAllCArtifacts().FindAll(item => item.type == type).ToList() ;
        
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

    public void Excavate(System.Action<bool> callback = null)
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.Excavate((isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                currentArtifact = GetCurrentArtifact();
            }
            callback?.Invoke(isSuccess);
        });
    }

    public List<LArtifact> GetAllArtifacts()
    {
        return DataManager.Instance.CurrentArtifacts.ToList();
    }

    public CArtifact GetCArtifact(LArtifact lArtifact)
    {
        return GetAllCArtifacts().Find(item => item.id == lArtifact.id);
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

    public int GetRemainDaysUntilAvailable()
    {
        var allArtifacts = GetAllArtifacts();
        try
        {
            var lastArtifact = allArtifacts.OrderBy(item => item.created_at).Last();

            var today = System.DateTime.Now.Date;
            var lastDate = Convert.DetailedStringToDateTime(lastArtifact.created_at);
            var days = (today - lastDate).Days;

            return Mathf.Max(0, 7 - days);
        }
        catch
        {
            return -1;
        }
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

    public void CompleteExcavate(LArtifact artifact)
    {
        DataManager.Instance.CompleteExcavate(artifact.id, (isSuccess, errMsg) =>
        {
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.CompletedExcavate();
                }
                UIManager.Instance.ShowExcavationDlg();
            }
        });
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
