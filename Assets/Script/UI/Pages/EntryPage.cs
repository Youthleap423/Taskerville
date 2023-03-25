using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPage : Page
{
    public LEntry entry = new LEntry();

    [Header("Info")]
    [SerializeField] private GameObject infoPageObj;

    public override void Initialize()
    {
        base.Initialize();
        ShowInfo(false);
    }

    virtual public void OnLoadPage()
    {

    }

    virtual public void OnLoadPage(NavigationPage page)
    {

    }

    virtual public void SetEntry(LEntry entry)
    {

    }

    virtual public void UpdateSubTask(LSubTask task)
    {

    }

    virtual public void DeleteSubTask(LSubTask task)
    {

    }

    virtual public void DeleteLinkTask(LTaskEntry task)
    {

    }

    virtual public void DeleteLinkHabit(LHabitEntry habit)
    {

    }

    virtual public void OnComplete(LEntry entry)
    {

    }

    virtual public void OnComplete(LSubTask task)
    {

    }

    public void ShowInfo(bool bShow)
    {
        infoPageObj.SetActive(bShow);
    }
}
