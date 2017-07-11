using UnityEngine;
using ImageMagick;
using System.IO;
using System.Threading;

public class PDFConvert
{
    private string inputFile;
    private string output;

    public PDFConvert(string read, string write)
    {
        this.inputFile = read;
        this.output = write;
    }

    public void Convert()
    {
        MagickReadSettings settings = new MagickReadSettings();
        settings.BackgroundColor = new MagickColor(255, 255, 255);
        settings.Density = new Density(150, 150);

        using (MagickImageCollection images = new MagickImageCollection())
        {
            images.Read(this.inputFile, settings);
            int page = 1;
            foreach (MagickImage image in images)
            {
                image.Alpha(AlphaOption.Remove);
                string num = page.ToString(); num = num.PadLeft(5, '0');
                image.Write(this.output + num + ".png");
                page++;
            }
        }
    }
}

public class PDFToImages : MonoBehaviour {

    public GameObject managerObject;
    public bool finished { get; private set; }

    private string saveDirectory = "";
    private Thread convertThread;
    private int pdfNumber = 0;
    private PageManager manager;

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

    public void Convert(string inputFile)
    {
        string imageSaveDir = GenerateNewDirectory(saveDirectory);
        Directory.CreateDirectory(imageSaveDir);
        finished = false;
        PDFConvert converter = new PDFConvert(inputFile, imageSaveDir);
        convertThread = new Thread(new ThreadStart(converter.Convert));
        convertThread.Start();
        // Unfortunately message sending doesn't work from a 2nd thread
        // so we have to repeatedly check if conversion has finished
        InvokeRepeating("CheckFinish", 0.5f, 1.0f);
    }

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

    private void ConvertHasFinished()
    {
        finished = true;
        if (manager != null)
            manager.SendMessage("LoadImages");
    }

    public void Interrupt()
    {
        // This is not actually stopping the thread
        // It just keeps the conversion running in the background without ever using the results
        // No idea how to stop ImageMagick from reading a PDF file it has already opened.
        if (!finished && convertThread != null && convertThread.IsAlive)
        {
            convertThread.Interrupt();
            convertThread.Abort();
            CancelInvoke("CheckFinish");
            convertThread = null;
            finished = true;
        }
    }

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
