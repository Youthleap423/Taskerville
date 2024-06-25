using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReportPage : PopUpDlg
{
    [SerializeField] private GameObject reportItemPrefab;
    [SerializeField] private Transform reportListGroup;
    [SerializeField] private bool bInSetting = false;

    #region Unity Members
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        if (bInSetting)
        {
            Show();
        }
    }

    public override void Show()
    {
        base.Show();
        DeleteReportList();

        var user = UserViewController.Instance.GetCurrentUser();

        //if (user.created_at == Convert.DateTimeToFDate(System.DateTime.Now))
        //{
        //    return;
        //}

        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.GetDailyReport(strList =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            CreateReportList(strList);
        });
        
    }

    private void CreateReportList(List<string> reportStrList)
    {
        for (int index = 0; index < reportStrList.Count; index++)
        {
            GameObject subItemObj = GameObject.Instantiate(reportItemPrefab, reportListGroup);
            subItemObj.GetComponent<Text>().text = string.Format("• {0}", reportStrList[index]);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(reportListGroup.transform.GetComponent<RectTransform>());
    }

    private void DeleteReportList()
    {
        foreach (Transform child in reportListGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion

    #region Public Members
    public void OnClose()
    {
        if (bInSetting)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Back();
        }
    }
    #endregion
}
