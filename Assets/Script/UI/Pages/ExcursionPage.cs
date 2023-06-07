using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExcursionPage : MonoBehaviour
{
    [SerializeField] private GameObject excursionPageObj;
    [SerializeField] private GameObject excursionProgressPageObj;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject beginBtn;
    [SerializeField] private Text txt_remainDays;

    private void OnEnable()
    {
        ReloadUI();
    }

    private void ReloadUI()
    {
        var currentState = ArtifactSystem.Instance.IsReadyForNewArtifact();
        if (currentState == EActionState.Wait)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            beginBtn.GetComponent<Image>().color = Color.gray;
            var days = ArtifactSystem.Instance.GetRemainDaysUntilAvailable();
            if (days > 0)
            {
                txt_remainDays.text = string.Format("(Archaeologist will be available in {0} {1})", days, Utilities.GetPluralWord("day"));
                txt_remainDays.gameObject.SetActive(true);
            }
            else
            {
                txt_remainDays.gameObject.SetActive(false);
            }
        }
        else
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            beginBtn.GetComponent<Image>().color = Color.blue;
            txt_remainDays.gameObject.SetActive(false);
        }

        if (currentState == EActionState.Progess)
        {
            excursionPageObj.SetActive(false);
            excursionProgressPageObj.SetActive(true);
        }
        else
        {
            excursionPageObj.SetActive(true);
            excursionProgressPageObj.SetActive(false);
        }
    }

    public void Begin()
    {
        var currentArtifact = ArtifactSystem.Instance.GetCurrentArtifact();
        if (currentArtifact == null)
        {
            if (ResourceViewController.Instance.CheckResource(ArtifactSystem.Instance.artifactRes))
            {
                ArtifactSystem.Instance.Excavate();
            }
            else
            {
                UIManager.Instance.ShowErrorDlg("Not enough resource to excavate");
            }
        }

        ShowProgressPage();
    }

    public void ShowProgressPage()
    {
        var currentArtifact = ArtifactSystem.Instance.GetCurrentArtifact();
        if (currentArtifact != null)
        {
            excursionPageObj.SetActive(false);
            excursionProgressPageObj.SetActive(true);
        }
    }

    public void OnCompleted()
    {
        ReloadUI();
    }

}
