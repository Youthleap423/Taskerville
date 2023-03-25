using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FundTabPage : TabPage
{
    // Start is called before the first frame update
    protected override void OnEnable()
    {
        base.OnEnable();
        /*var cg = tab_toggles[1].gameObject.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            if (AppManager.Instance.GetCurrentMode() == Game_Mode.Task_Only)
            {
                cg.alpha = 0.7f;
                cg.blocksRaycasts = false;
                cg.interactable = false;
            }
            else
            {
                cg.alpha = 1.0f;
                cg.blocksRaycasts = true;
                cg.interactable = true;
            }
        }*/
    }
}
