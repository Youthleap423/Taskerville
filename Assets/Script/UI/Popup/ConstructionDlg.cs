using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionDlg : MonoBehaviour
{
    [SerializeField] private Text amountTF;
    [SerializeField] private Image typeImage;
    [SerializeField] private HorizontalLayoutGroup hLayoutGroup;
    [SerializeField] private CanvasGroup proceedCGroup;
    [Header("Sprite")]
    [SerializeField] private Sprite ruby;
    [SerializeField] private Sprite diamond;
    // Start is called before the first frame update
    private EResources type = EResources.Ruby;
    
    void OnEnable()
    {
        int amount = (int)ResourceViewController.Instance.GetCurrentResourceValue(type);
        if (type == EResources.Ruby)
        {
            typeImage.sprite = ruby;
        }
        else
        {
            typeImage.sprite = diamond;
        }

        amountTF.text = "You have " + amount.ToString() + "  ";
        hLayoutGroup.enabled = false;
        hLayoutGroup.enabled = true;

        if (amount < 1)
        {
            proceedCGroup.alpha = 0.4f;
            proceedCGroup.blocksRaycasts = false;
            proceedCGroup.interactable = false;
        }
        else
        {
            proceedCGroup.alpha = 1f;
            proceedCGroup.blocksRaycasts = true;
            proceedCGroup.interactable = true;
        }
    }

    public void SetType(EResources type)
    {
        this.type = type;
    }

}

