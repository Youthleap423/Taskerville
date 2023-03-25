using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMode : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        DataManager.Instance.SaveData();
        Resources.UnloadUnusedAssets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
