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
    [SerializeField] private List<CTutor> taskEntries;
    [SerializeField] private List<CTutor> todoEntries;
    [SerializeField] private List<CTutor> habitEntries;
    [SerializeField] private List<CTutor> projectEntries;
    [SerializeField] private List<CTutor> shelters;
    [SerializeField] private List<CTutor> art;
    [SerializeField] private List<CTutor> trade;
    private List<CTutor> currentTutors = new List<CTutor>();
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
        currentTutors.Clear();
        switch (tutorId)
        {
            case "Task":
                titleTF.text = "Repeat Tasks";
                currentTutors.AddRange(taskEntries);
                break;
            case "Todo":
                titleTF.text = "To-Dos / Checklists";
                currentTutors.AddRange(todoEntries);
                break;
            case "Habit":
                titleTF.text = "Habits";
                currentTutors.AddRange(habitEntries);
                break;
            case "Project":
                titleTF.text = "Goals";
                currentTutors.AddRange(projectEntries);
                break;
            case "Shelter":
                titleTF.text = "S";
                currentTutors.AddRange(shelters);
                break;
            case "Art":
                titleTF.text = "Art & Artifacts";
                currentTutors.AddRange(art);
                break;
            case "Trade":
                titleTF.text = "Coalition Trading";
                currentTutors.AddRange(trade);
                break;
            default:
                break;
        }

        if (currentTutors.Count == 0)
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

        if (currentIndex == currentTutors.Count - 1)
        {
            nextBtnObj.SetActive(false);
        }
        
        image.sprite = currentTutors[currentIndex].sprite;
        titleTF.text = currentTutors[currentIndex].title;
        screenObj.SetActive(true);
    }

    private void Hide()
    {
        screenObj.SetActive(false);
    }


}
