using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningPage : Page
{
    [SerializeField] private Transform resourceGroupTransform;
    [SerializeField] private Transform provisionGroupTransform;
    [SerializeField] private GameObject productItemPrefab;
    [SerializeField] private List<EResources> resourceTypes = new List<EResources>();
    private List<LProduct> resource_ProductList = new List<LProduct>();
    private List<LProduct> provisions_ProductList = new List<LProduct>();

    private void OnEnable()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();

        foreach (Transform child in resourceGroupTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in provisionGroupTransform)
        {
            Destroy(child.gameObject);
        }

        resource_ProductList.Clear();
        provisions_ProductList.Clear();

        var allProducts = ResourceViewController.Instance.GetAllProducts();
        foreach (LProduct product in allProducts)
        {
            if (product.type == EResources.Happiness || product.type == EResources.Gold)
            {
                continue;
            }

            if (resourceTypes.Contains(product.type))
            {
                var oldOne = resource_ProductList.Find(item => item.type == product.type);
                if (oldOne == null)
                {
                    resource_ProductList.Add(product);
                }
                else
                {
                    oldOne.amount += product.amount;
                }
            }
            else
            {
                var oldOne = provisions_ProductList.Find(item => item.type == product.type);
                if (oldOne == null)
                {
                    provisions_ProductList.Add(product);
                }
                else
                {
                    oldOne.amount += product.amount;
                }
            }
        }

        foreach (LProduct product in resource_ProductList)
        {
            GameObject clonedObj = Instantiate(productItemPrefab, resourceGroupTransform);
            clonedObj.GetComponent<ProductionItem>().SetProduct(product);
        }

        foreach (LProduct product in provisions_ProductList)
        {
            GameObject clonedObj = Instantiate(productItemPrefab, provisionGroupTransform);
            clonedObj.GetComponent<ProductionItem>().SetProduct(product);
        }
    }
}
