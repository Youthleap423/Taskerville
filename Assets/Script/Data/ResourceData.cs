using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class ResourceData : ScriptableObject
{
	[FormerlySerializedAs("Resource")]
	public List<CResource> resources;
}