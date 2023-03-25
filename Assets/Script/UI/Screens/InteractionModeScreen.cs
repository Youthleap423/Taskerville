using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionModeScreen : IScreen
{
    [Space]
    [SerializeField] public Toggle partialToggle;
    [SerializeField] public Toggle manualToggle;

    private Interaction_Mode interaction_mode = Interaction_Mode.Partial_Management;

    #region Unity Members
    private void Start()
    {
        partialToggle.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged(partialToggle);
        });
    }
    #endregion

    #region Public Members
    public void Load()
    {
        interaction_mode = (Interaction_Mode)DataManager.Instance.GetCurrentSetting().interaction_mode;
        if (interaction_mode == Interaction_Mode.Partial_Management)
        {
            partialToggle.isOn = true;
            manualToggle.isOn = false;
        }
        else
        {
            partialToggle.isOn = false;
            manualToggle.isOn = true;
        }
    }

    public void OnBegin()
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.UpdateSetting(interaction_mode);

        UserViewController.Instance.SerializeUser(false, (isSuccess, err) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (!isSuccess)
            {
                UIManager.Instance.ShowErrorDlg(err);
            }
            else
            {
                ScreenManager.Instance.Show("main");
            }
        });
    }

    public void OnBack()
    {
        ScreenManager.Instance.Back();
    }
    #endregion

    #region Private Members
    private void ToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            interaction_mode = Interaction_Mode.Partial_Management;
        }
        else
        {
            interaction_mode = Interaction_Mode.Manual_Management;
        }
    }
    #endregion
}
