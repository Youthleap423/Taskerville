using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

public static class Utilities
{
	#region Properties

	public static double SystemTimeInMilliseconds { get { return (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalMilliseconds; } }
	public static string SystemTimeInMillisecondsString { get { return string.Format("{0}", SystemTimeInMilliseconds); } }
	public static long SystemTicks { get { return new System.DateTimeOffset(System.DateTime.Now).Ticks; } }
	public static float WorldWidth	{ get { return 2f * Camera.main.orthographicSize * Camera.main.aspect; } }
	public static float WorldHeight	{ get { return 2f * Camera.main.orthographicSize; } }
	public static float	XScale		{ get { return (float)UnityEngine.Screen.width / 1080f; } }	
	public static float	YScale		{ get { return (float)UnityEngine.Screen.height / 1920f; } }
	public static List<string> encourageSentences = new List<string> { "You could do it...Get your stuff done today!", "Come on now, don't slack off my friend.", "Diffcult time yesterday? I hope you have more success today!" };
	public static List<string> praiseSentencse = new List<string> { "Nice job on your Tasks yesterday!", "Awesome - keep it up!", "You're an inspiration to the coalition" };
	public static string[] m_compass = new string[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
	#endregion

	#region Delegates

	public delegate TResult MapFunc<out TResult, TArg>(TArg arg);
	public delegate bool FilterFunc<TArg>(TArg arg);

	#endregion

	#region Public Methods

	public static System.DateTime dateTimeFromTicksString(string str) {
		System.DateTime dt = new System.DateTime(long.Parse(str), System.DateTimeKind.Local);
		//dt = dt.AddSeconds(double.Parse(str));
		return dt;
	}

	public static List<TOut> Map<TIn, TOut>(List<TIn> list, MapFunc<TOut, TIn> func)
	{
		List<TOut> newList = new List<TOut>(list.Count);

		for (int i = 0; i < list.Count; i++)
		{
			newList.Add(func(list[i]));
		}

		return newList;
	}

	public static void Filter<T>(List<T> list, FilterFunc<T> func)
	{
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (func(list[i]))
			{
				list.RemoveAt(i);
			}
		}
	}

	public static void SwapValue<T>(ref T value1, ref T value2)
	{
		T temp = value1;
		value1 = value2;
		value2 = temp;
	}

	public static float EaseOut(float t)
	{
		return 1.0f - (1.0f - t) * (1.0f - t) * (1.0f - t);
	}
		
	public static float EaseIn(float t)
	{
		return t * t * t;
	}

	/// <summary>
	/// Returns to mouse position
	/// </summary>
	public static Vector2 MousePosition()
	{
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		return (Vector2)Input.mousePosition;
		#else
		if (Input.touchCount > 0)
		{
			return Input.touches[0].position;
		}

		return Vector2.zero;
		#endif
	}

	/// <summary>
	/// Returns true if a mouse down event happened, false otherwise
	/// </summary>
	public static bool MouseDown()
	{
		return Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began);
	}
		
	/// <summary>
	/// Returns true if a mouse up event happened, false otherwise
	/// </summary>
	public static bool MouseUp()
	{
		return (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended));
	}
		
	/// <summary>
	/// Returns true if no mouse events are happening, false otherwise
	/// </summary>
	public static bool MouseNone()
	{
		return (!Input.GetMouseButton(0) && Input.touchCount == 0);
	}

	public static char CharToLower(char c)
	{
		return (c >= 'A' && c <= 'Z') ? (char)(c + ('a' - 'A')) : c;
	}

	public static int GCD(int a, int b)
	{
		int start = Mathf.Min(a, b);
			
		for (int i = start; i > 1; i--)
		{
			if (a % i == 0 && b % i == 0)
			{
				return i;
			}
		}
			
		return 1;
	}

	public static Canvas GetCanvas(Transform transform)
	{
		if (transform == null)
		{
			return null;
		}

		Canvas canvas = transform.GetComponent<Canvas>();

		if (canvas != null)
		{
			return canvas;
		}

		return GetCanvas(transform.parent);
	}

	public static void CallExternalAndroid(string methodname, params object[] args)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass	unity			= new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject	currentActivity	= unity.GetStatic<AndroidJavaObject>("currentActivity");
		currentActivity.Call(methodname, args);
		#endif
	}

	public static T CallExternalAndroid<T>(string methodname, params object[] args)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass	unity			= new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject	currentActivity	= unity.GetStatic<AndroidJavaObject>("currentActivity");
		return currentActivity.Call<T>(methodname, args);
		#else
		return default(T);
		#endif
	}
		
	public static string ConvertToJsonString(object data, bool addQuoteEscapes = false)
	{
		string jsonString = "";
			
		if (data is IDictionary)
		{
			Dictionary<string, object> dic = data as Dictionary<string, object>;
				
			jsonString += "{";
				
			List<string> keys = new List<string>(dic.Keys);
				
			for (int i = 0; i < keys.Count; i++)
			{
				if (i != 0)
				{
					jsonString += ",";
				}

				if (addQuoteEscapes)
				{
					jsonString += string.Format("\\\"{0}\\\":{1}", keys[i], ConvertToJsonString(dic[keys[i]], addQuoteEscapes));
				}
				else
				{
					jsonString += string.Format("\"{0}\":{1}", keys[i], ConvertToJsonString(dic[keys[i]], addQuoteEscapes));
				}
			}
				
			jsonString += "}";
		}
		else if (data is IList)
		{
			IList list = data as IList;
				
			jsonString += "[";
				
			for (int i = 0; i < list.Count; i++)
			{
				if (i != 0)
				{
					jsonString += ",";
				}
					
				jsonString += ConvertToJsonString(list[i], addQuoteEscapes);
			}
				
			jsonString += "]";
		}
		else if (data is string)
		{
			// If the data is a string then we need to inclose it in quotation marks
			if (addQuoteEscapes)
			{
				jsonString += "\\\"" + data + "\\\"";
			}
			else
			{
				jsonString += "\"" + data + "\"";
			}
		}
		else if (data is bool)
		{
			jsonString += (bool)data ? "true" : "false";
		}
		else
		{
			// Else just return what ever data is as a string
			jsonString += data.ToString();
		}
			
		return jsonString;
	}

	public static void SetLayer(GameObject gameObject, int layer, bool applyToChildren = false)
	{
		gameObject.layer = layer;

		if (applyToChildren)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				SetLayer(gameObject.transform.GetChild(i).gameObject, layer, true);
			}
		}
	}

	public static List<string[]> ParseCSVFile(string fileContents, char delimiter)
	{
		List<string[]>	csvText	= new List<string[]>();
		string[]		lines	= fileContents.Split('\n');

		for (int i = 0; i < lines.Length; i++)
		{
			csvText.Add(lines[i].Split(delimiter));
		}

		return csvText;
	}

	public static void DestroyAllChildren(Transform parent)
	{
		for (int i = parent.childCount - 1; i >= 0; i--)
		{
			GameObject.Destroy(parent.GetChild(i).gameObject);
		}
	}

	public static string FindFile(string fileName, string directory)
	{
		List<string>	files		= new List<string>(System.IO.Directory.GetFiles(directory));
		string[]		directories	= System.IO.Directory.GetDirectories(directory);

		for (int i = 0; i < files.Count; i++)
		{
			if (fileName == System.IO.Path.GetFileNameWithoutExtension(files[i]))
			{
				return files[i];
			}
		}

		for (int i = 0; i < directories.Length; i++)
		{
			string path = FindFile(fileName, directories[i]);

			if (!string.IsNullOrEmpty(path))
			{
				return path;
			}
		}
			
		return null;
	}

	public static string CalculateMD5Hash(string input)
	{
		System.Security.Cryptography.MD5	md5			= System.Security.Cryptography.MD5.Create();
		byte[]								inputBytes	= System.Text.Encoding.ASCII.GetBytes(input);
		byte[]								hash		= md5.ComputeHash(inputBytes);
		System.Text.StringBuilder			sb			= new System.Text.StringBuilder();
			
		for (int i = 0; i < hash.Length; i++)
		{
			sb.Append(hash[i].ToString("x2"));
		}
			
		return sb.ToString();
	}

	public static bool CompareLists<T>(List<T> list1, List<T> list2)
	{
		if (list1.Count != list2.Count)
		{
			return false;
		}

		for (int i = list1.Count - 1; i >= 0; i--)
		{
			bool found = false;

			for (int j = 0; j < list2.Count; j++)
			{
				if (list1[i].Equals(list2[j]))
				{
					found = true;
					list1.RemoveAt(i);
					list2.RemoveAt(j);
					break;
				}
			}

			if (!found)
			{
				return false;
			}
		}

		return true;
	}

	public static void PrintList<T>(List<T> list)
	{
		string str = "";

		for (int i = 0; i < list.Count; i++)
		{
			if (i != 0)
			{
				str += ", ";
			}

			str += list[i].ToString();
		}

		Debug.Log(str);
	}

	public static Vector2 Rotate(Vector2 v, float degrees)
	{
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

		float tx = v.x;
		float ty = v.y;

		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);

		return v;
	}

	/// <summary>
	/// Creates a new texture with the given width, height, and base color
	/// </summary>
	public static Texture2D CreateTexture(int width, int height, Color color)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

		texture.filterMode = FilterMode.Point;

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				texture.SetPixel(x, y, color);
			}
		}

		texture.Apply();

		return texture;
	}

	public static List<string> GetFilesRecursively(string path, string searchPatter)
	{
		List<string> files = new List<string>();

		if (!System.IO.Directory.Exists(path))
		{
			return files;
		}

		List<string> directories = new List<string>() { path };

		while (directories.Count > 0)
		{
			string directory = directories[0];

			directories.RemoveAt(0);

			files.AddRange(System.IO.Directory.GetFiles(directory, searchPatter));
			directories.AddRange(System.IO.Directory.GetDirectories(directory));
		}

		return files;
	}

	public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
	{
		Vector2 localPoint;
		Vector2 fromPivotDerivedOffset	= new Vector2(from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
		Vector2 screenP					= RectTransformUtility.WorldToScreenPoint(null, from.position);

		screenP += fromPivotDerivedOffset;

		RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);

		Vector2 pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);

		return localPoint - pivotDerivedOffset;
	}

	public static List<T> GetList<T>(T[] datas)
	{
		var resultList = new List<T>();
		foreach (T data in datas)
		{
			resultList.Add(data);
		}
		return resultList;
	}

	public static string GetFormattedDate(int daysAgo = 0)
    {
		System.DateTime dateTime = System.DateTime.Now.AddDays(daysAgo); 
		return dateTime.ToString("yyyy_MM_dd");
	}
	/*
	public static float getProgress(FTaskEntry entry, FProjectEntry projectEntry)
    {
		List<System.DateTime> dateList = new List<System.DateTime>();

		switch ((Repeatition)entry.repeatition)
		{
			case Repeatition.Daily:
				dateList = DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), Convert.EntryDateToDateTime(projectEntry.endDate), Repeatition.Daily, entry.repeat_every).ToList();
				break;
			case Repeatition.Weekly:
				dateList = DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), Convert.EntryDateToDateTime(projectEntry.endDate), entry.repeat_every, Convert.IntToWeekday(entry.repeatDays)).ToList();
				break;
			case Repeatition.Monthly:
				dateList = DatesList(Convert.FDateToDateTime(entry.begin_date), Convert.EntryDateToDateTime(projectEntry.beginDate), Convert.EntryDateToDateTime(projectEntry.endDate), Repeatition.Monthly, entry.repeat_every).ToList();
				break;
			default:
				break;
		}

		List<string> skipDates = new List<string>();
		foreach (string date in entry.skip_Dates)
		{
			if (date.CompareTo(projectEntry.beginDate) >= 0 && date.CompareTo(projectEntry.endDate) <= 0)
			{
				skipDates.Add(date);
			}
		}

		return dateList.Count == 0 ? 1.0f : (float)(dateList.Count - skipDates.Count) / dateList.Count;
	}
	*/
	public static IEnumerable<System.DateTime> DatesList(System.DateTime from, System.DateTime to, int interval, List<System.DayOfWeek> weeklist)
    {
		for (System.DateTime day = from.Date; day.Date < to.Date; day = day.AddDays(interval))
		{
			if (weeklist.Contains(day.DayOfWeek))
			{
				yield return day;
			}
		}
    }

	public static IEnumerable<System.DateTime> DatesList(System.DateTime begin, System.DateTime from, System.DateTime to, int interval, List<System.DayOfWeek> weeklist)
	{
		for (System.DateTime day = begin.Date; day.Date <= to.Date; day = day.AddDays(interval))
		{
			if (weeklist.Contains(day.DayOfWeek) && day.Date >= from.Date)
			{
				yield return day;
			}
		}
	}

	public static IEnumerable<System.DateTime> DatesList(System.DateTime begin, System.DateTime from, System.DateTime to, Repeatition repeatition, int interval)
	{
		if (repeatition == Repeatition.Monthly)
		{
			for (System.DateTime day = begin.Date; day.Date <= to.Date; day = day.AddMonths(interval))
			{
				if (day.Day == begin.Day && day.Date >= from.Date)
				{
					yield return day;
				}
			}
		}

		if (repeatition == Repeatition.Daily)
		{
			for (System.DateTime day = begin.Date; day.Date <= to.Date; day = day.AddDays(interval))
			{
				if (day.Date >= from.Date)
                {
					yield return day;
				}
				
			}
		}

		if (repeatition == Repeatition.Yearly)
		{
			for (System.DateTime day = begin.Date; day.Date <= to.Date; day = day.AddMonths(interval))
			{
				if (day.Day == begin.Day && day.Month == begin.Month && day.Date >= from.Date)
				{
					yield return day;
				}
			}
		}
	}
	public static double GetDays(System.DateTime startDate, System.DateTime endDate)
    {
		return (endDate - startDate).TotalDays;
    }

	public static System.DateTime GetDate(System.DateTime startDate, double days)
    {
		return startDate.AddDays(days);
    }

	
	public static FVillager GetVillager(EVillagerType type, List<FVillager> resList)
	{
		foreach (FVillager res in resList)
		{
			if (res.type == type.ToString())
			{
				return res;
			}
		}

		return new FVillager();
	}

	

	public static int findnum(string str)
	{

		int n = str.Length;

		int count_after_dot = 0;
		bool dot_seen = false;

		int num = 0;
		for (int i = 0; i < n; i++)
		{
			if (str[i] != '.')
			{
				num = num * 10 + (str[i] - '0');
				if (dot_seen == true)
					count_after_dot++;
			}
			else
				dot_seen = true;
		}

		if (dot_seen == false)
			return 1;

		int dem = (int)Mathf.Pow(10, count_after_dot);

		return (dem / GCD(num, dem));
	}

	public static string GetEncourageString()
    {
		return encourageSentences[((int)(Random.value * 100)) % encourageSentences.Count];
    }

	public static string GetPraiseString()
	{
		return praiseSentencse[((int)(Random.value * 100)) % praiseSentencse.Count];
	}

	
	public static string GetDirectionString(Vector3 m_StartPos, Vector3 m_EndPos)
	{
		var rtnval = 0.0f;

		rtnval = Mathf.Atan2(m_EndPos.y - m_StartPos.y, m_EndPos.x - m_StartPos.x);
		rtnval *= 360.0f / (2 * Mathf.PI);

		rtnval = 90 - rtnval;

		if (rtnval < 0)
		{
			rtnval += 360;
		}

		rtnval /= 45;
		int index = Mathf.RoundToInt((float)rtnval);
		if (index == m_compass.Length)
		{
			index = 0;
		}
		return m_compass[index];
	}

	public static string GetPluralWord(string word)
    {
		var result = word;
		Regex g1 = new Regex(@"s\b|z\b|x\b|sh\b|ch\b");
		MatchCollection matches = g1.Matches(word);
		if (matches.Count > 0)
        {
			return result + "es";
        }

		if (word.EndsWith("y", true, CultureInfo.InvariantCulture)){
			Regex g2 = new Regex(@"(ay|ey|iy|oy|uy)\b");
			if (g2.Matches(word).Count <= 0)
            {
				return result.Substring(0, result.Length - 1) + "ies";
            }
            else
            {
				return result + "s";
            }
        }

		if (word.EndsWith("man", true, CultureInfo.InvariantCulture))
		{
			return result.Substring(0, result.Length - 3) + "men";
		}

		if (word.Equals("Child"))
        {
			return "Children";
        }

		return result + "s";
    }

	public static float GetZDepth(Vector3 pos)
    {
		//return -0.5f + (-291 + pos.y) / 10000.0f;
		return -0.51f +  pos.y / 10000.0f;
	}

	#endregion
}

