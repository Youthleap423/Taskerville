using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelterTabItem : TabItem
{
    [SerializeField] private Image shelter_on_image;
    [SerializeField] private Image shelter_off_image;

    
    [SerializeField] private Sprite red_storm_sprite;
    [SerializeField] private Sprite blue_storm_sprite;
    [SerializeField] private Sprite gray_storm_sprite;

    protected override void OnEnable()
    {
        base.OnEnable();

        bool shelter_storm = UserViewController.Instance.GetCurrentSetting().shelter_storm;
        ChangeIconState(shelter_storm);
    }

    public void ChangeIconState(bool isRed)
    {
        if (isRed)
        {
            shelter_on_image.sprite = red_storm_sprite;
            shelter_off_image.sprite = red_storm_sprite;
            if (PlayerPrefs.GetString("Storm_Sound") != Convert.DateTimeToFDate(System.DateTime.Now))
            {
                AudioManager.Instance.PlayFXSound(AudioManager.Instance.stormClip);
                PlayerPrefs.SetString("Storm_Sound", Convert.DateTimeToFDate(System.DateTime.Now));
            }
        }
        else
        {
            shelter_on_image.sprite = blue_storm_sprite;
            shelter_off_image.sprite = gray_storm_sprite;
            PlayerPrefs.SetString("Storm_Sound", "");
        }
    }
}
