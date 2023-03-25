using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class ArtifactData : ScriptableObject
{
    [FormerlySerializedAs("Artifact")]
    public List<CArtifact> commonList;
    public List<CArtifact> uncommonList;
    public List<CArtifact> rareList;
    [Space]
    public List<CArtwork> artworks;
}

