using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
//Set properties in C# example codes.
using UnityEngine;
using RainbowArt.CleanFlatUI;
public class ProgressBarCircularRoundAutoDemo : MonoBehaviour
{
    public ProgressBarCircularRoundAuto mProgressBar; //The ProgressBar object.
    void Start()
    {
        mProgressBar.MinValue = 25; //Set the minimum value of the Progress Bar.
        mProgressBar.MaxValue = 100; //Set the maximum value of the Progress Bar.
        mProgressBar.LoadSpeed = 0.2f; //Set the speed of the current progress value auto changing.
        mProgressBar.Loop = true; //Set whether to auto loop.  
        mProgressBar.HasText = false; //Set whether to show the text value.

        //If 'Clockwise' is true, then the Progress Bar will auto increase its current progress value clockwise.
        //If 'Clockwise' is false, then the Progress Bar will auto decrease its current progress value anticlockwise.
        mProgressBar.Clockwise = true;    

        //Set the start position of the Progress Bar which include four positions such as Top, Bottom, Left, Right.
        mProgressBar.CurOrigin = ProgressBarCircularRoundAuto.Origin.Bottom;          
    }
}
*/

namespace RainbowArt.CleanFlatUI
{
    [ExecuteAlways]
    public class ProgressBarCircularRoundAuto : MonoBehaviour
    {
        [SerializeField]
        float minValue = 0f;

        [SerializeField]
        float maxValue = 100.0f;

        float currentValue = 0f;
        
        [SerializeField]
        [Range(0,1)]
        float loadSpeed = 0.1f;

        [SerializeField]
        bool clockwise = true;

        [SerializeField]
        bool loop = true;

        [SerializeField]
        bool hasText = true;


        [SerializeField]
        Text text;

        [SerializeField]
        Image foreground;

        [SerializeField]
        RectTransform roundArea;      

        [SerializeField]  
        Image roundImage;
        
        public enum  Origin
        {
            Bottom,
            Right,
            Top,
            Left,            
        };

        [SerializeField]
        Origin origin = Origin.Bottom;     

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

        public bool Clockwise
        {
            get => clockwise;
            set
            {
                clockwise = value;
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
            if(clockwise)
            {
                currentValue = minValue;
            }
            else
            {
                currentValue = maxValue;
            }
        }

        void InitRoundImage()
        {
            if(currentValue <= 0f)
            {
                roundArea.gameObject.SetActive(false);
            }
            else
            {
                roundArea.gameObject.SetActive(true);
            }
        }

        void Start()
        {
            InitValue();
            InitRoundImage();
            UpdateGUI();
        }

        void Update()
        {
            if(Application.isPlaying)
            {
                if (currentValue < maxValue )
                {
                    currentValue += loadSpeed* (Time.deltaTime*100);
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
                if(bDelayedUpdate)
                {
                    bDelayedUpdate = false;
                    OnValueChanged();
                } 
            }     
        }

        public void UpdateGUI()
        {
            UpdateForeground();            
            UpdateRoundArea();                         
            UpdateText();
        }

        void UpdateForeground()
        {
            foreground.fillAmount = currentValue / maxValue;
            foreground.fillMethod = Image.FillMethod.Radial360;
            foreground.fillOrigin = (int)origin;
            foreground.fillClockwise = clockwise;
            if(clockwise)
            {
                roundImage.fillOrigin = (int)Origin.Right;
            }
            else
            {
                roundImage.fillOrigin = (int)Origin.Left;
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
                text.text = (int)((currentValue / maxValue) * 100) + "%";
            }
        }

        void UpdateRoundArea()
        {      
            if(currentValue <= 0f)
            {
                roundArea.gameObject.SetActive(false);
            }
            else
            {
                roundArea.gameObject.SetActive(true);
                Vector3 capRotaionValue = Vector3.zero;
                switch (origin)
                {
                    case Origin.Top:
                    {
                        if( clockwise)
                        {
                            capRotaionValue.z = 360 * (1 - foreground.fillAmount);
                            
                        }                        
                        else
                        {
                            capRotaionValue.z = 360 * (foreground.fillAmount);    
                        }
                        break;
                    }
                    case Origin.Bottom:
                    {
                        if( clockwise)
                        {
                            capRotaionValue.z = 360 * (1 - foreground.fillAmount)+180;
                            
                        }
                        else
                        {
                            capRotaionValue.z = 360 * (foreground.fillAmount)-180;    
                        }
                        break;
                    }
                    case Origin.Right:
                    {
                        if( clockwise)
                        {
                            capRotaionValue.z = 360 * (1 - foreground.fillAmount)+270;
                        }
                        else
                        {
                            capRotaionValue.z = 360 * (foreground.fillAmount)+270;    
                        }
                        break;
                    }
                    case Origin.Left:
                    {
                        if( clockwise)
                        {
                            capRotaionValue.z = 360 * (1 - foreground.fillAmount)+90;
                        }
                        else
                        {
                            capRotaionValue.z = 360 * (foreground.fillAmount)+90;    
                        }
                        break;
                    }
                }            
                capRotaionValue.z = capRotaionValue.z % 360;
                roundArea.localEulerAngles = capRotaionValue;
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