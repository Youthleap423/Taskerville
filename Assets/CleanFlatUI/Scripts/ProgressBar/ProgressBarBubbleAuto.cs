using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
//Set properties in C# example codes.
using UnityEngine;
using RainbowArt.CleanFlatUI;
public class ProgressBarBubbleAutoDemo : MonoBehaviour
{
    public ProgressBarBubbleAuto mProgressBar; //The Progress Bar object.
    void Start()
    {
        mProgressBar.MinValue = 25; //Set the minimum value of the Progress Bar.
        mProgressBar.MaxValue = 100; //Set the maximum value of the Progress Bar.
        mProgressBar.LoadSpeed = 0.2f; //Set the speed of the current progress value auto changing.
        mProgressBar.Forward = true; //Set whether the progress value increasing or decreasing.
        mProgressBar.Loop = true; //Set whether to auto loop.  
        mProgressBar.HasText = false; //Set whether to show the text value.
    }
}
*/

namespace RainbowArt.CleanFlatUI
{
    [ExecuteAlways]
    public class ProgressBarBubbleAuto : MonoBehaviour
    {
        [SerializeField]
        float minValue = 0f;

        [SerializeField]
        float maxValue = 100.0f;

        float currentValue = 0f;

        [Range(0,1)]
        [SerializeField]
        float loadSpeed = 0.1f;

        [SerializeField]
        bool forward = true;

        [SerializeField]
        bool loop = true;

        [SerializeField]
        bool hasText = true;

        [SerializeField]
        Text text;  

        [SerializeField]
        Image foreground;

        [SerializeField]
        RectTransform bubble;

        bool bDelayedUpdate = false;

        public float MinValue
        {
            get => minValue;
            set
            {
                if(minValue == value)
                {
                    return;
                }
                minValue = value;
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

        public float LoadSpeed
        {
            get => loadSpeed;
            set 
            { 
                loadSpeed = value; 
            }
        }

        public bool Forward
        {
            get => forward;
            set
            {
                forward = value;
            }
        }

        public bool Loop
        {
            get => loop;
            set
            {
                loop = value;
            }
        }       

        void OnValueChanged()
        {
            if(maxValue < 0)
            {
                maxValue = 100.0f;
            }
            if(minValue < 0)
            {
                minValue = 0f;
            }
            currentValue = Mathf.Clamp(minValue, 0, maxValue);
            UpdateGUI();
        }

        void InitValue()
        {
            if(forward)
            {
                currentValue = minValue;
            }
            else
            {
                currentValue = maxValue;
            }
        }

        void OnEnable()
        {
            InitValue();
        }

        void Start()
        {
            UpdateGUI();
        }

        void Update()
        {
            if(Application.isPlaying)
            {
                if(forward)
                {
                    if (currentValue < maxValue)
                    {
                        currentValue += loadSpeed * (Time.deltaTime * 100);
                        if (currentValue >= maxValue)
                        {
                            currentValue = maxValue;
                        }
                        UpdateGUI();                        
                    }
                    if(loop)
                    {
                        if (currentValue >= maxValue)
                        {
                            currentValue = minValue;
                        }
                    }
                }
                else
                {
                    if (currentValue > minValue)
                    {
                        currentValue -= loadSpeed * (Time.deltaTime * 100);
                        if (currentValue <= minValue)
                        {
                            currentValue = minValue;
                        }
                        UpdateGUI();
                    }
                    if(loop)
                    {
                        if (currentValue <= minValue)
                        {
                            currentValue = maxValue;
                        }
                    }
                }                
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
            UpdateForeground();  
            UpdateText();            
        }

        void UpdateForeground()
        {
            foreground.fillAmount = currentValue / maxValue;            
        }

        void UpdateText()
        {
            if (bubble != null && bubble.gameObject.activeSelf != hasText)
            {
                bubble.gameObject.SetActive(hasText);
            }
            if (hasText && (text != null) && (bubble != null))
            {
                text.text = (int)((currentValue / maxValue) * 100) + "%";
                float totalWidth = foreground.rectTransform.rect.width;
                float filledWidth = totalWidth * foreground.fillAmount;
                float x = -totalWidth / 2.0f + filledWidth;
                Vector3 bubblePos = bubble.anchoredPosition3D;
                bubblePos.x = x;
                bubble.anchoredPosition3D = bubblePos;
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