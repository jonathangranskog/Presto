using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour {

    public GameObject converterObject;

    private ImageLoader loader;
    private PDFToImages converter;
    private List<Texture2D> pages;
    private int currentPage = 0;

    void Start() {
        loader = GetComponent<ImageLoader>();
        converter = converterObject.GetComponent<PDFToImages>();
        LoadPDF("Test/input.pdf");
    }

    // Loads a pdf by first telling the converter to convert a pdf to images
    // Afterwards the converter sends a message to this object and calls LoadImages()
    public void LoadPDF(string file)
    {
        converter.Convert(file);
    }

    public void LoadImages()
    {
        pages = loader.LoadPages(converter.GetImageFolder());
    }

    public bool GetPageTexture(int i, out Texture2D tex)
    {
        if (i < pages.Count)
        {
            tex = pages[i];
            return true;
        } else
        {
            tex = null;
            return false;
        }
    }

    public Texture2D GetCurrent()
    {
        return pages[currentPage];
    }

    public Texture2D GetNext()
    {
        if (currentPage + 1 < pages.Count)
            currentPage++;

        return pages[currentPage];
    }

    public Texture2D GetPrevious()
    {
        if (currentPage - 1 >= 0)
            currentPage--;

        return pages[currentPage];
    }

}
