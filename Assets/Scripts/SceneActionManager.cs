using UnityEngine;

// Action Manager for a normal scene
public class SceneActionManager : ActionManager {

    // Enum for different objects that could have been hit by controller ray
    private enum HitObject
    {
        None,
        Menu,
        Timer,
        Screen
    }

    public GameObject pageManagerObject;
    public float rayCastMaxDist = 20.0f;
    public GameObject menuObject;
    public GameObject timerObject;

    private PageManager pageManager;
    private bool triggerHeld = false;
    private MenuManager menu;
    private TimerCounter timer;
    private Ray ray;
    private HitObject hitObj;

    void Start()
    {
        pageManager = pageManagerObject.GetComponent<PageManager>();
        menu = menuObject.GetComponent<MenuManager>();
        timer = timerObject.GetComponent<TimerCounter>();
        CreateCursor();
        int screenLayer = 1 << 31;
        cursorRaycastMask = screenLayer;
    }

    private void Update()
    {
        ray = new Ray(transform.position, transform.forward);
        cursorRenderer.enabled = false;
        hitObj = HitObject.None;

        // Ray intersect menu
        RaycastHit menuHit = new RaycastHit(); bool hitMenu;
        if (hitMenu = menu.Raycast(ray, out menuHit))
        {
            cursorRenderer.enabled = true;
            cursor.transform.position = menuHit.point;
            cursor.transform.forward = menuHit.normal;
            hitObj = HitObject.Menu;
        }

        // Ray intersect timer and see if closer than menu
        RaycastHit timerHit = new RaycastHit(); bool hitTimer;
        if (hitTimer = timer.Raycast(ray, out timerHit))
        {
            if (!hitMenu || timerHit.distance < menuHit.distance)
            {
                cursorRenderer.enabled = true;
                cursor.transform.position = timerHit.point;
                cursor.transform.forward = timerHit.normal;
                hitObj = HitObject.Timer;
            }
        }

        // Ray intersect PageScreens and see if closer than menu and timer
        if (triggerHeld)
        {
            RaycastHit screenHit = new RaycastHit();
            if (Physics.Raycast(ray, out screenHit, rayCastMaxDist, cursorRaycastMask))
            {
                if ((!hitMenu || screenHit.distance < menuHit.distance) &&
                    (!hitTimer || screenHit.distance < timerHit.distance))
                {
                    cursorRenderer.enabled = true;
                    cursor.transform.position = screenHit.point;
                    cursor.transform.forward = screenHit.normal;
                    hitObj = HitObject.Screen;
                }
            }
        }

    }

    // Run appropriate action when trigger is pressed depending on ray hit object
    override public void TriggerPress()
    {
        if (hitObj == HitObject.Menu)
        {
            menu.TriggerAction(ray);
        }
        else if (hitObj == HitObject.Timer)
        {
            timer.TriggerAction(ray);
        }

        triggerHeld = true;
    }

    override public void TriggerUnpress()
    {
        triggerHeld = false;
    }

    // Action for when Pad is clicked on the right side
    override public void PadRightClick()
    {
        if (hitObj == HitObject.Timer) timer.RightPadStep(); // Add time to timer
        else if (menu.isOpen()) menu.NextPage(); // Go to next page in file browser
        else pageManager.NextPage(); // Go to next page in PDF
    }

    // Action for when Pad is clicked on the left side
    override public void PadLeftClick()
    {
        if (hitObj == HitObject.Timer) timer.LeftPadStep(); // Remove time from timer
        else if (menu.isOpen()) menu.PreviousPage(); // Go to previous page in file browser
        else pageManager.PreviousPage(); // Go to previous page in PDF
    }

    override public void MenuClick()
    {
        if (pageManager.loading) pageManager.Interrupt(); // Interrupt if converting PDF
        else menu.Toggle(); // open or close menu
    }
}
