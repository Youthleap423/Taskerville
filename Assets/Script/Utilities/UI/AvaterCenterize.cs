using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvaterCenterize : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public float ratio = 1.0f;

    private void Awake()
    {
        RectTransform parentTrans = gameObject.GetComponentInParent<RectTransform>();
        float w  = parentTrans.rect.height;
        RectTransform trans = gameObject.GetComponent<RectTransform>();
        trans.sizeDelta = new Vector2(w * ratio, w);
        trans.localPosition = Vector2.zero;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
