using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Mask))]
[ExecuteInEditMode]
public class ImageOutline : MonoBehaviour
{

    private Sprite m_Sprite;

    [SerializeField]
    private Image m_imageComp;
    private Mask m_maskComp;


    public Sprite sprite
    {
        get
        {
            return m_Sprite;
        }

        set
        {
            m_Sprite = value;
            Build();
        }
    }

    // Start is called before the first frame update

    
    protected void Build()
    {
        m_imageComp = GetComponent<Image>();
        m_maskComp = GetComponent<Mask>();

        if (m_maskComp != null)
        {
            m_maskComp.showMaskGraphic = true;
        }

        if (m_imageComp != null)
        {
            m_imageComp.sprite = m_Sprite;
            m_imageComp.preserveAspect = true;
        }


        transform.GetChild(1).GetComponent<Image>().sprite = m_Sprite;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
