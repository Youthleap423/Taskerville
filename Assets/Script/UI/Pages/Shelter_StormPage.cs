using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Shelter_StormPage : Page
{
    [SerializeField] private ShelterTabItem shelter_tabItem;
    
    [SerializeField] private Toggle take_cloud_toggle;
    [SerializeField] private Toggle takeout_cloud_toggle;

    private void Start()
    {
        take_cloud_toggle.onValueChanged.AddListener(delegate
        {
            OnTakeCloudToggle(take_cloud_toggle.isOn);
        });
    }

    private void OnEnable()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();

        bool shelter_storm = UserViewController.Instance.GetCurrentSetting().shelter_storm;
        take_cloud_toggle.isOn = shelter_storm;
        takeout_cloud_toggle.isOn = !shelter_storm;
        ChangeIconState(shelter_storm);
    }

    private void OnTakeCloudToggle(bool bTake)
    {
        UserViewController.Instance.UpdateSetting(bTake);
        ChangeIconState(bTake);
    }

    private void ChangeIconState(bool isRed)
    {
        shelter_tabItem.ChangeIconState(isRed);
    }

}
