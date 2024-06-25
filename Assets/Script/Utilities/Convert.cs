using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Convert
{
    private static List<string> monthsList = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    private static Dictionary<string, string> timezoneDic = new Dictionary<string, string>
    {
        {"GMT", "Greenwich Mean Time"},
        {"EAT", "East Africa Time" },
        {"CET", "Central European Time" },
        {"WAT", "West Africa Time" },
        {"CAT", "Central Africa Time" },
        {"EET", "Eastern European Time" },
        {"SAST", "South African Standard Time" },
        {"HST", "Hawaii-Aleutian Standard Time"},
        {"AKST", "Alaska Standard Time" },
        {"AST", "Atlantic Standard Time" },
        {"EST", "Estern Standard Time" },
        {"CST", "Central Standard Time" },
        {"MST", "Mountain Standard Time" },
        {"PST", "Pacific Standard Time"},
        {"NST", "Newfoundland Standard Time" },
        {"AEST", "Australian Eastern Standard Time" },
        {"NZST", "New Zealand Standard Time" },
        {"IST", "Israel Standard Time" },
        {"HKT", "Hong Kong Time" },
        {"WIB", "Western Indonesia Time" },
        {"WIT", "Easetern Indonesia Time" },
        {"PKT", "Pakistan Standard Time" },
        {"WITA", "Central Indonesia Time" },
        {"KST", "Korea Standard Time" },
        {"JST", "Japan Standard Time" },
        {"WET", "Western European Time" },
        {"ACST", "Australian Central Standard Time" },
        {"AWST", "Australian Western Standard Time" },
        {"MSK", "Moscow Time" },
        {"ChST", "Chamorro Standard Time" },
        {"SST", "Samoa Standard Time" }
    };

    public static bool IntToBool(int number)
    {
        if (number == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static int BoolToInt(bool flag)
    {
        if (flag == true)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public static int Max(int a, int b)
    {
        if (a > b)
        {
            return a;
        }
        else
        {
            return b;
        }
    }

    public static int Min(int a, int b)
    {
        if (a > b)
        {
            return b;
        }
        else
        {
            return a;
        }
    }

    public static string FDateToEntryDate(string dateStr)
    {
        if (dateStr.Equals(""))
        {
            return dateStr;
        }

        System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
        string formatString = "yyyy_MM_dd";
        System.DateTime datetime = System.DateTime.ParseExact(dateStr, formatString, provider);
        return datetime.ToString("MMMM d, yyyy");
    }

    public static string EntryDateToFDate(string dateStr)
    {
        System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
        string formatString = "MMMM d, yyyy";
        System.DateTime datetime = System.DateTime.ParseExact(dateStr, formatString, provider);
        return datetime.ToString("yyyy_MM_dd");
    }

    public static System.DateTime FDateToDateTime(string dateStr)
    {
        System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
        string formatString = "yyyy_MM_dd";
        System.DateTime datetime = System.DateTime.ParseExact(dateStr, formatString, provider);
        return datetime;
    }

    public static System.DateTime DetailedStringToDateTime(string dateStr)
    {
        System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
        string formatString = "yyyy_MM_dd_hh:mm tt";
        System.DateTime datetime = System.DateTime.ParseExact(dateStr, formatString, provider);
        return datetime;
    }

    public static System.DateTime EntryDateToDateTime(string dateStr)
    {
        System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
        string formatString = "MMMM d, yyyy";
        System.DateTime datetime = System.DateTime.ParseExact(dateStr, formatString, provider);
        return datetime;
    }

    public static string DateTimeToFDate(System.DateTime dateTime)
    {
        return dateTime.ToString("yyyy_MM_dd");
    }

    public static string DateTimeToEntryDate(System.DateTime dateTime)
    {
        return dateTime.ToString("MMMM d, yyyy");
    }

    public static string DateTimeToDetailedString(System.DateTime dateTime)
    {
        return dateTime.ToString("yyyy_MM_dd_hh:mm tt");
    }

    public static List<string> DateTimeToFDate(List<System.DateTime> dateTimes)
    {
        List<string> list = new List<string>();
        foreach(System.DateTime day in dateTimes)
        {
            list.Add(DateTimeToFDate(day));
        }
        return list;
    }

    public static List<System.DayOfWeek> IntToWeekday(List<int> days)
    {
        List<System.DayOfWeek> list = new List<System.DayOfWeek>();
        foreach(int day in days)
        {
            list.Add((System.DayOfWeek)day);
        }

        return list;
    }

    public static string DateTimeToEntryDate(int Year, int Month, int Day)
    {
        return string.Format("{0} {1}, {2}", monthsList[Month], Day, Year);
    }

    public static string DateTimeToMessageTime(System.DateTime dateTime)
    {
        return string.Format("{0} - {1}", dateTime.ToString("M/d/yy"), dateTime.ToShortTimeString());
    }

    public static double TimeDifferenceMinutes(string startTime, System.DateTime dateTime)
    {
        System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
        string formatString = "yyyy_MM_dd_hh:mm tt";
        System.DateTime startDateTime = System.DateTime.ParseExact(startTime, formatString, provider);
        return (dateTime - startDateTime).TotalMinutes;
    }

    public static double TimeDifferenceSeconds(string startTime, System.DateTime dateTime)
    {
        System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
        string formatString = "yyyy_MM_dd_hh:mm tt";
        System.DateTime startDateTime = System.DateTime.ParseExact(startTime, formatString, provider);
        return (dateTime - startDateTime).TotalSeconds;
    }

    public static string GetTimeStringForServer(System.DateTime dateTime)
    {
        string formatString = "yyyy-MM-ddTHH:mm:ss.000Z";
        return dateTime.ToString(formatString);
    }

    public static string GetTimeString(string startTime, int hours, int minutes)
    {
        System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
        string formatString = "hh:mm tt";
        System.DateTime startDateTime = System.DateTime.ParseExact(startTime, formatString, provider);
        startDateTime = startDateTime.AddHours(hours);
        startDateTime = startDateTime.AddMinutes(minutes);

        return startDateTime.ToString(formatString);

    }

    public static string GetTimeString(System.DateTime dateTime)
    {
        System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
        string formatString = "hh:mm tt";
        
        return dateTime.ToString(formatString);
    }



    public static Dictionary<T, float> DicObjectToFloat<T>(Dictionary<string, object> sourceDic)
    {
        Dictionary<T, float> dic = new Dictionary<T, float>();
        foreach(string key in sourceDic.Keys)
        {
            dic.Add((T)Enum.Parse(typeof(T), key), float.Parse(sourceDic[key].ToString()));
        }
        return dic;
    }

    public static string GetSTZ(System.TimeZoneInfo info)
    {
        if (timezoneDic.ContainsKey(info.StandardName))
        {
            return info.StandardName;
        }
        else
        {
            return "GMT";
        }
    }

    public static string GetSTZName(string str)
    {
        return timezoneDic[str];
    }
}
