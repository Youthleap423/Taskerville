using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionItem : MonoBehaviour
{
    [SerializeField] private Text name_TF;
    [SerializeField] private Text amount_TF;
    [SerializeField] private Text duration_TF;

    private LProduct product = null;


    private void UpdateUI()
    {
        name_TF.text = "";
        amount_TF.text = "";
        duration_TF.text = "";
        if (product != null)
        {
            CResource cResource = ResourceViewController.Instance.GetCResource(product.type);
            if (cResource != null)
            {
                name_TF.text = cResource.name;
                amount_TF.text = product.amount < 2 ? product.amount + " " + cResource.unit_single : product.amount + " " + cResource.unit_plural;
                duration_TF.text = GetDurationString(product.duration);
            }
            
        }
        
    }

    private string GetDurationString(int days)
    {
        if (days == 1)
        {
            return "Daily";// days + " day";
        }

        if (days < 7)
        {
            return days + " days";
        }

        var week = days / 7;

        return week < 2 ? week + " week" : week + " weeks";
    }

    public void SetProduct(LProduct lProduct)
    {
        product = lProduct;
        UpdateUI();
    }
}
