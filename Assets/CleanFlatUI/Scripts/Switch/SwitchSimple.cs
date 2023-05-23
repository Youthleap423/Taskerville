using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/*
//Set properties in C# example codes.
using UnityEngine;
using RainbowArt.CleanFlatUI;
public class SwitchSimpleDemo : MonoBehaviour
{
    public SwitchSimple mSwitchSimple; //The SwitchSimple object.
    void Start()
    {
        //Add value changed event listener.
        mSwitchSimple.OnValueChanged.AddListener(SwitchSimpleValueChange);

        //Set the SwitchSimple current value.
        mSwitchSimple.IsOn = true;
    }
    public void SwitchSimpleValueChange(bool val)
    {
        Debug.Log("SwitchSimpleValueChange Current Value is: " + val);
    } 
}
*/

namespace RainbowArt.CleanFlatUI
{
    public class SwitchSimple : MonoBehaviour,IPointerDownHandler 
    {
        [SerializeField]
        bool isOn = false;  

        [SerializeField]
        RectTransform backgroundOn;

        [SerializeField]
        RectTransform backgroundOff;

        [SerializeField]
        RectTransform handleOn;

        [SerializeField]
        RectTransform handleOff;

        [SerializeField]
        RectTransform handleSlideArea;         
        
        CanvasGroup canvasGroupBGOn;
        CanvasGroup canvasGroupBGOff;
        CanvasGroup canvasGroupOn;
        CanvasGroup canvasGroupOff;
             
        public bool IsOn
        {
            get => isOn;
            set
            {
                if(isOn == value)
                {
                    return;
                }
                isOn = value;
                UpdateGUI();
            }
        }   

        [Serializable]
        public class SwitchSimpleEvent : UnityEvent<bool>{ }

        [SerializeField]
        SwitchSimpleEvent onValueChanged = new SwitchSimpleEvent();      

        public SwitchSimpleEvent OnValueChanged
        {
            get => onValueChanged;
            set
            {
                onValueChanged = value;
            }
        }        

        IEnumerator Start()
        {
            InitGUI();
            yield return null;
            UpdateGUI();                 
        }

        void InitGUI()
        {
            canvasGroupBGOn = backgroundOn.gameObject.GetComponent<CanvasGroup>();
            canvasGroupBGOff = backgroundOff.gameObject.GetComponent<CanvasGroup>();
            canvasGroupOn = handleOn.gameObject.GetComponent<CanvasGroup>();
            canvasGroupOff = handleOff.gameObject.GetComponent<CanvasGroup>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isOn = !isOn;  
            UpdateGUI();      
        }  

        void UpdateGUI()
        {  
            float maxWidth = handleSlideArea.rect.width; 
            handleOn.anchoredPosition3D = new Vector3(maxWidth, 0, 0);
            handleOff.anchoredPosition3D = new Vector3(0, 0, 0);
            
            if(isOn)
            {
                SetCanvasGroupAlpha(canvasGroupBGOn, 1.0f); 
                SetCanvasGroupAlpha(canvasGroupBGOff, 0f); 
                SetCanvasGroupAlpha(canvasGroupOn, 1.0f);                 
                SetCanvasGroupAlpha(canvasGroupOff, 0f);                    
                onValueChanged.Invoke(true);
            }
            else
            {
                SetCanvasGroupAlpha(canvasGroupBGOn, 0f); 
                SetCanvasGroupAlpha(canvasGroupBGOff, 1.0f); 
                SetCanvasGroupAlpha(canvasGroupOn, 0f);                 
                SetCanvasGroupAlpha(canvasGroupOff, 1.0f);       
                onValueChanged.Invoke(false);
            }        
        }       

        void SetCanvasGroupAlpha(CanvasGroup obj,float alpha)
        {
            obj.alpha = alpha;
        }   
    }
}