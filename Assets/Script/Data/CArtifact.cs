using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CArtifact : CData
{
    public string date = "unknown";
    public EResources type = EResources.Artifact_Common;
    public string imageURL;

    public string GetImagePath()
    {
        return ServerManager.Instance.GetServerBasePath() + "uploads" + imageURL;
    }
}
