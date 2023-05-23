using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryTabPage : TabPage
{
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
