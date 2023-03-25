using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayPage : Page
{
    [SerializeField] private GameObject gameplayObj;
    [SerializeField] private GameObject screenObj;
    [SerializeField] private GameObject prevBtnObj;
    [SerializeField] private GameObject nextBtnObj;

    [Header("Data")]
    [SerializeField] private List<GameObject> infoObjects;
    [SerializeField] private GameObject basicObj;
    private int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    public void ShowInfo(int id)
    {
        currentIndex = id;
        Show();
    }

    public void HideInfo()
    {
        foreach (GameObject obj in infoObjects)
        {
            obj.SetActive(false);
        }
        screenObj.SetActive(false);
    }

    public void Back()
    {
        Hide();
    }

    public void Next()
    {
        currentIndex++;
        Show();
    }

    public void Prev()
    {
        currentIndex--;
        Show();
    }

    private void Show()
    {
        prevBtnObj.SetActive(true);
        nextBtnObj.SetActive(true);

        if (currentIndex == 0)
        {
            prevBtnObj.SetActive(false);
        }

        if (currentIndex == infoObjects.Count - 1)
        {
            nextBtnObj.SetActive(false);
        }
        HideInfo();
        screenObj.SetActive(true);
        infoObjects[currentIndex].SetActive(true);
        gameplayObj.SetActive(true);
    }

    public void ShowBasicTaskerville()
    {
        HideInfo();
        basicObj.transform.parent.gameObject.SetActive(true);
        basicObj.SetActive(true);
        gameplayObj.SetActive(true);
    }

    private void Hide()
    {
        gameplayObj.SetActive(false);
    }

}
