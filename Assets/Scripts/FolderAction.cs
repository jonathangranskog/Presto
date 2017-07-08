using System.IO;
using UnityEngine;

public class FolderAction : MonoBehaviour {

    public MenuManager menuManager;
    public string path = "";

    public void OpenThisFolder()
    {
        if (menuManager != null && Directory.Exists(path))
        {
            menuManager.OpenFolder(path);
        } else
        {
            Debug.LogError("Menu Manager is null or directory does not exist!");
        }
    }

}
