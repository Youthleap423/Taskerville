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
            PlayerPrefs.SetString("DailyReminder", "Do not show");
        }
        else
        {
            PlayerPrefs.SetString("DailyReminder", Convert.DateTimeToEntryDate(System.DateTime.Now));
        }        
        Back();
    }
}
