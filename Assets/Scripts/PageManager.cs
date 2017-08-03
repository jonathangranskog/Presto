using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that controls what is displayed on PageScreens and manages PDF loading/conversion
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
    private int previousPage = 0;
    private bool converting = false;

    void Start() {
        loader = GetComponent<ImageLoader>();
        converter = converterObject.GetComponent<PDFToImages>();
        pages = new List<Texture2D>();
        loading = false;
    }

    // Loads a pdf by first telling the converter to convert a pdf to images
    // Afterwards the converter sends a message to this object and calls LoadImages()
    public void LoadPDF(string file)
    {
        previousPage = currentPage;
        converting = true;
        LoadStarted();
        converter.Convert(file);
    }

    // Loads images from disk and updates screens
    public void LoadImages()
    {
        converting = false;
        pages = loader.LoadPages(converter.GetImageFolder());
        currentPage = 0;
        UpdateScreens();
        LoadEnded();
    }

    // This function interrupts conversion, 
    // it is called by pressing the menu button while the loading sign is visible
    public void Interrupt()
    {
        if (converting)
        {
            converter.Interrupt();
            currentPage = previousPage;
            LoadEnded();
            UpdateScreens();
        }
    }

    // Moves to the next page of the slides if possible
    public void NextPage()
    {
        if (!loading && currentPage + 1 < pages.Count)
        {
            currentPage++;
            UpdateScreens();
        }
    }

    // Moves to the previous page of the slides if possible
    public void PreviousPage()
    {
        if (!loading && currentPage - 1 >= 0)
        {
            currentPage--;
            UpdateScreens();
        }
    }

    // Updates the texture on all screens in the screen list
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
