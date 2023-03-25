using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CArtifact : CData
{
    public string date = "unknown";
    public EResources type = EResources.Artifact_Common;
    public Sprite image;
}
