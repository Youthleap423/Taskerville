using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OpenCoalitionItem : MonoBehaviour
{
    [SerializeField] private GameObject line1;
    [SerializeField] private GameObject line2;
    [SerializeField] private TMP_Text content_TF;

    private FCoalition coalition = null;
    public void SetData(string timezoneStr)
    {
        line1.SetActive(true);
        line2.SetActive(true);
        content_TF.text = string.Format("{0} - {1}", timezoneStr, Convert.GetSTZName(timezoneStr));
        content_TF.color = new Color32(72, 72, 72, 255);
    }

    public void SetData(FCoalition coalition)
    {
        line1.SetActive(false);
        line2.SetActive(false);
        this.coalition = coalition;
        content_TF.text = coalition.name;
        content_TF.color = new Color32(0, 80, 255, 255);
    }

    public void JoinCoalition()
    {
        if (coalition == null)
        {
            return;
        }

        LUser user = UserViewController.Instance.GetCurrentUser();

        UIManager.Instance.ShowLoadingBar(true);
        CommunicationViewController.Instance.JoinCoalition(coalition.name, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            else
            {
                
                if (!user.created_coalition.ToLower().Equals(coalition.name.ToLower()))
                {
                    UIManager.Instance.ShowErrorDlg("A request to join this coalition has now been sent. Check notifications over the coming days to see if it was accepted.");
                }
            }
        });
    }


}
