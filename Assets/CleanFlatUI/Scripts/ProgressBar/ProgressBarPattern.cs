using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
//Set properties in C# example codes.
using UnityEngine;
using RainbowArt.CleanFlatUI;
public class ProgressBarPatternDemo : MonoBehaviour
{  
    public ProgressBarPattern mProgressBar; //The ProgressBar object.
    void Start()
    {    
        mProgressBar.CurrentValue = 50; //Set the current value of the Progress Bar.
        mProgressBar.MaxValue = 100; //Set the maximum value of the Progress Bar.
        mProgressBar.HasText = true; //Set whether to show the text value.      
        mProgressBar.PatternPlay = true; //Set whether to play pattern.      
        mProgressBar.PatternSpeed = 0.5f; //Set the speed of playing pattern.
        mProgressBar.PatternForward = true; //Set whether to play pattern forward.      
        mProgressBar.PatternScale = 5; //Set the scale of the pattern.
    }        
}
*/

namespace RainbowArt.CleanFlatUI
{
    [ExecuteAlways]
    public class ProgressBarPattern : MonoBehaviour
    {
        [SerializeField]
        float currentValue = 0f;

        [SerializeField]
        float maxValue = 100.0f;

        [SerializeField]
        bool hasText = true;

        [SerializeField]
        Text text;  

        [SerializeField]
        Image foreground;

        [SerializeField]
        RawImage patternImage;

        [SerializeField]
        RectTransform patternRect;

        [SerializeField]
        bool patternPlay = true;

        [SerializeField]
        float patternSpeed = 0.5f;

        [SerializeField]
        bool patternForward = true;
        
        [SerializeField]
        float patternScale = 5;

        bool bDelayedUpdate = false;

        public float CurrentValue
        {
            get => currentValue;
            set
            {
                if(currentValue == value)
                {
                    return;
                }
                currentValue = value;
                OnValueChanged();
            }
        }
        
        public float MaxValue
        {
            get => maxValue;
            set
            {
                if(maxValue == value)
                {
                    return;
                }
                maxValue = value;
                OnValueChanged();
            }
        }

        public bool HasText
        {
            get => hasText;
            set
            {
                if (hasText == value)
                {
                    return;
                }
                hasText = value;
                UpdateText();
            }
        }
        
        public bool PatternPlay
        {
            get => patternPlay;
            set
            {
                patternPlay = value;
            }
        }

        public float PatternSpeed
        {
            get => patternSpeed;
            set
            {
                patternSpeed = value;
            }
        }

        public bool PatternForward
        {
            get => patternForward;
            set
            {
                patternForward = value;
            }
        }       

        public float PatternScale
        {
            get => patternScale;
            set
            {
                patternScale = value;
            }
        }

        void OnValueChanged()
        {
            if(maxValue < 0)
            {
                maxValue = 100.0f;
            }
            if(currentValue < 0)
            {
                currentValue = 0f;
            }            
            currentValue = Mathf.Clamp(currentValue, 0, maxValue);
            Rect r = patternImage.uvRect;
            r.width = (currentValue / maxValue)*patternScale;
            patternImage.uvRect = r;
            UpdateGUI();
        }

        void Start()
        {
            UpdateGUI();
        }

        void Update()
        {
            if(Application.isPlaying)
            {
                UpdateGUI();
            }
            else
            {
                if(bDelayedUpdate)
                {
                    bDelayedUpdate = false;
                    OnValueChanged();
                }
            }   
        }
         
        void UpdateGUI()
        {            
            UpdateForegroundAndPattern();  
            UpdateText();
        }

        void UpdateForegroundAndPattern()
        {
            foreground.fillAmount = currentValue / maxValue;
            float maxWidth = foreground.GetComponent<RectTransform>().rect.width;
            Vector2 offsetMax = patternRect.offsetMax;
            offsetMax.x = -(maxWidth - maxWidth*(currentValue / maxValue));
            patternRect.offsetMax = offsetMax;            
            if(patternPlay)
            {
                Rect r = patternImage.uvRect;
                if(patternForward)
                {
                    r.x -= Time.deltaTime * patternSpeed;
                }
                else
                {
                    r.x += Time.deltaTime * patternSpeed;
                }
                r.width = (currentValue / maxValue)*patternScale;
                patternImage.uvRect = r;
            }
            else
            {
                Rect r = patternImage.uvRect;
                r.width = (currentValue / maxValue)*patternScale;
                patternImage.uvRect = r;
            } 
        }
 
        void UpdateText()
        {
            if (text != null && text.gameObject.activeSelf != hasText)
            {
                text.gameObject.SetActive(hasText);
            }
            if (hasText && (text != null))
            {
                text.text = (int)((currentValue/maxValue)*100) + "%";
            }
        }

        #if UNITY_EDITOR
        protected void OnValidate()
        {
            bDelayedUpdate = true;      
        }
        #endif
    }
}