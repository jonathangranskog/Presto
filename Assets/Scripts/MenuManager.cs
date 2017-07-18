using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MenuManager : MonoBehaviour {

    public GameObject canvasObject;
    public GameObject folderButtonObject;
    public GameObject fileButtonObject;
    public GameObject pageManagerObject;
    public GameObject pathbarTextObject;
    public GameObject settingsMenuObject;
    public GameObject fileFolderParentObject;
    public List<GameObject> toggles;

    private bool open = false;
    private Canvas canvas;
    private string currentDirectory;
    private FileInfo[] files;
    private DirectoryInfo[] folders;
    private List<GameObject> buttons;
    private PageManager pageManager;
    private Text pathbarText;
    private int width = 3;
    private int height = 4;
    private int currentPage = 0;
    private int totalCount = 0;
    private bool settingsOpen = false;
    private Camera mainCamera;
    
    private void Start()
    {
        pageManager = pageManagerObject.GetComponent<PageManager>();
        canvas = canvasObject.GetComponent<Canvas>();
        pathbarText = pathbarTextObject.GetComponent<Text>();
        string myDocuments = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        buttons = new List<GameObject>();
        mainCamera = Camera.main;
        OpenFolder(myDocuments);
    }

    public bool isOpen()
    {
        return open;
    }

    public bool isSettingsOpen()
    {
        return settingsOpen;
    }

    public void Toggle()
    {      
        if (open)
        {
            open = false;
            gameObject.SetActive(false);
            settingsOpen = false;
            settingsMenuObject.SetActive(false);
        }
        else
        {
            open = true;
            SetTransform();
            gameObject.SetActive(true);
        }
    }

    public void TriggerAction(Ray ray)
    {
        RaycastHit hit = new RaycastHit();
        if (ExtraUtils.RaycastCanvas(canvas, ray, out hit))
        {
            Vector3[] settingsCorners = new Vector3[4];
            settingsMenuObject.GetComponent<RectTransform>().GetWorldCorners(settingsCorners);

            if (settingsOpen && ExtraUtils.WithinPlane(hit.point, settingsCorners))
            {
                Toggle toggle = ExtraUtils.GetToggleAtPosition(toggles, hit.point);
                if (toggle != null)
                {
                    toggle.gameObject.GetComponent<TogglePress>().Toggle();
                }

            } else
            {
                Button button = ExtraUtils.GetButtonAtPosition(canvas, hit.point);
                if (button != null)
                {
                    button.onClick.Invoke();
                }
            }
        }
    }

    public void NextPage()
    {
        int maxPages = totalCount / (width * height);
        if (currentPage < maxPages)
        {
            currentPage++;
        }

        UpdateView();
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
        }

        UpdateView();
    }

    public void ToggleSettings()
    {
        settingsOpen = !settingsOpen;
        settingsMenuObject.SetActive(settingsOpen);
    }

    public void GoUpOneLevel()
    {
        DirectoryInfo currentDirInfo = new DirectoryInfo(currentDirectory);
        DirectoryInfo parent = currentDirInfo.Parent;
        if (parent == null) return;
        OpenFolder(parent.FullName);
    }

    public void OpenFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError("Directory does not exist!");
            return;
        }
        totalCount = 0;
        currentPage = 0;
        currentDirectory = path;
        UpdateInfo(path);
        UpdateView();
    }

    // Create game objects (buttons etc.) and set their properties
    private void UpdateInfo(string path)
    {
        DirectoryInfo newDir = new DirectoryInfo(path);
        DirectoryInfo[] allFolders = newDir.GetDirectories();
        FileInfo[] allFiles = newDir.GetFiles();

        List<FileInfo> documents = new List<FileInfo>();
        List<DirectoryInfo> directories = new List<DirectoryInfo>();

        // Do not display non-pdf documents
        for (int i = 0; i < allFiles.Length; i++)
        {
            if (allFiles[i].Extension == ".pdf")
            {
                documents.Add(allFiles[i]);
            }
        }

        // Do not display hidden folders
        for (int i = 0; i < allFolders.Length; i++)
        {
            if (allFolders[i].Name[0] != '.')
            {
                directories.Add(allFolders[i]);
            }
        }

        folders = directories.ToArray();
        files = documents.ToArray();

        pathbarText.text = ExtraUtils.ClampFrontName(path, 36);
    }

    private void DestroyButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            Destroy(buttons[i]);
        }
        buttons.Clear();
    }

    // Show an align buttons on Canvas
    private void UpdateView()
    {   
        int folderCount = folders.Length;
        int fileCount = files.Length;
        int startIndex = currentPage * width * height;
        int endIndex = (currentPage + 1) * width * height;
        totalCount = fileCount + folderCount;

        if (totalCount == 0)
        {
            DestroyButtons();
            return;
        }

        if (startIndex >= totalCount) return;
        
        // Clean up old game objects first
        DestroyButtons();

        int j = 0;

        for (int i = startIndex; i < endIndex; i++)
        {
            if (i < fileCount)
            {
                GameObject fileButton = Instantiate(fileButtonObject);
                FileProperties properties = fileButton.GetComponent<FileProperties>();
                properties.SetProperties(files[i], pageManager, this);
                RectTransform rectTransform = fileButton.GetComponent<RectTransform>();
                rectTransform.parent = fileFolderParentObject.GetComponent<RectTransform>();
                ExtraUtils.ResetRectTransform(rectTransform);
                SetButtonPosition(j, rectTransform);
                buttons.Add(fileButton);
            } else if (i < totalCount)
            { 
                int index = i - fileCount;
                GameObject folderButton = Instantiate(folderButtonObject);
                FolderProperties properties = folderButton.GetComponent<FolderProperties>();
                properties.SetProperties(folders[index], this);
                RectTransform rectTransform = folderButton.GetComponent<RectTransform>();
                rectTransform.parent = fileFolderParentObject.GetComponent<RectTransform>();
                ExtraUtils.ResetRectTransform(rectTransform);
                SetButtonPosition(j, rectTransform);
                buttons.Add(folderButton);
            } else
            {
                break;
            }

            j++;
        }

    }

    private void SetButtonPosition(int index, RectTransform rectTransform)
    {
        int i = index % width;
        int j = index / width;

        rectTransform.anchoredPosition = new Vector2(i * 110 - 110, 150 - j * 110);
    }

    public void SetTransform()
    {
        transform.position = mainCamera.transform.position + mainCamera.transform.forward * 1.5f;
        transform.LookAt(mainCamera.transform);
    }

    public bool Raycast(Ray ray, out RaycastHit hit)
    {
        hit = new RaycastHit();
        
        if (!open)
        {
            return false;
        }
        return ExtraUtils.RaycastCanvas(canvas, ray, out hit);
    }

    public void GoHome()
    {
#if !UNITY_EDITOR
        SceneManager.LoadSceneAsync(0);
#endif
    }
}
