using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]public bool dragOnSurfaces = true;
    [SerializeField]public static bool isDragging = false;
    [SerializeField] private int nIndex;

    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;

    
    
    private Slot slotComponent;

    private void Start()
    {
        slotComponent = FindInParents<Slot>(gameObject);
    }


    public void setIndex(int index)
    {
        nIndex = index;
    }

    public int getIndex()
    {
        return nIndex;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;

        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.
        

        slotComponent.CreateSpaceObj(nIndex, transform.parent.parent.gameObject);

        m_DraggingIcon = transform.parent.parent.gameObject;// new GameObject("icon");

        m_DraggingIcon.transform.SetParent(canvas.transform, false);
        m_DraggingIcon.transform.SetAsLastSibling();

        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData data)
    {
        if (m_DraggingIcon != null)
            SetDraggedPosition(data);
    }

    private void SetDraggedPosition(PointerEventData data)
    {
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = m_DraggingPlane.rotation;
        }
        slotComponent.OnItemDrag(nIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        slotComponent.DeleteSpaceObj(nIndex);
    }

    
    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}