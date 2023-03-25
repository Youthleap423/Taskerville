using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : SingletonComponent<TaskManager>
{
    // Start is called before the first frame update
    private bool isupdating = false;
    void Start()
    {
        UnityEngine.Screen.orientation = ScreenOrientation.Portrait;
        AppManager.Instance.singedIn = true;
        UIManager.Instance.UpdateTopProfile();
        CheckAllDailyUpdate();
        AppManager.Instance.HandleAIPlayer();
        AudioManager.Instance.FadeOut();
    }

    public void CheckAllDailyUpdate()
    {
        if (isupdating)
        {
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        isupdating = true;
        DataManager.Instance.SerializeUser(false, (isSuccess, err) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            TaskViewController.Instance.CheckDaily();
            ResourceViewController.Instance.CheckDailyMission();
            TradeViewController.Instance.CheckTrades();
            //ArtifactSystem.Instance.CheckArtifacts();
            ArtworkSystem.Instance.CheckArtwork();
            isupdating = false;
        });
    }

    private void OnApplicationFocus(bool focus)
    {
#if !UNITY_EDITOR
        if (focus)
        {
            CheckAllDailyUpdate();
        }
#endif
    }

    public void LoadGameScene()
    {
        AppManager.Instance.LoadGameScene();
    }

    
    private void OnDestroy()
    {
        
    }
}
