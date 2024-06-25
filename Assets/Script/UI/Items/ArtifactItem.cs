using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactItem : ImageOutline
{
    [SerializeField] private GameObject loadingBar;
    private CArtifact artifact = null;
    private int index = -1;

    public CArtifact GetData()
    {
        return artifact;
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetData(CArtifact cArtifact, int index)
    {
        this.artifact = cArtifact;
        this.index = index;
        loadingBar.SetActive(true);
        DownloadManager.instance.AddQueue(cArtifact.GetImagePath(), (_, texture) =>
        {
            loadingBar.SetActive(false);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        });
    }

}
