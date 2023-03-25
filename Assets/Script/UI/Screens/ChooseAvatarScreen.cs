using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseAvatarScreen : IScreen
{
    [Space]
    [SerializeField] public GameObject item_Prefab;
    [SerializeField] public Transform parent_Transform;

    private List<GameObject> avatar_objList = new List<GameObject>();
    private int selectedAvatarId = 0;

    #region Unity_Members
    // Start is called before the first frame update
    void Start()
    {
        avatar_objList.Clear();

        for(int index = 0; index < DataManager.Instance.avatar_Images.Length; index++)
        {
            Sprite sprite = DataManager.Instance.avatar_Images[index];
            GameObject obj = GameObject.Instantiate(item_Prefab);
            obj.name = sprite.name;
            obj.transform.parent = parent_Transform;
            avatar_objList.Add(obj);

            AvatarItem item = obj.GetComponentInChildren<AvatarItem>();
            if (item != null)
            {
                item.itemIndex = index;
                item.setSprite(sprite);
            }

            Button btn = obj.GetComponentInChildren<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => selectAvatar(obj.name));
            }
        }
        
        if (avatar_objList.Count > 0)
        {
            selectedAvatarId = DataManager.Instance.GetCurrentUser().AvatarId;
            selectAvatar(avatar_objList[selectedAvatarId].name);
        }
    }

    #endregion

    #region Public Members
    public void OnContinue()
    {
        DataManager.Instance.UpdateUserAvatarId(selectedAvatarId);
        if (ScreenManager.Instance.PreviousScreenId == "main")
        {
            ScreenManager.Instance.Back();
        }
        else
        {
            ScreenManager.Instance.Show("personalize");
        }
    }
    #endregion
    #region Private Members

    private void selectAvatar(string objName)
    {
        foreach (GameObject obj in avatar_objList)
        {
            AvatarItem item = obj.GetComponentInChildren<AvatarItem>();
            if (item != null)
            {
                if (obj.name == objName)
                {
                    item.onSelect();
                    selectedAvatarId = item.itemIndex;
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
