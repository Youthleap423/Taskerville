using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpDlg : MonoBehaviour
{
    public EPopUpDlg type = EPopUpDlg.None;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Cancel()
    {

    }

    protected virtual void OK()
    {

    }

    protected virtual void Back()
    {
        PlayerPrefs.SetString(type.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
        PopUpManager.Instance.Back();
    }
}
