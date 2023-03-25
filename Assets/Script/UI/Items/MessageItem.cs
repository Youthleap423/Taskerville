using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageItem : MonoBehaviour
{
    [SerializeField] private Text name_TF;
    [SerializeField] private Text time_TF;
    [SerializeField] private Text message_TF;
    [SerializeField] private Image avatar;

    public void SetData(LUser sender, FMessage fMessage)
    {
        message_TF.text = fMessage.content;
        name_TF.text = sender.GetFullName();
        System.DateTime dateTime = Utilities.dateTimeFromTicksString(fMessage.created_at);
        time_TF.text = Convert.DateTimeToMessageTime(dateTime);
        avatar.sprite = DataManager.Instance.GetAvatarSprite(sender.AvatarId);

        float left_item_width = ((RectTransform)(avatar.transform.parent)).rect.width;
        float total_width = ((RectTransform)(avatar.transform.parent.parent)).rect.width;
        ((RectTransform)(name_TF.transform.parent.parent)).sizeDelta = new Vector2(total_width - left_item_width, 0);
        
    }
}
