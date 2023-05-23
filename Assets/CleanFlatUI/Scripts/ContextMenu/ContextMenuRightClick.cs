using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace RainbowArt.CleanFlatUI
{
    public class ContextMenuRightClick : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        ContextMenu contextMenu;   

        RectTransform areaRect;
       
        void Start()
        {
            areaRect = GetComponent<RectTransform>();    
            contextMenu.gameObject.SetActive(false);     
        }

        public void OnPointerClick(PointerEventData eventData)
        {            
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                Vector2 mousePos = Vector2.zero;
                RectTransform contextMenuRect = contextMenu.gameObject.GetComponent<RectTransform>();
                RectTransform contextMenuParentRect = contextMenuRect.parent as RectTransform;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(contextMenuParentRect, Input.mousePosition, eventData.enterEventCamera, out mousePos);
                contextMenu.Show(mousePos, areaRect);                      
            }           
        }
    }
}