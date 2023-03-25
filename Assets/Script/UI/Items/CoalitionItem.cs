using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoalitionItem : MonoBehaviour
{
    [SerializeField] private TMP_Text name_TF;
    [SerializeField] private TMP_Text lastName_TF;
    [SerializeField] private TMP_Text village_TF;
    [SerializeField] private Image avatar;
    [SerializeField] private GameObject dismissObj;
    [SerializeField] private Button praiseBtn;
    [SerializeField] private GameObject loadingObj;
    // Start is called before the first frame update

    private LUser user = null;

    void Start()
    {
        
    }

    public void SetData(LUser user)
    {
        this.user = user;
        name_TF.text = user.First_Name;
        lastName_TF.text = user.Last_Name;

        village_TF.text = user.Village_Name;
        avatar.sprite = DataManager.Instance.GetAvatarSprite(user.AvatarId);

        LUser owner = UserViewController.Instance.GetCurrentUser();
        dismissObj.SetActive(owner.created_coalition.ToLower() == owner.joined_coalition.ToLower());
        //EnableButton(praiseBtn, false);
        //EnableButton(encourageBtn, false);

        //System.DateTime yesterday = System.DateTime.Now.AddDays(-1);
        //var taskList = TaskViewController.Instance.GetDailyTasks(yesterday);

        //if (taskList.Count > 0)
        //{
        //    int completedTaskCount = 0;
        //    foreach (LTaskEntry entry in taskList)
        //    {
        //        if (entry.completed_Week.Count > 0 && entry.completed_Week.Contains(Convert.DateTimeToFDate(yesterday)))
        //        {
        //            completedTaskCount++;
        //        }
        //    }

        //    float percent = completedTaskCount / taskList.Count;
        //    EnableButton(praiseBtn, percent > 0.99);
        //    EnableButton(encourageBtn, percent < 0.25);
        //}
    }

    private void EnableButton(Button Btn, bool isEnabled)
    {
        TMP_Text tmp_Text = Btn.gameObject.GetComponentInChildren<TMP_Text>();
        Btn.enabled = isEnabled;
        if (tmp_Text != null)
        {
            tmp_Text.color = isEnabled ? new Color(2f / 255, 161f / 255, 75f / 255) : new Color(188f / 255, 188f / 255, 188f / 255);
        }
    }

    public void ShowProfile()
    {
        UIManager.Instance.ShowProfile(user, true);
    }

    public void ShowMuseum()
    {

        CoalitionPage page = gameObject.GetComponentInParent<CoalitionPage>();
        if (page != null)
        {
            page.ShowCoalitionGallery(this.user);
        }
        else
        {
            UIManager.LogError("Can't find");
        }
    }

    public void Encourage()
    {
        string str = Utilities.GetEncourageString();
        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.SendFMessage(user, str, (isSuccess, errMsg, message) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public void Praise()
    {
        string str = Utilities.GetPraiseString();
        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.SendFMessage(user, str, (isSuccess, errMsg, message) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
        });
    }

    public void OpenTrade()
    {
        TabPage page = gameObject.GetComponentInParent<TabPage>();
        if (page != null)
        {
            page.ShowPage("trade");
        }
        else
        {
            UIManager.LogError("Can't find");
        }
    }

    public void Dismiss()
    {
        transform.GetComponentInParent<CoalitionPage>().DismissUser(user);
    }
}
