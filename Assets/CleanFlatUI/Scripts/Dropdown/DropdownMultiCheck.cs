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
public class DropdownMultiCheckDemo : MonoBehaviour
{  
    public DropdownMultiCheck mDropdownMultiCheck; //The DropdownMultiCheck object.
    void Start()
    {
        //Add select changed event listener.
        mDropdownMultiCheck.OnSelectValueChanged.AddListener(DropdownSelectValueChanged);
    }

    // set all of the selected option indexes
    public void SetSelectedOptions()
    {
        mDropdownMultiCheck.SelectedOptions = new int[] { 1, 2 ,7};
    } 

    // set an option to be selected
    public void SetOneSelectedOption()
    {
        mDropdownMultiCheck.SetOptionSelected(2, true);
    }   

    // unselect all options
    public void UnSelectAllOptions()
    {
        mDropdownMultiCheck.UnSelecteAll();
    } 

    // check whether an option is selected
    public bool IsOptionSelected(int index)
    {
        return mDropdownMultiCheck.IsOptionSelected(index);
    }

    public void DropdownSelectValueChanged()
    {
        int[] selectedIndexes = mDropdownMultiCheck.SelectedOptions;
        Debug.Log("current selected indexes changed");
    }  
}
*/

namespace RainbowArt.CleanFlatUI
{
    public class DropdownMultiCheck : Dropdown
    {        
        [SerializeField]
        List<int> selectedOptions = new List<int>();

        [Serializable]
        public class DropdownMultiCheckEvent : UnityEvent { }

        [SerializeField]
        DropdownMultiCheckEvent onSelectValueChanged = new DropdownMultiCheckEvent();

        public DropdownMultiCheckEvent OnSelectValueChanged
        {
            get => onSelectValueChanged;
            set
            {
                onSelectValueChanged = value;
            }
        }  

        Toggle[] toggleList;
        HashSet<int> selectedOptionsHashSet = new HashSet<int>();

        public int[] SelectedOptions
        {
            get
            {
                int[] ret = new int[selectedOptionsHashSet.Count];
                selectedOptionsHashSet.CopyTo(ret);
                return ret;
            }
            set
            {
                selectedOptionsHashSet.Clear();
                if (value != null)
                {
                    foreach (int index in value)
                    {
                        selectedOptionsHashSet.Add(index);
                    }
                }
                onSelectValueChanged.Invoke();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            foreach(int index in selectedOptions)
            {
                selectedOptionsHashSet.Add(index);
            }
        }

        public bool IsOptionSelected(int index)
        {
            return selectedOptionsHashSet.Contains(index);
        }

        public void SetOptionSelected(int index,bool selected,bool sendEvent = true)
        {
            if (IsOptionSelected(index) == selected)
            {
                return;
            }
            if(selected)
            {
                selectedOptionsHashSet.Add(index);
            }
            else
            {
                selectedOptionsHashSet.Remove(index);
            }
            if(sendEvent)
            {
                onSelectValueChanged.Invoke();
            }
        }

        public void UnSelecteAll()
        {
            int count = selectedOptionsHashSet.Count;
            selectedOptionsHashSet.Clear();
            if(count > 0)
            {
                onSelectValueChanged.Invoke();
            }
        }

        public new void Show()
        {
            Transform dropdownList = transform.Find("Dropdown List");
            if(dropdownList != null)
            {
                return;
            }
            base.Show();
            Transform contentTransform = transform.Find("Dropdown List/Viewport/Content");
            toggleList = contentTransform.GetComponentsInChildren<Toggle>(false);

            for (int i = 0; i < toggleList.Length; i++)
            {
                int index = i;
                Toggle item = toggleList[i];
                item.onValueChanged.RemoveAllListeners();
                item.onValueChanged.AddListener(x => OnSelectItemCustom(index,x));
                item.SetIsOnWithoutNotify(IsOptionSelected(i));
            } 
        }

        public new void Hide()
        {
            base.Hide();  
        }
       
        public override void OnPointerClick(PointerEventData eventData)
        {
           Show();
        }

        void OnSelectItemCustom(int selectedIndex, bool isSelected)
        {
            SetOptionSelected(selectedIndex, isSelected);
        }
    }
}