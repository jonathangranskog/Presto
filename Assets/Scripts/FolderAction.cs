using System.IO;
using UnityEngine;

// Action for folder buttons, opens folder from menumanager if pressed
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
