using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarItem : MonoBehaviour
{
    [SerializeField] public Image backgroundImage;
    [SerializeField] public Image avatar_Image;

    public int itemIndex;
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setSprite(Sprite sprite)
    {
        avatar_Image.sprite = sprite;
    }

    public void onSelect()
    {
        backgroundImage.color = Color.red;
    }

    public void onDeSelect()
    {
        backgroundImage.color = Color.black;
    }
}
