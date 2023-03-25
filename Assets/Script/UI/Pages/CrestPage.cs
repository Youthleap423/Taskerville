using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrestPage : Page
{
    [Header("Artifact Exchange")]
    [SerializeField] private TMP_Text RA_text;
    [SerializeField] private TMP_Text UA_text;
    [SerializeField] private TMP_Text CA_text;

    [Header("Gem Exchange")]
    [SerializeField] private TMP_Text DtoP_text;
    [SerializeField] private TMP_Text RtoD_text;
    [SerializeField] private TMP_Text DtoR_text;
    [SerializeField] private TMP_Text GtoR_text;

    private void OnEnable()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();

        var list = ArtifactSystem.Instance.GetAllArtifacts().FindAll(item => item.progress >= 1.0f && item.isExchanged == false);
        var commonList = list.FindAll(item => int.Parse(item.id) <= 150);
        var uncommonList = list.FindAll(item => int.Parse(item.id) >= 151 && int.Parse(item.id) <= 250);
        var rareList = list.FindAll(item => int.Parse(item.id) >= 251);

        RA_text.text = string.Format("{0}", rareList.Count);
        UA_text.text = string.Format("{0}", uncommonList.Count);
        CA_text.text = string.Format("{0}", commonList.Count);

        DtoP_text.text = string.Format("Diamonds [{0}]", (int)ResourceViewController.Instance.GetResourceValue(EResources.Diamond));
        RtoD_text.text = string.Format("Rubies [{0}]", (int)ResourceViewController.Instance.GetResourceValue(EResources.Ruby));
        DtoR_text.text = string.Format("Diamonds [{0}]", (int)ResourceViewController.Instance.GetResourceValue(EResources.Diamond));
        GtoR_text.text = string.Format("Gold [{0}]", (int)ResourceViewController.Instance.GetResourceValue(EResources.Gold));
        
    }

    public void Back()
    {
        gameObject.SetActive(false);
    }

    public void OnDiamondToPaintExchange()
    {
        var curDiamond = ResourceViewController.Instance.GetCurrentResourceValue(EResources.Diamond);
        if (curDiamond < 1f)
        {
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);

        ResourceViewController.Instance.UpdateResource(EResources.Diamond.ToString(), -1f, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                ArtworkSystem.Instance.Pick(EArtworkReason.Buy);
                Initialize();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            
        });
    }

    public void OnDiamondToRubyExchange()
    {
        var curDiamond = ResourceViewController.Instance.GetCurrentResourceValue(EResources.Diamond);
        if (curDiamond < 1f)
        {
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        ResourceViewController.Instance.OnExchange(EResources.Diamond, 1f, EResources.Ruby, 5f, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                Initialize();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }

        });
    }

    public void OnRubyToDiamondExchange()
    {
        var curRuby = ResourceViewController.Instance.GetCurrentResourceValue(EResources.Ruby);
        if (curRuby < 5f)
        {
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        ResourceViewController.Instance.OnExchange(EResources.Ruby, 5f, EResources.Diamond, 1f, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                Initialize();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }

        });
    }

    public void OnGoldToRubyExchange()
    {
        var curGold = ResourceViewController.Instance.GetCurrentResourceValue(EResources.Gold);
        if (curGold < 100f)
        {
            return;
        }

        UIManager.Instance.ShowLoadingBar(true);
        ResourceViewController.Instance.OnExchange(EResources.Gold, 100f, EResources.Ruby, 1f, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                Initialize();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }

        });
    }
}
