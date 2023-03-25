using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponsiveScreen : MonoBehaviour
{
    [SerializeField] private Vector2 sizeDelta;
    [Header("Landscape")]
    [SerializeField] private Vector2 anchorMin1;
    [SerializeField] private Vector2 anchorMax1;
    [Header("Portrait")]
    [SerializeField] private Vector2 anchorMin2;
    [SerializeField] private Vector2 anchorMax2;

    private ScreenOrientation orientation = ScreenOrientation.AutoRotation;
    private RectTransform rectTrans;
    // Start is called before the first frame update
    void Start()
    {
        rectTrans = GetComponent<RectTransform>();
        ChangeRotation(ScreenOrientation.Portrait);
        orientation = Screen.orientation;
    }

    // Update is called once per frame
    void Update()
    {
        if (orientation != Screen.orientation)
        {
            ChangeRotation(Screen.orientation);
        }
    }

    private void ChangeRotation(ScreenOrientation orientation)
    {
        this.orientation = orientation;
        if (orientation == ScreenOrientation.Landscape || orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight)
        {
            rectTrans.sizeDelta = sizeDelta;
            rectTrans.anchorMin = anchorMin1;//low-left
            rectTrans.anchorMax = anchorMax1;//upper-right
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans);
        }
        else if (orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown)
        {
            rectTrans.sizeDelta = sizeDelta;
            rectTrans.anchorMin = anchorMin2;//low-left
            rectTrans.anchorMax = anchorMax2;//upper-right
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans);
        }
    }
}
