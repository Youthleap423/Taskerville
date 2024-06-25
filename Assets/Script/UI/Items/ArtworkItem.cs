using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtworkItem : ImageOutline
{
    [SerializeField] private GameObject loadingBar;
    [SerializeField] private Image image;
    private CArtwork artwork = null;
    private int index = -1;

    public CArtwork GetData()
    {
        return artwork;
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetData(CArtwork cArtwork, int index)
    {
        this.artwork = cArtwork;
        this.index = index;
        this.sprite = null;
        image.sprite = null;
        loadingBar.SetActive(true);
        DownloadManager.instance.AddQueue(cArtwork.GetImagePath(), (_, texture) =>
        {
            loadingBar.SetActive(false);
            this.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        });
    }

}
