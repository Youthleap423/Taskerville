using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader
{
    private static System.Action<Sprite> action = null;

    public static IEnumerator Start(string path, System.Action<Sprite> callback)
    {
        byte[] imageData;
        Texture2D texture = new Texture2D(2, 2);
        

        if (path.Contains("://") || path.Contains(":///"))
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();
            imageData = www.downloadHandler.data;
            texture.LoadImage(imageData);

            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), pivot);

            yield return new WaitForEndOfFrame();
            callback(sprite);
        }
        else
        {
            
            callback(Resources.Load<Sprite>("artwork/" + path));// Application.streamingAssetsPath + " / " + path;
            
        }
    }
}
