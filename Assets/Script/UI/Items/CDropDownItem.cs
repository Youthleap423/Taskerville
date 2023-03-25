using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CDropDownItem : MonoBehaviour
{
    //public List<string> filters;
    public Text valueTF;
    
    // Start is called before the first frame update
    void Start()
    {
        if (valueTF.text != "" && valueTF.text.Contains(":"))
        {
            valueTF.color = Color.green;
            Toggle toggle = gameObject.GetComponent<Toggle>();
            if (toggle)
            {
                toggle.enabled = false;
            }
        }
        else
        {
            valueTF.color = Color.white;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
