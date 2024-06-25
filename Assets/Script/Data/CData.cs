using System;
using UnityEngine;

[Serializable]
public class CData : Data
{
    public string name = "";
    public string id = "";
}

[Serializable]
public class Data{

}
[Serializable]
struct CTutor
{
    public string title;
    public Sprite sprite;
}