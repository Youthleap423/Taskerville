using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamantineBulletinPage : Page
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Public Members
    public void Back()
    {
        transform.parent.parent.GetComponent<NavPage>().Back();
    }

    public void OpenCoalition()
    {
        transform.parent.GetComponent<NavPage>().Show("open_coalition");
    }
    #endregion
}
