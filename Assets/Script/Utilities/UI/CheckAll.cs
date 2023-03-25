using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckAll : MonoBehaviour
{
    [SerializeField] public Toggle all_Toggle;
    [SerializeField] public List<Toggle> children_Toggles;

    #region Unity Members
    // Start is called before the first frame update
    void Start()
    {
        all_Toggle.onValueChanged.AddListener(delegate
        {
            AllToggleValueChanged(all_Toggle);
        });
    }

    #endregion

    #region Public Members
    public void AddChildToggle(Toggle toggle)
    {
        children_Toggles.Add(toggle);
    }

    public void ClearChildrenToggles()
    {
        children_Toggles.Clear();
    }
    #endregion

    #region Private Members
    private void AllToggleValueChanged(Toggle parentToggle)
    {
        foreach(Toggle childToggle in children_Toggles)
        {
            childToggle.isOn = parentToggle.isOn;
        }
    }
    #endregion

}
