using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class UIManager : SingletonComponent<UIManager>
{
    [SerializeField] private GameObject profilePage;
    [SerializeField] private GameObject crestPage;
    [SerializeField] private GameObject loadingBar;

    [Header("Profile")]
    [SerializeField] private Text happinessTF;
    [SerializeField] private Text goldCoinTF;

    [Header("ErrorDlg")]
    [SerializeField] private GameObject errMsgObj;
    [SerializeField] private GameObject errMsgObjOnGame;

    [Header("Message")]
    [SerializeField] private GameObject messageObj;
    [SerializeField] private RectTransform message_scrollView_RectTransform;
    [SerializeField] private Transform messageGroup;

    [SerializeField] private GameObject backgroundObj;

    private bool bGameMode = false;
    private List<PopUpDlg> popupQuequ = new List<PopUpDlg>();
    #region Unity_Members
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();

        if (ArtworkSystem.Instance != null)
        {
            ArtworkSystem.Instance.artwork_picked += Instance_artwork_picked;
        }
    }

    void Start()
    {
        
    }

    private void OnDestroy()
    {
        if (ArtworkSystem.Instance != null)
        {
            ArtworkSystem.Instance.artwork_picked -= Instance_artwork_picked;
        }
    }

    private void Instance_artwork_picked(bool isSuccess, LArtwork artwork, string errMsg)
    {
        if (isSuccess && artwork != null)
        {
            PopUpManager.Instance.Add(EPopUpDlg.NewArtwork);
        }
    }
    #endregion

    #region Public_Members

    public void ShowBackground(bool isActive)
    {
        backgroundObj.SetActive(isActive);
    }

    public void ShowProfile(bool isActive)
    {
        if (isActive)
        {
            profilePage.GetComponent<ProfilePage>().SetUser(UserViewController.Instance.GetCurrentUser());
        }
        
        profilePage.SetActive(isActive);
    }

    public void ShowProfile(LUser user, bool isActive)
    {
        if (isActive)
        {
            profilePage.GetComponent<ProfilePage>().SetUser(user);
        }
        //if (user != null)
        //{
        //    profilePage.GetComponent<ProfilePage>().SetUser(user);
        //}
        profilePage.SetActive(isActive);
    }

    
    public void ShowCrest(bool isActive)
    {
        crestPage.SetActive(isActive);
    }

    public void ShowRewardMessage(string prefixStr)
    {
        ShowRewardMessage(prefixStr, "", null, 0);
    }

    public void ShowRewardMessage(string prefixStr, string suffixStr, Sprite sprite, float amount)
    {
        CreateNewMessage(prefixStr, suffixStr, sprite, amount);
        UpdateTopProfile();
    }

    public void UpdateTopProfile()
    {
        if (happinessTF != null)
        {
            happinessTF.text = string.Format("{0:0.0}%", ResourceViewController.Instance.GetCurrentResourceValue(EResources.Happiness));
        }

        if (goldCoinTF != null)
        {
            goldCoinTF.text = string.Format(" {0:0.0}", ResourceViewController.Instance.GetCurrentResourceValue(EResources.Gold));
        }   
    }

    public void ShowLoadingBar(bool isActive)
    {
        loadingBar.SetActive(isActive);
    }

    /// <summary>
    /// PopUp Dlgs
    /// </summary>
    /// <param name="obj"></param>
    public void ShowAdministratorPrompt(string strMsg, System.Action<bool> callback)
    {
        PopUpManager.Instance.ShowAdministratorPrompt(strMsg, callback);
    }

    public void ShowUncheckedTaskDlg()
    {
        PopUpManager.Instance.Add(EPopUpDlg.MissingTask);
    }

    public void ShowReport()
    {
        PopUpManager.Instance.Add(EPopUpDlg.DailyReport);
    }

    public void ShowExcavationDlg()
    {
        Debug.LogError("Popup>>>");
        PopUpManager.Instance.Add(EPopUpDlg.NewArtifact);
    }

    /// <summary>
    /// Error
    /// </summary>
    /// <param name="obj"></param>

    public void ShowErrorDlg(string errMsg)
    {

        //TODO
        if (AppManager.Instance.currentScene == "GameScene")
        {
            //if (AppManager.Instance.GetCurrentMode() == Game_Mode.Task_Only)//disabled for quick build dlg to show in this mode 
            //{
            //    return;
            //}
            errMsgObjOnGame.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = errMsg;
            errMsgObjOnGame.SetActive(true);
            return;
        }

        errMsgObj.GetComponentInChildren<TMP_Text>().text = errMsg;
        if (errMsgObj.activeSelf)
        {
            CancelInvoke("HideErrorDlg");
        }
        else
        {
            errMsgObj.SetActive(true);
        }
        Invoke("HideErrorDlg", 3.0f);
        //Debug.LogError(errMsg);
    }

    public static void LogError(object obj)
    {
        LogError(obj.ToString());
    }
    public static void LogError(string msg)
    {
#if UNITY_EDITOR
        Debug.LogError(msg);
#endif
    }
#endregion

#region Private_Members

    private void CreateNewMessage(string prefixStr, string suffixStr, Sprite sprite, float amount)
    {
        GameObject newObj = GameObject.Instantiate(messageObj, messageGroup);
        newObj.GetComponent<Log_Message_Item>().SetMessage(prefixStr, suffixStr, sprite, amount);
        newObj.GetComponent<Log_Message_Item>().setDelayTime(messageGroup.childCount);
        //AdjustSubTaskScrollViewRect();
    }

    private void AdjustSubTaskScrollViewRect()
    {
        Vector2 sizeDelta = message_scrollView_RectTransform.sizeDelta;
        int count = Convert.Min(messageGroup.childCount, 4);
        message_scrollView_RectTransform.sizeDelta = new Vector2(sizeDelta.x, (float)(count * 100.0f));
        message_scrollView_RectTransform.offsetMax = Vector2.zero; //right-top
        message_scrollView_RectTransform.offsetMin = Vector2.zero; //left-bottom
        StartCoroutine(AdjustSubTaskScrollView());
    }

    private IEnumerator AdjustSubTaskScrollView()
    {
        yield return new WaitForEndOfFrame();

        message_scrollView_RectTransform.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 0.0f;
        message_scrollView_RectTransform.gameObject.GetComponent<ScrollRect>().verticalScrollbar.value = 1;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }

    private void HideErrorDlg()
    {
        errMsgObj.SetActive(false);
    }

#endregion


}

