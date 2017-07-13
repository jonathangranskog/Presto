using UnityEngine;
using UnityEngine.UI;

public class TimerCounter : MonoBehaviour {

    public GameObject textObject;
    public int startMinutes = 0;
    public int startSeconds = 0;
    
    private int minutes;
    private int seconds;
    private Text timerText;
    private bool reversed = false;
    private Canvas canvas;
    private bool running = false;

	void Start () {
        timerText = textObject.GetComponent<Text>();
        canvas = GetComponentInChildren<Canvas>();
    }

    public void TriggerAction(Ray ray)
    {
        RaycastHit hit;
        if (Raycast(ray, out hit))
        {
            if (running) StopTick();
            else StartTick();
        }
    }

    public void Show(bool value)
    {
        if (!value) Pause();
        gameObject.SetActive(value);
    }

    public void SetReversed(bool value)
    {
        reversed = value;
    }

    public void StartTick()
    {
        ResetTime();
        running = true;
        CancelInvoke("Tick");
        InvokeRepeating("Tick", 0.0f, 1.0f);
    }

    public void Pause()
    {
        running = false;
        CancelInvoke("Tick");
    }

    public void StopTick()
    {
        running = false;
        CancelInvoke("Tick");
        Invoke("ResetTime", 2.5f);
    }

    private void ResetTime()
    {
        minutes = startMinutes;
        seconds = startSeconds;
        UpdateText();
    }

    private void Tick()
    {
        if (reversed)
        {
            seconds--;
            if (seconds < 0)
            {
                seconds = 59;
                minutes--;

                if (minutes < 0)
                {
                    seconds = 0;
                    minutes = 0;
                    StopTick();
                }
            } 
        } else
        {
            seconds++;
            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;

                if (minutes > 99)
                {
                    minutes = 99;
                    seconds = 59;
                    StopTick();
                }

            }
        }

        UpdateText();
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
