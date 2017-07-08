using System.IO;
using UnityEngine.UI;
using UnityEngine;

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

    public void SetProperties(FileInfo file, PageManager manager)
    {
        // Set path
        path = file.FullName;
        action.pageManager = manager;
        action.path = path;
        // Set name
        fileName = file.Name;

        // TODO: Make sure display text is max length N
        text.text = fileName;
    }
}
