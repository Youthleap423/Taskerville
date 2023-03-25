using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExcursionProgressPage : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    private LArtifact currentArtifact = null;

    void Start()
    {

    }

    private void OnEnable()
    {
        AppManager.Instance.excavationDigIndex = -1;
        currentArtifact = ArtifactSystem.Instance.GetCurrentArtifact();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentArtifact != null)
        {
            progressBar.value = Mathf.Clamp01(ArtifactSystem.Instance.GetCurrentProgress());
        }
        else
        {
            CompleteExcavate();
        }
    }

    public void ShowDigOnMap()
    {
        if (currentArtifact != null)
        {
            UIManager.Instance.ShowLoadingBar(true);
            AppManager.Instance.excavationDigIndex = currentArtifact.dig;
            AppManager.Instance.LoadGameScene();
            //TODO - show Dig on Map
            //BuildManager.Instance.ShowDigOnMap(currentArtifact.dig);
            //UIManager.Instance.showGameUI();
        }
    }

    private void CompleteExcavate()
    {
        //ArtifactSystem.Instance.CompleteExcavate(currentArtifact);
        transform.parent.GetComponent<ExcursionPage>().OnCompleted();

        //TODO- show popup with excavated artifact
    }

}
