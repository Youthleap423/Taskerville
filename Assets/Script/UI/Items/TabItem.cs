using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class TabItem : MonoBehaviour
{
    [SerializeField] private string id = "";
    [SerializeField] private GameObject pageObject;

    private Toggle toggle;

    public string Id { get { return id; } }
    public GameObject PageObject { get { return pageObject; } }

    protected virtual void OnEnable()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate
        {
            OnValueChanged(toggle);
        });
    }

    protected void OnValueChanged(Toggle toggle)
    {
        //if (PageManager.Instance.CurrentPageId != "main")
        //{
        //    PageManager.Instance.Show("main");
        //}
        pageObject.SetActive(toggle.isOn);
        if (toggle.isOn)
        {
            var tabPage = transform.GetComponentInParent<TabPage>();
            if (tabPage != null)
            {
                tabPage.ChangedTab();
            }
        }
    }
}
