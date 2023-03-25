using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopUpManager : SingletonComponent<PopUpManager>
{
    [SerializeField] private GameObject missingTaskListPanel;
    [SerializeField] private GameObject administratorMessageObj;
    [SerializeField] private TMP_Text administratorMessageText;

    [SerializeField] private List<PopUpDlg> popupDlgList;


	private List<PopUpDlg> popupStack = new List<PopUpDlg>();
	private PopUpDlg currentPopUp = null;

	// Start is called before the first frame update
	void Start()
    {
		// Initialize and hide all the pages
		for (int i = 0; i < popupDlgList.Count; i++)
		{
			PopUpDlg dlg = popupDlgList[i];
            dlg.Close();
		}
	}

    public void Add(EPopUpDlg type)
    {
        if (CheckVisibleForToday(type))
        {
            var popup = FindPopup(type);
			if (popup != null)
            {
                popupStack.Add(popup);
            }
            Show();
        }
    }

	public void Back()
    {
		if (currentPopUp != null)
        {
            currentPopUp.Close();
            popupStack.Remove(currentPopUp);
			currentPopUp = null;
        }
        Show();
    }

    private bool CheckVisibleForToday(EPopUpDlg type)
    {
        if (type == EPopUpDlg.NewArtwork)
        {
            return true;
        }
        return PlayerPrefs.GetString(type.ToString()) != Convert.DateTimeToFDate(System.DateTime.Now);
    }

	private void Show()
    {
        if (currentPopUp == null && popupStack.Count > 0)
        {
			currentPopUp = popupStack[0];
            currentPopUp.Show();
        }
    }

	private PopUpDlg FindPopup(EPopUpDlg type)
    {
		foreach(PopUpDlg dlg in popupDlgList)
        {
			if (dlg.type == type)
            {
				return dlg;
            }
        }

		return null;
    }


    //////Admin Propmpt//////

    public void ShowAdministratorPrompt(string strMsg, System.Action<bool> callback)
    {
        var popup = FindPopup(EPopUpDlg.AdminPrompt);
        if (popup == null)
        {
            return;
        }

        popup.GetComponent<AdminPrompt>().SetData(strMsg, callback);
        Add(EPopUpDlg.AdminPrompt);
    }
}
