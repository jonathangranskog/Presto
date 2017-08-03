using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Action class for FileButtons, if clicked it loads the corresponding PDF.
public class FileAction : MonoBehaviour {

    public PageManager pageManager;
    public MenuManager menuManager;
    public string path = "";
    
    public void LoadThisFile()
    {
        if (pageManager != null && File.Exists(path))
        {
            menuManager.Toggle();
            pageManager.LoadPDF(path);
        } else
        {
            Debug.LogError("PageManager is either null or file does not exist!");
        }
    }

}
