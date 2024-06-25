using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ExcavationDlg : PopUpDlg
{
    [Space]
    [SerializeField] private ImageOutline image;
    [SerializeField] private Text nameTF;
    [SerializeField] private Text dateTF;
    [SerializeField] private GameObject loadingBar;
    private LArtifact artifact = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Show()
    {
        base.Show();

        artifact = ArtifactSystem.Instance.GetLastestCompletedArtifact();
        
        if (artifact == null)
        {
            return;
        }
        var cArtifact = ArtifactSystem.Instance.GetCArtifact(artifact);
        image.sprite = null;
        loadingBar.SetActive(true);
        DownloadManager.instance.AddQueue(cArtifact.GetImagePath(), (_, texture) =>
        {
            loadingBar.SetActive(false);
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        });
        nameTF.text = cArtifact.name;
        dateTF.text = cArtifact.date;

        AudioManager.Instance.PlayFXSound(AudioManager.Instance.celebrationClip);
    }

    public void OnClose()
    {
        Back();
    }

    //protected override void Back()
    //{
    //    PopUpManager.Instance.Back();
    //}
}
