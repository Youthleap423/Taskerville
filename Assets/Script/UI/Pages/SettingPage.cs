using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPage : Page
{
    [SerializeField] private GameObject dailyReportObj;
    [Space]
    [SerializeField] public Toggle gear_Toggle;

    [Header("Sound")]
    [SerializeField] public Slider bgSoundSlider;
    [SerializeField] public Slider fxSoundSlider;
    #region Unity Members
    // Use this for initialization
    void Start()
    {
        bgSoundSlider.onValueChanged.AddListener(delegate
        {
            AudioManager.Instance.SetBGAudioVolumn(bgSoundSlider.value);
        });

        fxSoundSlider.onValueChanged.AddListener(delegate
        {
            AudioManager.Instance.SetFXAudioVolumn(fxSoundSlider.value);
        });
    }

    private void OnEnable()
    {
        dailyReportObj.SetActive(false);
        bgSoundSlider.value = AudioManager.Instance.GetBGAudioVolumn();
        fxSoundSlider.value = AudioManager.Instance.GetFXAudioVolumn();
    }

    private void OnDisable()
    {
        Debug.Log("Setting Page Disabled");
    }
    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region Public Members
    public void OnAlarm()
    {
        transform.parent.GetComponent<NavPage>().Show("setting_alarm");
    }

    public void OnResetGame()
    {
        transform.parent.GetComponent<NavPage>().Show("setting_reset_game");
    }

    public void OnCyclopaedia()
    {
        transform.parent.GetComponent<NavPage>().Show("setting_cyclopaedia");
    }

    public void OnGamePlay()
    {
        transform.parent.GetComponent<NavPage>().Show("setting_gameplay");
    }

    public void OnCredit()
    {
        transform.parent.GetComponent<NavPage>().Show("setting_credit");
    }

    public void OnDailyReport()
    {
        dailyReportObj.SetActive(true);
    }

    public void OnChooseAvatar()
    {
        ScreenManager.Instance.Show("chooseAvatar");
    }

    public void OnSignout()
    {
        UIManager.Instance.ShowLoadingBar(true);
        UserViewController.Instance.SerializeUser(true, (isSuccess, err) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            AppManager.Instance.SignOut();
        });
    }

    public void OnBack()
    {
        gear_Toggle.isOn = false;
    }
    #endregion
}
