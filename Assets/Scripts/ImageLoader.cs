using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageLoader : MonoBehaviour {

    // Loads the images that were converted from a PDF, called by PageManager when conversion is finished
    public List<Texture2D> LoadPages(string saveDirectory)
    {
        List<Texture2D> pages = new List<Texture2D>();
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

        return pages;
    }
}
