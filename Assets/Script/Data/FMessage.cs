using System;
using System.Collections.Generic;

[Serializable]
public class FMessage : Data
{
    public string receiver = "";
    public string sender = "";
    public string content = "";
    public string created_at = "";
    public EMessageType type = EMessageType.Public;
    public bool isRead = false;
    public List<string> members = new List<string>();

    
    public FMessage()
    {

    }
}
