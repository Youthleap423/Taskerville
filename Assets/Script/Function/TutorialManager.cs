using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : SingletonComponent<TutorialManager>
{
    [SerializeField] private GameObject screenObj;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text titleTF;
    [SerializeField] private GameObject prevBtnObj;
    [SerializeField] private GameObject nextBtnObj;

    [Header("Data")]
    [SerializeField] private List<GameObject> infoObjects;

    [Header("Data")]
    [SerializeField] private List<Sprite> taskEntries;
    [SerializeField] private List<Sprite> todoEntries;
    [SerializeField] private List<Sprite> habitEntries;
    [SerializeField] private List<Sprite> projectEntries;
    [SerializeField] private List<Sprite> shelters;
    [SerializeField] private List<Sprite> art;
    [SerializeField] private List<Sprite> trade;
    private List<Sprite> currentSprites = new List<Sprite>();
    private int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    public void ShowInfo(int id)
    {
        foreach(GameObject obj in infoObjects)
        {
            obj.SetActive(false);
        }
        infoObjects[id].SetActive(true);
    }

    public void Show(string tutorId)
    {
        currentSprites.Clear();
        switch (tutorId)
        {
            case "Task":
                titleTF.text = "Repeat Tasks";
                currentSprites.AddRange(taskEntries);
                break;
            case "Todo":
                titleTF.text = "To-Dos / Checklists";
                currentSprites.AddRange(todoEntries);
                break;
            case "Habit":
                titleTF.text = "Habits";
                currentSprites.AddRange(habitEntries);
                break;
            case "Project":
                titleTF.text = "Goals";
                currentSprites.AddRange(projectEntries);
                break;
            case "Shelter":
                titleTF.text = "S";
                currentSprites.AddRange(shelters);
                break;
            case "Art":
                titleTF.text = "Art & Artifacts";
                currentSprites.AddRange(art);
                break;
            case "Trade":
                titleTF.text = "Coalition Trading";
                currentSprites.AddRange(trade);
                break;
            default:
                break;
        }

        if (currentSprites.Count == 0)
        {
            Hide();
        }
        else
        {
            currentIndex = 0;
            Show();
        }
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

        if (currentIndex == currentSprites.Count - 1)
        {
            nextBtnObj.SetActive(false);
        }

        image.sprite = currentSprites[currentIndex];
        screenObj.SetActive(true);
    }

    private void Hide()
    {
        screenObj.SetActive(false);
    }


}
