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
public class SwitchDemo : MonoBehaviour
{
    public Switch mSwitch; //The Switch object.
    void Start()
    {
        //Add value changed event listener.
        mSwitch.onValueChanged.AddListener(SwitchValueChange);

        //Set the Switch current value.
        mSwitch.IsOn = true;
    }
    public void SwitchValueChange(bool val)
    {
        Debug.Log("SwitchValueChange Current Value is: " + val);
    } 
}
*/

namespace RainbowArt.CleanFlatUI
{
    public class WindowDragger : MonoBehaviour
    {

        private bool isDragging = false;
        private Vector3 offset = Vector3.zero;

        void OnMouseDown() 
        {
            isDragging = true;
            offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        void OnMouseDrag() 
        {
            if (isDragging) 
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = mousePos + offset;
            }
        }

        void OnMouseUp() 
        {
            isDragging = false;
            offset = Vector3.zero;
        }
    }
}