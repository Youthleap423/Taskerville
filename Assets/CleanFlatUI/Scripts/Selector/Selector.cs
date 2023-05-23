using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/* 
//Set properties in C# example codes.
using UnityEngine;
using RainbowArt.CleanFlatUI;
public class SelectorDemo : MonoBehaviour
{
    public Selector mSelector; //The Selector object.
    void Start()
    {
        //Add value changed event listener.
        mSelector.OnValueChanged.AddListener(SelectorValueChange);

        //Set the Selector current value.
        mSelector.CurrentIndex = 1;
    }
    public void SelectorValueChange(int val)
    {
        Debug.Log("SelectorValueChange Current Value is: " + val);
    }
}
*/

namespace RainbowArt.CleanFlatUI
{
    public class Selector : MonoBehaviour 
    {
        [SerializeField]
        Button buttonPrevious;

        [SerializeField]
        Button buttonNext;

        [SerializeField]
        Image imageNew;

        [SerializeField]
        Image imageCurrent;

        [SerializeField]
        Text textNew;

        [SerializeField]
        Text textCurrent;

        [SerializeField]
        bool loop = false;

        [SerializeField]
        bool hasIndicator = false;

        [SerializeField]
        Text indicator;

        [SerializeField]
        RectTransform indicatorRect;

        [SerializeField]
        Animator animator;        

        [SerializeField]
        int startIndex = 0;
        
        [Serializable]
        public class OptionItem
        {
            public string optionText = "option"; 
            public Sprite optionImage;                       
        }

        public OptionItem[] options;

        [Serializable]
        public class SelectorEvent : UnityEvent<int>{ }

        [SerializeField]
        SelectorEvent onValueChanged = new SelectorEvent();

        public SelectorEvent OnValueChanged
        {
            get => onValueChanged;
            set
            {
                onValueChanged = value;
            }
        }

        bool changed = true;
        int newIndex = 0;
        int currentIndex = 0;    

        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                SetCurrentOptions(value);
                onValueChanged.Invoke(currentIndex);
            }
        }

        public int StartIndex
        {
            get => startIndex;
            set
            {
                startIndex = value;
            }
        }

        public bool HasIndicator
        {
            get => hasIndicator;
            set
            {
                hasIndicator = value;
                if (indicator != null && indicator.gameObject.activeSelf != hasIndicator)
                {
                    indicator.gameObject.SetActive(hasIndicator);
                }
            }
        }

        void Start()
        {
            if(buttonPrevious != null)
            {
                buttonPrevious.onClick.AddListener(OnButtonClickPrevious); 
            } 
            if(buttonNext != null)
            {
                buttonNext.onClick.AddListener(OnButtonClickNext); 
            }
            CurrentIndex = startIndex;
        }

        public void OnButtonClickPrevious()
        {
            UpdateOptions(false);            
            if(changed)
            {
                animator.enabled = false; 
                animator.enabled = true;
                animator.Play("Previous",0,0);
                onValueChanged.Invoke(CurrentIndex);
            }                    
        }
        
        public void OnButtonClickNext()
        {
            UpdateOptions(true);                    
            if(changed)
            {
                animator.enabled = false;   
                animator.enabled = true;
                animator.Play("Next",0,0);
                onValueChanged.Invoke(CurrentIndex);
            }                 
        }

        void SetCurrentOptions(int newCurrentIndex)
        {
            currentIndex = newCurrentIndex;
            newIndex = newCurrentIndex;
            textCurrent.text = options[currentIndex].optionText;
            if (imageCurrent != null)
            {
                imageCurrent.sprite = options[currentIndex].optionImage;
            }
            textNew.text = options[newIndex].optionText;
            if (imageNew != null)
            {
                imageNew.sprite = options[newIndex].optionImage;
            }
            if (hasIndicator && (indicator != null))
            {
                indicator.text = (currentIndex + 1) + " / " + options.Length;
            }
        }

        void UpdateOptions(bool bNext)
        {
            changed = true;
            if( bNext )
            {
                if(currentIndex == options.Length -1)
                {
                    if(loop)
                    {
                        newIndex = 0;
                    }
                    else
                    {
                        changed = false;
                    }                    
                }
                else
                {                  
                    newIndex = currentIndex + 1;                    
                }             
            }
            else
            {
                if(currentIndex == 0)
                {
                    if(loop)
                    {
                        newIndex = options.Length -1;
                    }
                    else
                    {
                        changed = false;
                    }                    
                }
                else
                {                 
                    newIndex = currentIndex - 1;                    
                }                 
            } 
            if(changed)
            {                
                textCurrent.text = options[currentIndex].optionText;
                if(imageCurrent != null)
                {
                    imageCurrent.sprite = options[currentIndex].optionImage;
                }                
                textNew.text = options[newIndex].optionText; 
                if(imageNew != null)
                {
                    imageNew.sprite = options[newIndex].optionImage;
                }    
                if(hasIndicator &&(indicator != null))
                {
                    indicator.text = (newIndex+1) +" / "+ options.Length;
                }
                currentIndex = newIndex;
            }          
        }
        #if UNITY_EDITOR
        protected void OnValidate()
        {
            if(indicatorRect != null)
            {
                indicatorRect.gameObject.SetActive(hasIndicator);
            } 
        }
        #endif
    }
}