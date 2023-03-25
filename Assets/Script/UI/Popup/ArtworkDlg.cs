using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
public class ArtworkDlg :PopUpDlg
{
    [Space]
    [SerializeField] private Text titleTF;
    [SerializeField] private ImageOutline image;
    [SerializeField] private Text nameTF;
    [SerializeField] private Text dateTF;

    [SerializeField] GameObject DescObj;
    [SerializeField] GameObject DetailObj;

    private LArtwork artwork = null;

    private void ShowDetailView()
    {
        this.artwork = ArtworkSystem.Instance.GetAllArtworks().Last();
        if (artwork == null)
        {
            OnDetailClose();
        }

        DescObj.SetActive(false);
        DetailObj.SetActive(true);
        
        var item = ArtworkSystem.Instance.GetCArtwork(artwork);
        StartCoroutine(ImageLoader.Start(item.image_path, (sprite =>
        {
            image.sprite = sprite;
        })));

        var reason = (EArtworkReason)Enum.Parse(typeof(EArtworkReason), artwork.reason);
        var prefix = "You've been awarded a work of art for ";
        var suffix = "";
        switch (reason)
        {
            case EArtworkReason.Happy85:
                suffix = "raising your village happiness to 85%";
                break;
            case EArtworkReason.Happy90:
                suffix = "raising your village happiness to 90%";
                break;
            case EArtworkReason.Happy95:
                suffix = "raising your village happiness to 95%";
                break;
            case EArtworkReason.Happy100:
                suffix = "raising your village happiness to 100%";
                break;
            case EArtworkReason.Population65:
                suffix = "raising your village population to 65%";
                break;
            case EArtworkReason.Population75:
                suffix = "raising your village population to 75%";
                break;
            case EArtworkReason.Population85:
                suffix = "raising your village population to 85%";
                break;
            case EArtworkReason.Population95:
                suffix = "raising your village population to 95%";
                break;
            case EArtworkReason.Population100:
                suffix = "raising your village population to 100%";
                break;
            case EArtworkReason.Build_Gallery:
                suffix = "constructing an Art Gallery";
                break;
            case EArtworkReason.Build_Museum:
                suffix = "constructing a Museum";
                break;
            case EArtworkReason.Build_Park:
                suffix = "constructing a Park";
                break;
            case EArtworkReason.Build_Religious:
                suffix = "constructing a Spiritual structure";
                break;
            case EArtworkReason.WeeklyTaskComplete:
                suffix = "completing all your Repeat Tasks and Habits for 7 days straight!";
                break;
            case EArtworkReason.Trade:
                suffix = "completing your trade offer";
                break;
            case EArtworkReason.Buy:
                prefix = "You've bought a work of art";
                suffix = "";
                break;
            default:
                prefix = "";
                suffix = "";
                break;
        }
        titleTF.text =  prefix + suffix;
        nameTF.text = item.name;//item.artist_name;
        dateTF.text = item.artist_name;// item.contactInfo;
        

        LayoutRebuilder.ForceRebuildLayoutImmediate(DetailObj.GetComponent<RectTransform>());
        var mainObject = DetailObj.transform.GetChild(0).gameObject;
        var mainObjectH = mainObject.GetComponent<RectTransform>().rect.height;
        var titleObject = mainObject.transform.GetChild(0).gameObject;
        var imageObject = mainObject.transform.GetChild(1).gameObject;
        var titleH = titleObject.GetComponent<Text>().preferredHeight;
        var sprite = imageObject.GetComponent<Image>().sprite;
        var aspectRatio = sprite == null ? 1.0f : (sprite.rect.width == 0 ? 1.0f : sprite.rect.height / sprite.rect.width);
        var imageHeight = imageObject.GetComponent<RectTransform>().rect.width * aspectRatio;
        imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(imageObject.GetComponent<RectTransform>().rect.width, aspectRatio >= 1.0f ? mainObjectH - titleH - 80.0f : imageHeight);
    
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainObject.GetComponent<RectTransform>());
    }

    public override void Show()
    {
        base.Show();

        if (ArtworkSystem.Instance.GetAllArtworks().Count == 1)
        {
            DescObj.SetActive(true);
            DetailObj.SetActive(false);
        }
        else
        {
            ShowDetailView();
        }
        
    }


    public void OnDetailClose()
    {
        Back();
    }

    public void OnDescriptionClose()
    {
        ShowDetailView();
    }

        //protected override void Back()
        //{
        //    PopUpManager.Instance.Back();
        //}
}