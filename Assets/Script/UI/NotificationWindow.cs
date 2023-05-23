using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationWindow : MonoBehaviour
{
    [SerializeField]
    Text title;

    [SerializeField]
    Text subTitle;

    [SerializeField]
    Text description;

    [SerializeField]
    Animator animator;

    [SerializeField]
    float showTime = 1.3f;

    float disableTime = 1.0f;
    List<Canvas> tempCanvasList = new List<Canvas>();
    IEnumerator transitionCoroutine;
    IEnumerator diableCoroutine;
    bool bDelayedUpdate = false;
    float elapsedTime = 0f;

    Vector3? initAnchoredPosition;
    Vector3 InitPosition
    {
        get
        {
            if (initAnchoredPosition == null)
            {
                initAnchoredPosition = GetComponent<RectTransform>().anchoredPosition3D;
            }
            return initAnchoredPosition ?? Vector3.zero;
        }
    }

    public float ShowTime
    {
        get => showTime;
        set
        {
            showTime = value;
        }
    }

    public void ShowNotification()
    {
        gameObject.SetActive(true);
        if (animator != null)
        {
            animator.enabled = false;
            animator.gameObject.transform.localScale = Vector3.one;
            animator.gameObject.transform.localEulerAngles = Vector3.zero;
        }
        UpdatePosition();
        PlayAnimation(true);
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }
        transitionCoroutine = UpdateTransition();
        StartCoroutine(transitionCoroutine);
    }

    public void HideNotification()
    {
        if (diableCoroutine != null)
        {
            StopCoroutine(diableCoroutine);
            diableCoroutine = null;
        }
        diableCoroutine = DisableTransition();
        StartCoroutine(diableCoroutine);
    }

    public void SetData(string title, string subtitle, string description)
    {
        this.title.text = title;
        this.subTitle.text = subtitle;
        this.description.text = description;
    }

    void Update()
    {
        if (bDelayedUpdate)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 0.3)
            {
                bDelayedUpdate = false;
            }
        }
    }

    void UpdatePosition()
    {
        tempCanvasList.Clear();
        GetComponentsInParent(false, tempCanvasList);
        if (tempCanvasList.Count == 0)
        {
            return;
        }
        Canvas rootCanvas = tempCanvasList[tempCanvasList.Count - 1];
        for (int i = 0; i < tempCanvasList.Count; i++)
        {
            if (tempCanvasList[i].isRootCanvas)
            {
                rootCanvas = tempCanvasList[i];
                break;
            }
        }
        tempCanvasList.Clear();
        RectTransform rectTrans = GetComponent<RectTransform>();
        rectTrans.anchoredPosition3D = InitPosition;

        Vector3[] corners = new Vector3[4];
        rectTrans.GetWorldCorners(corners);
        RectTransform rootCanvasRect = rootCanvas.transform as RectTransform;
        Vector3 corner = rootCanvasRect.InverseTransformPoint(corners[0]);
        float diff = rootCanvasRect.rect.yMin - corner.y;
        if (diff > 0)
        {
            Vector3 pos = rectTrans.anchoredPosition3D;
            pos.y = pos.y + diff;
            rectTrans.anchoredPosition3D = pos;
        }
    }

    void PlayAnimation(bool bStart)
    {
        if (animator != null)
        {
            if (animator.enabled == false)
            {
                animator.enabled = true;
            }
            if (bStart)
            {
                animator.Play("In", 0, 0);
            }
            else
            {
                animator.Play("Out", 0, 0);
            }
        }
    }

    IEnumerator UpdateTransition()
    {
        yield return new WaitForSeconds(showTime);
        PlayAnimation(false);
        HideNotification();
    }

    IEnumerator DisableTransition()
    {
        yield return new WaitForSeconds(disableTime);
        gameObject.SetActive(false);
    }

}

