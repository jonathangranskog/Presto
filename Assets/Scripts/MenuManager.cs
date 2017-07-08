using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MenuManager : MonoBehaviour {

    public GameObject canvasObject;
    public GameObject folderButtonObject;
    public GameObject fileButtonObject;
    public GameObject pageManagerObject;

    private bool open = false;
    private Canvas canvas;
    private RectTransform canvasTransform;
    private string currentDirectory;
    private FileInfo[] files;
    private DirectoryInfo[] folders;
    private List<GameObject> buttons;
    private PageManager pageManager;
    private int width = 3;
    private int height = 4;

    private void Start()
    {
        pageManager = pageManagerObject.GetComponent<PageManager>();
        canvas = canvasObject.GetComponent<Canvas>();
        canvasTransform = canvasObject.GetComponent<RectTransform>();
        string myDocuments = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        buttons = new List<GameObject>();
        OpenFolder(myDocuments);
    }

    public bool isOpen()
    {
        return open;
    }

    public void Toggle()
    {      
        if (open)
        {
            open = false;
            gameObject.SetActive(false);
        }
        else
        {
            open = true;
            SetTransform();
            gameObject.SetActive(true);
        }
    }

    public void GoUpOneLevel()
    {
        DirectoryInfo currentDirInfo = new DirectoryInfo(currentDirectory);
        DirectoryInfo parent = currentDirInfo.Parent;
        OpenFolder(parent.FullName);
    }

    public void OpenFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError("Directory does not exist!");
            return;
        }
        currentDirectory = path;
        UpdateInfo(path);
        UpdateView(0);
    }

    // Create game objects (buttons etc.) and set their properties
    private void UpdateInfo(string path)
    {
        buttons.Clear();
        DirectoryInfo newDir = new DirectoryInfo(path);
        folders = newDir.GetDirectories();
        FileInfo[] allFiles = newDir.GetFiles();

        List<FileInfo> documents = new List<FileInfo>();

        for (int i = 0; i < allFiles.Length; i++)
        {
            if (allFiles[i].Extension == ".pdf")
            {
                documents.Add(allFiles[i]);
            }
        }

        files = documents.ToArray();        
    }

    // Show an align buttons on Canvas
    private void UpdateView(int page)
    {   
        int folderCount = folders.Length;
        int fileCount = files.Length;
        int totalCount = fileCount + folderCount;
        int startIndex = page * width * height;
        if (startIndex >= totalCount) return;
        int endIndex = (page + 1) * width * height;

        // Clean up old game objects first
        foreach (GameObject button in buttons) Destroy(button);
        buttons.Clear();

        for (int i = startIndex; i < endIndex; i++)
        {
            if (i < fileCount)
            {
                GameObject fileButton = Instantiate(fileButtonObject);
                FileProperties properties = fileButton.GetComponent<FileProperties>();
                properties.SetProperties(files[i], pageManager);
                RectTransform rectTransform = fileButton.GetComponent<RectTransform>();
                rectTransform.parent = canvasObject.GetComponent<RectTransform>();
                ResetButtonTransform(rectTransform);
                SetButtonPosition(i, rectTransform);
                buttons.Add(fileButton);
            } else if (i < totalCount)
            { 
                int index = i - fileCount;
                GameObject folderButton = Instantiate(folderButtonObject);
                FolderProperties properties = folderButton.GetComponent<FolderProperties>();
                properties.SetProperties(folders[index], this);
                RectTransform rectTransform = folderButton.GetComponent<RectTransform>();
                rectTransform.parent = canvasObject.GetComponent<RectTransform>();
                ResetButtonTransform(rectTransform);
                SetButtonPosition(i, rectTransform);
                buttons.Add(folderButton);
            } else
            {
                break;
            }
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
