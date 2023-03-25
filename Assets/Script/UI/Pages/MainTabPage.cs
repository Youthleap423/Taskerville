using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainTabPage : TabPage
{
    // Start is called before the first frame update
    protected override void OnEnable()
    {
        string notifyPageId = PlayerPrefs.GetString("NotifyPage");
        if (notifyPageId == "")
        {
            base.OnEnable();
        }
    }

    override public void ShowPage(string pageId)
    {
        base.ShowPage(pageId);
    }
}
