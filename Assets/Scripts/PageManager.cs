﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour {

    public GameObject converterObject;
    public GameObject loadingSignObject;
    public List<ScreenControl> screens;
    public bool loading { get; private set; }

    private ImageLoader loader;
    private PDFToImages converter;
    private List<Texture2D> pages;
    private GameObject loadingSign;
    private int currentPage = 0;

    void Start() {
        loader = GetComponent<ImageLoader>();
        converter = converterObject.GetComponent<PDFToImages>();
        pages = new List<Texture2D>();
        loading = false;
        LoadPDF("Test/input.pdf");
    }

    // Loads a pdf by first telling the converter to convert a pdf to images
    // Afterwards the converter sends a message to this object and calls LoadImages()
    public void LoadPDF(string file)
    {
        LoadStarted();
        converter.Convert(file);
    }

    public void LoadImages()
    {
        pages = loader.LoadPages(converter.GetImageFolder());
        UpdateScreens();
        LoadEnded();
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
        if (pages.Count == 0) return;

        foreach (ScreenControl screen in screens)
        {
            screen.SetTexture(pages[currentPage]);
        }
    }    

    private void LoadStarted()
    {
        loading = true;
        loadingSign = Instantiate(loadingSignObject);
    }

    private void LoadEnded()
    {
        loading = false;
        Destroy(loadingSign);
    }

}
