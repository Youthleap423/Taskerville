using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class DBHandler
{

    public static void SaveToJSON<T>(List<T> toSave, string filename)
    {
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        //WriteFile(GetPath(filename), content);
        PlayerPrefs.SetString(filename, content);
    }

    public static void SaveToJSON<T>(T toSave, string filename)
    {
        string content = JsonConvert.SerializeObject(toSave);
        //WriteFile(GetPath(filename), content);
        PlayerPrefs.SetString(filename, content);
    }

    public static List<T> ReadListFromJSON<T>(string content)
    {
        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }

        List<T> res = JsonHelper.FromJson<T>(content).ToList();

        return res;

    }

    public static T ReadFromJSON<T>(string content)
    {
        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return default(T);
        }

        T res = JsonConvert.DeserializeObject<T>(content);

        return res;

    }

    private static string GetPath(string filename)
    {
#if UNITY_EDITOR
        return Application.streamingAssetsPath + "/" + filename + ".txt";
#else
        return Path.Combine(Application.persistentDataPath, (filename + ".json"));
#endif

    }

    private static void WriteFile(string path, string content)
    {

        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    private static string ReadFile(string path)
    {
        if (File.Exists(path))
        {

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        return "";
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        if (json == null || json == "")
        {
            return new T[0];
        }
        Wrapper<T> wrapper = JsonConvert.DeserializeObject<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonConvert.SerializeObject(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    private class Wrapper<T>
    {
        public T[] Items;
    }
}