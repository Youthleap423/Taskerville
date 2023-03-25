using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcursionTabPage : TabPage
{
    // Start is called before the first frame update
    [SerializeField] private GameObject artExchangeObj;
    protected override void OnEnable()
    {
        base.OnEnable();
        /*
        var cg = tab_toggles[1].gameObject.GetComponent<CanvasGroup>();
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
        }
        */
        //cg = tab_toggles[3].gameObject.GetComponent<CanvasGroup>();
        //if (cg != null)
        //{
        //    if (AppManager.Instance.GetCurrentMode() == Game_Mode.Task_Only)
        //    {
        //        cg.alpha = 0.7f;
        //        cg.blocksRaycasts = false;
        //        cg.interactable = false;
        //    }
        //    else
        //    {
        //        cg.alpha = 1.0f;
        //        cg.blocksRaycasts = true;
        //        cg.interactable = true;
        //    }
        //}
    }

    public override void ShowPage(string pageId)
    {
        base.ShowPage(pageId);
    }

    public override void ChangedTab()
    {
        artExchangeObj.SetActive(false);
    }

}
