using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoalitionGalleryPage : Page
{
    [Space]
    [SerializeField] private TMP_Text title_TF;
    [SerializeField] private GameObject item_prefab;
    [SerializeField] private Transform group_Transform;

    private LUser selectedUser = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUser(LUser user)
    {
        this.selectedUser = user;
        LoadUI();
    }

    private void LoadUI()
    {
        if (this.selectedUser == null)
        {
            Back();
        }

        List<LArtwork> artList = DataManager.Instance.GetOtherUserArtworks(this.selectedUser);
        
        title_TF.text = string.Format("Gallery of {0}", this.selectedUser.Village_Name);
        foreach (Transform child in group_Transform)
        {
            Destroy(child.gameObject);
        }

        foreach (LArtwork artwork in artList)
        {
            GameObject obj = Instantiate(item_prefab, group_Transform);
            CArtwork cArtwork = ArtworkSystem.Instance.GetCArtwork(artwork);
            obj.GetComponentInChildren<Text>().text = string.Format("- {0}, {1}", cArtwork.artist_name, cArtwork.name);
        }
    }

    public void Back()
    {
        this.gameObject.SetActive(false);
    }

}
