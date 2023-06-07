using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReligionScreen : IScreen
{
    [Space]
    [SerializeField] public Image avatar_Image;
    [SerializeField] public Text name_TF;
    [SerializeField] public Text village_TF;

    [Space]
    [SerializeField] public GameObject item_Prefab;
    [SerializeField] public Transform parent_Transform;

    private List<GameObject> religion_objList = new List<GameObject>();
    private string selectedReligionName = "";

    #region Unity Members
    void Start()
    {
        religion_objList.Clear();
        
        foreach(CReligion religion in DataManager.Instance.ResourcesCategoryData.religions)
        {
            GameObject obj = GameObject.Instantiate(item_Prefab);
            obj.name = religion.name;
            obj.transform.parent = parent_Transform;
            religion_objList.Add(obj);

            ReligionItem item = obj.GetComponentInChildren<ReligionItem>();
            if (item != null)
            {
                item.setText(obj.name);
                item.onDeSelect();
            }

            Button btn = obj.GetComponentInChildren<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => selectReligion(obj.name));
            }
        }
    }
    #endregion

    #region Public Members
    public void Load()
    {
        LUser currentUser = DataManager.Instance.GetCurrentUser();
        name_TF.text = currentUser.GetFullName();
        village_TF.text = string.Format("Village of {0}", currentUser.Village_Name);
        avatar_Image.sprite = DataManager.Instance.GetCurrentAvatarSprite();
        if (religion_objList.Count > 0)
        {
            selectedReligionName = currentUser.Religion_Name;
            selectReligion(selectedReligionName);
        }
    }

    public void OnContinue()
    {
        DataManager.Instance.UpdateReligion(selectedReligionName);
        ScreenManager.Instance.Show("gamemode");
    }
    #endregion

    #region Private Members

    private void selectReligion(string religionName)
    {
        for (int index = 0; index < religion_objList.Count; index++)
        {
            GameObject obj = religion_objList[index];
            ReligionItem item = obj.GetComponentInChildren<ReligionItem>();
            if (item != null)
            {
                if (obj.name == religionName)
                {
                    selectedReligionName = religionName;
                    item.onSelect();
                }
                else
                {
                    item.onDeSelect();
                }
            }
        }
        
    }

    #endregion
}
