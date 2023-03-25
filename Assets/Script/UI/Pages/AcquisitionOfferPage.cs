using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquisitionOfferPage : Page
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Back()
    {
        gameObject.GetComponentInParent<NavPage>().Back();
    }
}
