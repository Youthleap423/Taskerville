using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactDetailPage : Page
{
    [SerializeField] private GameObject prebBtnObj;
    [SerializeField] private GameObject nextBtnObj;
    [SerializeField] private GameObject loadingBar;
    [Space]
    [SerializeField] private ImageOutline image;
    [SerializeField] private Text nameTF;
    [SerializeField] private Text dateTF;

    private List<LArtifact> artifactList = new List<LArtifact>();
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

        if (currentIndex >= artifactList.Count - 1)
        {
            nextBtnObj.SetActive(false);
        }
        else
        {
            nextBtnObj.SetActive(true);
        }

        if (currentIndex < 0 || currentIndex >= artifactList.Count)
        {
            UIManager.Instance.ShowErrorDlg("Can't find the artifact.");
            return;
        }

        var cArtifact = ArtifactSystem.Instance.GetCArtifact(artifactList[currentIndex]);
        var str = "CA";
        int id = int.Parse(cArtifact.id);
        if (id > 150 && id <= 250)
        {
            str = "UA";
        }else if (id > 250)
        {
            str = "RA";
        }

        loadingBar.SetActive(true);
        DownloadManager.instance.AddQueue(cArtifact.GetImagePath(), (_, texture) =>
        {
            loadingBar.SetActive(false);
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        });
        nameTF.text = cArtifact.name;
        dateTF.text = cArtifact.date + "(" + str + ")";
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

    public void onBack()
    {
        transform.gameObject.SetActive(false);
    }

    public void SetData(List<LArtifact> list, int index)
    {
        artifactList = list;
        currentIndex = index;
        ReloadUI();
    }
}
