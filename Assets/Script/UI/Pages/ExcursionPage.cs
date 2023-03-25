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
        }
        else
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            beginBtn.GetComponent<Image>().color = Color.blue;
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
