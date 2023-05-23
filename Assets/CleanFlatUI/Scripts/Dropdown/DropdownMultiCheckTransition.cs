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
public class DropdownMultiCheckTransitionDemo : MonoBehaviour
{  
    public DropdownMultiCheckTransition mDropdownMultiCheckTransition; //The DropdownMultiCheckTransition object.
    void Start()
    {
        //Add select changed event listener.
        mDropdownMultiCheckTransition.OnSelectValueChanged.AddListener(DropdownSelectValueChanged);
    }

    // set all of the selected option indexes
    public void SetSelectedOptions()
    {
        mDropdownMultiCheckTransition.SelectedOptions = new int[] { 1, 2 ,7};
    } 

    // set an option to be selected
    public void SetOneSelectedOption()
    {
        mDropdownMultiCheckTransition.SetOptionSelected(2, true);
    }   

    // unselect all options
    public void UnSelectAllOptions()
    {
        mDropdownMultiCheckTransition.UnSelecteAll();
    } 

    // check whether an option is selected
    public bool IsOptionSelected(int index)
    {
        return mDropdownMultiCheckTransition.IsOptionSelected(index);
    }

    public void DropdownSelectValueChanged()
    {
        int[] selectedIndexes = mDropdownMultiCheckTransition.SelectedOptions;
        Debug.Log("current selected indexes changed");
    }       

}
*/

namespace RainbowArt.CleanFlatUI
{
    public class DropdownMultiCheckTransition : Dropdown
    {     
        [SerializeField]
        List<int> selectedOptions = new List<int>();

        [Serializable]
        public class DropdownMultiCheckTransitionEvent : UnityEvent { }

        [SerializeField]
        DropdownMultiCheckTransitionEvent onSelectValueChanged = new DropdownMultiCheckTransitionEvent();
        
        public DropdownMultiCheckTransitionEvent OnSelectValueChanged
        {
            get => onSelectValueChanged;
            set
            {
                onSelectValueChanged = value;
            }
        }  

        Toggle[] toggleList;
        Animator animatorList;   

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
            if (dropdownList != null)
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

            if(animatorList == null)
            {
                Transform listTransform = transform.Find("Dropdown List");
                animatorList = listTransform.gameObject.GetComponent<Animator>();                
            }       
            PlayAnimation(true);      
        }

        public new void Hide()
        {
            if(animatorList == null)
            {
                Transform listTransform = transform.Find("Dropdown List");
                animatorList = listTransform.gameObject.GetComponent<Animator>();                
            } 
            PlayAnimation(false);
            
            base.Hide();  
        }

        void PlayAnimation(bool bStart)
        {
            if(animatorList != null)
            {
                if(animatorList.enabled == false)
                {
                    animatorList.enabled = true;
                }
                if(bStart)
                {
                    animatorList.Play("In",0,0);  
                }
                else
                {
                    animatorList.Play("Out",0,0);  
                }
            }            
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