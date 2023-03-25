using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtworkItem : ImageOutline
{
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
        StartCoroutine(ImageLoader.Start(cArtwork.image_path, (sprite =>
        {
            this.sprite = sprite;
        })));
    }

}
