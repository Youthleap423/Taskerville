using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommunicationPage : Page
{
    [Space]
    [Header("Join Coalition")]
    [SerializeField] private InputField coalition_search_IF;
    [SerializeField] private TMP_Text coalition_search_tint_TF;

    [Space]
    [Header("Invite Player")]
    [SerializeField] private InputField player_search_IF;
    [SerializeField] private CanvasGroup invitation_CG;

    [Space]
    [Header("Create Coalition")]
    [SerializeField] private InputField coalition_creation_IF;
    [SerializeField] private CanvasGroup startBTN_CG;

    [Space]
    [SerializeField] private GameObject leave_coalition_BtnObj;

    [Space]
    [SerializeField] private GameObject artExchangePageObj;
    private FCoalition coalition = null;
    #region Unity Members
    // Start is called before the first frame update
    void Start()
    {
        //coalition_search_IF.shouldHideMobileInput = true;

        //coalition_creation_IF.onValueChanged.AddListener(delegate {
        //    CoalitionSearchIndexChanged();
        //});
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void Update()
    {
        //if (coalition_search_IF.isFocused)
        //{
        //    ScreenManager.Instance.InputFieldActive = true;
        //    ScreenManager.Instance.childRectTransform = transform.GetComponent<RectTransform>();
        //}
    }
    #endregion

    #region Private Members
    private void LoadPage()
    {
        coalition_search_tint_TF.text = "";
        coalition_search_IF.text = "";
        LUser user = UserViewController.Instance.GetCurrentUser();
        coalition_creation_IF.text = user.created_coalition;
        if (user.created_coalition.Equals(""))
        {
            coalition_creation_IF.enabled = true;
            startBTN_CG.interactable = true;
            invitation_CG.interactable = false;
        }
        else
        {
            coalition_creation_IF.enabled = false;
            startBTN_CG.interactable = false;
            invitation_CG.interactable = true;
        }

        leave_coalition_BtnObj.SetActive(!user.joined_coalition.Equals(""));
    }

    private void CoalitionSearchIndexChanged()
    {
        string strIndex = coalition_search_IF.text.TrimStart();
        if (strIndex.Length > 0)
        {
            coalition_search_tint_TF.text = CommunicationViewController.Instance.GetSimilarNames(strIndex);
        }
    }
    #endregion

    #region Public Members

    public override void Initialize()
    {
        base.Initialize();

        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.LoadCoalitions((isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                LoadPage();
            }
            else
            {
                
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public void ShowMessengerPage()
    {
        transform.parent.GetComponent<NavPage>().Show("messenger");
    }

    public void ShowCoalitionPage()
    {
        transform.parent.GetComponent<NavPage>().Show("coalition");
    }

    public void ShowNotificationPage()
    {
        transform.parent.GetComponent<NavPage>().Show("notification");
    }

    public void ShowArtExchangePage()
    {
        artExchangePageObj.SetActive(true);
    }

    public void ShowAdamantineBulletinPage()
    {
        //transform.parent.GetComponent<NavPage>().Show("adamantine_nav_page");
        transform.parent.GetComponent<NavPage>().Show("open_coalition");
        
    }

    public void ChangePublic(bool isOpen)
    {
        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.ChangePublic(isOpen, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public void CreateCoalition()
    {
        if (coalition_creation_IF.text.Trim().Equals(""))
        {
            UIManager.Instance.ShowErrorDlg("Please input coalition name!");
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.CreateCoalition(coalition_creation_IF.text.Trim(), (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            Debug.LogError(errMsg);
            Initialize();
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public void JoinCoalition()
    {
        if (coalition_search_IF.text.Trim().Equals(""))
        {
            UIManager.Instance.ShowErrorDlg("Please input coalition name for search!");
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.JoinCoalition(coalition_search_IF.text.Trim(), (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                LUser user = UserViewController.Instance.GetCurrentUser();
                if (!user.created_coalition.ToLower().Equals(coalition_search_IF.text.Trim().ToLower()))
                {
                    UIManager.Instance.ShowErrorDlg("You've sent invitation for joining coalition successfully.");
                }

                Initialize();
            }
        });
    }

    public void LeaveCoalition()
    {
        UIManager.Instance.ShowLoadingBar(true);
        FCoalition fCoalition = CommunicationViewController.Instance.GetCurrentCoalition();
        CommunicationViewController.Instance.LeaveCoalition((isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            Initialize();
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                if (fCoalition.id == UserViewController.Instance.GetCurrentUser().id)
                {
                    UIManager.Instance.ShowErrorDlg("I can't leave a coalition I created, but instead I am no longer a member of my own coalition");
                }
                else
                {
                    UIManager.Instance.ShowErrorDlg("You've left coalition successfully.");
                }

                Initialize();
            }
        });
    }

    public void InvitePeople()
    {
        if (player_search_IF.text.Trim().Equals(""))
        {
            UIManager.Instance.ShowErrorDlg("Please input player name for search!");
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.InviteUserToCoalition(player_search_IF.text.Trim(), (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowErrorDlg("You've sent invitation successfully.");
            }
        });
    }

    public void InviteVillage()
    {
        if (player_search_IF.text.Trim().Equals(""))
        {
            UIManager.Instance.ShowErrorDlg("Please input village name for search!");
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.InviteVillagerToCoalition(player_search_IF.text, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                UIManager.Instance.ShowErrorDlg("You've sent invitation successfully.");
            }
        });
    }

    #endregion
}
