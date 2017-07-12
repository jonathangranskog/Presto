﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MenuManager : MonoBehaviour {

    public GameObject canvasObject;
    public GameObject folderButtonObject;
    public GameObject fileButtonObject;
    public GameObject pageManagerObject;
    public GameObject pathbarTextObject;
    public GameObject settingsMenuObject;
    public GameObject fileFolderParentObject;

    private bool open = false;
    private Canvas canvas;
    private RectTransform canvasTransform;
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

    private void Start()
    {
        pageManager = pageManagerObject.GetComponent<PageManager>();
        canvas = canvasObject.GetComponent<Canvas>();
        canvasTransform = canvasObject.GetComponent<RectTransform>();
        pathbarText = pathbarTextObject.GetComponent<Text>();
        string myDocuments = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        buttons = new List<GameObject>();
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

        pathbarText.text = ExtraUtils.ClampFrontName(path, 40);
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
                ResetButtonTransform(rectTransform);
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
                ResetButtonTransform(rectTransform);
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

    private void ResetButtonTransform(RectTransform rectTransform)
    {
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
    }

    public void SetTransform()
    {
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
        transform.LookAt(Camera.main.transform);
    }

    public bool RaycastCanvas(Ray ray, out RaycastHit hit)
    {
        RaycastHit h = new RaycastHit();
        hit = h;
        Vector3[] canvasCorners = new Vector3[4];
        if (canvasTransform == null) return false;
        canvasTransform.GetWorldCorners(canvasCorners);
        Plane canvasPlane = new Plane(canvasCorners[0], canvasCorners[1], canvasCorners[2]);

        float t;
        
        if (RaycastBoundedPlane(ray, canvasCorners, canvasPlane, out t))
        {
            hit.distance = t;
            hit.point = ray.GetPoint(t);
            hit.normal = canvasPlane.normal;
            return true;
        }

        return false;
    }

    // Assumes point is on the same plane as button
    public Button GetButtonAtPosition(Vector3 pos)
    {
        IList<Graphic> graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);

        for (int i = 0; i < graphics.Count; i++)
        {
            Graphic graphic = graphics[i];
            RectTransform transform = graphic.rectTransform;
            GameObject obj = transform.gameObject;

            if (obj.GetComponent<Button>() != null)
            {
                Vector3[] corners = new Vector3[4];
                transform.GetWorldCorners(corners);
                if (WithinPlane(pos, corners))
                {
                    return obj.GetComponent<Button>();
                }
            }
        }

        return null;
    }

    private bool RaycastBoundedPlane(Ray ray, Vector3[] corners, Plane plane, out float t)
    {
        float dist = -1.0f;

        if (plane.Raycast(ray, out dist))
        {
            t = dist;
            Vector3 point = ray.GetPoint(dist);
            if (WithinPlane(point, corners))
                return true;
            else
                return false;
        } else
        {
            t = dist;
            return false;
        }
    }

    private bool WithinPlane(Vector3 point, Vector3[] corners)
    {
        bool tri1 = WithinTriangle(point, corners[0], corners[1], corners[2]);
        bool tri2 = WithinTriangle(point, corners[2], corners[3], corners[0]);
        return tri1 || tri2;
    }

    private bool WithinTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 e0 = b - a;
        Vector3 e1 = c - a;
        Vector3 e2 = p - a;

        float d00 = Vector3.Dot(e0, e0);
        float d01 = Vector3.Dot(e0, e1);
        float d11 = Vector3.Dot(e1, e1);
        float d20 = Vector3.Dot(e2, e0);
        float d21 = Vector3.Dot(e2, e1);
        float denom = d00 * d11 - d01 * d01;
        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;

        if (u >= 0 && u <= 1.0f && v >= 0 && v <= 1.0f && u + v <= 1.0f)
        {
            return true;
        }

        return false;
    }

}
