using UnityEngine;
using UnityEngine.UI;

// Timer script
public class TimerCounter : MonoBehaviour {

    public GameObject textObject;
    public int startMinutes = 0;
    public int startSeconds = 0;
    public int padStep = 30;
    
    private int minutes;
    private int seconds;
    private Text timerText;
    private bool reversed = false;
    private Canvas canvas;
    private bool running = false;
    
	void Start () {
        timerText = textObject.GetComponent<Text>();
        canvas = GetComponentInChildren<Canvas>();
        ResetTime();
        gameObject.SetActive(false);
    }

    // If trigger is pressed and pointing at timer,
    // then either stop or continue
    public void TriggerAction(Ray ray)
    {
        RaycastHit hit;
        if (Raycast(ray, out hit))
        {
            if (running) Pause();
            else Run();
        }
    }

    public void Show(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetReversed(bool value)
    {
        reversed = value;
    }

    // When running, add 1 repeatedly.
    private void Run()
    {
        running = true;
        CancelInvoke("Tick");
        InvokeRepeating("Tick", 0.0f, 1.0f);
    }

    private void Pause()
    {
        running = false;
        CancelInvoke("Tick");
    }

    // When 00:00 or 99:99 is reached: Stop, set color to orange and then reset time after 2.5 seconds
    private void StopTick()
    {
        Pause();
        timerText.color = new Color(1.0f, 0.788f, 0.561f);
        Invoke("ResetTime", 2.5f);
        // Could also play some ringing sound here
    }

    private void SetTime(int mins, int secs)
    {
        minutes = mins; seconds = secs;
    }

    // Reset color and set back to start time
    private void ResetTime()
    {
        timerText.color = new Color(0.914f, 0.914f, 0.914f);
        SetTime(startMinutes, startSeconds);
        UpdateText();
    }

    // left pad clicked, remove padStep seconds from time
    public void LeftPadStep()
    {
        StepBack(padStep);
    }

    // right pad clicked, add padStep second to time
    public void RightPadStep()
    {
        StepForward(padStep);
    }

    // Function to remove secs seconds from time
    private void StepBack(int secs)
    {
        seconds -= secs;

        int sub = 0;
        while (seconds < 0)
        {
            seconds += 60; sub++;
        }

        minutes -= sub;

        if (minutes < 0)
        {
            SetTime(0, 0);
            if (running) StopTick();
        }

        UpdateText();
    }

    // Add secs seconds to time
    private void StepForward(int secs)
    {
        seconds += secs; minutes += seconds / 60;
        seconds %= 60;

        if (minutes > 99)
        {
            SetTime(99, 59);
            if (running) StopTick();
        }
        UpdateText();
    }

    private void Tick()
    {
        if (reversed) StepBack(1);
        else StepForward(1);
    }

    private void UpdateText()
    {
        string formatted = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = formatted;
    }

    public bool Raycast(Ray ray, out RaycastHit hit)
    {
        hit = new RaycastHit();
        hit.distance = 1000.0f;

        if (!isVisible())
        {
            return false;
        }

        return ExtraUtils.RaycastCanvas(canvas, ray, out hit);
    }

    public bool isVisible()
    {
        return gameObject.activeInHierarchy;
    }

}
