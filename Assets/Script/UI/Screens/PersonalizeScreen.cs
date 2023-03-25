using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalizeScreen : IScreen
{
    [Space]
    [SerializeField] public Image avatar_Image;
    [SerializeField] public InputField firstName_IF;
    [SerializeField] public InputField secondName_IF;
    [SerializeField] public InputField villageName_IF;
    [SerializeField] public Toggle vegetarianToggle;
    [SerializeField] public Toggle religionToggle;

    private bool isVegetarian = false;
    private bool hasReligion = false;

    #region Public Members
    // Start is called before the first frame update
    void Start()
    {
        vegetarianToggle.onValueChanged.AddListener(delegate
        {
            VegetarainValueChanged(vegetarianToggle);
        });

        religionToggle.onValueChanged.AddListener(delegate
        {
            ReligionValueChanged(religionToggle);
        });
    }
    #endregion

    #region Public Members
    public void Load()
    {
        LUser currentUser = DataManager.Instance.GetCurrentUser();
        firstName_IF.text = currentUser.First_Name;
        secondName_IF.text = currentUser.Last_Name;
        villageName_IF.text = currentUser.Village_Name;
        avatar_Image.sprite = DataManager.Instance.GetCurrentAvatarSprite();
        isVegetarian = currentUser.isVegetarian;
        hasReligion = currentUser.hasReligion;
        vegetarianToggle.isOn = isVegetarian;
        religionToggle.isOn = hasReligion;
    }

    public void OnContinue()
    {
        UIManager.Instance.ShowLoadingBar(true);
        DataManager.Instance.UpdateUser(firstName_IF.text, secondName_IF.text, villageName_IF.text, isVegetarian, hasReligion, (isSuccess, errMsg) =>
        {
            UIManager.Instance.ShowLoadingBar(false);
            if (isSuccess)
            {
                if (hasReligion)
                {
                    ScreenManager.Instance.Show("religion");
                }
                else
                {
                    ScreenManager.Instance.Show("gamemode");
                }
            }
            else
            {
                UIManager.Instance.ShowErrorDlg(errMsg);
            }
            
        });
    }

    public void OnBack()
    {
        ScreenManager.Instance.Back();
    }
    #endregion

    #region Private Members
    private void VegetarainValueChanged(Toggle toggle)
    {
        isVegetarian = toggle.isOn;
    }

    private void ReligionValueChanged(Toggle toggle)
    {
        hasReligion = toggle.isOn;
    }
    #endregion
}
