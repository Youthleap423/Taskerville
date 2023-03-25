using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditPage : Page
{
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        AudioManager.Instance.FadeOut();
    }
}
