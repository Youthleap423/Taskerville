using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReminderDlg : PopUpDlg
{
    [Space]
    [SerializeField] private Toggle toggle;

    public void OnClose()
    {
        if (toggle.isOn)
        {
            PlayerPrefs.SetString("DRemind", "Do not show");
        }
        else
        {
            if (PlayerPrefs.GetString("DRemind") != "Do not show")
            {
                PlayerPrefs.SetString("DRemind", Convert.DateTimeToEntryDate(System.DateTime.Now));
            }
        }
        Back();
    }
}
