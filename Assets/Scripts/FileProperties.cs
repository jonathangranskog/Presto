using System.IO;
using UnityEngine.UI;
using UnityEngine;

// Class containing information about PDF files and their button + action
public class FileProperties : MonoBehaviour {

    public GameObject textObject;
    public GameObject buttonObject;
    public GameObject imageObject;

    private string fileName = "";
    private string path = "";
    private Button button;
    private FileAction action;
    private Image image;
    private Text text;

    void Awake()
    {
        action = buttonObject.GetComponent<FileAction>();
        button = buttonObject.GetComponent<Button>();
        text = textObject.GetComponent<Text>();
        image = imageObject.GetComponent<Image>();
    }

    public void SetProperties(FileInfo file, PageManager pageManager, MenuManager menuManager)
    {
        // Set path
        path = file.FullName;
        action.pageManager = pageManager;
        action.menuManager = menuManager;
        action.path = path;
        // Set name
        fileName = file.Name;
        text.text = ExtraUtils.ClampName(fileName, 12);
    }
}
