using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdminPrompt : PopUpDlg
{
    [SerializeField] private TMP_Text msgTF;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

    private string strMsg = "";
    // Start is called before the first frame update

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    private void RemoveListeners()
    {
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
    }

    public override void Show()
    {
        base.Show();

        msgTF.text = strMsg;
    }

    public void SetData(string msg, System.Action<bool> callback)
    {
        strMsg = msg;
        RemoveListeners();

        yesBtn.onClick.AddListener(() =>
         {
             callback(true);
             Back();
         });
        noBtn.onClick.AddListener(() =>
        {
            callback(false);
            Back();
        });
    }
}
