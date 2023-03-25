using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReportPage : PopUpDlg
{
    [SerializeField] private GameObject reportItemPrefab;
    [SerializeField] private Transform reportListGroup;
    [SerializeField] private bool bInSetting = false;

    private List<string> reportStrList = new List<string>();
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

        var user = UserViewController.Instance.GetCurrentUser();

        if (user.created_at == Convert.DateTimeToFDate(System.DateTime.Now))
        {
            Debug.LogError(user.created_at);
            return;
        }


        reportStrList = DataManager.Instance.GetDailyReport();

        CreateReportList();
    }

    private void CreateReportList()
    {
        DeleteReportList();
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
