using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
//Set properties in C# example codes.
using UnityEngine;
using RainbowArt.CleanFlatUI;
public class ContextMenuDemo : MonoBehaviour
{  
    public ContextMenu mContextMenu; //The ContextMenu object.
    void Start()
    {
        //Add select changed event listener.
        mContextMenu.OnValueChanged.AddListener(onSelectValueChanged);
    }
    public void onSelectValueChanged(int index)
    {
        Debug.Log("current selected indexes changed");
    }  
}
*/

namespace RainbowArt.CleanFlatUI
{
    [ExecuteAlways]
    public class ContextMenu : MonoBehaviour, IPointerDownHandler
    {
        protected internal class ContextMenuItem : MonoBehaviour, IPointerEnterHandler
        {
            public Text itemText;
            public Image itemImage;
            public Button button;
            public virtual void OnPointerEnter(PointerEventData eventData)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }

        [SerializeField]
        GameObject itemTemplate;

        [SerializeField]
        Text itemText;

        [SerializeField]
        Image itemImage;

        [SerializeField]
        Animator animator;

        [SerializeField] 
        RectOffset padding = new RectOffset();

        [SerializeField]
        float spacing = 2;

        [Serializable]
        public class OptionItem
        {
            public string text;
            public Sprite icon;
        }

        [SerializeField]
        List<OptionItem> options = new List<OptionItem>();

        [Serializable]
        public class ContextMenuEvent: UnityEvent<int> { }

        [SerializeField]
        ContextMenuEvent onValueChanged = new ContextMenuEvent();            
        
        enum Origin
        {
            RightBottom,
            LeftBottom, 
            RightTop,
            LeftTop, 
        }   
        Origin origin = Origin.RightBottom;   

        List<ContextMenuItem> menuItems = new List<ContextMenuItem>();
        GameObject clickerBlocker;
        IEnumerator diableCoroutine; 
        float disableTime = 0.15f; 

        float distanceX = 2.0f;
        float distanceY = 2.0f;

        public ContextMenuEvent OnValueChanged
        {
            get => onValueChanged;
            set
            {
                onValueChanged = value;
            }
        }  

        public RectOffset Padding
        {
            get => padding;
            set
            {
                padding = value;
            }
        }

        public float Spacing
        {
            get => spacing;
            set
            {
                spacing = value;
            }
        }

        public void Show(Vector2 mousePosition, RectTransform areaRect)
        {
            if(options.Count > 0)
            {
                gameObject.SetActive(true);
                DestroyAllMenuItems();
                DestroyClickBlocker();
                SetupTemplate();
                CreateAllMenuItems(options);
                UpdatePosition(mousePosition,areaRect);
                CreateClickBlocker();
                PlayAnimation(true);
            }            
        }


        public void AddOptions(List<OptionItem> optionList)
        {
            options.AddRange(optionList);
        }

        public void ClearOptions()
        {
            options.Clear();
        }

        void SetupTemplate()
        {
            ContextMenuItem menuItem = itemTemplate.GetComponent<ContextMenuItem>();
            if (menuItem == null)
            {
                menuItem = itemTemplate.AddComponent<ContextMenuItem>();
                menuItem.itemText = itemText;
                menuItem.itemImage = itemImage;                
                menuItem.button = itemTemplate.GetComponent<Button>();
            }
            itemTemplate.SetActive(false);
        }

        void CreateAllMenuItems(List<OptionItem> options)
        {
            float itemWidth = itemTemplate.GetComponent<RectTransform>().rect.width;
            RectTransform templateParentTransform = itemTemplate.transform.parent as RectTransform;
            int dataCount = options.Count;
            float curY = -padding.top;
            for (int i = 0; i < dataCount; ++i)
            {
                OptionItem itemData = options[i];
                int index = i;
                GameObject go = Instantiate(itemTemplate) as GameObject;
                go.transform.SetParent(itemTemplate.gameObject.transform.parent, false);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                go.SetActive(true);
                go.name = "MenuItem" + i;
                ContextMenuItem curMenuItem = go.GetComponent<ContextMenuItem>();
                menuItems.Add(curMenuItem);
                curMenuItem.itemText.text = itemData.text;
                curMenuItem.itemImage.sprite = itemData.icon;
                curMenuItem.button.onClick.AddListener(delegate { OnItemClicked(index); });
                
                RectTransform curRectTransform = go.GetComponent<RectTransform>();
                curRectTransform.anchoredPosition3D = new Vector3(padding.left, curY, 0);
                float curItemHeight = curRectTransform.rect.height;
                curY = curY - curItemHeight;
                if (i < (dataCount - 1))
                {
                    curY = curY - spacing;
                }                
            }
            
            gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(curY) + padding.bottom);
            gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemWidth + padding.left+padding.right);
        }


        Canvas GetRootCanvas()
        {
            List<Canvas> list = new List<Canvas>();
            gameObject.GetComponentsInParent(false, list);
            if (list.Count == 0)
            {
                return null;
            }
            var listCount = list.Count;
            Canvas rootCanvas = list[listCount - 1];
            for (int i = 0; i < listCount; i++)
            {
                if (list[i].isRootCanvas || list[i].overrideSorting)
                {
                    rootCanvas = list[i];
                    break;
                }
            }
            return rootCanvas;
        }


        RectTransform GetRootCanvasRectTrans()
        {
            Canvas rootCanvas = GetRootCanvas();
            if (rootCanvas == null)
            {
                return null;
            }
            return rootCanvas.GetComponent<RectTransform>();
        }

        void CreateClickBlocker()
        {
            Canvas rootCanvas = GetRootCanvas();
            if(rootCanvas == null)
            {
                return;
            }
            clickerBlocker = new GameObject("ClickBlocker");
            RectTransform blockerRect = clickerBlocker.AddComponent<RectTransform>();
            blockerRect.anchorMin = new Vector2(0.5f, 0.5f);
            blockerRect.anchorMax = new Vector2(0.5f, 0.5f);
            blockerRect.pivot = new Vector2(0.5f, 0.5f);
            Image blockerImage = clickerBlocker.AddComponent<Image>();
            blockerImage.color = Color.clear;
            RectTransform rootCanvasRect = rootCanvas.GetComponent<RectTransform>();
            float rootCanvasWidth = rootCanvasRect.rect.width;
            float rootCanvasHeight = rootCanvasRect.rect.height;
            blockerRect.SetParent(rootCanvas.transform, false);
            blockerRect.localPosition = Vector3.zero;
            blockerRect.SetParent(gameObject.transform, true);
            blockerRect.SetAsFirstSibling();
            blockerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rootCanvasHeight);
            blockerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rootCanvasWidth);
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            Hide(true);
        }


        void OnItemClicked(int index)
        {
            onValueChanged.Invoke(index);
            Hide(true);
        }

        void DestroyAllMenuItems()
        {
            var itemsCount = menuItems.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                if (menuItems[i] != null)
                {
                    Destroy(menuItems[i].gameObject);
                }
            }
            menuItems.Clear();
        }

        void DestroyClickBlocker()
        {
            if(clickerBlocker != null)
            {
                Destroy(clickerBlocker);
                clickerBlocker = null;
            }
        }

        public void Hide(bool playAnim)
        {
            if (diableCoroutine != null)
            {
                StopCoroutine(diableCoroutine);
                diableCoroutine = null;
            }
            if(playAnim)
            {
                PlayAnimation(false);
                diableCoroutine = DisableTransition();
                StartCoroutine(diableCoroutine);
            }
            else
            {
                if (animator != null)
                {
                    animator.enabled = false;
                    animator.gameObject.transform.localScale = Vector3.one;
                    animator.gameObject.transform.localEulerAngles = Vector3.zero;
                }
                gameObject.SetActive(false);
                DestroyAllMenuItems();
                DestroyClickBlocker();
            }
                 
        }   

        IEnumerator DisableTransition()
        {
            yield return new WaitForSeconds(disableTime);
            gameObject.SetActive(false);
            DestroyAllMenuItems();       
        }    

        public bool IsShowing()
        {
            return gameObject.activeSelf;
        }

        void UpdatePosition(Vector2 mousePosition, RectTransform areaRect)
        {
            if (areaRect == null)
            {
                areaRect = GetRootCanvasRectTrans();
                if (areaRect == null)
                {
                    return;
                }
            }
            RectTransform contextMenuRect = GetComponent<RectTransform>();      
            contextMenuRect.localPosition = new Vector3(mousePosition.x,mousePosition.y,0);                                
            Vector3[] corners = new Vector3[4];
            contextMenuRect.GetWorldCorners(corners);
            Vector3[] cornersInArea = new Vector3[4];
            for(int i = 0; i < 4; i++)
            {
                cornersInArea[i] = areaRect.InverseTransformPoint(corners[i]); 
            } 
            if(cornersInArea[2].x >= areaRect.rect.xMax)
            {
                if(cornersInArea[0].y < areaRect.rect.yMin)
                {
                    origin = Origin.LeftTop;
                }
                else
                {
                    origin = Origin.LeftBottom;
                }
            }            
            else if(cornersInArea[0].y < areaRect.rect.yMin)
            {
                origin = Origin.RightTop;                          
            }   
            else
            {
                origin = Origin.RightBottom;     
            }                     
            float contextMenuPosX = contextMenuRect.anchoredPosition3D.x;
            float contextMenuPosY = contextMenuRect.anchoredPosition3D.y;
            float contextMenuWidth = contextMenuRect.rect.width;
            float contextMenuHeight = contextMenuRect.rect.height;
            switch (origin)
            {
                case Origin.RightBottom:
                {
                    contextMenuPosX = contextMenuPosX + distanceX;
                    contextMenuPosY = contextMenuPosY - distanceY;
                    break;
                }
                case Origin.RightTop:
                {
                    contextMenuPosX = contextMenuPosX + distanceX;
                    contextMenuPosY = contextMenuPosY + contextMenuHeight + distanceY;
                    break;
                }
                case Origin.LeftTop:
                {
                    contextMenuPosX = contextMenuPosX - contextMenuWidth - distanceX;
                    contextMenuPosY = contextMenuPosY + contextMenuHeight + distanceY; 
                    break;
                }               
                case Origin.LeftBottom:
                {
                    contextMenuPosX = contextMenuPosX - contextMenuWidth - distanceX;
                    contextMenuPosY = contextMenuPosY - distanceY;                     
                    break;
                } 
            }         
            Vector3 contextMenuPos = new Vector3(contextMenuPosX,contextMenuPosY,0);            
            contextMenuRect.anchoredPosition3D = contextMenuPos;  
        }

        void PlayAnimation(bool bOpen)
        {
            if (animator != null)
            {
                animator.enabled = false;
                animator.gameObject.transform.localScale = Vector3.one;
                animator.gameObject.transform.localEulerAngles = Vector3.zero;
            }
            if (animator != null)
            {
                if(animator.enabled == false)
                {
                    animator.enabled = true;
                }
                string animationStr = null; 
                if(bOpen)
                {      
                    animationStr =  "In Right Bottom";               
                    switch (origin)
                    {
                        case Origin.RightBottom:
                        {                            
                            break;
                        }
                        case Origin.RightTop:
                        {
                            animationStr = "In Right Top"; 
                            break;
                        }
                        case Origin.LeftTop:
                        {
                            animationStr = "In Left Top"; 
                            break;
                        }               
                        case Origin.LeftBottom:
                        {
                            animationStr = "In Left Bottom"; 
                            break;
                        } 
                    }                       
                }
                else
                {
                    animationStr = "Out Right Bottom"; 
                    switch (origin)
                    {
                        case Origin.RightBottom:
                        {                            
                            break;
                        }
                        case Origin.RightTop:
                        {
                            animationStr = "Out Right Top"; 
                            break;
                        }
                        case Origin.LeftTop:
                        {
                            animationStr = "Out Left Top"; 
                            break;
                        }               
                        case Origin.LeftBottom:
                        {
                            animationStr = "Out Left Bottom"; 
                            break;
                        } 
                    }                    
                }
                animator.Play(animationStr,0,0);
            }            
        }   
    }
}