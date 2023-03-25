using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGamePage : Page
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBlast()
    {
        UIManager.Instance.ShowLoadingBar(true);
        UserViewController.Instance.SerializeUser(true, (isSuccess, err) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            AppManager.Instance.SignOut();
        });
    }
}
