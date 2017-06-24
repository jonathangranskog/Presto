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

    public string folder;
    public string file;
    public GameObject loaderObject;
    public bool finished { get; private set; }

    private string saveDirectory = "Assets/temp/cpdfs/";
    private Thread convertThread;
    private int pdfNumber = 0;
    private ImageLoader loader;

	void Awake () {
        if (Directory.Exists(saveDirectory))
            DeleteDir(saveDirectory);

        // TODO: Get a temporary directory from Windows that can be saved to
        Directory.CreateDirectory(saveDirectory);

        finished = false;
        loader = loaderObject.GetComponent<ImageLoader>();
    }

    private void OnApplicationQuit()
    {
        CleanImages();
    }

    public void Convert()
    {
        Convert(folder + file, saveDirectory);
    }

    public void Convert(string inputFile, string outputFolder)
    {
        string imageSaveDir = GenerateNewDirectory(outputFolder);
        Directory.CreateDirectory(imageSaveDir);
        finished = false;
        PDFConvert converter = new PDFConvert(inputFile, imageSaveDir);
        convertThread = new Thread(new ThreadStart(converter.Convert));
        convertThread.Start();
        // Unfortunately message sending doesn't work on a 2nd thread
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
        if (loader == null)
            loader.SendMessage("LoadPages");
    }

    public void Interrupt()
    {
        // TODO: Does this actually interrupt and stop processing? Need to backtrack also.
        // If interrupted before convert has finished, then ImageLoader should contain old pdf
        // Might be enough to Check current pdfNumber, delete the newly created directory
        // and subtract from pdf number
        if (!finished && convertThread != null)
        {
            convertThread.Interrupt();
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
