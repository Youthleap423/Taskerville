using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LProduct
{
    public EResources type;
    public float amount;
    public int duration;

    public LProduct(EResources type, float amount, int duration)
    {
        this.type = type;
        this.amount = amount;
        this.duration = duration;
    }
}
