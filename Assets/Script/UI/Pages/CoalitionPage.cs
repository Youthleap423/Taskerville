using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoalitionPage : Page
{
    [Space]
    [SerializeField] private TMP_Text title_TF;
    [SerializeField] private GameObject item_prefab;
    [SerializeField] private Transform group_Transform;

    [SerializeField] private GameObject gallery_Obj;

    private List<LUser> userList = new List<LUser>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Initialize();
    }

    #region Private Members
    private void LoadUI()
    {
        LUser me = UserViewController.Instance.GetCurrentUser();
        title_TF.text = me.joined_coalition;
        foreach(Transform child in group_Transform)
        {
            Destroy(child.gameObject);
        }

        foreach(LUser user in userList)
        {
            GameObject obj = Instantiate(item_prefab, group_Transform);
            obj.GetComponent<CoalitionItem>().SetData(user);
        }
    }
    #endregion

    #region Public Members
    public override void Initialize()
    {
        base.Initialize();

        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.GetCoalitionMembers((isSuccess, errMsg, userList) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                this.userList = userList;
                LoadUI();
            }
            else
            {
                this.userList.Clear();
                LoadUI();
                UIManager.Instance.ShowErrorDlg(errMsg);
            }

        });
    }

    public void Back()
    {
        transform.parent.GetComponent<NavPage>().Back();
    }

    public void DismissUser(LUser user)
    {
        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.KickCoalitionMember(user, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                Initialize();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public void ShowCoalitionGallery(LUser user)
    {
        if (user == null)
        {
            return;
        }

        gallery_Obj.SetActive(true);
        gallery_Obj.GetComponent<CoalitionGalleryPage>().SetUser(user);
    }
    #endregion
}
