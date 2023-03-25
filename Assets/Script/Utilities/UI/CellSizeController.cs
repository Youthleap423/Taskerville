using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellSizeController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GridLayoutGroup gridGroup;
    [SerializeField] private int column_count = 1;

    void Start()
    {
        RectTransform rectTransfrom = gameObject.GetComponent<RectTransform>();
        float cellWidth = (rectTransfrom.rect.size.x - gridGroup.spacing.x)/ column_count;
        gridGroup.cellSize = new Vector2(cellWidth, cellWidth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
