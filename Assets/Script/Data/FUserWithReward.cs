using System;
using System.Collections.Generic;

[Serializable]
public class FUserWithReward : Data
{
    public FUser fUser;
    public Dictionary<EResources, float> reward;
}
