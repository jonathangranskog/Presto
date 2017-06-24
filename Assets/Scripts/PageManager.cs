using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour {

    public GameObject converterObject;
    public List<ScreenControl> screens;

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
        UpdateScreens();
    }

    public void NextPage()
    {
        if (currentPage + 1 < pages.Count)
        {
            currentPage++;
            UpdateScreens();
        }
    }

    public void PreviousPage()
    {
        if (currentPage - 1 >= 0)
        {
            currentPage--;
            UpdateScreens();
        }
    }

    private void UpdateScreens()
    {
        foreach (ScreenControl screen in screens)
        {
            screen.SetTexture(pages[currentPage]);
        }
    }    

}
