using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class RewardSystem : SingletonComponent<RewardSystem>
{
    #region Unity Members
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Public Members
    public void OnFiveTaskComplete()
    {
        Dictionary<EResources, float> dic = new Dictionary<EResources, float>();
        dic.Add(EResources.Lumber, 3);
        dic.Add(EResources.Corn, 5);
        dic.Add(EResources.Iron, 3);
        dic.Add(EResources.Apples, 1);
        dic.Add(EResources.Bread, 5);
        dic.Add(EResources.Stone, 5);
        dic.Add(EResources.Eggs, 10);

        System.Random rnd = new System.Random();

        var selectedDic = dic.ElementAt(rnd.Next(dic.Keys.Count));

        //GivesReward(selectedDic.Key, selectedDic.Value);
        
        //TODO - show gift select window 
        //Can trigger one of the following
        //3lumber, 5 stalks of corn, 3 Iron, 1 basket of fruit(apples, oranges, grapes), 5 loaves of bread, 5 stone, 10eggs

    }

    public void OnWeekTaskComplete()
    {
        Dictionary<EResources, float> dic = new Dictionary<EResources, float>();
        dic.Add(EResources.Wine, 1);
        dic.Add(EResources.Pears, 1);
        dic.Add(EResources.Stone, 5);
        dic.Add(EResources.Iron, 3);
        dic.Add(EResources.Lumber, 3);
        
        //GivesReward(dic);

        //UIManager.Instance.ShowRewardMessage("You've completed this task in a week");
        //TODO - show gift select window 
        //Can trigger all of the following
        //1 bottle of wine, 1 basket of fruit(apples, oranges, grapes), 5 stone, 3 Iron, 3lumber
    }

    public void OnThreeToDoComplete()
    {
        Dictionary<EResources, float> dic = new Dictionary<EResources, float>();
        dic.Add(EResources.Lumber, 3);
        dic.Add(EResources.Corn, 5);
        dic.Add(EResources.Iron, 3);
        dic.Add(EResources.Melons, 1);
        dic.Add(EResources.Bread, 5);
        dic.Add(EResources.Stone, 5);
        dic.Add(EResources.Eggs, 10);

        System.Random rnd = new System.Random();

        var selectedDic = dic.ElementAt(rnd.Next(dic.Keys.Count));

        //GivesReward(selectedDic.Key, selectedDic.Value);
        //TODO - show gift select window 
        //Can trigger one of the following
        //3lumber, 5 stalks of corn, 3 Iron, 1 basket of fruit(apples, oranges, grapes), 5 loaves of bread, 5 stone, 10eggs
    }

    public void OnProjectComplete()
    {
        UIManager.Instance.ShowRewardMessage("You've got", "a set of reward", DataManager.Instance.set_Sprite, 1);
        //TODO - show gift select window 
        //Can trigger all of the following
        //3 bottle of wine, 5% of happiness points village wide, 5 stalks of corn, 1 basket of fruit(apples, oranges, grapes), 5 loaves of bread, 5 stone, 3 Iron, 3lumber, 10 eggs
    }

    public void OnComplete(LTask entry)
    {
        //TODO - gives count of golds
        GivesReward(EResources.Gold, entry.goldCount);
    }

    public void OnComplete(float goldCount)
    {
        //TODO - gives count of golds
        GivesReward(EResources.Gold, goldCount);
    }

    public void OnComplete(LProjectEntry entry)
    {
        if (Convert.FDateToDateTime(entry.completedDate).CompareTo(Convert.EntryDateToDateTime(entry.endDate)) <= 0)
        {
            //Dictionary<EResources, float> dic = new Dictionary<EResources, float>();
            //dic.Add(EResources.Gold, entry.goldCount);
            //dic.Add(EResources.Happiness, 2.0f);
            //dic.Add(EResources.Wine, 5.0f);
            //GivesReward(dic);
            GivesReward(EResources.Gold, entry.goldCount);
            GivesReward(EResources.Happiness, 2.0f);
            GivesReward(EResources.Wine, 5.0f);
        }
        else
        {
            //drop 5% of happiness
            GivesReward(EResources.Happiness, -5f);
        }
    }

    public void OnComplete(LTaskEntry entry)
    {
        GivesReward(EResources.Gold, entry.goldCount);
    }

    public void OnComplete(LToDoEntry entry)
    {
        //Dictionary<EResources, float> dic = new Dictionary<EResources, float>();
        //dic.Add(EResources.Gold, entry.goldCount);
        //dic.Add(EResources.Happiness, 0.1f);
        GivesReward(EResources.Gold, entry.goldCount);
        GivesReward(EResources.Happiness, 0.25f);
    }

    public void OnComplete(LAutoGoal entry)
    {
        //TODO - gives count of golds
        GivesReward(EResources.Gold, entry.goldCount);
        if (entry.happiness > 0f)
        {
            GivesReward(EResources.Gold, entry.happiness);
        }
    }

    public void OnCompleteTaskEntryWithoutSkip(LTaskEntry entry)
    {
        //TODO - gives 15 golds - document 7/6/2022 
        GivesReward(EResources.Gold, 15);
    }

    public void OnCompleteHabitEntryWithoutSkip(LHabitEntry entry)
    {
        //TODO - gives 15 golds - document 1/25/2023 
        GivesReward(EResources.Gold, 15);
    }

    public void OnCompleteAllTasksInAWeek()
    {
        GivesReward(EResources.Happiness, 0.5f);
    }

    public void OnAllDailyTaskComplete()
    {
        //GivesReward(EResources.Gold, 15f);
        GivesReward(EResources.Sapphire, 1f);
        GivesReward(EResources.Happiness, 1f);
    }

    public void CancelComplete(LTask entry)
    {
        //TODO - gives count of golds
        var setting = UserViewController.Instance.GetCurrentSetting();
        if (!setting.shelter_storm)
        {
            GivesReward(EResources.Gold, -entry.goldCount);
        }
        
    }

    public void OnFailed(LTask entry)
    {
        //TODO - gives count of golds
        var setting = UserViewController.Instance.GetCurrentSetting();
        if (!setting.shelter_storm)
        {
            GivesReward(EResources.Happiness, -0.5f);
        }
    }

    public void OnFailed(LHabitEntry entry)
    {
        //TODO - gives count of golds
        if (entry.isPositive)
        {
            return;//12/05/2022
        }

        var setting = UserViewController.Instance.GetCurrentSetting();
        if (!setting.shelter_storm)
        {
            GivesReward(EResources.Gold, -10.0f);
            GivesReward(EResources.Happiness, -2.0f);
        }
    }

    public void OnFailed(LAutoGoal entry)
    {
        //TODO - gives count of golds
        var setting = UserViewController.Instance.GetCurrentSetting();
        if (!setting.shelter_storm)
        {
            GivesReward(EResources.Happiness, -3f);
        }
    }

    public void OnBuiltComplete()
    {
        Dictionary<EResources, float> dic = new Dictionary<EResources, float>();
        dic.Add(EResources.Paint, 1);
        //dic.Add(EResources.Culture, 3);
        dic.Add(EResources.Happiness, 1);

        GivesReward(dic);
        //GivesReward(selectedDic.Key, selectedDic.Value);
        //TODO - show gift select window 
        //Can trigger one of the following
        //3lumber, 5 stalks of corn, 3 Iron, 1 basket of fruit(apples, oranges, grapes), 5 loaves of bread, 5 stone, 10eggs
    }

    public void GiveHappinessReward(float amount)
    {
        GivesReward(EResources.Gold, amount);
        DataManager.Instance.ResetDates(System.DateTime.Now);
    }

    public void OnCompleteNegativeHabit()
    {
        GivesReward(EResources.Gold, -30f);
        GivesReward(EResources.Happiness, -2f);
    }

    public void OnCompleteWith(EResources type, float goldAmount)
    {
        //+2 for the negative habits
        GivesReward(type, goldAmount + 2f);
    }

    public void GivesTradeReward(EResources res)
    {
        var happiness = 0f;
        switch (res)
        {
            case EResources.Cherries:
            case EResources.Raspberries:
            case EResources.Grapes:
            case EResources.Peaches:
            case EResources.Ale:
                happiness = 1.0f;
                break;
            case EResources.Wine:
                happiness = 2f;
                break;
            default:
                break;
        }
        if (happiness > 0f)
        {
            GivesReward(EResources.Happiness, 0.5f);
        }
    }

    public void GivesProduceReward(EResources res)
    {
        var happiness = 0f;
        switch (res)
        {
            case EResources.Cherries:
            case EResources.Raspberries:
            case EResources.Grapes:
            case EResources.Peaches:
            case EResources.Ale:
                happiness = 0.5f;
                break;
            case EResources.Wine:
                happiness = 1f;
                break;
            default:
                break;
        }
        if (happiness > 0f)
        {
            GivesReward(EResources.Happiness, 0.5f);
        }
    }

    public void GivesProduceReward(List<EResources> resList)
    {
        foreach(EResources res in resList)
        {
            GivesProduceReward(res);
        }
    }

    public void GivesBuyReward(EResources res)
    {
        var happiness = 0f;
        switch (res)
        {
            case EResources.Wine:
                happiness = 1f;
                break;
            case EResources.Jewelry:
                happiness = 3f;
                break;
            case EResources.Fine_Clothes:
                happiness = 2.5f;
                break;
            case EResources.Clothes:
            case EResources.Spices:
                happiness = 2f;
                break;
            case EResources.Ale:
            case EResources.Garlic:
            case EResources.Onion:
                happiness = 0.5f;
                break;
            default:
                break;
        }
        if (happiness > 0f)
        {
            GivesReward(EResources.Happiness, 0.5f);
        }
    }

    public void GivesSellReward(EResources res)
    {
        var happiness = 0f;
        switch (res)
        {
            case EResources.Goat:
            case EResources.Swine:
                happiness = 0.5f;
                break;
            default:
                break;
        }
        if (happiness > 0f)
        {
            GivesReward(EResources.Happiness, 0.5f);
        }
    }
    #endregion

    #region Private Members
    private void GivesReward(EResources resource, float amount)
    {
        if (DataManager.Instance.GetCurrentSetting().shelter_storm)
        {
            UIManager.Instance.ShowErrorDlg("Take shelter from the storm.  All tasks and habits are frozen.");
        }
        else
        {
            ResourceViewController.Instance.UpdateResource(resource.ToString(), amount, (isSuccess, errMsg) =>
            {
                if (isSuccess)
                {
                    CheckHappiness();
                    if (amount > 0)
                    {
                        UIManager.Instance.ShowRewardMessage("You've got ", resource.ToString().ToLower(), DataManager.Instance.GetSprite(resource), amount);
                    }
                    else if (amount < 0)
                    {
                        UIManager.Instance.ShowRewardMessage("You've lost ", resource.ToString().ToLower(), DataManager.Instance.GetSprite(resource), amount);
                    }
                }
            });
        }
    }

    private void GivesReward(Dictionary<EResources, float> dic)
    {
        if (DataManager.Instance.GetCurrentSetting().shelter_storm)
        {
            UIManager.Instance.ShowErrorDlg("Take shelter from the storm.  All tasks and habits are frozen.");
        }
        else
        {
            ResourceViewController.Instance.UpdateResource(dic, (isSuccess, errMsg) =>
            {
                if (isSuccess)
                {
                    CheckHappiness();
                    UIManager.Instance.ShowRewardMessage("You've got",  "a set of reward", DataManager.Instance.set_Sprite, 1);
                }
            });
        }
    }

    private void CheckHappiness()
    {
        if (ResourceViewController.Instance.GetCurrentResourceValue(EResources.Happiness) < 50.0f)
        {
            AudioManager.Instance.PlayFXSound(AudioManager.Instance.angryClip);
            UIManager.Instance.ShowAdministratorPrompt("Your villagers are quite unhappy, Mayor. A group of them have gathered in the village square and are burning you in effigy. You may throw a festival in the village square for $50 gold to raise morale and happiness (happiness will increase 10%)", (result) =>
            {
                if (result == true)
                {
                    if (ResourceViewController.Instance.GetCurrentResourceValue(EResources.Gold) >= 50)
                    {
                        Dictionary<EResources, float> dic = new Dictionary<EResources, float>();
                        dic.Add(EResources.Gold, -50);
                        dic.Add(EResources.Happiness, 10.0f);
                        DataManager.Instance.BuyResource(dic, (isSuccess, errMsg) =>
                        {
                            UIManager.Instance.UpdateTopProfile();
                            //TODO After happiness raised 15% up.
                        });
                    }
                    else
                    {
                        UIManager.Instance.ShowErrorDlg("Not Enough Gold.");
                    }
                        
                }
            });
        }
    }

    private void GiftsForAToDo(FToDoEntry entry)
    {
        //give golds according to difficulty
        entry.goldCount = entry.diffculty + 2;
    }

    private void GiftsForASubTask(FTask task)
    {
        //give golds according to difficulty
        
    }

    #endregion
}
