using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtworkDetailPage : Page
{
    [SerializeField] private GameObject prebBtnObj;
    [SerializeField] private GameObject nextBtnObj;
    [SerializeField] private GameObject infoBtnObj;
    [Space]
    [SerializeField] private ImageOutline image;
    [SerializeField] private Text nameTF;
    [SerializeField] private Text dateTF;
    [Space]
    [SerializeField] private GameObject infoObj;
    [SerializeField] private Text contact_nameTF;
    [SerializeField] private Text contactTF;
    
    private List<LArtwork> artworkList = new List<LArtwork>();
    private int currentIndex = 0;
    void Start()
    {

    }

    private void ReloadUI(){
        if (currentIndex == 0)
        {
            prebBtnObj.SetActive(false);
        }
        else
        {
            prebBtnObj.SetActive(true);
        }

        if (currentIndex >= artworkList.Count - 1)
        {
            nextBtnObj.SetActive(false);
        }
        else
        {
            nextBtnObj.SetActive(true);
        }

        if (currentIndex < 0 || currentIndex >= artworkList.Count)
        {
            UIManager.Instance.ShowErrorDlg("Can't find the artwork.");
            return;
        }

        var cArtwork = ArtworkSystem.Instance.GetCArtwork(artworkList[currentIndex]);
        
        StartCoroutine(ImageLoader.Start(cArtwork.image_path, (sprite =>
        {
            image.sprite = sprite;
        })));
        nameTF.text = cArtwork.name;
        dateTF.text = cArtwork.artist_name;
        contactTF.text = cArtwork.contactInfo;
        contact_nameTF.text = cArtwork.artist_name;// "Eric N. Duran";// cArtwork.artist_name;
        infoBtnObj.SetActive(cArtwork.isOriginal);
        infoObj.SetActive(false);
    }

    public void onPrev()
    {
        currentIndex--;
        ReloadUI();
    }

    public void onNext()
    {
        currentIndex++;
        ReloadUI();
    }

    public void onInfo()
    {
        infoObj.SetActive(!infoObj.activeSelf);
    }

    public void onBack()
    {
        transform.gameObject.SetActive(false);
    }

    public void SetData(List<LArtwork> list, int index)
    {
        artworkList = list;
        currentIndex = index;
        ReloadUI();
    }
}
