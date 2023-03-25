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
        image.sprite = cArtifact.image;
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
