using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageLoader : MonoBehaviour {

    public GameObject converterObject;

    private List<Texture2D> pages;
    private PDFToImages converter;
    
    private void Start()
    {
        converter = converterObject.GetComponent<PDFToImages>();
        pages = new List<Texture2D>();
    }

    private void OnDestroy()
    {
        pages.Clear();
    }
    
    public void LoadPages()
    {
        pages.Clear();
        string saveDirectory = converter.GetImageFolder();
        DirectoryInfo dir = new DirectoryInfo(saveDirectory);
        FileInfo[] files = dir.GetFiles();

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Extension == ".png")
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(File.ReadAllBytes(files[i].FullName));
                pages.Add(tex);
            }
        }

        Debug.Log("Loaded List<Texture2D> of length: " + pages.Count);
    }
}
