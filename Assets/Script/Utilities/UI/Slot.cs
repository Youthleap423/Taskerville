using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public List<GameObject> items;
    public GameObject spaceObjPrefab;
    public GameObject draggingObj;

    public System.Action<bool> OnDragDropFinished;
    bool isEntered;

    private GameObject spaceObj = null;
    private int nCurrentIndex = 0;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isEntered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEntered = false;
    }

    public void CreateSpaceObj(int index, GameObject dragObj)
    {
        nCurrentIndex = index;

        foreach (GameObject item in items)
        {
            int itemIndex = item.GetComponentInChildren<DragHandler>().getIndex();
            if (itemIndex >= index)
            {
                item.GetComponentInChildren<DragHandler>().setIndex(itemIndex -1);
            }
        }

        draggingObj = dragObj;
        spaceObj = GameObject.Instantiate(spaceObjPrefab, transform);
        spaceObj.transform.SetSiblingIndex(index);
    }

    public void DeleteSpaceObj(int index)
    {
        if (spaceObj != null)
        {
            Destroy(spaceObj);
        }

        draggingObj.transform.parent = transform;
        draggingObj.transform.SetSiblingIndex(nCurrentIndex);

        foreach (GameObject item in items)
        {
            int itemIndex = item.GetComponentInChildren<DragHandler>().getIndex();
            if (itemIndex >= nCurrentIndex)
            {
                item.GetComponentInChildren<DragHandler>().setIndex(itemIndex + 1);
            }
        }

        draggingObj.GetComponentInChildren<DragHandler>().setIndex(nCurrentIndex);

        draggingObj = null;
        spaceObj = null;

        OnDragDropFinished(true);

    }

    public void MoveSpaceObj(int index)
    {
        
        if (spaceObj != null)
        {
            nCurrentIndex = index;
            spaceObj.transform.SetSiblingIndex(index);
        }
    }

    public void OnItemDrag(int index)
    {
        bool shouldBeLastIndex = true;
        foreach (GameObject item in items)
        {
            if (item == draggingObj)
            {
                continue;
            }

            RectTransform invPanel = item.transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition))
            {
                MoveSpaceObj(item.GetComponentInChildren<DragHandler>().getIndex());
                shouldBeLastIndex = false;
            }
            else
            {
                if (Input.mousePosition.y >= item.transform.position.y)
                {
                    shouldBeLastIndex = false;
                }
            }
        }
        if (items.Count > 0 && shouldBeLastIndex == true)
        {
            MoveSpaceObj(items.Count);
        }
        
    }
}