using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileAction : MonoBehaviour {

    public PageManager pageManager;
    public string path = "";
    
    public void LoadThisFile()
    {
        if (pageManager != null && File.Exists(path))
        {
            pageManager.LoadPDF(path);
        } else
        {
            Debug.LogError("PageManager is either null or file does not exist!");
        }
    }

}
