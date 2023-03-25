using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : SingletonComponent<GameManager>
{
    private string lastSceneName = "";

    [Header("Roate Screen Tip")]
    [SerializeField] private CanvasGroup rotateInfoCG;
    [SerializeField] private CanvasGroup constructCG;
    [SerializeField] private CanvasGroup uniqueConstructCG;
    [Space]
    [Header("Loading...")]
    [SerializeField] private CanvasGroup loadingCG;
    [SerializeField] private Text loadingTF;
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Screen.orientation = ScreenOrientation.AutoRotation;
        Debug.LogError(">>>>>>>>>>>>>>>>>1");
        loadingCG.alpha = 1f;
        loadingCG.blocksRaycasts = false;
        loadingCG.interactable = false;

        rotateInfoCG.alpha = 1;
        StartCoroutine(FadeOut(rotateInfoCG, 0.5f, 3f));
        BuildManager.Instance.LoadBuildings();
        //AppManager.Instance.singedIn = true;
        //UIManager.Instance.UpdateTopProfile();
        //CheckAllDailyUpdate();
        //BuildManager.Instance.CheckVisibility();


        if (AppManager.Instance.GetCurrentMode() == Game_Mode.Task_Only)
        {
            constructCG.alpha = 0.4f;
            constructCG.blocksRaycasts = false;
            constructCG.interactable = false;
            uniqueConstructCG.alpha = 0.4f;
            uniqueConstructCG.blocksRaycasts = false;
            uniqueConstructCG.interactable = false;
        }
        else
        {
            constructCG.alpha = 1.0f;
            constructCG.blocksRaycasts = true;
            constructCG.interactable = true;
            uniqueConstructCG.alpha = 1.0f;
            uniqueConstructCG.blocksRaycasts = true;
            uniqueConstructCG.interactable = true;
        }
        
        Resources.UnloadUnusedAssets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void CheckAllDailyUpdate()
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.SerializeUser(false, (isSuccess, err) =>
        {
            UIManager.Instance.ShowLoadingBar(false);

            TaskViewController.Instance.CheckDaily();
            ResourceViewController.Instance.CheckDailyMission();
            TradeViewController.Instance.CheckTrades();
            //ArtifactSystem.Instance.CheckArtifacts();
            ArtworkSystem.Instance.CheckArtwork();
        });
        
    }

    public void BuildingsLoaded()
    {
        StartCoroutine(FadeOut(1.5f));
    }

    IEnumerator FadeOut(float fadeTime)
    {
        Debug.LogError(">>>>>>>>>>>>>>>>>2");
        yield return new WaitForSeconds(1f);//asked by Duran

        while (loadingCG.alpha > 0)
        {
            loadingCG.alpha -= Time.deltaTime / fadeTime;
            yield return null;
        }
        loadingCG.blocksRaycasts = false;
        loadingCG.interactable = false;
        Debug.LogError(">>>>>>>>>>>>>>>>>3");
        AudioManager.Instance.PlayBackgroundSound(AudioManager.Instance.villagerClip);
        AudioManager.Instance.PlayFXSound(AudioManager.Instance.bellClip, false);
    }

    public void LoadTaskScene()
    {
        AppManager.Instance.LoadTaskScene();
    }

    IEnumerator FadeOut(CanvasGroup cg, float delayTime, float fadeTime)
    {
        yield return new WaitForSeconds(delayTime);

        while (cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime / fadeTime; ;
            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.FadeOut();
        }
    }
}
