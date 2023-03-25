using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReligionItem : MonoBehaviour
{
    [SerializeField] private Outline outline;
    [SerializeField] private Text textObj;
    // Start is called before the first frame update
    void Awake()
    {
        outline = gameObject.GetComponent<Outline>();
        textObj = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setText(string strText)
    {
        textObj.text = strText;
    }

    public void onSelect()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void onDeSelect()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
