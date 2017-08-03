using UnityEngine;
using ImageMagick;
using System.IO;
using System.Threading;

// Class that convers PDFs to PNG files using ImageMagick and GhostScript
public class PDFConvert
{
    private string inputFile;
    private string output;
    private volatile bool stop = false;

    public PDFConvert(string read, string write)
    {
        this.inputFile = read;
        this.output = write;
    }

    public void RequestStop()
    {
        stop = true;
    }

    // Convert PDF to PNGs
    public void Convert()
    {
        MagickReadSettings settings = new MagickReadSettings();
        settings.BackgroundColor = new MagickColor(255, 255, 255);
        settings.Density = new Density(150, 150);

        // This could probably be improved somehow
        // Try-catch because in order to check for interrupts
        // we need to load a small amount of pages at a time and check if stop is requested
        // I chose to convert 5 pages a time. The try catch also prevents crashing when there are no more pages
        // This is not a very nice way, but I could not find a way to get the total amount of pages in a file
        // without reading the whole file (SLOW AF)
        try
        {
            int page = 1;
            settings.FrameIndex = 0;
            settings.FrameCount = 5;

            while (!stop)
            {
                MagickImageCollection images = new MagickImageCollection();

                images.Read(this.inputFile, settings);
                foreach (MagickImage image in images)
                {
                    image.Alpha(AlphaOption.Remove); // Prevent transparency in Latex generated PDFs
                    string num = page.ToString(); num = num.PadLeft(5, '0');
                    image.Write(this.output + num + ".png");
                    page++;
                    settings.FrameIndex++;
                } 
            }
        } catch (System.Exception e)
        {
            // Some management would probably be a good idea
            // For example, what exception occurs if GhostScript is not installed?
        }

    }
}

// Class that manages conversion process
public class PDFToImages : MonoBehaviour {

    public GameObject managerObject;
    public bool finished { get; private set; }

    private string saveDirectory = "";
    private Thread convertThread;
    private int pdfNumber = 0;
    private PageManager manager;
    private PDFConvert converter;

    // Creates a temporary directory (in OS temp location) where images will be stored
    // while application is running
	void Awake () {
        saveDirectory = Path.GetTempPath();
        saveDirectory.Replace(@"\", "/");
        saveDirectory += "Presto/pdfs/";
        Directory.CreateDirectory(saveDirectory);
        
        finished = false;
        manager = managerObject.GetComponent<PageManager>();
    }

    private void OnApplicationQuit()
    {
        CleanImages();
    }

    // Run conversion in a second thread
    public void Convert(string inputFile)
    {
        string imageSaveDir = GenerateNewDirectory(saveDirectory);
        Directory.CreateDirectory(imageSaveDir);
        finished = false;
        converter = new PDFConvert(inputFile, imageSaveDir);
        //convertThread = new Thread(new ThreadStart(converter.Convert));
        convertThread = new Thread(converter.Convert);
        convertThread.Start();
        // Unfortunately message sending doesn't work from a 2nd thread
        // so we have to repeatedly check if conversion has finished
        InvokeRepeating("CheckFinish", 0.5f, 1.0f);
    }

    // Each pdf is stored in a separate folder in the temporary directory
    // Folder name is only based on the number of PDFs that have been loaded
    public string GetImageFolder()
    {
        return saveDirectory + pdfNumber.ToString() + "/";
    }

    private void CheckFinish()
    {
        if (IsFinished())
        {
            CancelInvoke("CheckFinish");
            convertThread = null;
            ConvertHasFinished();
        }
    }

    private bool IsFinished()
    {
        if (convertThread == null)
        {
            return finished;
        }
        else
        {
            return !convertThread.IsAlive;
        }
    }

    // If finished, tell PageManager
    private void ConvertHasFinished()
    {
        finished = true;
        if (manager != null)
            manager.SendMessage("LoadImages");
    }

    // Interrupts conversion by setting request stop and aborting the thread
    public void Interrupt()
    {
        if (!finished && convertThread != null && convertThread.IsAlive)
        {
            converter.RequestStop();
            convertThread.Interrupt();
            convertThread.Abort();
            CancelInvoke("CheckFinish");
            convertThread = null;
            converter = null;
            finished = true;
        }
    }

    // Deletes all folders containing PDF images from disk
    private void CleanImages()
    {
        DeleteDir(saveDirectory);
    }

    private void DeleteDir(string dir)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(saveDirectory);
        DeleteDir(dirInfo);
    }

    private void DeleteDir(DirectoryInfo dirInfo)
    {
        // Recursive delete of directories
        DirectoryInfo[] dirs = dirInfo.GetDirectories();
        for (int i = 0; i < dirs.Length; i++)
        {
            DeleteDir(dirs[i]);
        }

        DeleteFiles(dirInfo);
        dirInfo.Delete();
    }

    private void DeleteFiles(DirectoryInfo dir)
    {
        FileInfo[] files = dir.GetFiles();

        for (int i = 0; i < files.Length; i++)
        {
            files[i].Delete();
        }
    }

    // Get the folder name that should be used for the next pdf.
    // Until there is a number that doesn't exist
    private string GenerateNewDirectory(string outputFolder)
    {
        string pdfDir = outputFolder + pdfNumber.ToString() + "/";
        while (Directory.Exists(pdfDir))
        {
            pdfNumber++;
            pdfDir = saveDirectory + pdfNumber.ToString() + "/";
        }

        return pdfDir;
    }
}
