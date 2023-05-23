using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
//Set properties in C# example codes.
using UnityEngine;
using RainbowArt.CleanFlatUI;
public class ProgressBarCircularMoveAutoDemo : MonoBehaviour
{
    public ProgressBarCircularMoveAuto mProgressBar; //The ProgressBar object.
    void Start()
    {
        mProgressBar.MinValue = 25; //Set the minimum value of the Progress Bar.
        mProgressBar.MaxValue = 100; //Set the maximum value of the Progress Bar.
        mProgressBar.LoadSpeed = 0.2f; //Set the speed of the current progress value auto changing.
        mProgressBar.Forward = true; //Set whether the progress value increasing or decreasing.
        mProgressBar.Loop = true; //Set whether to auto loop.  
        mProgressBar.HasText = false; //Set whether to show the text value.

        //Set the start position of the Progress Bar which include four positions such as Top, Bottom, Left, Right.
        mProgressBar.CurOrigin = ProgressBarCircularMoveAuto.Origin.Top;

        mProgressBar.PatternPlay = true; //Set whether to play pattern.      
        mProgressBar.PatternSpeed = 1.5f; //Set the speed of playing pattern.

        //Set the direction of the pattern moving.
        //If the origin is Top or Bottom, then the patternOrigin is PatternOriginHorizontal.Left or PatternOriginHorizontal.Right.
        //If the origin is Left or Right, then the patternOrigin is PatternOriginVertical.Top or PatternOriginVertical.Bottom.        
        mProgressBar.PatternOrigin = (int)ProgressBarCircularMoveAuto.PatternOriginHorizontal.Left;
    }
}
*/

namespace RainbowArt.CleanFlatUI
{
    [ExecuteAlways]
    public class ProgressBarCircularMoveAuto : MonoBehaviour
    {
        [SerializeField]
        float minValue = 0f;

        [SerializeField]
        float maxValue = 100.0f;

        float currentValue = 0f;

        [Range(0, 1)]
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
        RectTransform foregroundArea;

        public enum Origin
        {
            Bottom,
            Top,
            Left,
            Right,           
        };

        public enum PatternOriginVertical
        {
            Bottom,
            Top,
        };

        public enum PatternOriginHorizontal
        {
            Left,
            Right,
        };

        [SerializeField]   
        RawImage patternImage;  

        [SerializeField] 
        RectTransform patternRect;

        [SerializeField]
        Origin origin = Origin.Bottom;

        [SerializeField]
        bool patternPlay = true;

        [SerializeField]
        float patternSpeed = 1.5f;

        [SerializeField]
        int patternOrigin;

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

        public Origin CurOrigin
        {
            get => origin;
            set
            {
                origin = value;
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

        public int PatternOrigin
        {
            get => patternOrigin;
            set
            {
                patternOrigin = value;
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

        void Start()
        {
            InitValue();
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
            UpdatePattern();
            UpdateText();
        }
        void UpdatePattern()
        {   
            if(patternPlay && Application.isPlaying)
            {
                switch (origin)
                {
                    case Origin.Bottom:
                    case Origin.Top:
                    {
                        Rect r = patternImage.uvRect;
                        if(patternOrigin == (int)PatternOriginHorizontal.Left)
                        {
                            r.x -= Time.deltaTime * patternSpeed;
                        }
                        else
                        {
                            r.x += Time.deltaTime * patternSpeed;
                        }
                        patternImage.uvRect = r;
                        break;
                    }
                    case Origin.Left:
                    case Origin.Right:
                    {
                        Rect r = patternImage.uvRect;
                        if(patternOrigin == (int)PatternOriginVertical.Bottom)
                        {
                            r.y -= Time.deltaTime * patternSpeed;
                        }
                        else
                        {
                            r.y += Time.deltaTime * patternSpeed;
                        }
                        patternImage.uvRect = r;
                        break;
                    }
                }                
            }
        }

        void  UpdateForeground()
        {
            if(currentValue == 0)
            {
                foregroundArea.gameObject.SetActive(false);
            }
            else
            {
                foregroundArea.gameObject.SetActive(true);
                ResetForegroundOrigon();
                switch (origin)
                {
                    case Origin.Bottom:
                    {
                        UpdateForegroundFromBottom();
                        break;
                    }
                    case Origin.Top:
                    {
                        UpdateForegroundFromTop();
                        break;
                    }
                    case Origin.Left:
                    {

                        UpdateForegroundFromLeft();
                        break;
                    }
                    case Origin.Right:
                    {
                        UpdateForegroundFromRight();
                        break;
                    }
                }
            }            
        }

        void ResetForegroundOrigon()
        {
            patternRect.offsetMax = Vector2.zero;
            patternRect.offsetMin = Vector2.zero;
        }

        void UpdateForegroundFromBottom()
        {
            float maxHeight = foregroundArea.rect.height;
            Vector2 offsetMax = patternRect.offsetMax;
            offsetMax.y = -(maxHeight - maxHeight*(currentValue / maxValue));
            Vector2 offsetMin = patternRect.offsetMin;
            offsetMin.y = -(maxHeight - maxHeight*(currentValue / maxValue));
            patternRect.offsetMax = offsetMax;
            patternRect.offsetMin = offsetMin;
        }

         void UpdateForegroundFromTop()
        {
            float maxHeight = foregroundArea.rect.height;
            Vector2 offsetMax = patternRect.offsetMax;
            offsetMax.y = maxHeight - maxHeight*(currentValue / maxValue);
            Vector2 offsetMin = patternRect.offsetMin;
            offsetMin.y = maxHeight - maxHeight*(currentValue / maxValue);
            patternRect.offsetMax = offsetMax;
            patternRect.offsetMin = offsetMin;
        }

        void UpdateForegroundFromLeft()
        {
            float maxWidth = foregroundArea.rect.width;
            Vector2 offsetMax = patternRect.offsetMax;
            offsetMax.x = -(maxWidth - maxWidth*(currentValue / maxValue));
            Vector2 offsetMin = patternRect.offsetMin;
            offsetMin.x = -(maxWidth - maxWidth*(currentValue / maxValue));
            patternRect.offsetMax = offsetMax;
            patternRect.offsetMin = offsetMin;
        }

        void UpdateForegroundFromRight()
        {
            float maxWidth = foregroundArea.rect.width;
            Vector2 offsetMax = patternRect.offsetMax;
            offsetMax.x = maxWidth - maxWidth*(currentValue / maxValue);
            Vector2 offsetMin = patternRect.offsetMin;
            offsetMin.x = maxWidth - maxWidth*(currentValue / maxValue);
            patternRect.offsetMax = offsetMax;
            patternRect.offsetMin = offsetMin;
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

