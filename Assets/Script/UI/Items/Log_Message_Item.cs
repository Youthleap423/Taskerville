using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Log_Message_Item : MonoBehaviour
{
    [SerializeField] private float message_display_time;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text prefixMsg_TF;
    [SerializeField] private TMP_Text suffixMsg_TF;
    [SerializeField] private Image stateIcon;
    [SerializeField] private Image resourceIcon;

    [Space]
    [SerializeField] Sprite grow_up;
    [SerializeField] Sprite grow_down;

    private float message_delay_time = 0.0f;
    // Start is called before the first frame update
    void Start()
    {

    }


    IEnumerator FadeIn(float delayTime, float fadeTime)
    {
        yield return new WaitForSeconds(delayTime);

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    IEnumerator FadeOut(float delayTime, float fadeTime)
    {
        yield return new WaitForSeconds(delayTime);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime; ;
            yield return null;
        }
    }

    private void ShowMessage()
    {
        StartCoroutine(FadeIn(message_delay_time, message_display_time / 3.0f));
        StartCoroutine(FadeOut(message_delay_time + message_display_time * 2.0f / 3.0f, message_display_time / 3.0f));
        Destroy(gameObject, message_delay_time + message_display_time);
    }

    public void setDelayTime(int nCount)
    {
        message_delay_time = nCount > 0 ? (nCount - 1) * message_display_time : 0.0f;
        if (resourceIcon.sprite != null && resourceIcon.sprite == DataManager.Instance.sapphire_Sprite)
        {
            AudioManager.Instance.PlayFXSound(AudioManager.Instance.gemClip, true, message_delay_time);
        }
        ShowMessage();
    }



    public void SetMessage(string prefixStr, string suffixStr, Sprite sprite, float amount)
    {
        if (sprite == null)
        {
            resourceIcon.gameObject.SetActive(false);
        }
        else
        {
            resourceIcon.gameObject.SetActive(true);
            resourceIcon.sprite = sprite;
        }

        if (amount > 0)
        {
            stateIcon.gameObject.SetActive(true);
            stateIcon.sprite = grow_up;
            //stateIcon.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 270));
            //stateIcon.color = Color.green;
        }
        else if (amount < 0)
        {
            stateIcon.gameObject.SetActive(true);
            stateIcon.sprite = grow_down;
            //stateIcon.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
            //stateIcon.color = Color.red;
        }else if (amount == 0)
        {
            stateIcon.gameObject.SetActive(false);
        }

        //prefixMsg_TF.text = prefixStr;
        //suffixMsg_TF.text = string.Format(" {0}", Mathf.Abs(amount));

        //if (amount == 0)
        //{
        //    suffixMsg_TF.gameObject.SetActive(false);
        //}
        //else
        //{
        //    suffixMsg_TF.gameObject.SetActive(true);
        //}
    }
}
