using UnityEngine;
using UnityEngine.UI;
using System.IO;

// Properties for a folder, button + action
public class FolderProperties : MonoBehaviour {

    public GameObject textObject;
    public GameObject buttonObject;
    public GameObject imageObject;

    private string folderName = "";
    private string path = "";
    private Button button;
    private FolderAction action;
    private Image image;
    private Text text;

    void Awake()
    {
        action = buttonObject.GetComponent<FolderAction>();
        button = buttonObject.GetComponent<Button>();
        text = textObject.GetComponent<Text>();
        image = imageObject.GetComponent<Image>();
    }

    public void SetProperties(DirectoryInfo folder, MenuManager manager)
    {
        // Set path
        path = folder.FullName;
        action.menuManager = manager;
        action.path = path;
        // Set name
        folderName = folder.Name;
        text.text = ExtraUtils.ClampName(folderName, 13);
    } 
}
