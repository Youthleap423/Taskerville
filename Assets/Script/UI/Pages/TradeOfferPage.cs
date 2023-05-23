using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeOfferPage : Page
{
    [SerializeField] private GameObject tradeRequestItem;
    [SerializeField] private Transform offer_parent_transform;

    private List<LUser> coalitionUserList = new List<LUser>();
    private LUser currentUser = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void LoadCoalitionUsers()
    {
        foreach (Transform child in offer_parent_transform.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (LUser fUser in coalitionUserList)
        {
            GameObject obj = Instantiate(tradeRequestItem, offer_parent_transform);
            obj.GetComponent<TradeRequestItem>().SetData(currentUser, fUser);
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        currentUser = UserViewController.Instance.GetCurrentUser();
        if (currentUser.joined_coalition.Equals("")){
            UIManager.Instance.ShowErrorDlg("You've not joined any coalition!!!");
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);

        UserViewController.Instance.LoadCurrentUserList((isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                coalitionUserList.Clear();
                Debug.LogError(UserViewController.Instance.GetCurrentUserList().Count);
                foreach(LUser fUser in UserViewController.Instance.GetCurrentUserList())
                {
                    if (fUser.joined_coalition.Equals(currentUser.joined_coalition) && !fUser.id.Equals(currentUser.id))
                    {
                        coalitionUserList.Add(fUser);
                    }
                }
                LoadCoalitionUsers();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public void Back()
    {
        gameObject.GetComponentInParent<NavPage>().Back();
    }

    
}
