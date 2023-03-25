using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LUser: LData
{
    public string UserName = "";
    public string First_Name = "";
    public string Last_Name = "";
    public string Email = "";
    public int AvatarId = 0;
    public string Village_Name = "";
    public bool isVegetarian = false;
    public bool hasReligion = false;
    public string Religion_Name = "";
    public float sales_amount = 0f;
    public float buy_amoumt = 0f;
    public Dictionary<string, string> dates = new Dictionary<string, string>();
    public int completed_Tasks = 0;
    public int completed_ToDos = 0;
    public string created_coalition = "";
    public string joined_coalition = "";
    public int population = 46;
    public string mode_at = "";
    public bool fJC = false;//first Joined Coalition
    public bool fBU = false;//first Build Unique Building
    public Dictionary<string, float> exports = new Dictionary<string, float>();
    public LUser()
    {
        UserName = "";
        First_Name = "";
        Last_Name = "";
        Email = "";
        AvatarId = 0;
        Village_Name = "";
        isVegetarian = false;
        hasReligion = false;
        Religion_Name = "";
        sales_amount = 0f;
        buy_amoumt = 0f;
        dates = new Dictionary<string, string>();
        exports = new Dictionary<string, float>();
        completed_Tasks = 0;
        completed_ToDos = 0;
        created_coalition = "";
        joined_coalition = "";
        population = 46;
        fJC = false;
        fBU = false;
        mode_at = Convert.DateTimeToFDate(System.DateTime.Now);
    }

    public LUser(string id)
    {
        this.id = id;
        created_at = Convert.DateTimeToFDate(System.DateTime.Now);
        dates = new Dictionary<string, string>();
        exports = new Dictionary<string, float>();
    }

    public LUser(string id, string UserName, string First_Name, string Last_Name, string Email, int AvatarId, string VillageName, bool isVegetarian, bool hasReligion, string Religion_Name)
    {
        this.id = id;
        this.UserName = UserName;
        this.First_Name = First_Name;
        this.Last_Name = Last_Name;
        this.Email = Email;
        this.AvatarId = AvatarId;
        this.Village_Name = VillageName;
        this.isVegetarian = isVegetarian;
        this.hasReligion = hasReligion;
        this.Religion_Name = Religion_Name;
        sales_amount = 0;
        buy_amoumt = 0;
        completed_Tasks = 0;
        completed_ToDos = 0;
        created_coalition = "";
        joined_coalition = "";
        created_at = Convert.DateTimeToFDate(System.DateTime.Now);
        dates = new Dictionary<string, string>();
        exports = new Dictionary<string, float>();
        mode_at = Convert.DateTimeToFDate(System.DateTime.Now);
    }

    public void Save()
    {
        DBHandler.SaveToJSON(this, DataManager.Instance.userFN);
        this.Serialize();
    }

    public string GetFullName()
    {
        return string.Format("{0} {1}", First_Name, Last_Name);
    }

    public string GetDailyTaskDate()
    {
        if (dates.ContainsKey(EDates.DailyTask.ToString()))
        {
            return dates[EDates.DailyTask.ToString()];
        }

        return "";
    }

    public string GetSalaryDate()
    {
        if (dates.ContainsKey(EDates.Salary.ToString()))
        {
            return dates[EDates.Salary.ToString()];
        }

        return created_at;
    }

    public string GetMaintenanceDate()
    {
        if (dates.ContainsKey(EDates.Maintenance.ToString()))
        {
            return dates[EDates.Maintenance.ToString()];
        }

        return created_at;
    }

    public string GetMealDate()
    {
        if (dates.ContainsKey(EDates.Meal.ToString()))
        {
            return dates[EDates.Meal.ToString()];
        }

        return created_at;
    }

    public string GetDate(EDates date)
    {
        if (dates.ContainsKey(date.ToString()))
        {
            return dates[date.ToString()];
        }

        return created_at;
    }

    public void AddExport(EResources res, float amount)
    {
        if (exports.ContainsKey(res.ToString()))
        {
            exports[res.ToString()] += amount;
        }
        else
        {
            exports.Add(res.ToString(), amount);
        }
        Save();
    }

    public float GetExport(EResources res)
    {
        if (exports.ContainsKey(res.ToString()))
        {
            return exports[res.ToString()];
        }

        return 0f;
    }

    public void ClearExport(EResources res)
    {
        if (exports.ContainsKey(res.ToString()))
        {
            exports[res.ToString()] = 0f;
        }
        Save();
    }

    public void ClearAllExports()
    {
        exports.Clear();
        Save();
    }

    public float GetBuy(System.DateTime dateTime)
    {
        var result = 0.0f;
        if (dates.ContainsKey(EDates.Buy.ToString()))
        {
            if (dates[EDates.Buy.ToString()] == Convert.DateTimeToFDate(dateTime))
            {
                result = buy_amoumt;
            }
        }

        return result;
    }

    public float GetSales(System.DateTime dateTime)
    {
        var result = 0.0f;
        if (dates.ContainsKey(EDates.Sale.ToString()))
        {
            if (dates[EDates.Sale.ToString()] == Convert.DateTimeToFDate(dateTime))
            {
                result = sales_amount;
            }
        }

        return result;
    }

    public int GetAgesAsDays()
    {
        return (int)(System.DateTime.Now - Convert.FDateToDateTime(mode_at)).TotalDays;
    }

    public bool hasGotAssist()
    {
        var dateStr = GetDate(EDates.Assist);
        if (dateStr == "")
        {
            return false;
        }
        return dateStr.CompareTo(Convert.DateTimeToFDate(System.DateTime.Now)) >= 0;
    }

    public void Update(string userid, string username, string email)
    {
        id = userid;
        UserName = username;
        Email = email;
        Save();
    }

    public void UpdateModeDate(System.DateTime dateTime)
    {
        mode_at = Convert.DateTimeToFDate(dateTime);
        Save();
    }

    public void Update(string firstName, string secondName, string villageName, bool isVegetarian, bool hasReligion)
    {
        this.First_Name = firstName;
        this.Last_Name = secondName;
        this.Village_Name = villageName;
        this.isVegetarian = isVegetarian;
        this.hasReligion = hasReligion;
        Save();
    }

    public void Update(string created_coalition, string joined_coalition, System.Action<bool, string> callback)
    {
        this.created_coalition = created_coalition;
        this.joined_coalition = joined_coalition;
        Save();
        callback(true, "");
    }

    public void UpdateJoinCoalition(string coalition)
    {
        this.joined_coalition = coalition;
        Save();
    }

    public void UpdateReligion(string religion)
    {
        this.Religion_Name = religion;
        Save();
    }
    
    public void UpdateAvatarId(int nId)
    {
        this.AvatarId = nId;
        Save();
    }

    public void UpdateResource()
    {
        Save();
    }

    public void UpdatePopulation(int value)
    {
        population = value;
        Save();
    }

    public void updateDailyTaskDate(System.DateTime dateTime)
    {
        updateDates(EDates.DailyTask.ToString(), Convert.DateTimeToFDate(dateTime));
        Save();
    }

    public void updateWeeklyTaskDate(System.DateTime dateTime)
    {
        updateDates(EDates.WeeklyTaskHabit.ToString(), Convert.DateTimeToFDate(dateTime));
        Save();
    }

    public void updateSalaryDate(System.DateTime dateTime)
    {
        updateDates(EDates.Salary.ToString(), Convert.DateTimeToFDate(dateTime));
        Save();
    }

    public void updateMaintenanceDate(System.DateTime dateTime)
    {
        updateDates(EDates.Maintenance.ToString(), Convert.DateTimeToFDate(dateTime));
        Save();
    }

    public void updateMealDate(System.DateTime dateTime)
    {
        updateDates(EDates.Meal.ToString(), Convert.DateTimeToFDate(dateTime));
        Save();
    }

    public void updateSales(System.DateTime dateTime, float amount)
    {
        var bToday = false;
        if (dates.ContainsKey(EDates.Sale.ToString()))
        {
            if (dates[EDates.Sale.ToString()] == Convert.DateTimeToFDate(dateTime))
            {
                bToday = true;
            }
            else
            {
                dates[EDates.Sale.ToString()] = Convert.DateTimeToFDate(dateTime);
            }
        }
        else
        {
            dates.Add(EDates.Sale.ToString(), Convert.DateTimeToFDate(dateTime));
        }

        if (bToday)
        {
            sales_amount += amount;
        }
        else
        {
            sales_amount = amount;
        }

        Save();
    }

    public void updateBuy(System.DateTime dateTime, float amount)
    {
        var bToday = false;
        if (dates.ContainsKey(EDates.Buy.ToString()))
        {
            if (dates[EDates.Buy.ToString()] == Convert.DateTimeToFDate(dateTime))
            {
                bToday = true;
            }
            else
            {
                dates[EDates.Buy.ToString()] = Convert.DateTimeToFDate(dateTime);
            }
        }
        else
        {
            dates.Add(EDates.Buy.ToString(), Convert.DateTimeToFDate(dateTime));
        }

        if (bToday)
        {
            buy_amoumt += amount;
        }
        else
        {
            buy_amoumt = amount;
        }

        Save();
    }

    public void updateHappiness(float curHappyValue, float amount)
    {
        switch (curHappyValue)
        {
            case float f when (f < 70.0f):
                updateDates(EDates.Happy70Below.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                updateDates(EDates.Happy7075.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                updateDates(EDates.Happy7580.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                updateDates(EDates.Happy8085.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                updateDates(EDates.Happy8590.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                updateDates(EDates.Happy9095.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                updateDates(EDates.Happy95Above.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                break;
            case float f when (f >= 70.0 && f < 75.0):
                if (f - amount < 70.0)
                {
                    updateDates(EDates.Happy70Below.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy7075.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                }
                else if (f - amount >= 75.0)
                {
                    updateDates(EDates.Happy7580.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy8085.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy8590.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy9095.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy95Above.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                }
                break;
            case float f when (f >= 75.0 && f < 80.0):
                if (f - amount < 75.0)
                {
                    updateDates(EDates.Happy70Below.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy7075.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy7580.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                }
                else if (f - amount >= 80.0)
                {
                    updateDates(EDates.Happy8085.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy8590.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy9095.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy95Above.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                }

                break;
            case float f when (f >= 80.0 && f < 85.0):
                if (f - amount < 80.0)
                {
                    updateDates(EDates.Happy70Below.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy7075.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy7580.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy8085.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                }
                else if (f - amount >= 85.0)
                {
                    updateDates(EDates.Happy8590.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy9095.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy95Above.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                }

                break;
            case float f when (f >= 85.0 && f < 90.0):
                if (f - amount < 85.0)
                {
                    updateDates(EDates.Happy70Below.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy7075.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy7580.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy8085.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy8590.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                }
                else if (f - amount >= 90)
                {
                    updateDates(EDates.Happy9095.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy95Above.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                }
                break;
            case float f when (f >= 90.0 && f < 95.0):
                if (f - amount < 90.0)
                {
                    updateDates(EDates.Happy70Below.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy7075.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy7580.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy8085.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy8590.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy9095.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                }
                else if (f - amount >= 95.0)
                {
                    updateDates(EDates.Happy95Above.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                }
                break;
            case float f when (f >= 95.0):
                if (f - amount < 95.0)
                {
                    updateDates(EDates.Happy70Below.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy7075.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy7580.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy8085.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy8590.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy9095.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                    updateDates(EDates.Happy95Above.ToString(), Convert.DateTimeToFDate(System.DateTime.Now));
                }
                break;
            default:
                break;
        }

        Save();
    }


    public void updateDates(string key, string value)
    {
        if (dates.ContainsKey(key))
        {
            dates[key] = value;
        }
        else
        {
            dates.Add(key, value);
        }
    }

    public void resetDates(System.DateTime dateTime)
    {
        updateDates(EDates.Happy70Below.ToString(), Convert.DateTimeToFDate(dateTime));
        updateDates(EDates.Happy7075.ToString(), Convert.DateTimeToFDate(dateTime));
        updateDates(EDates.Happy7580.ToString(), Convert.DateTimeToFDate(dateTime));
        updateDates(EDates.Happy8085.ToString(), Convert.DateTimeToFDate(dateTime));
        updateDates(EDates.Happy8590.ToString(), Convert.DateTimeToFDate(dateTime));
        updateDates(EDates.Happy9095.ToString(), Convert.DateTimeToFDate(dateTime));
        updateDates(EDates.Happy95Above.ToString(), Convert.DateTimeToFDate(dateTime));
        Save();
    }

    public void OnComplete(FEntry entry)
    {
        if (entry.Type == EntryType.DailyTask)
        {
            completed_Tasks += 1;
        }
        else if (entry.Type == EntryType.ToDo)
        {
            completed_ToDos += 1;
        }
    }
}
