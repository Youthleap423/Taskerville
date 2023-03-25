using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIControllersAndData.Store;
public class ConstructingItem : MonoBehaviour
{
    [SerializeField] private Text builderTF;
    [SerializeField] private Text buildingNameTF;
    [SerializeField] private Slider progressBar;

    private GameObject buildingObject = null;
    private ConstructionPage parentPage = null;
    //private StructureSelector selector = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    private void FixedUpdate()
    {
        if (buildingObject != null)
        {
            ConstructionSelector constructionSelector = buildingObject.GetComponent<ConstructionSelector>();
            if (constructionSelector != null)
            {
                progressBar.value = 1.0f - (float)(constructionSelector.remainingTime) / (float)(constructionSelector.buildingTime);
            }
        }
    }

    public void SetBuildingObject(GameObject obj, StructureSelector selector,ConstructionPage page)
    {
        parentPage = page;
        this.selector = selector;
        if (obj == null)
        {
            return;
        }
        buildingObject = obj;
        ConstructionSelector cSel = buildingObject.GetComponent<ConstructionSelector>();
        if (cSel != null)
        {
            var structure = ShopData.Instance.GetCategoryData(cSel.Id, cSel.CategoryType);
            buildingNameTF.text = structure.GetName();
            builderTF.text = "Builder";
            progressBar.value = 1.0f - (float)(cSel.remainingTime) / (float)(cSel.buildingTime);
        }
        //builderTF.text = 
    }

    public void CancelConstruction()
    {
        if (parentPage != null)
        {
            parentPage.CancelConstruction(this.buildingObject, this.selector);
        }
    }

    public void QuickBuildTapped()
    {
        if (buildingObject != null)
        {
            ConstructionSelector constructionSelector = buildingObject.GetComponent<ConstructionSelector>();
            if (constructionSelector != null)
            {
                progressBar.value = 1.0f;
                constructionSelector.SetProgress(1.0f);
                constructionSelector.OnComplete();
            }
        }
    }
    */
}
