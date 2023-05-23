using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/*
//Set properties in C# example codes.
using UnityEngine;
using RainbowArt.CleanFlatUI;
public class TabViewSimpleDemo : MonoBehaviour
{
    public TabViewSimple mTabViewSimple; //The TabViewSimple object.
    void Start()
    {
        //Add value changed event listener.
        mTabViewSimple.OnValueChanged.AddListener(TabViewSimpleValueChange);

        //Set the Tab View current value.
        mTabViewSimple.CurrentIndex = 1;        
    }
    public void TabViewSimpleValueChange(int val)
    {
        Debug.Log("TabViewSimpleValueChange Current Value is: " + val);
    }
}
*/

namespace RainbowArt.CleanFlatUI
{
    public class TabViewSimple : MonoBehaviour
    {
        [SerializeField]
        int startIndex = 0;
       
        [SerializeField]
        TabViewSimpleItem[] TabViewSimples;

        [Serializable]
        public class TabViewSimpleItem
        {
            public GameObject tab; 
            public GameObject view;         
        }       

        [Serializable]
        public class TabViewSimpleEvent : UnityEvent<int>{ }

        [SerializeField]
        TabViewSimpleEvent onValueChanged = new TabViewSimpleEvent();  

        public TabViewSimpleEvent OnValueChanged
        {
            get => onValueChanged;
            set
            {
                onValueChanged = value;
            }
        }  

        int currentIndex = 0;  

        public int StartIndex
        {
            get => startIndex;
            set
            {
                startIndex = value;
            }
        }   

        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                if(currentIndex == value)
                {
                    return;
                }
                SetCurrentIndex(value);
                onValueChanged.Invoke(currentIndex);
            }
        }   

        void OnEnable()
        {
            InitTabViewSimples();                    
        }
        
        public void OnDisable()
        {
            for (int i = 0; i < TabViewSimples.Length; i++)
            {
                int index = i;
                TabViewSimpleItem item = TabViewSimples[i];
                Toggle toggle = item.tab.GetComponent<Toggle>();
                toggle.onValueChanged.RemoveAllListeners();
            }
        }

        public void InitTabViewSimples()
        {
            SetCurrentIndex(startIndex);             
            onValueChanged.Invoke(currentIndex);
            for (int i = 0; i < TabViewSimples.Length; i++)
            {
                int index = i;
                TabViewSimpleItem item = TabViewSimples[i];
                Toggle toggle = item.tab.GetComponent<Toggle>();
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((bool value) => TabValueChanged(index, value));
            }
        }

        void SetCurrentIndex(int newCurrentIndex)
        {
            for (int i = 0; i < TabViewSimples.Length; i++)
            {
                int index = i;
                TabViewSimpleItem item = TabViewSimples[i];
                Toggle toggle = item.tab.GetComponent<Toggle>();
                if(i == newCurrentIndex)
                {
                    toggle.SetIsOnWithoutNotify(true);                    
                    CanvasGroup canvasGroup  = item.view.GetComponent<CanvasGroup>();
                    SetCanvasGroupAlpha(canvasGroup, 1.0f);
                    item.tab.GetComponent<TabSimple>().SetTabOn(true);
                }
                else
                {
                    toggle.SetIsOnWithoutNotify(false);
                    CanvasGroup canvasGroup  = item.view.GetComponent<CanvasGroup>();
                    SetCanvasGroupAlpha(canvasGroup, 0f);
                    item.tab.GetComponent<TabSimple>().UpdateStatusContent();
                }
            }
            currentIndex = newCurrentIndex;           
        }

        public void TabValueChanged(int index, bool value)
        {
            TabViewSimpleItem item = TabViewSimples[index];
            Toggle toggle = item.tab.GetComponent<Toggle>();     
            Tab tab = item.tab.GetComponent<Tab>();                    
            if (toggle.isOn)
            {
                currentIndex = index;
                onValueChanged.Invoke(currentIndex);
                CanvasGroup canvasGroup  = item.view.GetComponent<CanvasGroup>();
                SetCanvasGroupAlpha(canvasGroup, 1.0f);  
                item.tab.GetComponent<TabSimple>().SetTabOn(true);
            }
            else
            {
                CanvasGroup canvasGroup  = item.view.GetComponent<CanvasGroup>();
                SetCanvasGroupAlpha(canvasGroup, 0f);
                item.tab.GetComponent<TabSimple>().SetTabOn(false);
            }
        }         

        void SetCanvasGroupAlpha(CanvasGroup obj,float alpha)
        {
            obj.alpha = alpha;
        }   
    }
}