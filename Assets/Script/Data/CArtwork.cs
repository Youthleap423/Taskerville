using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CArtwork : CData
{
    public string artist_name = "";
    public string contactInfo = "";
    public string imageURL = "";
    public bool isOriginal = false;

    public string GetImagePath()
    {
        return ServerManager.Instance.GetServerBasePath() + "uploads" + imageURL;
    }
}
