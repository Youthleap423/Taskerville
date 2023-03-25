using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalesPurchasePage : Page
{
    [SerializeField] private CanvasGroup merchantFromCG;
    [SerializeField] private CanvasGroup merchantToCG;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        var dayOfWeek = System.DateTime.Now.DayOfWeek;
        var visiblity = DataManager.Instance.availableDaysOfShip.Contains(dayOfWeek);
        SetVisibility(merchantFromCG, visiblity);
        SetVisibility(merchantToCG, visiblity);
    }

    private void SetVisibility(CanvasGroup CG, bool isVisible)
    {
        CG.alpha = isVisible ? 1f : 0.5f;
        CG.interactable = isVisible ? true : false;
        CG.blocksRaycasts = isVisible ? true : false;
    }
}
