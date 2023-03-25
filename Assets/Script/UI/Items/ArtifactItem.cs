using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactItem : ImageOutline
{
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
        sprite = artifact.image;
    }

}
